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

        public Avtomobili(int idAvto, string nazivAvta, string prevozeniKm, string menjalnik, string motor, string cena, string slika)
        {
            this.idAvto = idAvto;
            this.nazivAvta = nazivAvta;
            this.prevozeniKm = prevozeniKm;
            this.menjalnik = menjalnik;
            this.motor = motor;
            this.cena = cena;
            this.slika = slika;
        }

        public List<Avtomobili> avtomobiliList = new List<Avtomobili>();
    }
}
