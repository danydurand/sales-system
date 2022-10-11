using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SistemaVenta.DAL.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity: class
    {
        Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro);
        Task<TEntity> Crear(TEntity entidad);
        Task<bool> Editar(TEntity entidad);
        Task<bool> Eliminar(TEntity entidad);
        Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null);

    }
}