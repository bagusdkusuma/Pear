using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.User;
using DSLNG.PEAR.Web.DependencyResolution;
using DSLNG.PEAR.Web.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DSLNG.PEAR.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string message)
        {
            ViewBag.message = message;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserLoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (IsValid(user.Email, user.Password))
                {
                    //FormsAuthentication.SetAuthCookie(user.Username, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect Login Data");
                }

            }
            else
            {
                ModelState.AddModelError("", "Incorrect Login Credential");
            }
            return View(user);
        }

        private bool IsValid(string email, string password)
        {
            var user = _userService.Login(new LoginUserRequest { Email = email, Password = password });
            if (user != null)
            {
                /* Try Get Current User Role
                 */
                //this._createRole(user.RoleName);
                //this._userAddToRole(user.Username, user.RoleName);
                FormsAuthentication.SetAuthCookie(user.Username, false);
                return user.IsSuccess;
            }
            return false;
        }

        private void _userAddToRole(string Username, string RoleName)
        {
            if (!Roles.IsUserInRole(Username, RoleName))
            {
                Roles.AddUserToRole(Username, RoleName);
            }
        }

        private void _createRole(string rolename)
        {
            if (!Roles.RoleExists(rolename))
            {
                Roles.CreateRole(rolename);
            }
        }
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

    }
}