using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace primerjanjeAvtomobilov.Models
{
    public class Avtomobili
    {
        /* Ime avta
        Letnik
        Tehnični pregled
        Prevoženi km
        Barva
        Cena
        Gorivo
        Oblika
        Slika*/

        public int idAvto { get; set; }

        public string letnikAvto { get; set; }

        public string nazivAvta { get; set; }

        public string tehnicniPregled { get; set; }

        public string prevozeniKilometri { get; set; }

        public string barva { get; set; }

        public string cena { get; set; }

        public string vrstaGoriva { get; set; }

        public string oblikaAvta { get; set; }

        public string slikaAvta { get; set; }

        public List<Avtomobili> avtomobiliList = new List<Avtomobili>();
    }
}
