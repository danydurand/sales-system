using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Net;
using System.Net.Mail;
using SistemaVenta.Entity;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public CorreoService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<bool> EnviarCorreo(string correoDestino, string asunto, string mensaje)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = asunto,
                    Body = mensaje,
                    IsBodyHtml = true
                };
                correo.To.Add(new MailAddress(correoDestino));

                var clienteServidor = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true
                };

                clienteServidor.Send(correo);
                return true;

            }
            catch (System.Exception ex)
            {
                
                return false;
            }
        }

    }
}