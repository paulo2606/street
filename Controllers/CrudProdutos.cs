using Microsoft.AspNetCore.Mvc;

namespace street.Controllers
{
    public class CrudProdutos : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
