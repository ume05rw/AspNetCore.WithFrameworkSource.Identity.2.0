using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using WithSource.Identity.Models;
using WithSource.Identity.Models.Api.ErrorResponse;
using Newtonsoft.Json;

namespace WithSource.Identity.Controllers
{
    public class AuthController : Controller
    {
        private Models.Auth _auth;

        public AuthController(UserManager<User> userManager,
                              SignInManager<User> signInManager)
        {
            this._auth = new Auth(userManager, signInManager);
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return this.RedirectToAction("Login");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if (this.Request.Method.ToUpper() == "POST")
            {
                //postbacked
                var dictionary = new Dictionary<string, string>();
                dictionary.Add("LoginName", this.Request.Form["LoginName"]);
                dictionary.Add("Password", this.Request.Form["Password"]);
                var result = await this._auth.SignInAsync(dictionary);
                this.ViewData["LoginResult"] = result;
            }
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await this._auth.SignOutAsync();
            return this.RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        
        [Authorize]
        public IActionResult MemberOnly()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Api()
        {
            try
            {
                var queryJson = this.Request.Query
                                            .Select(q => q.Key)
                                            .FirstOrDefault(k => k.IndexOf('{') >= 0
                                                                 && k.IndexOf('}') >= 0);

                var bodyString = Xb.Str.GetString(this.Request.Body);
                var passingString = !string.IsNullOrEmpty(bodyString)
                    ? bodyString
                    : Xb.Net.Http.DecodeUri(queryJson);
                
                Dictionary<string, string> dictionary;
                try
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(passingString);
                }
                catch (Exception)
                {
                    return Json(new ParseErrorResponse(bodyString));
                }

                switch (this.Request.Method.ToUpper())
                {
                    case "GET":
                        return Json(await this._auth.SignInAsync(dictionary));

                    case "POST":
                    case "PUT":
                    case "PATCH":

                        return Json(await this._auth.CreateAsync(dictionary));

                    case "DELETE":
                        return Json(await this._auth.SignOutAsync());

                    case "HEAD":
                    case "CONNECT":
                    case "OPTIONS":
                    case "TRACE":
                    default:
                        throw new NotImplementedException($"Undefineded method: {this.Request.Method}");
                }
            }
            catch (Exception ex)
            {
                return Json(new UnexpectedErrorResponse(ex));
            }
        }
    }
}
