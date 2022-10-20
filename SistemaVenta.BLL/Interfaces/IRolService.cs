using System;
using System.Text;
using System.Linq;
using SistemaVenta.Entity;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;


namespace SistemaVenta.BLL.Interfaces
{
    public interface IRolService
    {
        
        Task<List<Rol>> Lista();

    }
}