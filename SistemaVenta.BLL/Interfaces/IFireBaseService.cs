using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SistemaVenta.BLL.Interfaces
{
    public interface IFireBaseService
    {
        
        Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo);
        Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo);
    }
}