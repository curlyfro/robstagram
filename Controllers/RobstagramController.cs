using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    public class RobstagramController : Controller
    {
        #region Variables
        private readonly UserManager<AppUser> _userManager;
        private readonly ClaimsPrincipal _caller;
        private readonly ApplicationDbContext _appDbContext;
        private readonly IHostingEnvironment _hostingEnvironment;
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
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // retrieve the user info of the current authenticated user
            //HttpContext.User
            var userId = _caller.Claims.Single(c => c.Type == "id");
            var customer = await _appDbContext.Customers.Include(c => c.Identity)
                .SingleAsync(c => c.Identity.Id == userId.Value);

            return new OkObjectResult(new
            {
                Message = "This is secure API and user data!",
                customer.Identity.FirstName,
                customer.Identity.LastName,
                customer.Identity.PictureUrl,
                customer.Identity.FacebookId,
                customer.Location,
                customer.Locale,
                customer.Gender
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
        [HttpPost]
        public async Task<IActionResult> Entries([FromForm]EntryViewModel model)
        {
            if (!ModelState.IsValid || model.Image == null || !model.Image.ContentType.ToLower().StartsWith("image/"))
            {
                return BadRequest(ModelState);
            }

            var file = model.Image;
            var uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, Configuration.UploadFolder);
            var filePath = Path.Combine(uploadFolder, Path.GetRandomFileName());
            filePath = Path.ChangeExtension(filePath, "jpg");
            var relativePath = Path.GetRelativePath(_hostingEnvironment.WebRootPath, filePath);
            var fileSize = file.Length;

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            MemoryStream ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            image.Resize(640, 640).Save(filePath);

            Models.Entities.Image imageEntity = new Image()
            {
                Name = file.FileName,
                Url = relativePath,
                Data = null,
                Size = fileSize,
                Width = image.Width,
                Height = image.Height,
                ContentType = file.ContentType
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
        [HttpGet]
        public async Task<IActionResult> Entries()
        {
            var entries = await _appDbContext.Entries
                .Include(e => e.Owner).ThenInclude(o => o.Identity)
                .Include(e => e.Picture)
                .Include(e => e.Likes)
                .Include(e => e.Comments)
                .ToListAsync();

            var response = entries.Select(e => new
            {
                owner =  e.Owner.Identity.FirstName,
                imageUrl = e.Picture.Url,
                description = e.Description,
                likes = e.Likes.ToList(),
                comments = e.Comments.ToList()
            }).ToList();

            return new OkObjectResult(response);
        }

        #endregion
    }
}