using Microsoft.AspNetCore.Mvc;

namespace TimpusProject.Controllers
{
    public class Products : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
