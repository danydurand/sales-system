using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class ProductoController : Controller
{
    private readonly ILogger<ProductoController> _logger;

    public ProductoController(ILogger<ProductoController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


}
