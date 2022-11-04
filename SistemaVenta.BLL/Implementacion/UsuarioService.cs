using System;
using System.Text;
using System.Linq;
using static System.Console;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Net;
using SistemaVenta.Entity;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFireBaseService _fireBaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;

        public UsuarioService(
            IGenericRepository<Usuario> repositorio,
            IFireBaseService fireBaseService,
            IUtilidadesService utilidadesService,
            ICorreoService correoService
        )
        {
            _repositorio = repositorio;
            _fireBaseService = fireBaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }

        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.Include(r => r.IdRolNavigation).ToList();
        }
        
        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto="", string UrlPlantillaCorreo="")
        {
            Usuario usuarioExiste = await _repositorio.Obtener(u => u.Correo == entidad.Correo);
            if (usuarioExiste != null) {
                throw new TaskCanceledException("El correo ya existe");
            }


            try
            {
                string claveGenerada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(claveGenerada);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null) {
                    Console.WriteLine($"Si hay foto...");
                    string urlFoto = await _fireBaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    Console.WriteLine($"El servicio FireBase retorno: {urlFoto}");
                    entidad.UrlFoto = urlFoto;
                }

                Usuario usuarioCreado = await _repositorio.Crear(entidad);

                if (usuarioCreado.IdUsuario == 0) {
                    Console.WriteLine($"Error creado el Usuario...");
                    throw new TaskCanceledException("Error creando el Usuario");
                }

                if (UrlPlantillaCorreo != "") {
                    UrlPlantillaCorreo = UrlPlantillaCorreo
                        .Replace("[correo]", usuarioCreado.Correo)
                        .Replace("[clave]", claveGenerada);
                    
                    string htmlCorreo = "";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK) {
                        using (Stream dataStream = response.GetResponseStream()) {
                            StreamReader readerStream = null;
                            if (response.CharacterSet == null) {
                                readerStream = new StreamReader(dataStream);
                            } else {
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }
                    if (htmlCorreo != "") {
                        await _correoService.EnviarCorreo(usuarioCreado.Correo, "Cuenta Creada", htmlCorreo);
                    }
                }
                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(r => r.IdRolNavigation).First();
                return usuarioCreado;                
            }
            catch (System.Exception ex)
            {
                throw;
            }

        }
        
        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto="")
        {
            Usuario usuarioExiste = await _repositorio.Obtener(
                u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);
            if (usuarioExiste != null) {
                throw new TaskCanceledException("El correo ya existe");
            }

            try
            {
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);
                Usuario usuarioEditar = queryUsuario.First();
                usuarioEditar.Nombre = entidad.Nombre;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.Telefono = entidad.Telefono;
                usuarioEditar.EsActivo = entidad.EsActivo;
                usuarioEditar.IdRol = entidad.IdRol;

                if (usuarioEditar.NombreFoto == "") {
                    usuarioEditar.NombreFoto = NombreFoto;
                }
                if (Foto != null) {
                    string urlFoto = await _fireBaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    usuarioEditar.UrlFoto = urlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuarioEditar);
                if (!respuesta) {
                    throw new TaskCanceledException("No se pudo modificar el Usuario");
                }

                usuarioEditar = queryUsuario.Include(r => r.IdRolNavigation).First();
                return usuarioEditar;

            }
            catch (System.Exception)
            {
                throw;
            }

        
        }

        public async Task<bool> Eliminar(int IdUsuario)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == IdUsuario);
                if (usuarioEncontrado == null) {
                    throw new TaskCanceledException("El Usuario no existe");
                }
                string nombreFoto = usuarioEncontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuarioEncontrado);
                if (respuesta) {
                    await _fireBaseService.EliminarStorage("carpeta_usuario", nombreFoto);
                }
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        
        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string claveEncriptada = _utilidadesService.ConvertirSha256(clave);
            Usuario usuarioEncontrado = await _repositorio.Obtener(
                u => u.Correo.Equals(correo) && u.Clave.Equals(claveEncriptada));
            return usuarioEncontrado;
        }

        public async Task<Usuario> ObtenerPorId(int IdUsuario) 
        {
            IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == IdUsuario);
            Usuario resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();
            return resultado;
        }
        
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == entidad.IdUsuario);
                if (usuarioEncontrado == null) {
                    throw new TaskCanceledException("El Usuario no existe");
                }
                usuarioEncontrado.Correo = entidad.Correo;
                usuarioEncontrado.Telefono = entidad.Telefono;

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);
                return respuesta;

            }
            catch (System.Exception)
            {
                throw;
            }
        }
        
        public async Task<bool> CambiarClave(int IdUsuario, string ClaveActual, string ClaveNueva)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == IdUsuario);
                if (usuarioEncontrado == null) {
                    throw new TaskCanceledException("El Usuario no existe");
                }
                if (usuarioEncontrado.Clave == _utilidadesService.ConvertirSha256(ClaveActual)) {
                    throw new TaskCanceledException("La clave actual no coincide");
                }
                string claveEncriptada = _utilidadesService.ConvertirSha256(ClaveNueva);
                usuarioEncontrado.Clave = claveEncriptada;

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);
                return respuesta;

            }
            catch (System.Exception)
            {
                throw;
            }       
        }
        
        public async Task<bool> ReestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.Correo == correo);
                if (usuarioEncontrado == null) {
                    throw new TaskCanceledException("No hay ningun Usuario asociado al correo");
                }
                string claveGenerada = _utilidadesService.GenerarClave();
                usuarioEncontrado.Clave = claveGenerada;

                if (UrlPlantillaCorreo != "") {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", claveGenerada);
                    
                    string htmlCorreo = "";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK) {
                        using (Stream dataStream = response.GetResponseStream()) {
                            StreamReader readerStream = null;
                            if (response.CharacterSet == null) {
                                readerStream = new StreamReader(dataStream);
                            } else {
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }
                    bool correEnviado = false;
                    if (htmlCorreo != "") {
                        correEnviado = await _correoService.EnviarCorreo(correo, "Clave Reestablecida", htmlCorreo);
                    }
                    if (correEnviado == false) {
                        throw new TaskCanceledException("Error al enviar el correo.  Por favor intente mas tarde");
                    }
                }

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);
                return respuesta;
                
            }
            catch (System.Exception)
            {
                
                throw;
            }       
        }

    }
}