using System;
using System.Text;
using System.Linq;
using SistemaVenta.Entity;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;


namespace SistemaVenta.BLL.Interfaces
{
    public interface IUsuarioService
    {
        
        Task<List<Usuario>> Lista();
        
        Task<Usuario> Crear(Usuario entidad, 
            Stream Foto = null, 
            string NombreFoto="", 
            string UrlPlantillaCorreo="");
        
        Task<Usuario> Editar(Usuario entidad, 
            Stream Foto = null, 
            string NombreFoto="");

        Task<bool> Eliminar(int IdUsuario);
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave); 
        Task<Usuario> ObtenerPorId(int IdUsuario); 
        Task<bool> GuardarPerfil(Usuario entidad);
        Task<bool> CambiarClave(int IdUsuario, string ClaveActual, string ClaveNueva);
        Task<bool> ReestablecerClave(string correo, string UrlPlantillaCorreo);
    }
}