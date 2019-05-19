using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace primerjanjeAvtomobilov.Controllers
{
    public class IskanjeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]  
        public IActionResult scraper(string url)  
        {  
            var Scaper = new Models.MyWebScraper();
            
            Scaper.scrapeData(url);
            return Index();
        } 
    }
}