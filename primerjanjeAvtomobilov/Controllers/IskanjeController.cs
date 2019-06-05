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
            var prepoznava = new RazpoznavaAvtomobila();
            string pathGlavnegaAvta = "";
            string pathAvtaKiGaPrimerjamo = "";
            double rezultatPrimerjave;

            Scaper.scrapeData(url).Wait();
            //Thread.Sleep(5000);
            Scaper.GetAvto(enAvto).Wait();

            pathGlavnegaAvta = Scaper.avto.slikaAvta;
            Console.WriteLine("PathGlavnegaAvta" + Scaper.ComputeSha256Hash(pathGlavnegaAvta));
            pathGlavnegaAvta = "C:/temp/"+Scaper.ComputeSha256Hash(pathGlavnegaAvta)+".png";


            string oblikaTabele = "<table><thead>" +
                "<tr><th>Naziv</th><th>Cena</th>" +
                "<th>Registracija</th>" +
                "<th>Podobnost avtomobilov</th>" +
                "<th>Slika</th>" +
                "</tr></thead><tbody>"; 
            foreach(var item in Scaper.listAvti){
                pathAvtaKiGaPrimerjamo = item.slikaAvta;
                pathAvtaKiGaPrimerjamo = "C:/temp/" + Scaper.ComputeSha256Hash(pathAvtaKiGaPrimerjamo) + ".png";
                rezultatPrimerjave = prepoznava.compare(pathGlavnegaAvta, pathAvtaKiGaPrimerjamo);
                oblikaTabele += "<tr><td>" + item.nazivAvta + "</td>";
                oblikaTabele += "<td>" + item.cena + " €</td>";
                oblikaTabele += "<td>" + item.letnikAvto + "</td>";
                oblikaTabele += "<td>" + (rezultatPrimerjave > 0.5 ? "Da" : "Ne") + "</td>";
                oblikaTabele += "<td><img src=" + item.slikaAvta + " /></td>";
                oblikaTabele += "</tr>";
            }
            oblikaTabele += "</tbody></table>";
            TempData["oblika"] = oblikaTabele;//MyWebScraper.listAvti;
            Scaper.listAvti.Clear();
            return View("Index");
        } 
    }
}
