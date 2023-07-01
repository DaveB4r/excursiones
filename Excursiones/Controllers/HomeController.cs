using Excursiones.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Excursiones.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public class PesoCalorias
        {
            public int Peso { get; set; }
            public int Calorias { get; set; }
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // capturo el mensaje despues de hacer los respectivos calculos
            string successMessage = TempData["SuccessMessage"] as string;
            // guardo el mensaje en ViewBag
            ViewBag.SuccessMessage = successMessage;
            return View();
        }
        [HttpPost]
        public ActionResult Calcular(ExcursionModel oExcursion)
        {
            // creo un diccionario de objetos con las diferentes medidas (peso y calorias) de cada elemento
            Dictionary<string, PesoCalorias> pesosCalorias = new Dictionary<string, PesoCalorias>
            {
                {"E1", new PesoCalorias { Peso = 5, Calorias = 3} },
                {"E2", new PesoCalorias { Peso = 3, Calorias = 5} },
                {"E3", new PesoCalorias { Peso = 5, Calorias = 2} },
                {"E4", new PesoCalorias { Peso = 1, Calorias = 8} },
                {"E5", new PesoCalorias { Peso = 2, Calorias = 3} }
            };

            //Convierto el diccionario en una lista para despues ordenarla segun el numero de calorias de mayor a menor
            List<KeyValuePair<string, PesoCalorias>> pesosCaloriasList = pesosCalorias.ToList();
            pesosCaloriasList.Sort((a, b) => b.Value.Calorias - a.Value.Calorias);

            int minCalorias = oExcursion.minCalorias;
            int maxPeso = oExcursion.maxPeso;
            int pesoAcum = 0;
            int caloriasAcum = 0;
            List<string> result = new List<string>();

            //recorro los elementos de la lista y donde acumulo los pesos y las calorias de los elementos siempre y cuando no se pasen del peso maximo
            foreach(KeyValuePair<string, PesoCalorias> pair in pesosCaloriasList)
            {
                int pesoActual = pair.Value.Peso;
                int caloriaActual = pair.Value.Calorias;
                if(pesoAcum + pesoActual <= maxPeso)
                {
                    caloriasAcum += caloriaActual;
                    pesoAcum += pesoActual;
                    result.Insert(0, pair.Key);
                }
                if (caloriasAcum >= minCalorias)
                    break;
            }
            // ordeno el resultado basado en los elementos
            result.Sort((a,b) => int.Parse(a.Substring(1)) - int.Parse(b.Substring(1)));
            // creo un mensaje temporal
            TempData["SuccessMessage"] = $"En Este caso los elementos viables son: {string.Join(" ", result)}, ya que su peso total sería {pesoAcum} y brindan {caloriasAcum} calorías";
            // vuelvo al inicio
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}