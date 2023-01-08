using Microsoft.AspNetCore.Mvc;
using Module3_Lab8.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.Json;

namespace Module3_Lab8.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            
        }

        


        [HttpGet]
        public  IActionResult Index()
        {
            var fracts =  GetFractionsFromFile();

            if (fracts != null)
            {
                ViewBag.fract1 = fracts[0];
                ViewBag.fract2 = fracts[1];

                return View();
            }

            ViewBag.fract1 = null;
            ViewBag.fract2 = null;

            return View();
        }

        [HttpPost]
        public IActionResult Result()
        {

            ViewBag.fract1 = new Fraction(int.Parse(Request.Form["num1"]), int.Parse(Request.Form["denum1"]));
            ViewBag.fract2 = new Fraction(int.Parse(Request.Form["num2"]), int.Parse(Request.Form["denum2"]));

            SaveToFile(ViewBag.fract1, ViewBag.fract2);


            var result = GetResult(ViewBag.fract1,ViewBag.fract2);

            if (result == null)
            {
                ViewBag.fract1 = GetShorterFraction(ViewBag.fract1.numerator,ViewBag.fract1.denumerator);
                ViewBag.fract2 = GetShorterFraction(ViewBag.fract2.numerator,ViewBag.fract2.denumerator);

                return View("Index");
            }

            int fullPart;

            if (Math.Abs(result.numerator) > result.denumerator)
            {
                fullPart = result.numerator / result.denumerator;
                var num=result.numerator-fullPart*result.denumerator;
                var denum = result.denumerator;

                result = new Fraction(num, denum);
                ViewBag.fullPart = fullPart;
            }

            ViewBag.result = result;
            

            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }



        private  List<Fraction> GetFractionsFromFile()
        {
            try
            {
                using(var fs=new FileStream("fractions.json", FileMode.OpenOrCreate))
                {
                    var list =  JsonSerializer.Deserialize<List<Fraction>>(fs);

                    return list;
                }
                
            }
            catch(Exception ex)
            {
                return null;
            }
        }




        private async void SaveToFile(Fraction fract1,Fraction fract2)
        {
            using(var fs=new FileStream("fractions.json", FileMode.OpenOrCreate))
            {
                var list=new List<Fraction>() {fract1,fract2}; 
                await JsonSerializer.SerializeAsync(fs, list);
            }
        } 




        
        
        private Fraction GetResult(Fraction fract1,Fraction fract2)
        {

            var operation = Request.Form["operation"];


            if (operation == "plus")
            {
                return GetPlusFractions(fract1, fract2);
            }
            else if (operation == "minus")
            {
                return GetMinusFractions(fract1, fract2);
            }
            else if (operation == "mult")
            {
                return GetMultFractions(fract1, fract2);
            }
            else if (operation == "div")
            {
                return GetDivFractions(fract1, fract2);
            }
            else return null;

        }


        private Fraction GetDivFractions(Fraction fract1, Fraction fract2)
        {
            var num = fract1.numerator * fract2.denumerator;
            var denum = fract1.denumerator * fract2.numerator;

            return GetShorterFraction(num, denum);
        }


        private Fraction GetMultFractions(Fraction fract1, Fraction fract2)
        {
            var num=fract1.numerator*fract2.numerator;
            var denum=fract1.denumerator*fract2.denumerator;

            return GetShorterFraction(num, denum);
        }


        private Fraction GetMinusFractions(Fraction fract1, Fraction fract2)
        {
            var num=fract1.numerator - fract2.numerator;
            var denum = fract1.denumerator;

            if (fract1.denumerator != fract2.denumerator)
            {
                int resNum1 = fract1.numerator * fract2.denumerator;
                int resNum2 = fract2.numerator * fract1.denumerator;

                int resDenum = fract1.denumerator * fract2.denumerator;

                num = resNum1 -resNum2;
                denum = resDenum;
            }

            return GetShorterFraction(num, denum);
        }


        


        private Fraction GetPlusFractions(Fraction fract1, Fraction fract2)
        {
            var num = fract1.numerator + fract2.numerator;
            var denum=fract1.denumerator;


            if (fract1.denumerator != fract2.denumerator)
            {
                int resNum1 = fract1.numerator * fract2.denumerator;
                int resNum2 = fract2.numerator * fract1.denumerator;

                int resDenum = fract1.denumerator * fract2.denumerator;

                num=resNum1+ resNum2;
                denum = resDenum;
            }


            return  GetShorterFraction(num, denum);
        }



        private Fraction GetShorterFraction(int num,int denum)
        {
            int divider = 1;

            for (int i = denum; i >= 2; i--)
            {
                if (denum % i == 0)
                {
                    for (int j = Math.Abs(num); j >= 2; j--)
                    {
                        if (num % j == 0)
                        {
                            if (i == j)
                            {
                                divider = j;
                                break;
                            }
                            if (i % j == 0)
                            {
                                divider = j;
                                break;
                            }
                            if (j % i == 0)
                            {
                                divider = i;
                                break;
                            }
                        }
                    }
                }

                if (divider != 1) break;
            }


            


            return new Fraction(num/divider, denum/divider);

        }


        

        
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //public record class Fraction(int numerator,int denumerator);
    }
}