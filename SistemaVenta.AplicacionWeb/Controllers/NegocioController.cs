using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class NegocioController : Controller
{
    private readonly ILogger<NegocioController> _logger;

    public NegocioController(ILogger<NegocioController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


}
