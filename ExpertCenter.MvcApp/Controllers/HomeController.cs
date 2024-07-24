using Microsoft.AspNetCore.Mvc;

namespace ExpertCenter.MvcApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
