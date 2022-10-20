using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class FireBaseService : IFireBaseService
    {

        private readonly IGenericRepository<Configuracion> _repositorio;

        public FireBaseService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Firebase_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));

                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(Config[NombreArchivo])
                    .PutAsync(StreamArchivo, cancellation.Token);

                UrlImagen = await task;

            }
            catch (System.Exception)
            {
                UrlImagen = "";
            }

            return UrlImagen;
        }
        
        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Firebase_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));

                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(Config[NombreArchivo])
                    .DeleteAsync();

                await task;

                return true;

            }
            catch (System.Exception)
            {
                return false;
            }

        }

    }
}