using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class DashBoardController : Controller
{
    private readonly ILogger<DashBoardController> _logger;

    public DashBoardController(ILogger<DashBoardController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


}
