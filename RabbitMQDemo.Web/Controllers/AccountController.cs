using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
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
                    //,new Claim(ClaimTypes.Email,"emailaccount@microsoft.com")  
                }; 
         
                //init the identity instances 
                var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "SuperSecureLogin")); 
         
                //signin 
                await HttpContext.Authentication.SignInAsync("Cookie", userPrincipal, new AuthenticationProperties 
                { 
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20), 
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
            await HttpContext.Authentication.SignOutAsync("Cookie"); 
         
            return RedirectToAction("Index", "Home"); 
        } 
    }
}
