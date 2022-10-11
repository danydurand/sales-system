using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IVentaRepository : IGenericRepository<Venta>
    {
        Task<Venta> Registrar(Venta entidad);
        Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin);

    }
}