using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RabbitMQDemo.Web.Models;

namespace RabbitMQDemo.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet] 
        public IActionResult Login() 
        { 
            return View(); 
        } 
         
        [HttpPost] 
        public async Task<IActionResult> Login(User userFromFore) 
        { 
            var userFromStorage = TestUserStorage.UserList 
                .FirstOrDefault(m => m.UserName == userFromFore.UserName && m.Password == userFromFore.Password); 
         
            if (userFromStorage != null) 
            { 
                //you can add all of ClaimTypes in this collection 
                var claims = new List<Claim>() 
                { 
                    new Claim(ClaimTypes.Name,userFromStorage.UserName)
                }; 
         
                //init the identity instances 
                var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)); 
         
                //signin 
                await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties 
                { 
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(10), 
                    IsPersistent = false, 
                    AllowRefresh = false 
                }); 

         
                return RedirectToAction("Index", "Home"); 
            } 
            else 
            { 
                ViewBag.ErrMsg = "UserName or Password is invalid"; 
         
                return View(); 
            } 
        } 
         
        public async Task<IActionResult> Logout() 
        { 
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
         
            return RedirectToAction("Index", "Home"); 
        } 
    }
}
