using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;

using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class ProductoController : Controller
{
    private readonly IMapper _mapper;
    private readonly IProductoService _productoServicio;
    private readonly ILogger<ProductoController> _logger;

    public ProductoController(IMapper mapper, IProductoService productoService)
    {
        _mapper = mapper;
        _productoServicio = productoService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Lista()
    {
        var lista = await _productoServicio.Lista();
        List<VMProducto> listaProductos = _mapper.Map<List<VMProducto>>(lista);
        return StatusCode(StatusCodes.Status200OK, new { data = listaProductos});
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string modelo)
    {
        GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();
        try
        {
            VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);
            
            string nombreImagen = String.Empty;
            Stream imagenStream = null;

            if (imagen != null) {
                System.Console.WriteLine("Hay imagen");
                string nombreCodificado = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(imagen.FileName);
                nombreImagen = string.Concat(nombreCodificado, extension);
                imagenStream = imagen.OpenReadStream();
            }

            Producto productoCreado = await _productoServicio.Crear(
                _mapper.Map<Producto>(vmProducto), 
                imagenStream, 
                nombreImagen 
            );
            vmProducto = _mapper.Map<VMProducto>(productoCreado);

            gResponse.Estado = true;
            gResponse.Objeto = vmProducto;
        }
        catch (System.Exception ex) 
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }


    [HttpPut]
    public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
    {

        GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();

        try
        {
            VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);
            
            string nombreImagen = String.Empty;
            Stream imagenStream = null; 

            if (imagen != null) {
                Console.WriteLine($"Viene una imagen...");
                string nombreCodificado = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(imagen.FileName);
                nombreImagen = string.Concat(nombreCodificado, extension);
                Console.WriteLine($"Image name: {nombreImagen}");
                imagenStream = imagen.OpenReadStream();
            }

            Producto productoEditado = await _productoServicio.Editar(_mapper.Map<Producto>(vmProducto), imagenStream, nombreImagen);
            vmProducto = _mapper.Map<VMProducto>(productoEditado);

            gResponse.Estado = true;
            gResponse.Objeto = vmProducto;
        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }


    [HttpDelete]
    public async Task<IActionResult> Eliminar(int IdProducto)
    {
        GenericResponse<string> gResponse = new GenericResponse<string>();

        try
        {
            gResponse.Estado = await _productoServicio.Eliminar(IdProducto);
        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }

        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

}
