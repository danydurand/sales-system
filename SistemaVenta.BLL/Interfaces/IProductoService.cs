using System;
using System.Text;
using System.Linq;
using SistemaVenta.Entity;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;


namespace SistemaVenta.BLL.Interfaces
{
    public interface IProductoService
    {
        
        Task<List<Producto>> Lista();
        
        Task<Producto> Crear(Producto entidad, 
            Stream Imagen = null, 
            string NombreImagen="");
        
        Task<Producto> Editar(Producto entidad, 
            Stream Imagen = null, 
            string NombreImagen="");

        Task<bool> Eliminar(int idProducto);
    }
}