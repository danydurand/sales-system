using System;
using System.Text;
using System.Linq;
using static System.Console;
using System.Threading.Tasks;
using System.Collections.Generic;

using SistemaVenta.BLL.Interfaces;
using System.Security.Cryptography;

namespace SistemaVenta.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {
        public string GenerarClave()
        {
            Console.WriteLine("Llegando a generar la clave");
            string clave = Guid.NewGuid().ToString("N").Substring(0,6);
            Console.WriteLine($"La clave es: {clave}");
            return clave;
        }

        public string ConvertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();

            using(SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

    }
}