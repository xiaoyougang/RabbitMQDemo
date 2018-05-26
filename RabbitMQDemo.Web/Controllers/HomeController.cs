using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQDemo.Web.Models;
using RabbitMQDemo.Service;
using Microsoft.Extensions.Options;

namespace RabbitMQDemo.Web.Controllers
{
    public class HomeController : Controller
    {
        public RabbitmqSetting RabbitmqConnInfo;
        public HomeController(IOptions<RabbitmqSetting> rmqSetting)
        {
            RabbitmqConnInfo = rmqSetting.Value;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        public IActionResult AuthPage()
        {
            var userName = HttpContext.User.Identity.Name;
            ViewBag.UserName = userName;
            return View();
        }

        public IActionResult Publish(string message, string etype)
        {
            QueueServiceBroker.Produce(message, etype, RabbitmqConnInfo);
            return null;
        }

        public IActionResult Receive(string username)
        {
            var message = QueueServiceBroker.Consume(username, RabbitmqConnInfo);
            return Content(message);
        }
    }
}
