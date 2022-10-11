using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using SistemaVenta.Entity;
using System.Linq.Expressions;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SistemaVenta.DAL.Implementacion
{
    public class VentaRepository: GenericRepository<Venta>, IVentaRepository
    {
        private readonly DBVENTAContext _dbContext;

        public VentaRepository(DBVENTAContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta();

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in entidad.DetalleVenta)
                    {
                        Producto prod = _dbContext.Productos.Where(p=>p.IdProducto == dv.IdProducto).First();
                        prod.Stock = prod.Stock - dv.Cantidad;
                        _dbContext.Productos.Update(prod);
                    }
                    await _dbContext.SaveChangesAsync();

                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(n => n.Gestion == "venta").First();
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;
                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = numeroVenta;

                    await _dbContext.Venta.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;

                    transaction.Commit();

                }
                catch (System.Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return ventaGenerada;
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin)
        {
            List<DetalleVenta> listaResumen = await _dbContext.DetalleVenta
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(u => u.IdUsuarioNavigation)
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio &&
                    dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaFin).ToListAsync();
            return listaResumen;
        }



    }
}