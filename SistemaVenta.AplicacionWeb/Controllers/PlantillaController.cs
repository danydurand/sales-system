using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class PlantillaController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public PlantillaController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult EnviarClave(string correo, string clave)
    {
        ViewData["Correo"] = correo;
        ViewData["Clave"] = clave;
        ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";

        return View();
    }

    public IActionResult ReestablecerClave(string clave)
    {
        ViewData["Clave"] = clave;

        return View();
    }

}
