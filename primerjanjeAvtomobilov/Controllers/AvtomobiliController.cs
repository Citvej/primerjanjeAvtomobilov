using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using primerjanjeAvtomobilov.Models;

namespace primerjanjeAvtomobilov.Controllers
{
    public class AvtomobiliController : Controller
    {
        public static List<Avtomobili> avtomobiliList = new List<Avtomobili>();

        public IActionResult Index()
        {
            return View();
        }

        /* Testni podatki */
        public IActionResult GetAvtomobilPodatki()
        {

            Avtomobili avto = new Avtomobili();
            //https://www.avto.net/Ads/details.asp?id=14235566
            avto.idAvto = 1;
            avto.nazivAvta = "Audi A4 Avant 2.0 TDI Sport S tronic VIRTUALštevciPANORAMA";
            avto.letnikAvto = 2016;
            avto.oblikaAvta = "karavan, 5 vr.";
            avto.slikaAvta = "https://images.avto.net/photo/14235566/1054430.jpg";
            avto.tehnicniPregled = "1/2020";
            avto.vrstaGoriva = "diesel motor";
            avto.cena = 22.699;
            avto.barva = "siva metalik";
            avto.prevozeniKilometri = 169896;

            Avtomobili avto1 = new Avtomobili();
            //https://www.avto.net/Ads/details.asp?id=14235399&display=Volkswagen%20Golf%20Ocean%201.9%20TDI
            avto1.idAvto = 2;
            avto1.nazivAvta = "Volkswagen Golf Ocean 1.9 TDI";
            avto1.letnikAvto = 2003;
            avto1.oblikaAvta = "kombilimuzina / hatchback, 5 vr.";
            avto1.slikaAvta = "https://images.avto.net/photo/14235399/1049960.jpg";
            avto1.tehnicniPregled = "5/2020";
            avto1.vrstaGoriva = "diesel motor";
            avto1.cena = 1.700;
            avto1.barva = "Nan";
            avto1.prevozeniKilometri = 284000;


            avtomobiliList.Add(avto);
            avtomobiliList.Add(avto1);

            return View(avtomobiliList);
        }

    }
}