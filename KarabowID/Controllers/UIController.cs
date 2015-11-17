using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KarabowID.Controllers;
using System.Web.Http;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Web.Http.Routing;

namespace KarabowID.Controllers
{
   [System.Web.Http.RoutePrefix("UI")]
    public class UIController : Controller
    {
        //
        // GET: /UI/
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Register()
        {
            return View();
        }

        
        public ActionResult Login()
        {
            ViewBag.Title = "Login";
            return View();
        }

        
        public ActionResult ApiCall()
        {
            return View();
        }

        //[System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Email_Confirmation", Name = "EmailActivate")]
        public ActionResult Email_Confirmation(string userId, string code)
        {
           //using (var client = new HttpClient())
           // {
           //    // //client.BaseAddress = new Uri("http://localhost:20204/");
           //    // client.DefaultRequestHeaders.Accept.Clear();
           //    // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
           //    // //code.Replace("%2","+");
           //    //string uri = "http://localhost:20204/api/accounts/ConfirmEmail?userId=" + userId + "&code=" + code;
           //    // //var callbackUrl = new Uri("http://localhost:20204/api/accounts/ConfirmEmail?userId=" + userId + "&code=" + code);
           //    //// var callbackUrl = new Uri(Url.Encode(uri));
           //    // HttpResponseMessage response = client.GetAsync(uri).Result;
           //     if (response.IsSuccessStatusCode)
           //     {
           //         return View();
           //     }
           //    else
           //     {
           //         ViewBag.Message = response.ReasonPhrase + code;
           //         return View();
           //     }

           //}

            return View();
        }
	}

    //public abstract class face
    //{
    //     public void age()
    //    {
    //        Console.WriteLine("");
    //    }
    //  //  protected int age;
    //}
}