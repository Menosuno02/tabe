using System.Security.Cryptography;
using System.Text;

namespace ProyectoASPNET.Helpers
{
    public class HelperCryptography
    {
        public static byte[] EncryptPassword
            (string password, string salt)
        {
            string contenido = password + salt;
            SHA512 sha512 = SHA512.Create();
            byte[] salida = Encoding.UTF8.GetBytes(contenido);
            for (int i = 1; i <= 114; i++)
            {
                salida = sha512.ComputeHash(salida);
            }
            sha512.Clear();
            return salida;
        }
    }
}
