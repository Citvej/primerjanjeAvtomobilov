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

        public string nazivAvta { get; set; }

        public string prevozeniKm { get; set; }
		
		public string menjalnik { get; set; }
		
		public string motor { get; set; }

        public string cena { get; set; }

        public string slika { get; set; }

        public List<Avtomobili> avtomobiliList = new List<Avtomobili>();
    }
}
