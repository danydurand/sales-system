using System;
using System.Text;
using System.Linq;
using System.Linq.Expresions;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SistemaVenta.BLL.Interfaces
{
    public interface ICorreoService
    {
        
        Task<bool> EnviarCorreo(string correoDestino, string asunto, string mensaje);
    }
}