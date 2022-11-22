using System;
using System.Text;
using System.Linq;
using static System.Console;
using System.Threading.Tasks;
using System.Collections.Generic;

using SistemaVenta.Entity;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SistemaVenta.BLL.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _repositorio;
        private readonly IFireBaseService _fireBaseService;

        public ProductoService(
            IGenericRepository<Producto> repositorio,
            IFireBaseService fireBaseService
        )
        {
            _repositorio = repositorio;
            _fireBaseService = fireBaseService;
        }

        public async Task<List<Producto>> Lista()
        {
            IQueryable<Producto> query = await _repositorio.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }
        
        public async Task<Producto> Crear(Producto entidad, Stream Imagen = null, string NombreImagen="")
        {
            Producto productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra);
            if (productoExiste != null) {
                throw new TaskCanceledException("El producto ya existe");
            }

            try
            {
                entidad.NombreImagen = NombreImagen;

                if (Imagen != null) {
                    Console.WriteLine($"Si hay imagen...");
                    string urlImagen = await _fireBaseService.SubirStorage(Imagen, "carpeta_producto", NombreImagen);
                    Console.WriteLine($"El servicio FireBase retorno: {urlImagen}");
                    entidad.UrlImagen = urlImagen;
                }

                Producto productoCreado = await _repositorio.Crear(entidad);

                if (productoCreado.IdProducto == 0) {
                    Console.WriteLine($"Error creado el Producto...");
                    throw new TaskCanceledException("Error creando el Producto");
                }

                IQueryable<Producto> query = await _repositorio.Consultar(p => p.IdProducto == productoCreado.IdProducto);
                productoCreado = query.Include(r => r.IdCategoriaNavigation).First();
                return productoCreado;                
            }
            catch (System.Exception ex)
            {
                throw;
            }

        }
        
        public async Task<Producto> Editar(Producto entidad, Stream Imagen = null, string NombreImagen="")
        {
            Producto productoExiste = await _repositorio.Obtener(
                p => p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto
            );
            if (productoExiste != null) {
                throw new TaskCanceledException("Ya existe un producto con el codigo de barra indicado");
            }

            try
            {
                IQueryable<Producto> queryProducto = await _repositorio.Consultar(p => p.IdProducto == entidad.IdProducto);
                Producto productoEditar = queryProducto.First();
                productoEditar.CodigoBarra = entidad.CodigoBarra;
                productoEditar.Marca = entidad.Marca;
                productoEditar.Descripcion = entidad.Descripcion;
                productoEditar.Stock = entidad.Stock;
                productoEditar.Precio = entidad.Precio;
                productoEditar.EsActivo = entidad.EsActivo;
                productoEditar.IdCategoria = entidad.IdCategoria;
                if (productoEditar.NombreImagen == "") {
                    productoEditar.NombreImagen = NombreImagen;
                }

                if (Imagen != null) {
                    string urlImagen = await _fireBaseService.SubirStorage(Imagen, "carpeta_producto", NombreImagen);
                    productoEditar.UrlImagen = urlImagen;
                }

                bool respuesta = await _repositorio.Editar(productoEditar);
                if (!respuesta) {
                    throw new TaskCanceledException("No se pudo modificar el Producto");
                }

                productoEditar = queryProducto.Include(p => p.IdCategoriaNavigation).First();
                return productoEditar;

            }
            catch (System.Exception)
            {
                throw;
            }

        
        }

        public async Task<bool> Eliminar(int IdProducto)
        {
            try
            {
                Producto productoEncontrado = await _repositorio.Obtener(p => p.IdProducto == IdProducto);
                if (productoEncontrado == null) {
                    throw new TaskCanceledException("El Producto no existe");
                }
                string nombreImagen = productoEncontrado.NombreImagen;
                bool respuesta = await _repositorio.Eliminar(productoEncontrado);
                if (respuesta) {
                    await _fireBaseService.EliminarStorage("carpeta_producto", nombreImagen);
                }
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        
    }
}