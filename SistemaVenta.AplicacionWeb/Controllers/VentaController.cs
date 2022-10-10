using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class VentaController : Controller
{
    private readonly ILogger<VentaController> _logger;

    public VentaController(ILogger<VentaController> logger)
    {
        _logger = logger;
    }

    public IActionResult NuevaVenta()
    {
        return View();
    }

    public IActionResult Historial()
    {
        return View();
    }

    public IActionResult ReporteVenta()
    {
        return View();
    }


}
