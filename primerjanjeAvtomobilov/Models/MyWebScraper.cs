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

namespace primerjanjeAvtomobilov.Models
{
    public class MyWebScraper
    {
        // url: https://www.avto.net/Ads/results.asp?znamka=Renault&model=Clio&modelID=&tip=katerikoli%20tip&znamka2=&model2=&tip2=katerikoli%20tip&znamka3=&model3=&tip3=katerikoli%20tip&cenaMin=0&cenaMax=999999&letnikMin=0&letnikMax=2090&bencin=0&starost2=999&oblika=0&ccmMin=0&ccmMax=99999&mocMin=&mocMax=&kmMin=0&kmMax=150000&kwMin=0&kwMax=999&motortakt=&motorvalji=&lokacija=0&sirina=&dolzina=&dolzinaMIN=&dolzinaMAX=&nosilnostMIN=&nosilnostMAX=&lezisc=&presek=&premer=&col=&vijakov=&EToznaka=&vozilo=&airbag=&barva=&barvaint=&EQ1=1000000000&EQ2=1000000000&EQ3=1000000000&EQ4=100000000&EQ5=1000000000&EQ6=1000000000&EQ7=1110100120&EQ8=1010000001&EQ9=100000000&KAT=1010000000&PIA=&PIAzero=&PSLO=&akcija=&paketgarancije=&broker=&prikazkategorije=&kategorija=&zaloga=&arhiv=&presort=&tipsort=&stran=
        private static string url = "https://www.avto.net/Ads/results.asp?znamka=Renault&model=Clio&modelID=&tip=katerikoli%20tip&znamka2=&model2=&tip2=katerikoli%20tip&znamka3=&model3=&tip3=katerikoli%20tip&cenaMin=0&cenaMax=999999&letnikMin=0&letnikMax=2090&bencin=0&starost2=999&oblika=0&ccmMin=0&ccmMax=99999&mocMin=&mocMax=&kmMin=0&kmMax=150000&kwMin=0&kwMax=999&motortakt=&motorvalji=&lokacija=0&sirina=&dolzina=&dolzinaMIN=&dolzinaMAX=&nosilnostMIN=&nosilnostMAX=&lezisc=&presek=&premer=&col=&vijakov=&EToznaka=&vozilo=&airbag=&barva=&barvaint=&EQ1=1000000000&EQ2=1000000000&EQ3=1000000000&EQ4=100000000&EQ5=1000000000&EQ6=1000000000&EQ7=1110100120&EQ8=1010000001&EQ9=100000000&KAT=1010000000&PIA=&PIAzero=&PSLO=&akcija=&paketgarancije=&broker=&prikazkategorije=&kategorija=&zaloga=&arhiv=&presort=&tipsort=&stran=";

        public async void scrapeData(string url)
        {
            string imgUrl, naslov, letnik, prevozeni = "", motor = "", menjalnik = "", cena, menjalnikWebScraperError;
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
                string[] asd = new string[3];
                lastnostiAvtomobila = htmlOglas.Descendants("ul").FirstOrDefault();
                letnik = lastnostiAvtomobila.ChildNodes[1].InnerText;
                asd[0] = lastnostiAvtomobila.ChildNodes[2].InnerText;
                asd[1] = lastnostiAvtomobila.ChildNodes[3].InnerText;
                asd[2] = lastnostiAvtomobila.ChildNodes[4].InnerText;
                menjalnikWebScraperError = lastnostiAvtomobila.ChildNodes[5].InnerText;

                cena = Regex.Match(
                    htmlOglas.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("ResultsAdPrice")).FirstOrDefault().InnerText
                , @"(\d+.\d+)|^[\d+,\d+]").ToString();

                //CHECK IF MOTOR OR PREVOZENI OR MENJALNIK ARE SWITCHED
                for (int i = 0; i < 3; i++)
                {
                    if (asd[i].Contains("km"))
                    {
                        prevozeni = asd[i];
                    }

                    if (asd[i].Contains("motor"))
                    {
                        motor = asd[i];
                    }

                    if (asd[i].Contains("menjalnik"))
                    {
                        menjalnik = asd[i];
                    }
                }

                //CHECK IF ANY IS EMPTY
                if (string.IsNullOrWhiteSpace(prevozeni))
                {
                    prevozeni = "Empty";
                }
                if (string.IsNullOrWhiteSpace(menjalnik))
                {
                    menjalnik = "Empty";
                }
                if (string.IsNullOrWhiteSpace(motor))
                {
                    motor = "Empty";
                }

                if (!string.IsNullOrWhiteSpace(menjalnikWebScraperError) && menjalnikWebScraperError.Contains("menjalnik"))
                {
                    menjalnik = menjalnikWebScraperError;
                }

                // Prevozeni regex
                prevozeni = prevozeni.Replace(" km", "");

                // Menjalnik regex
                string checkRocni = menjalnik.Substring(0, 2);
                if (checkRocni == "ro")
                {
                    menjalnik = "ročni" + menjalnik.Substring(5);
                }

                //Cena regex
                cena = cena.Replace(".", string.Empty);

                // Motor regex
                string CCM, regexMotor, KW, KM;

                try
                {
                    CCM = Regex.Match(motor, @"\d+").Value;

                    regexMotor = "" + motor.Replace(CCM, "");

                    KW = Regex.Match(regexMotor, @"\d+").Value;

                    regexMotor = "" + regexMotor.Replace(KW, "");

                    KM = Regex.Match(regexMotor, @"\d+").Value;

                    motor = CCM + "|" + KW + "|" + KM;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("ERROR: " + ex);
                }

                // Naslov regex
                naslov = naslov.Replace("<small>", "");
                naslov = naslov.Replace("</small>", "");
                naslov = naslov.Replace("&nbsp", "");
                naslov = naslov.Replace(";", " ");


                //Console.WriteLine("Naslov: " + naslov + "/");
                //Console.WriteLine("img url: " + imgUrl);
                //Console.WriteLine("Letnik: " + letnik + "/");
                //Console.WriteLine("Prevozeni: " + prevozeni + "/");
                //Console.WriteLine("Motor: " + motor + "/");
                //Console.WriteLine("Menjalnik: " + menjalnik + "/");
                //Console.WriteLine("Cena: " + cena + "/");

                //Console.WriteLine("----------------------------------------------");

                //System.Threading.Thread.Sleep(5000);

            }


        }

        private static async void GetAvto(string url)
        {
            string imgUrl, naslov="", letnik="", prevozeni="", motor="", menjalnik="", cena="0";
            List<HtmlNode> imgNodes, carNodes;
            string OglasDataLeft, OglasDataRight, Base64OglasDataLeft;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            imgNodes = htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("id", "").Equals("lightgallery")).ToList();

            naslov = htmlDocument.DocumentNode.Descendants("h1").FirstOrDefault().InnerHtml;
            
            foreach(var imgNode in imgNodes) // for zanka za pozneje
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
                //Console.WriteLine(OglasDataLeft + " : " +  Base64OglasDataLeft);
                
                switch (Base64OglasDataLeft)
                {
                    case "TGV0bmlrOg==":
                        letnik = Regex.Match(
                            OglasDataRight
                            , @"(\d+.\d+)|^[\d+,\d+]").ToString();
                        break;
                    case "UHJldm/vv71lbmkga206":
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

            //Cena regex
            cena = cena.Replace(".", string.Empty);


            // Menjalnik regex
            string checkRocni = menjalnik.Substring(0, 2);
            if (checkRocni == "ro")
            {
                menjalnik = "ročni" + menjalnik.Substring(5);
            }


            // Motor regex
            string CCM, regexMotor, KW, KM;

            try
            {
                CCM = Regex.Match(motor, @"\d+").Value;

                regexMotor = "" + motor.Replace(CCM, "");

                KW = Regex.Match(regexMotor, @"\d+").Value;

                regexMotor = "" + regexMotor.Replace(KW, "");

                KM = Regex.Match(regexMotor, @"\d+").Value;

                motor = CCM + "|" + KW + "|" + KM;
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }

            // Naslov regex
            naslov = naslov.Replace("<small>","");
            naslov = naslov.Replace("</small>", "");
            naslov = naslov.Replace("&nbsp", "");
            naslov = naslov.Replace(";", " ");

            //Console.WriteLine("Naslov: " + naslov + "/");
            //Console.WriteLine("Letnik: " + letnik + "/");
            //Console.WriteLine("Prevozeni: " + prevozeni + "/");
            //Console.WriteLine("Motor: " + motor + "/");
            //Console.WriteLine(/*"Motor :" + motor +*/ " | CCM: " + CCM + "| KW: " + KW + " | KM: " + KM + "/");
            //Console.WriteLine("Menjalnik: " + menjalnik + "/");
            //Console.WriteLine("Cena: " + cena + "/");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}