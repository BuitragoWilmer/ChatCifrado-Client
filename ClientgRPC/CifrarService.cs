using CifrarMensaje.Services.Salsa20;
using System;
using System.Text;

namespace ChatCifrado_Servidor.Services
{
    public class CifrarService
    {
        private readonly string key;
        private readonly Salsa20Service salsa20Service = new Salsa20Service();

        public CifrarService(string key)
        {
            this.key = key;
        }

        public string CifrarSalsa20(string plainText)
        {
            string cipherText = salsa20Service.EncriptSalsa20(plainText, key);

            return cipherText;
        }

        public string DecifrarSalsa20(string secret)
        {
            string cipherText = salsa20Service.DesencriptSalsa20(secret, key);

            return cipherText;
        }

    }
}
