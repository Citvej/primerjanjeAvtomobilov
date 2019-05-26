using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace primerjanjeAvtomobilov.Models
{
    public class MyWebScraper
    {
        // url: https://www.avto.net/Ads/results.asp?znamka=Renault&model=Clio&modelID=&tip=katerikoli%20tip&znamka2=&model2=&tip2=katerikoli%20tip&znamka3=&model3=&tip3=katerikoli%20tip&cenaMin=0&cenaMax=999999&letnikMin=0&letnikMax=2090&bencin=0&starost2=999&oblika=0&ccmMin=0&ccmMax=99999&mocMin=&mocMax=&kmMin=0&kmMax=150000&kwMin=0&kwMax=999&motortakt=&motorvalji=&lokacija=0&sirina=&dolzina=&dolzinaMIN=&dolzinaMAX=&nosilnostMIN=&nosilnostMAX=&lezisc=&presek=&premer=&col=&vijakov=&EToznaka=&vozilo=&airbag=&barva=&barvaint=&EQ1=1000000000&EQ2=1000000000&EQ3=1000000000&EQ4=100000000&EQ5=1000000000&EQ6=1000000000&EQ7=1110100120&EQ8=1010000001&EQ9=100000000&KAT=1010000000&PIA=&PIAzero=&PSLO=&akcija=&paketgarancije=&broker=&prikazkategorije=&kategorija=&zaloga=&arhiv=&presort=&tipsort=&stran=
        private static string url = "https://www.avto.net/Ads/results.asp?znamka=Renault&model=Clio&modelID=&tip=katerikoli%20tip&znamka2=&model2=&tip2=katerikoli%20tip&znamka3=&model3=&tip3=katerikoli%20tip&cenaMin=0&cenaMax=999999&letnikMin=0&letnikMax=2090&bencin=0&starost2=999&oblika=0&ccmMin=0&ccmMax=99999&mocMin=&mocMax=&kmMin=0&kmMax=150000&kwMin=0&kwMax=999&motortakt=&motorvalji=&lokacija=0&sirina=&dolzina=&dolzinaMIN=&dolzinaMAX=&nosilnostMIN=&nosilnostMAX=&lezisc=&presek=&premer=&col=&vijakov=&EToznaka=&vozilo=&airbag=&barva=&barvaint=&EQ1=1000000000&EQ2=1000000000&EQ3=1000000000&EQ4=100000000&EQ5=1000000000&EQ6=1000000000&EQ7=1110100120&EQ8=1010000001&EQ9=100000000&KAT=1010000000&PIA=&PIAzero=&PSLO=&akcija=&paketgarancije=&broker=&prikazkategorije=&kategorija=&zaloga=&arhiv=&presort=&tipsort=&stran=";

        public async void scrapeData(string url)
        {
            string imgUrl, naslov, letnik, prevozeni, motor, menjalnik, cena;
            List<HtmlNode> htmlSeznamOglasov;
            HtmlNode lastnostiAvtomobila;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            htmlSeznamOglasov = htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("ResultsAd")).ToList();

            foreach (var htmlOglas in htmlSeznamOglasov)
            {
                imgUrl = htmlOglas.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");

                naslov = htmlOglas.Descendants("span").FirstOrDefault().InnerText;

                lastnostiAvtomobila = htmlOglas.Descendants("ul").FirstOrDefault();
                letnik = lastnostiAvtomobila.ChildNodes[1].InnerText;
                prevozeni = lastnostiAvtomobila.ChildNodes[2].InnerText;
                motor = lastnostiAvtomobila.ChildNodes[3].InnerText;
                menjalnik = lastnostiAvtomobila.ChildNodes[4].InnerText;

                cena = Regex.Match(
                    htmlOglas.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("ResultsAdPrice")).FirstOrDefault().InnerText
                , @"(\d+.\d+)|^[\d+,\d+]").ToString();

                Console.WriteLine("DEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEELA"+cena);
            }


        }
    }
}
