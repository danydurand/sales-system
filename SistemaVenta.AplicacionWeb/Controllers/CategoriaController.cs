using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class CategoriaController : Controller
{
    private readonly ILogger<CategoriaController> _logger;

    public CategoriaController(ILogger<CategoriaController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


}
