using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


}
