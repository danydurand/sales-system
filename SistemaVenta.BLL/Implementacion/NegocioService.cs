using System;
using System.Text;
using System.Linq;
using static System.Console;
using System.Threading.Tasks;
using System.Collections.Generic;

using SistemaVenta.Entity;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;

namespace SistemaVenta.BLL.Implementacion
{
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repositorio;
        private readonly IFireBaseService _fireBaseService;

        public NegocioService(IGenericRepository<Negocio> repositorio, IFireBaseService fireBaseService)
        {
            _repositorio = repositorio;
            _fireBaseService = fireBaseService;
        }

        public async Task<Negocio> Obtener()
        {

            try
            {
                Negocio negocioEncontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);
                return negocioEncontrado;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        
        public async Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string NombreLogo="")
        {

            try
            {
                Negocio negocioEncontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);
                if (negocioEncontrado == null) {
                    throw new TaskCanceledException("El Nogocio no existe");
                }

                negocioEncontrado.NumeroDocumento = entidad.NumeroDocumento;
                negocioEncontrado.Nombre = entidad.Nombre;
                negocioEncontrado.Correo = entidad.Correo;
                negocioEncontrado.Direccion = entidad.Direccion;
                negocioEncontrado.Telefono = entidad.Telefono;
                negocioEncontrado.PorcentajeImpuesto = entidad. PorcentajeImpuesto;
                negocioEncontrado.SimboloMoneda = entidad.SimboloMoneda;
                negocioEncontrado.NombreLogo = NombreLogo;

                if (Logo != null) {
                    string urlLogo = await _fireBaseService.SubirStorage(Logo, "carpeta_logo", NombreLogo);
                    negocioEncontrado.UrlLogo = urlLogo;
                }

                await _repositorio.Editar(negocioEncontrado);

                return negocioEncontrado;
            }
            catch (System.Exception ex)
            {
                throw;
            }

        }
        

    }
}