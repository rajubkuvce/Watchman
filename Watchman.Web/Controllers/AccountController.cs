﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Watchman.BusinessLogic.Models.Data;
using Watchman.BusinessLogic.Models.Users;
using Watchman.BusinessLogic.Services;
using Watchman.Web.Models;

namespace Watchman.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITokenService tokenService;
        private readonly IUserManager<WatchmanUser, Guid> userManager;
        private readonly IJwtValidator jwtValidator;
        
        public AccountController(ITokenService tokenService, IUserManager<WatchmanUser, Guid> userManager, IJwtValidator jwtValidator)
        {
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.jwtValidator = jwtValidator;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            var token = await tokenService.GetTokenAsync(viewModel.Email, viewModel.Password);
            if (String.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError("", "Wrong credentials");
                return View(viewModel);
            }
            else
            {
                HttpContext.Response.Cookies.Append("access_token", token);                
                var claimsPricipal = jwtValidator.GetClaimsPrincipal(token);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPricipal);

                return RedirectToAction("Index", "Home");
            }            
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            try
            {
                PersonalInformation info = new PersonalInfo(viewModel);
                await userManager.RegisterAsync(info, viewModel.Password);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(viewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}