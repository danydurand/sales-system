using System;
using System.Text;
using System.Linq;
using SistemaVenta.Entity;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;


namespace SistemaVenta.BLL.Interfaces
{
    public interface ICategoriaService
    {
        
        Task<List<Categoria>> Lista();
        
        Task<Categoria> Crear(Categoria entidad);
        
        Task<Categoria> Editar(Categoria entidad);

        Task<bool> Eliminar(int idCategoria);
    }
}