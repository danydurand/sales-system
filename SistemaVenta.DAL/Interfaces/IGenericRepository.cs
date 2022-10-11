using System;
using System.Text;
using System.Linq;
using System.Linq.Expresions;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SistemaVenta.DAL.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity: class
    {
        Task<TEntity> Obtener(Expresion<Func<TEntity, bool>> filtro);
        Task<TEntity> Crear(TEntity entidad);
        Task<bool> Editar(TEntity entidad);
        Task<bool> Eliminar(TEntity entidad);
        Task<IQueryable<TEntity>> Consultar(Expresion<Func<TEntity, bool>> filtro = null);

    }
}