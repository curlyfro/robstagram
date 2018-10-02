using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using robstagram.Data;
using robstagram.Helpers;
using robstagram.Models.Entities;
using robstagram.ViewModels;

namespace robstagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {
        #region Variables

        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly ClaimsPrincipal _caller;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public AccountsController(UserManager<AppUser> userManager, IMapper mapper, ApplicationDbContext appDbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _caller = httpContextAccessor.HttpContext.User;
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        #endregion

        #region Api

        /// <summary>
        /// Register a new user for the app
        /// </summary>
        /// <remarks>
        /// Creates a new user in database
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // create identity user
            var userIdentity = _mapper.Map<AppUser>(model);
            var result = await _userManager.CreateAsync(userIdentity, model.Password);
            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            // create customer user object in db
            await _appDbContext.Customers.AddAsync(new Customer
            {
                IdentityId = userIdentity.Id,
                Location = model.Location
            });
            await _appDbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }

        /// <summary>
        /// Get profile information for current user
        /// </summary>
        /// <remarks>
        /// Returns profile information for current authenticated user
        /// </remarks>
        /// <returns></returns>
        [HttpGet("profile")]
        public async Task<ActionResult<ProfileData>> GetProfile()
        {
            // retrieve the user info of the current authenticated user
            var userId = _caller.Claims.Single(c => c.Type == "id");
            var customer = await _appDbContext.Customers.Include(c => c.Identity)
                .SingleAsync(c => c.Identity.Id == userId.Value);

            return new OkObjectResult(new ProfileData
            {
                Message = "This is secure API and user data!",
                FirstName = customer.Identity.FirstName,
                LastName = customer.Identity.LastName,
                PictureUrl = customer.Identity.PictureUrl,
                //FacebookId = customer.Identity.FacebookId.Value,
                Location = customer.Location,
                Locale = customer.Locale,
                Gender = customer.Gender
            });
        }

        #endregion
    }
}