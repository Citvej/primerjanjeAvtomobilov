using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using primerjanjeAvtomobilov.Models;

namespace primerjanjeAvtomobilov.Models
{

    public class MyWebScraper
    {
        public List<Avtomobili> listAvti = new List<Avtomobili>();
        public Avtomobili avto = new Avtomobili();
        // url: https://www.avto.net/Ads/results.asp?znamka=Renault&model=Clio&modelID=&tip=katerikoli%20tip&znamka2=&model2=&tip2=katerikoli%20tip&znamka3=&model3=&tip3=katerikoli%20tip&cenaMin=0&cenaMax=999999&letnikMin=0&letnikMax=2090&bencin=0&starost2=999&oblika=0&ccmMin=0&ccmMax=99999&mocMin=&mocMax=&kmMin=0&kmMax=150000&kwMin=0&kwMax=999&motortakt=&motorvalji=&lokacija=0&sirina=&dolzina=&dolzinaMIN=&dolzinaMAX=&nosilnostMIN=&nosilnostMAX=&lezisc=&presek=&premer=&col=&vijakov=&EToznaka=&vozilo=&airbag=&barva=&barvaint=&EQ1=1000000000&EQ2=1000000000&EQ3=1000000000&EQ4=100000000&EQ5=1000000000&EQ6=1000000000&EQ7=1110100120&EQ8=1010000001&EQ9=100000000&KAT=1010000000&PIA=&PIAzero=&PSLO=&akcija=&paketgarancije=&broker=&prikazkategorije=&kategorija=&zaloga=&arhiv=&presort=&tipsort=&stran=
        private static string url = "https://www.avto.net/Ads/results.asp?znamka=Renault&model=Clio&modelID=&tip=katerikoli%20tip&znamka2=&model2=&tip2=katerikoli%20tip&znamka3=&model3=&tip3=katerikoli%20tip&cenaMin=0&cenaMax=999999&letnikMin=0&letnikMax=2090&bencin=0&starost2=999&oblika=0&ccmMin=0&ccmMax=99999&mocMin=&mocMax=&kmMin=0&kmMax=150000&kwMin=0&kwMax=999&motortakt=&motorvalji=&lokacija=0&sirina=&dolzina=&dolzinaMIN=&dolzinaMAX=&nosilnostMIN=&nosilnostMAX=&lezisc=&presek=&premer=&col=&vijakov=&EToznaka=&vozilo=&airbag=&barva=&barvaint=&EQ1=1000000000&EQ2=1000000000&EQ3=1000000000&EQ4=100000000&EQ5=1000000000&EQ6=1000000000&EQ7=1110100120&EQ8=1010000001&EQ9=100000000&KAT=1010000000&PIA=&PIAzero=&PSLO=&akcija=&paketgarancije=&broker=&prikazkategorije=&kategorija=&zaloga=&arhiv=&presort=&tipsort=&stran=";

        public async Task scrapeData(string url)
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
                Avtomobili avto = new Avtomobili();
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

                prenesiSliko(imgUrl);
                //Console.WriteLine(naslov);
                //Console.WriteLine("img url: " + imgUrl);
                //Console.WriteLine(letnik);
                //Console.WriteLine(prevozeni);
                //Console.WriteLine(motor);
                //Console.WriteLine(menjalnik);
                //Console.WriteLine("Cena: " + cena):
                avto.nazivAvta = naslov;
                avto.cena = cena;
                avto.letnikAvto = letnik;
                avto.prevozeniKilometri = prevozeni;
                avto.slikaAvta = imgUrl;

                listAvti.Add(avto);
            }
            
        }

        public async Task GetAvto(string url)
        {
            string imgUrl = "", naslov = "", letnik = "", prevozeni = "", motor = "", menjalnik = "", cena;
            List<HtmlNode> imgNodes, carNodes;
            string OglasDataLeft, OglasDataRight, Base64OglasDataLeft;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            imgNodes = htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("id", "").Equals("lightgallery")).ToList();

            foreach (var imgNode in imgNodes) // for zanka za pozneje
            {
                imgUrl = imgNode.Descendants("p").FirstOrDefault().GetAttributeValue("data-src", "");
            }

            carNodes = htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("OglasData")).ToList();

            cena = Regex.Match(
                htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("OglasPrice")).First().InnerText
                , @"(\d+.\d+)|^[\d+,\d+]").ToString();

            foreach (var carNode in carNodes)
            {
                OglasDataLeft = carNode.Descendants("div").FirstOrDefault().InnerText;
                try
                {
                    OglasDataRight = carNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("OglasDataRight")).First().InnerText;
                    //    
                }
                catch
                {
                    OglasDataRight = "";
                }

                //zakodiram v base64 ker so nekateri znaki v unicode

                Base64OglasDataLeft = Base64Encode(OglasDataLeft);

                switch (Base64OglasDataLeft)
                {
                    case "TGV0bmlrOg==":
                        letnik = Regex.Match(
                            OglasDataRight
                            , @"(\d+.\d+)|^[\d+,\d+]").ToString();
                        break;
                    case "UHJldm/vv71lbmkga206==":
                        prevozeni = OglasDataRight;
                        break;
                    case "TW90b3I6":
                        motor = OglasDataRight;
                        break;
                    case "TWVuamFsbmlrOg==":
                        menjalnik = OglasDataRight;
                        break;
                    default:
                        break;
                }



                //Console.WriteLine(OglasDataLeft); // + " " + Base64OglasDataLeft);
                //Console.WriteLine(OglasDataRight);


            }
            prenesiSliko(imgUrl);
            avto.nazivAvta = naslov;
            avto.cena = cena;
            avto.letnikAvto = letnik;
            avto.prevozeniKilometri = prevozeni;
            avto.slikaAvta = imgUrl;
            //Console.WriteLine(letnik);
            //Console.WriteLine(prevozeni);
            //Console.WriteLine(menjalnik);
            //Console.WriteLine(motor);



        }
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string StaticComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static void prenesiSliko(string url)
        {
            if (url == "../_graphics/0.jpg") url = "https://avto.net/_graphics/0.jpg";
            string imgName = StaticComputeSha256Hash(url);

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(url), @"c:\temp\" + imgName + ".png");
            }
        }
    }
}
