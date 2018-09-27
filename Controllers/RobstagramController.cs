using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using robstagram.Data;
using robstagram.Extensions;
using robstagram.Helpers;
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

        private readonly int _pageSize = 5;

        #endregion

        #region Constructors

        public RobstagramController(ApplicationDbContext appDbContext, IHostingEnvironment hostingEnvironment)
        {
            _appDbContext = appDbContext;
            _hostingEnvironment = hostingEnvironment;
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
            Models.Entities.Image imageEntity = new Image()
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
            // get base url to return relative image url paths for posts
            var baseUrl = string.Format("{0}://{1}{2}/", Request.Scheme, Request.Host.ToUriComponent(),
                Request.PathBase.ToUriComponent());

            // get posts from database
            var entries = forUser.Value
                ? await _appDbContext.Entries
                    .Include(e => e.Owner).ThenInclude(o => o.Identity)
                    .Include(e => e.Picture)
                    .Include(e => e.Likes).ThenInclude(l => l.Customer).ThenInclude(c => c.Identity)
                    .Include(e => e.Comments)
                    .Where(e => e.Owner == GetCurrentCustomer().Result)
                    .OrderByDescending(e => e.DateCreated)
                    .Skip((page - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToListAsync()
                : await _appDbContext.Entries
                    .Include(e => e.Owner).ThenInclude(o => o.Identity)
                    .Include(e => e.Picture)
                    .Include(e => e.Likes).ThenInclude(l => l.Customer).ThenInclude(c => c.Identity)
                    .Include(e => e.Comments)
                    .OrderByDescending(e => e.DateCreated)
                    .Skip((page - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToListAsync();

            var response = entries.Select(e => new PostData
            {
                Id = e.Id,
                Owner = e.Owner.Identity.FirstName,
                ImageUrl = baseUrl + e.Picture.Url,
                Description = e.Description,
                Likes = e.Likes.Select(l => l.Customer.Identity.FirstName).ToList(),
                Comments = e.Comments.Select(c => c.Text).ToList(),
                DateCreated = e.DateCreated
            }).OrderByDescending(x => x.DateCreated).ToList();

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
            // get base url to return relative image url paths for posts
            var baseUrl = string.Format("{0}://{1}{2}/", Request.Scheme, Request.Host.ToUriComponent(),
                Request.PathBase.ToUriComponent());

            // fetch post by id from db
            var entry = await _appDbContext.Entries
                .Include(e => e.Owner).ThenInclude(o => o.Identity)
                .Include(e => e.Picture)
                .Include(e => e.Likes).ThenInclude(l => l.Customer).ThenInclude(c => c.Identity)
                .Include(e => e.Comments)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null)
            {
                return NotFound(id);
            }

            var response = new PostData
            {
                Id = entry.Id,
                Owner = entry.Owner.Identity.FirstName,
                ImageUrl = baseUrl + entry.Picture.Url,
                Description = entry.Description,
                Likes = entry.Likes.Select(l => l.Customer.Identity.FirstName).ToList(),
                Comments = entry.Comments.Select(c => c.Text).ToList(),
                DateCreated = entry.DateCreated
            };

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
            var post = await _appDbContext.Entries
                .Include(e => e.Likes)
                .Include(e => e.Comments)
                .Include(e => e.Owner).ThenInclude(o => o.Identity)
                .Include(e => e.Picture)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (post == null)
            {
                return NotFound($"Entry with id ${id} not found.");
            }

            // like/unlike this post for current user
            var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentCustomer = await _appDbContext.Customers
                .SingleAsync(c => c.Identity.UserName == currentUserName);

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

            // get base url to return relative image path for post
            var baseUrl = string.Format("{0}://{1}{2}/", Request.Scheme, Request.Host.ToUriComponent(),
                Request.PathBase.ToUriComponent());

            var response = new PostData
            {
                Id = post.Id,
                Owner = post.Owner.Identity.FirstName,
                ImageUrl = baseUrl + post.Picture.Url,
                Description = post.Description,
                Likes = post.Likes.Select(l => l.Customer.Identity.FirstName).ToList(),
                Comments = post.Comments.Select(c => c.Text).ToList(),
                DateCreated = post.DateCreated
            };

            return new OkObjectResult(response);
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

        #endregion
    }
}