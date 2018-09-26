using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    [Authorize(Policy = "ApiUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class RobstagramController : Controller
    {
        #region Variables

        private readonly UserManager<AppUser> _userManager;
        private readonly ClaimsPrincipal _caller;
        private readonly ApplicationDbContext _appDbContext;
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly int _pageSize = 5;

        #endregion

        #region Constructors

        public RobstagramController(UserManager<AppUser> userManager, ApplicationDbContext appDbContext,
            IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _caller = httpContextAccessor.HttpContext.User;
            _appDbContext = appDbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        #endregion

        #region Profile

        // GET api/robstagram/profile
        [HttpGet("profile")]
        public async Task<ActionResult<ProfileData>> GetProfile()
        {
            // retrieve the user info of the current authenticated user
            //HttpContext.User
            var userId = _caller.Claims.Single(c => c.Type == "id");
            var customer = await _appDbContext.Customers.Include(c => c.Identity)
                .SingleAsync(c => c.Identity.Id == userId.Value);

            //return new OkObjectResult(new
            //{
            //    Message = "This is secure API and user data!",
            //    customer.Identity.FirstName,
            //    customer.Identity.LastName,
            //    customer.Identity.PictureUrl,
            //    customer.Identity.FacebookId,
            //    customer.Location,
            //    customer.Locale,
            //    customer.Gender
            //});

          return new OkObjectResult(new ProfileData
          {
            Message = "This is secure API and user data!",
            FirstName = customer.Identity.FirstName,
            LastName = customer.Identity.LastName,
            PictureUrl = customer.Identity.PictureUrl,
            FacebookId = customer.Identity.FacebookId.Value,
            Location = customer.Location,
            Locale = customer.Locale,
            Gender = customer.Gender
          });
    }

        #endregion

        #region Entries

        /// <summary>
        /// POST api/robstagram/entries
        /// Lets the Api user create a new entry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("entries")]
        public async Task<ActionResult<string>> PostEntry([FromBody]EntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MemoryStream ms = new MemoryStream();
            var fullPath = Path.Combine(_hostingEnvironment.WebRootPath, model.ImageUrl);
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                await stream.CopyToAsync(ms);
            }

            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            image.Resize(640, 640).Save(fullPath);

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

            // NOTE as we are using Bearer Token Auth the following code does not work
            //var currentUser = await _userManager.GetUserAsync(User);
            // instead we have to use the name identifier and look up the user by username
            var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var currentAppUser = await _userManager.FindByNameAsync(currentUserName);
            var currentCustomer = await _appDbContext.Customers
                .SingleAsync(c => c.Identity.UserName == currentUserName);

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
        /// GET api/robstagram/entries
        /// Lets the Api user receive all Entry objects
        /// </summary>
        /// <returns></returns>
        [HttpGet("entries")]
        public async Task<ActionResult<List<PostData>>> GetEntries(int page)
        {
            //var baseUrl = string.Format("http://192.168.0.59:12345/", Request.Scheme, Request.Host.ToUriComponent(),
            //    Request.PathBase.ToUriComponent());
            var baseUrl = string.Format("{0}://{1}{2}/", Request.Scheme, Request.Host.ToUriComponent(),
                Request.PathBase.ToUriComponent());

            var entries = await _appDbContext.Entries
                .Include(e => e.Owner).ThenInclude(o => o.Identity)
                .Include(e => e.Picture)
                .Include(e => e.Likes).ThenInclude(l => l.Customer).ThenInclude(c => c.Identity)
                .Include(e => e.Comments)
                .OrderByDescending(e => e.DateCreated)
                .Skip((page-1)*_pageSize)
                .Take(_pageSize)
                .ToListAsync();

            //var response = entries.Select(e => new
            //{
            //    id = e.Id,
            //    owner = e.Owner.Identity.FirstName,
            //    imageUrl = baseUrl + e.Picture.Url,
            //    description = e.Description,
            //    likes = e.Likes.Select(l => l.Customer.Identity.FirstName).ToList(),
            //    comments = e.Comments.ToList(),
            //    created = e.DateCreated
            //}).OrderByDescending(x => x.created).ToList();

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
        /// GET api/robstagram/entries/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("entries/{id}")]
        public async Task<ActionResult<PostData>> GetEntry(int id)
        {
          // var baseUrl = string.Format("http://192.168.0.59:12345/", Request.Scheme, Request.Host.ToUriComponent(),
          //       Request.PathBase.ToUriComponent());
          var baseUrl = string.Format("{0}://{1}{2}/", Request.Scheme, Request.Host.ToUriComponent(),
              Request.PathBase.ToUriComponent());

          var entries = await _appDbContext.Entries
            .Include(e => e.Owner).ThenInclude(o => o.Identity)
            .Include(e => e.Picture)
            .Include(e => e.Likes).ThenInclude(l => l.Customer).ThenInclude(c => c.Identity)
            .Include(e => e.Comments)
            .ToListAsync();

          var entry = entries.FirstOrDefault(e => e.Id == id);
          if (entry == null)
          {
            return NotFound(id);
          }

          //var response = new
          //{
          //  id = entry.Id,
          //  owner = entry.Owner.Identity.FirstName,
          //  imageUrl = baseUrl + entry.Picture.Url,
          //  description = entry.Description,
          //  likes = entry.Likes.Select(l => l.Customer.Identity.FirstName).ToList(),
          //  comments = entry.Comments.ToList(),
          //  created = entry.DateCreated
          //};

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
        /// POST api/robstagram/entries/{id}/likes
        /// </summary>
        /// <returns></returns>
        [HttpPost("entries/{id}/likes")]
        public async Task<ActionResult<PostData>> PostLike(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entries = await _appDbContext.Entries
                .Include(e => e.Likes)
                .Include(e => e.Comments)
                .Include(e => e.Owner).ThenInclude(o => o.Identity)
                .Include(e => e.Picture)
                .ToListAsync();

            //var entriesWithLikes = entries.Where(e => e.Likes != null && e.Likes.Count > 0).ToList();
            var post = entries.FirstOrDefault(e => e.Id == id);
                //.FindAsync(id);

            if (post == null)
            {
                return NotFound($"Entry with id ${id} not found.");
            }

            var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentCustomer = await _appDbContext.Customers
                .SingleAsync(c => c.Identity.UserName == currentUserName);

            if (post.Likes == null)
            {
                post.Likes = new List<Like>();
            }

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

            var baseUrl = string.Format("{0}://{1}{2}/", Request.Scheme, Request.Host.ToUriComponent(),
              Request.PathBase.ToUriComponent());

            //var response = new
            //{
            //    id = post.Id,
            //    owner = post.Owner.Identity.FirstName,
            //    imageUrl = baseUrl + post.Picture.Url,
            //    description = post.Description,
            //    likes = post.Likes.Select(l => l.Customer.Identity.FirstName).ToList(),
            //    comments = post.Comments.ToList(),
            //    created = post.DateCreated
            //};

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
    }
}