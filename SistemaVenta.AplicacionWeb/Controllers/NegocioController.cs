using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.Entity;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.AplicacionWeb.Models;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class NegocioController : Controller
{
    private readonly IMapper _mapper;
    private readonly INegocioService _negocioServicio;
    private readonly ILogger<NegocioController> _logger;


    public NegocioController(ILogger<NegocioController> logger, INegocioService negocioService, IMapper mapper)
    {
        _logger = logger;
        _negocioServicio = negocioService;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Obtener()
    {
        GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

        try
        {
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioServicio.Obtener());
            gResponse.Estado = true;
            gResponse.Objeto = vmNegocio;
        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    [HttpPost]
    public async Task<IActionResult> GuardarCambios([FromForm] IFormFile logo, [FromForm] string modelo)
    {
        GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

        try
        {
            VMNegocio vmNegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);
            
            string nombreLogo = "";
            Stream logoStream = null;

            if (logo != null) {
                string nombreCodificado = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(logo.FileName);
                nombreLogo = string.Concat(nombreCodificado, extension);
                logoStream = logo.OpenReadStream();
            }

            Negocio negocioEditado = await _negocioServicio.GuardarCambios(
                _mapper.Map<Negocio>(vmNegocio),
                logoStream,
                nombreLogo
            );

            vmNegocio = _mapper.Map<VMNegocio>(negocioEditado);

            gResponse.Estado = true;
            gResponse.Objeto = vmNegocio;

        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;

        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }


}
