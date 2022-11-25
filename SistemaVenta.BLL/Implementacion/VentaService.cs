using System;
using System.Text;
using System.Linq;
using System.Globalization;
using static System.Console;
using System.Threading.Tasks;
using System.Collections.Generic;

using SistemaVenta.Entity;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SistemaVenta.BLL.Implementacion
{
    public class VentaService : IVentaService
    {
        private readonly IGenericRepository<Producto> _repositorioProducto;
        private readonly IVentaRepository _repositorioVenta;

        public VentaService(IGenericRepository<Producto> repositorioProducto,
            IVentaRepository<Venta> repositorioVenta)
        {
            _repositorioProducto = repositorioProducto;
            _repositorioVenta = repositorioVenta;
        }

        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> query = await _repositorioProducto.Consultar(
                p => p.EsActivo == true &&
                p.Stock > 0 &&
                string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda)
            );

            return query.include(c => c.IdCategoriaNavigation).ToList();
        }


        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _repositorioVenta.Registrar(entidad);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar();
            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;

            if (fechaInicio != "" && fechaFin != "")
            {

                DateTime fechaInit = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-VE"));
                DateTime fechaEnd = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-VE"));

                return query.Where(v =>
                    v.FechaRegistro.Value.Date >= fechaInit.Date &&
                    v.FechaRegistro.Value.Date <= fechaEnd.Date
                )
                .include(t => t.IdTipoDocumentoNavigation)
                .include(u => u.IdUsuarioNavitagion)
                .include(d => d.DetalleVenta)
                .ToList();

            }
            else
            {

                return query.Where(v =>
                    v.NumeroVenta == numeroVenta
                )
                .include(t => t.IdTipoDocumentoNavigation)
                .include(u => u.IdUsuarioNavitagion)
                .include(d => d.DetalleVenta)
                .ToList();

            }
        }

        public async Task<Venta> Detalle(string numeroVenta)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.NumeroVenta == entidad.NumeroVenta);
            return query
                .include(t => t.IdTipoDocumentoNavigation)
                .include(u => u.IdUsuarioNavitagion)
                .include(d => d.DetalleVenta)
                .First();

        }

        public async Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin)
        {
            DateTime fechaInit = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-VE"));
            DateTime fechaEnd = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-VE"));

            List<DetalleVenta> lista = await _repositorioVenta.Reporte(fechaInit, fechaEnd);

            return lista;
        }





    }
}