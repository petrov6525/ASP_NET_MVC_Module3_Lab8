using Microsoft.AspNetCore.Mvc;
using Module3_Lab8.Models;
using System.Text.Json;

namespace Module3_Lab8.Controllers
{
    public class Task2Controller : Controller
    {
        public IActionResult Index()
        {
            

            ViewBag.car = GetCarFromFile();

            return View("Task2");
        }


        public IActionResult ChangeCar()
        {
            Car car = GetCarFromFile();
            return View(car);
        }


        public IActionResult SaveChanges([FromForm]Car car)
        {
            WriteCarToFile(car);

            return RedirectToAction("Index");
        }


        private Car GetCarFromFile()
        {
            try
            {
                using(var fs=new FileStream("car.json", FileMode.OpenOrCreate))
                {
                    Car? car =JsonSerializer.Deserialize<Car>(fs);
                    return car;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private void WriteCarToFile(Car car)
        {
            try
            {
                using(var fs=new FileStream("car.json", FileMode.OpenOrCreate))
                {
                    JsonSerializer.Serialize(fs, car);
                }
            }
            catch(Exception ex)
            {
                
            }
        }
    }
}
