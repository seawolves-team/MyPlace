﻿
namespace MyPlace.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Authorization;
    using MyPlace.Common;
    using MyPlace.DataModels;
    using MyPlace.Models.Account;
    using MyPlace.Services.Contracts;
    using MyPlace.Areas.Administrator.Models;
    using MyPlace.Infrastructure.Logger;
    using System.IO;

    [Area("Administrator")]
    [Authorize(Roles = GlobalConstants.AdminRole)]
    public class AdminController : Controller
    {
        private readonly SignInManager<User> _signIn;
        private readonly IAdminService _adminService;
        private readonly IDatabaseLogger _logger;
        private readonly IHostingEnvironment _enviroment;

        public AdminController(SignInManager<User> signIn, IAdminService adminService, IDatabaseLogger logger, IHostingEnvironment env)
        {
            _signIn = signIn;
            _adminService = adminService;
            _logger = logger;
            _enviroment = env;
        }


        public IActionResult Index() => View();


        [HttpGet]
        public IActionResult Activity() =>
            View(new ActivityIndexModel { ActivityList = _adminService.GetActivity<ActivityListingModel>().Result });

        
        [HttpGet]
        public IActionResult CreateEntity() => View();


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = GlobalConstants.AdminRole)]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            //await _adminService.EditCommentAsync(entityId, commentId, newComment);
            await _logger
                .Type(type => type.Type = GlobalConstants.INFO)
                .Log($"{this.User.Identity.AuthenticationType} {this.User.Identity.Name.ToUpper()} created a new Entity.");
            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateEntity(CreateEntityBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var file = _enviroment.WebRootPath + "/images/" + model.Title;
                using ( var fileStream = new FileStream(file, FileMode.Create))
                {
                    await model.ImageUrl.CopyToAsync(fileStream);
                }
                await _adminService.CreateEntityAsync(model.Title, model.Address, model.Description, model.ImageUrl.ToString());

                await _logger
                    .Type(type => type.Type = GlobalConstants.INFO)
                    .Log($"Administrator {this.User.Identity.Name.ToUpper()} created a new Entity.");
            }
            return View();
        }


        [HttpGet]
        public IActionResult CreateUser() => View();


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateUser([FromForm]RegisterBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User() { UserName = model.UserName };

                IdentityResult irUser = await _signIn.UserManager.CreateAsync(user, model.Password);

                if (irUser.Succeeded)
                {
                    await _signIn.UserManager.AddToRoleAsync(user, model.Role);
                    await _logger
                        .Type(type => type.Type = GlobalConstants.INFO)
                        .Log($"Administrator {this.User.Identity.Name.ToUpper()} created a new account with role {model.Role}.");

                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                return View("Register");
            }
            return View(model);
        }


        public async Task<IActionResult> ChangePassword()
        {
            await _logger
                .Type(type => type.Type = GlobalConstants.INFO)
                .Log($"Administrator {this.User.Identity.Name.ToUpper()} change password for user    .");

            return View();
        }


        public async Task<IActionResult> Delete(int entityId, int commentId)
        {
            await _adminService.DeleteAsync(entityId, commentId);

            await _logger
                .Type(type => type.Type = GlobalConstants.INFO)
                .Log($"Administrator {this.User.Identity.Name.ToUpper()} delete comment.");

            return RedirectToAction("Establishment", "Catalog", new { area = "", id = entityId });
        }


        public async Task<JsonResult> CheckUsernameAvailability(string name)
        {
            var registeredUsers = await _adminService.RegisteredUsers();
            return Json(registeredUsers.Contains(name));
        }
    }
}
