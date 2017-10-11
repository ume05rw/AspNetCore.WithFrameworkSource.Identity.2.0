using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace WithSource.Identity.Models
{
    public class Auth
    {
        public UserManager<User> UserManager { get; protected set; }
        public SignInManager<User> SignInManager { get; protected set; }

        public Auth(UserManager<User> userManager,
                    SignInManager<User> signInManager)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }

        public async Task<Api.Response> CreateAsync(Dictionary<string, string> dictionary)
        {
            var response = new Api.Response();
            var errList = response.Errors;

            if (!dictionary.ContainsKey("LoginName")
                || string.IsNullOrEmpty(dictionary["LoginName"]))
                errList.Add(new Api.Error()
                {
                    Item = "LoginName",
                    Code = Api.ErrorCode.ItemNotFound,
                    Message = "LoginName not found."
                });

            if (!dictionary.ContainsKey("Password")
                || string.IsNullOrEmpty(dictionary["Password"]))
                errList.Add(new Api.Error()
                {
                    Item = "Password",
                    Code = Api.ErrorCode.ItemNotFound,
                    Message = "Password not found"
                });

            if (!dictionary.ContainsKey("PasswordRetype")
                || string.IsNullOrEmpty(dictionary["PasswordRetype"]))
                errList.Add(new Api.Error()
                {
                    Item = "PasswordRetype",
                    Code = Api.ErrorCode.ItemNotFound,
                    Message = "PasswordRetype not found"
                });

            //項目が一つでも不足しているとき、エラー応答。
            if (errList.Count > 0)
                return response;

            if (dictionary["Password"] != dictionary["PasswordRetype"])
            {
                errList.Add(new Api.Error()
                {
                    Item = "PasswordRetype",
                    Code = Api.ErrorCode.ValueNotSame,
                    Message = "Password and PasswordRetype value not same."
                });
                return response;
            }

            var loginName = dictionary["LoginName"];
            var password = dictionary["Password"];
            var user = new User()
            {
                LoginName = loginName,
                NormalizedLoginName = this.UserManager.NormalizeKey(loginName),
                ScreenName = loginName,
            };
            var result = await this.UserManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                errList.AddRange(result.Errors.Select(e => new Api.Error()
                {
                    Item = "",
                    //Code = e.Code,
                    Message = e.Description
                }));

                return response;
            }

            response.Succeeded = true;
            return response;
        }


        public async Task<Api.Response> SignInAsync(Dictionary<string, string> dictionary)
        {
            var response = new Api.Response();
            var errList = response.Errors;

            if (!dictionary.ContainsKey("LoginName")
                || string.IsNullOrEmpty(dictionary["LoginName"]))
                errList.Add(new Api.Error()
                {
                    Item = "LoginName",
                    Code = Api.ErrorCode.ItemNotFound,
                    Message = "LoginName not found."
                });

            if (!dictionary.ContainsKey("Password")
                || string.IsNullOrEmpty(dictionary["Password"]))
                errList.Add(new Api.Error()
                {
                    Item = "Password",
                    Code = Api.ErrorCode.ItemNotFound,
                    Message = "Password not found"
                });

            //項目が一つでも不足しているとき、エラー応答。
            if (errList.Count > 0)
                return response;

            var userName = dictionary["LoginName"];

            var result = await this.SignInManager.PasswordSignInAsync(
                userName,
                dictionary["Password"], 
                false, 
                false
            );

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    errList.Add(new Api.Error() {
                        Item = "",
                        Code = Api.ErrorCode.Lockouted,
                        Message = "Account locked",
                    });

                if (result.IsNotAllowed)
                    errList.Add(new Api.Error()
                    {
                        Item = "",
                        Code = Api.ErrorCode.NotAllowed,
                        Message = "Not Allowd",
                    });

                if (result.RequiresTwoFactor)
                    errList.Add(new Api.Error()
                    {
                        Item = "",
                        Code = Api.ErrorCode.RequiresTwoFactor,
                        Message = "Required 2-Factor Auth",
                    });

                return response;
            }

            response.Succeeded = true;
            return response;
        }


        public async Task<Api.Response> SignOutAsync()
        {
            var response = new Api.Response();
            var errList = response.Errors;

            await this.SignInManager.SignOutAsync();

            response.Succeeded = true;
            return response;
        }
    }
}
