using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using primerjanjeAvtomobilov.Models;
using System.Threading;

namespace primerjanjeAvtomobilov.Controllers
{
    public class IskanjeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]  
        public IActionResult scraper(string url, string enAvto)  
        {  
            var Scaper = new Models.MyWebScraper();
            
            Scaper.scrapeData(url).Wait();
            //Thread.Sleep(5000);
            //Scaper.GetAvto();
            //
            //TODO: double primerjava = HOG.compare() ... ozadje 
            string oblikaTabele = "<table><thead>" +
                "<tr><th>Naziv</th><th>Cena</th>" +
                "<th>Tehnicni pregled</th><th>Prevozeni kilometri</th>" +
                "<th>Barva</th><th>Vrsta motora</th>" +
                "<th>Oblika vozila</th></tr>" +
                "</thead><tbody>"; 
            foreach(var item in MyWebScraper.listAvti){
                oblikaTabele += "<tr><td>" + item.nazivAvta + "</td>";
                oblikaTabele += "<td>" + item.cena + "</td>";
                oblikaTabele += "<td>" + item.tehnicniPregled + "</td>";
                oblikaTabele += "<td>" + item.prevozeniKilometri + "</td>";
                oblikaTabele += "<td>" + item.barva + "</td>";
                oblikaTabele += "<td>" + item.vrstaGoriva + "</td>";
                oblikaTabele += "<td>" + item.oblikaAvta + "</td>";
                oblikaTabele += "<td>" + item.slikaAvta + "</td>";
                oblikaTabele += "</tr>";
            }
            oblikaTabele += "</tbody></table>";
            TempData["oblika"] = oblikaTabele;//MyWebScraper.listAvti;
            MyWebScraper.listAvti.Clear();
            return View("Index");
        } 
    }
}
