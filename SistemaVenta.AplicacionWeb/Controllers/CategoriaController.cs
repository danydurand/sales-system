using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;

using AutoMapper;
using SistemaVenta.Entity;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;

namespace SistemaVenta.AplicacionWeb.Controllers;

public class CategoriaController : Controller
{
    private readonly IMapper _mapper;
    private readonly ICategoriaService _categoriaServicio;
    private readonly ILogger<CategoriaController> _logger;

    public CategoriaController(ILogger<CategoriaController> logger, ICategoriaService categoriaService, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
        _categoriaServicio = categoriaService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Lista()
    {
        var lista = await _categoriaServicio.Lista();
        List<VMCategoria> listaCategorias = _mapper.Map<List<VMCategoria>>(lista);
        return StatusCode(StatusCodes.Status200OK, new { data = listaCategorias});
    }


    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] VMCategoria modelo)
    {
        GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();
        try
        {
            
            Categoria categoriaCreada = await _categoriaServicio.Crear(_mapper.Map<Categoria>(modelo));
            modelo = _mapper.Map<VMCategoria>(categoriaCreada);

            gResponse.Estado = true;
            gResponse.Objeto = modelo;
        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    [HttpPut]
    public async Task<IActionResult> Editar([FromBody] VMCategoria modelo)
    {

        GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

        try
        {
            Categoria categoriaEditada = await _categoriaServicio.Editar(_mapper.Map<Categoria>(modelo));
            modelo = _mapper.Map<VMCategoria>(categoriaEditada);

            gResponse.Estado = true;
            gResponse.Objeto = modelo;
        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    [HttpDelete]
    public async Task<IActionResult> Eliminar(int IdCategoria)
    {
        GenericResponse<string> gResponse = new GenericResponse<string>();

        try
        {
            gResponse.Estado = await _categoriaServicio.Eliminar(IdCategoria);
        }
        catch (System.Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }

        return StatusCode(StatusCodes.Status200OK, gResponse);
    }


}
