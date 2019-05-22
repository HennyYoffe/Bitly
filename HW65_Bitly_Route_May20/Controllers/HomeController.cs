using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HW65_Bitly_Route_May20.Models;
using ClassLibrary1;
using Microsoft.Extensions.Configuration;
using shortid;

namespace HW65_Bitly_Route_May20.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ShortenUrl(string url)
        {
            var db = new BitlyManager(_connectionString);
            string hash = ShortId.Generate(7);
            while (db.CheckIfContainedHash(hash))
            {
                hash = ShortId.Generate(7);
            }
            Bitly bitly = new Bitly();
            bitly.Url = url;
            bitly.Hash = hash;
            if (User.Identity.IsAuthenticated)
            {
                User user = db.GetByEmail(User.Identity.Name);
                bitly.UserId = user.Id;
            }
            db.AddBitly(bitly);
            string h = "https://localhost:44372/"+hash;           
            return Json(h);
        }
        [Route("/{hash}")]
        public IActionResult GoToUrl(string hash)
        {
            var db = new BitlyManager(_connectionString);
            Bitly bitly = db.GetBitlyForHash(hash);
            db.AddView(bitly.Id, bitly.Views);           
            return Redirect(bitly.Url);
        }
        public IActionResult MyUrl()
        {
            var db = new BitlyManager(_connectionString);
            User user = db.GetByEmail(User.Identity.Name);
            List<Bitly> bitlies = db.GetBitlyForUser(user.Id);
            return View(bitlies);
        }
    }
}
//Create a simple URL Shortener.
//    The purpose of this application is to give users the ability to enter a long url, and be given a much smaller shorter URL,
//        that when accessed, will redirect the user to the original URL.


//Here's the basic functionality:


//On the home page, there should be a textbox and a button. 
//        When the button is clicked, via ajax, display the "shortened" url to the user beneath the textbox.
//        E.g. if your site is at "http://localhost:12345" and the user submits a url to be shortened, it should display something like: 
//        "http://localhost:12345/fjopUsS".


//There will also be a login system for this application.
//        The purpose of this is that if a logged in user enters a URL to be shortened, 
//we will give them a way to view the amount of times that shortened URL was accessed.
//        The way we'll do that is that for logged in users, there will be a link on top that says "My URLs" 
//        that when clicked, will take them to a page that will display all the URLs they've ever shortened,
//        as well as a number indicating how many views it got.


//Use the shortid library from Nuget to generate the short ids. Use attribute routing to enable the dynamic url part.

//Good luck!