using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.Entity;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.AplicacionWeb.Models;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;

using static System.Console;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class UsuarioController : Controller
{
    private readonly IMapper _mapper;
    private readonly IRolService _rolServicio;
    private readonly IUsuarioService _usuarioServicio;


    public UsuarioController(IUsuarioService usuarioService, 
        IRolService rolService, 
        IMapper mapper)
    {
        _usuarioServicio = usuarioService;
        _rolServicio = rolService;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ListaRoles()
    {
        var lista = await _rolServicio.Lista();
        List<VMRol> listaRoles = _mapper.Map<List<VMRol>>(lista);
        return StatusCode(StatusCodes.Status200OK, listaRoles);
    }

    [HttpGet]
    public async Task<IActionResult> Lista()
    {
        var lista = await _usuarioServicio.Lista();
        List<VMUsuario> listaUsuarios = _mapper.Map<List<VMUsuario>>(lista);
        return StatusCode(StatusCodes.Status200OK, new { data = listaUsuarios});
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
    {
        GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
        try
        {
            VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
            
            string nombreFoto = String.Empty;
            Stream fotoStream = null;

            if (foto != null) {
                System.Console.WriteLine("Hay foto");
                string nombreCodificado = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(foto.FileName);
                nombreFoto = string.Concat(nombreCodificado, extension);
                fotoStream = foto.OpenReadStream();
            }

            string urlPlantillaCorreo = 
                $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]"; 
            Usuario usuarioCreado = await _usuarioServicio.Crear(
                _mapper.Map<Usuario>(vmUsuario), 
                fotoStream, 
                nombreFoto, 
                urlPlantillaCorreo
            );
            vmUsuario = _mapper.Map<VMUsuario>(usuarioCreado);

            gResponse.Estado = true;
            gResponse.Objeto = vmUsuario;
        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    [HttpPut]
    public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
    {

        GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

        try
        {
            VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
            
            string nombreFoto = String.Empty;
            Stream fotoStream = null;

            if (foto != null) {
                string nombreCodificado = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(foto.FileName);
                nombreFoto = string.Concat(nombreCodificado, extension);
                fotoStream = foto.OpenReadStream();
            }

            Usuario usuarioEditado = await _usuarioServicio.Editar(_mapper.Map<Usuario>(vmUsuario), 
                fotoStream, nombreFoto);
            vmUsuario = _mapper.Map<VMUsuario>(usuarioEditado);

            gResponse.Estado = true;
            gResponse.Objeto = vmUsuario;
        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    [HttpDelete]
    public async Task<IActionResult> Eliminar(int IdUsuario)
    {
        GenericResponse<string> gResponse = new GenericResponse<string>();

        try
        {
            gResponse.Estado = await _usuarioServicio.Eliminar(IdUsuario);
        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }

        return StatusCode(StatusCodes.Status200OK, gResponse);
    }
}
