using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using robstagram.Data;
using robstagram.Extensions;
using robstagram.Models.Entities;
using robstagram.ViewModels;

namespace robstagram.Controllers
{
    /// <summary>
    /// Manages application operations like creating/receiving/deleting posts. User have to be
    /// authorized to use this Web API.
    /// </summary>
    [Authorize(Policy = "ApiUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class RobstagramController : Controller
    {
        #region Variables

        private readonly ApplicationDbContext _appDbContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;

        private readonly int _pageSize = 5;

        #endregion

        #region Constructors

        public RobstagramController(ApplicationDbContext appDbContext, IHostingEnvironment hostingEnvironment,
            IMapper mapper)
        {
            _appDbContext = appDbContext;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
        }

        #endregion

        #region Api

        /// <summary>
        /// Create a new post
        /// </summary>
        /// <remarks>
        /// Create a single post in database
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("posts")]
        public async Task<ActionResult<string>> CreatePost([FromBody] PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // read uploaded image from relative url
            MemoryStream ms = new MemoryStream();
            var fullPath = Path.Combine(_hostingEnvironment.WebRootPath, model.ImageUrl);
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                await stream.CopyToAsync(ms);
            }

            // convert image and save
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            image.Resize(640, 640).Save(fullPath);

            // create image in database
            Image imageEntity = new Image()
            {
                Name = Path.GetFileName(fullPath),
                Url = model.ImageUrl,
                Data = null,
                Size = model.Size,
                Width = image.Width,
                Height = image.Height,
                ContentType = image.RawFormat.ToString()
            };
            await _appDbContext.Images.AddAsync(imageEntity);
            await _appDbContext.SaveChangesAsync();

            // create post in database
            var currentCustomer = await GetCurrentCustomer();
            await _appDbContext.Entries.AddAsync(new Entry()
            {
                Owner = currentCustomer,
                Picture = imageEntity,
                Description = model.Description
            });
            await _appDbContext.SaveChangesAsync();

            return new OkObjectResult("Entry created");
        }

        /// <summary>
        /// Delete the post with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("posts/{id}")]
        public async Task<ActionResult<string>> DeletePost(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await GetFullyResolvedPostsQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                return NotFound(id);
            }

            _appDbContext.Entries.Remove(post);
            _appDbContext.SaveChanges();

            return new OkObjectResult("Post deleted");
        }

        /// <summary>
        /// Returns all posts
        /// </summary>
        /// <remarks>
        /// Returns all posts for current PAGE with page LIMIT 5. If FORUSER flag is set only posts
        /// which were created by current user are returned
        /// </remarks>
        /// <returns></returns>
        [HttpGet("posts")]
        public async Task<ActionResult<List<PostData>>> GetPosts(int page, bool? forUser = false)
        {
            // get posts from database
            var query = GetFullyResolvedPostsQuery();

            // query posts
            var entries = forUser.Value
                ? await query
                    .Where(e => e.Owner == GetCurrentCustomer().Result)
                    .OrderByDescending(e => e.DateCreated)
                    .Skip((page - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToListAsync()
                : await query
                    .OrderByDescending(e => e.DateCreated)
                    .Skip((page - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToListAsync();

            var response = entries.Select(e => _mapper.Map<PostData>(e))
                .OrderByDescending(x => x.DateCreated).ToList();

            return new OkObjectResult(response);
        }

        /// <summary>
        /// Find post by ID
        /// </summary>
        /// <remarks>
        /// Returns the post specified by ID
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("posts/{id}")]
        public async Task<ActionResult<PostData>> GetPost(int id)
        {
            // fetch post by id from db
            var entry = await GetFullyResolvedPostsQuery()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null)
            {
                return NotFound(id);
            }

            var response = _mapper.Map<PostData>(entry);
            return new OkObjectResult(response);
        }

        /// <summary>
        /// Toggle like for post specified by ID
        /// </summary>
        /// <remarks>
        /// Toggle like for post specified by ID. If post has already been
        /// liked it is unliked for current user.
        /// </remarks>
        /// <returns></returns>
        [HttpPost("posts/{id}/likes")]
        public async Task<ActionResult<PostData>> PostLike(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // get post by id from db
            var post = await GetFullyResolvedPostsQuery()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (post == null)
            {
                return NotFound($"Entry with id ${id} not found.");
            }

            // like/unlike this post for current user
            var currentCustomer = await GetCurrentCustomer();

            // init collection if this is the first like
            if (post.Likes == null)
            {
                post.Likes = new List<Like>();
            }

            // like/unlike post
            if (!post.Likes.Select(l => l.Customer).ToList().Contains(currentCustomer))
            {
                post.Likes.Add(new Like {Entry = post, Customer = currentCustomer});
                _appDbContext.Entries.Update(post);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                post.Likes.Remove(post.Likes.First(l => l.Customer == currentCustomer));
                _appDbContext.Entries.Update(post);
                await _appDbContext.SaveChangesAsync();
            }

            var response = _mapper.Map<PostData>(post);
            return new OkObjectResult(response);
        }

        /// <summary>
        /// Create new comment for post with given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpPost("posts/{id}/comments")]
        public async Task<ActionResult<PostData>> CreateComment(int id, string text)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // get post by id from db
            var post = await GetFullyResolvedPostsQuery()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (post == null)
            {
                return NotFound($"Entry with id ${id} not found.");
            }

            var currentCustomer = await GetCurrentCustomer();

            if (post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }

            post.Comments.Add(new Comment {Owner = currentCustomer, Text = text});
            _appDbContext.Entries.Update(post);
            await _appDbContext.SaveChangesAsync();

            var response = _mapper.Map<PostData>(post);
            return new OkObjectResult(response);
        }

        /// <summary>
        /// Delete a comment with given id and return updated post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("comments/{id}")]
        public async Task<ActionResult<PostData>> DeleteComment(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // get comment by id from db
            var comment = await _appDbContext.Comments
                .Include(x => x.Owner).ThenInclude(x => x.Identity)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null)
            {
                return NotFound($"Comment with id ${id} not found.");
            }

            // check if customer is owner of comment
            var currentCustomer = await GetCurrentCustomer();
            if (!comment.Owner.Equals(currentCustomer))
            {
                return Unauthorized();
            }

            // get related post
            var post = await GetFullyResolvedPostsQuery()
                .FirstOrDefaultAsync(x => x.Comments.Contains(comment));

            post.Comments.Remove(comment);
            _appDbContext.Entries.Update(post);
            await _appDbContext.SaveChangesAsync();

            var response = _mapper.Map<PostData>(post);
            return new OkObjectResult(response);
        }

        /// <summary>
        /// Returns the activities for the authorized user posts
        /// </summary>
        /// <returns></returns>
        [HttpGet("activities")]
        public async Task<ActionResult<Dictionary<ActivityTimeRange, List<ActivityData>>>> GetActivities()
        {
            // get the current authenticated user
            var customer = await GetCurrentCustomer();

            // prepare data
            var query = GetFullyResolvedPostsQuery();
            var posts = await query.Where(x => x.Owner == customer).ToListAsync();

            var response = new Dictionary<ActivityTimeRange, List<ActivityData>>();
            var today = new List<ActivityData>();
            var month = new List<ActivityData>();

            #region Likes

            if (posts.SelectMany(x => x.Likes).Any())
            {
                #region Today

                today.AddRange(posts
                    .Where(post => post.Likes.Any(like => like.DateCreated.Date == DateTime.Today.Date))
                    .GroupBy(post => post.Id)
                    .Select(x =>
                        new ActivityData
                        {
                            Action = ActivityAction.Liked,
                            PostId = x.Key,
                            ImageUrl = "/" + posts.FirstOrDefault(post => post.Id == x.Key)?.Picture.Url,
                            Actors = x.ToList().SelectMany(y => y.Likes
                                .Where(li => li.DateCreated.Date == DateTime.Today.Date)
                                .Select(l => l.Customer.Identity.FirstName)).ToList(),
                            LastChange = x.ToList().SelectMany(y => y.Likes
                                .Where(li => li.DateCreated.Date == DateTime.Today.Date)
                                .Select(l => l.DateCreated)).OrderByDescending(y => y).FirstOrDefault()
                        }
                    ).ToList());

                #endregion

                #region Month

                month.AddRange(posts
                    .Where(post => post.Likes.Any(like =>
                        like.DateCreated.Date >= DateTime.Today.Date.Subtract(TimeSpan.FromDays(31)) &&
                        like.DateCreated.Date <= DateTime.Today.Date))
                    .GroupBy(post => post.Id)
                    .Select(x =>
                        new ActivityData
                        {
                            Action = ActivityAction.Liked,
                            PostId = x.Key,
                            ImageUrl = "/" + posts.FirstOrDefault(post => post.Id == x.Key)?.Picture.Url,
                            Actors = x.ToList().SelectMany(y => y.Likes
                                .Where(like =>
                                    like.DateCreated.Date >= DateTime.Today.Date.Subtract(TimeSpan.FromDays(31)) &&
                                    like.DateCreated.Date <= DateTime.Today.Date)
                                .Select(l => l.Customer.Identity.FirstName)).ToList(),
                            LastChange = x.ToList().SelectMany(y => y.Likes
                                .Where(like =>
                                    like.DateCreated.Date >= DateTime.Today.Date.Subtract(TimeSpan.FromDays(31)) &&
                                    like.DateCreated.Date <= DateTime.Today.Date)
                                .Select(l => l.DateCreated)).OrderByDescending(y => y).FirstOrDefault()
                        }
                    ).ToList());

                #endregion
            }

            #endregion

            #region Comments

            if (posts.SelectMany(x => x.Comments).Any())
            {
                #region Today

                today.AddRange(posts
                    .Where(post => post.Comments.Any(like => like.DateCreated.Date == DateTime.Today.Date))
                    .GroupBy(post => post.Id)
                    .Select(x =>
                        new ActivityData
                        {
                            Action = ActivityAction.Commented,
                            PostId = x.Key,
                            ImageUrl = "/" + posts.FirstOrDefault(post => post.Id == x.Key)?.Picture.Url,
                            Actors = x.ToList().SelectMany(y => y.Comments
                                .Where(li => li.DateCreated.Date == DateTime.Today.Date)
                                .Select(l => l.Owner.Identity.FirstName)).ToList(),
                            LastChange = x.ToList().SelectMany(y => y.Comments
                                .Where(li => li.DateCreated.Date == DateTime.Today.Date)
                                .Select(l => l.DateCreated)).OrderByDescending(y => y).FirstOrDefault()
                        }
                    ).ToList());

                #endregion

                #region Month

                month.AddRange(posts
                    .Where(post => post.Comments.Any(like => like.DateCreated.Date == DateTime.Today.Date))
                    .GroupBy(post => post.Id)
                    .Select(x =>
                        new ActivityData
                        {
                            Action = ActivityAction.Commented,
                            PostId = x.Key,
                            ImageUrl = "/" + posts.FirstOrDefault(post => post.Id == x.Key)?.Picture.Url,
                            Actors = x.ToList().SelectMany(y => y.Comments
                                .Where(comment =>
                                    comment.DateCreated.Date >= DateTime.Today.Date.Subtract(TimeSpan.FromDays(31)) &&
                                    comment.DateCreated.Date <= DateTime.Today.Date)
                                .Select(l => l.Owner.Identity.FirstName)).ToList(),
                            LastChange = x.ToList().SelectMany(y => y.Comments
                                .Where(comment =>
                                    comment.DateCreated.Date >= DateTime.Today.Date.Subtract(TimeSpan.FromDays(31)) &&
                                    comment.DateCreated.Date <= DateTime.Today.Date)
                                .Select(l => l.DateCreated)).OrderByDescending(y => y).FirstOrDefault()
                        }
                    ).ToList());

                #endregion
            }

            #endregion

            // postprocess 
            today = today.OrderByDescending(x => x.LastChange).ToList();
            response.Add(ActivityTimeRange.Today, today);
            month = month.OrderByDescending(x => x.LastChange).ToList();
            response.Add(ActivityTimeRange.Month, month);

            return response;
        }

        #endregion

        #region Methods

        private async Task<Customer> GetCurrentCustomer()
        {
            var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentCustomer = await _appDbContext.Customers
                .SingleAsync(c => c.Identity.UserName == currentUserName);
            return currentCustomer;
        }

        private string GetBaseUrl()
        {
            var baseUrl = string.Format("{0}://{1}{2}/", Request.Scheme, Request.Host.ToUriComponent(),
                Request.PathBase.ToUriComponent());

            return baseUrl;
        }

        private IIncludableQueryable<Entry, List<Comment>> GetFullyResolvedPostsQuery()
        {
            return _appDbContext.Entries
                .Include(e => e.Owner).ThenInclude(o => o.Identity)
                .Include(e => e.Picture)
                .Include(e => e.Likes).ThenInclude(l => l.Customer).ThenInclude(c => c.Identity)
                .Include(e => e.Comments);
        }

        #endregion
    }
}