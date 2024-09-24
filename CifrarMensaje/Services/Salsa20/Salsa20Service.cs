using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CifrarMensaje.Services.Salsa20
{
    public class Salsa20Service
    {
        private readonly InBlock block = new InBlock();
        private readonly Salsa20 salsa = new Salsa20();

        public string DesencriptSalsa20(string secret, string key)
        {
            int sizeKey = key.Length;
            if (sizeKey != 32)
            {
                key = ExpandKey(key);
            }
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            uint[] inblock = block.InitializeBlock(keyBytes);
            uint[] outblock = salsa.Salsa20Block(inblock);
            byte[] secretBytes = HexStringToByteArray(secret);
            byte[] mensaje = XorBytesAndUInt(secretBytes, outblock);

            return Encoding.UTF8.GetString(mensaje);
        }
        public string EncriptSalsa20(string message, string key)
        {
            int sizeKey = key.Length;
            if (sizeKey != 32)
            {
                key=ExpandKey(key);
            }
            byte[] mensajeBytes = Encoding.UTF8.GetBytes(message);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            uint[] inblock = block.InitializeBlock(keyBytes);
            uint[] outblock = salsa.Salsa20Block(inblock);
            byte[] secret = XorBytesAndUInt(mensajeBytes, outblock);

            return BitConverter.ToString(secret).Replace("-", "").ToLower();
        }

        private string ExpandKey(string key)
        {
            while (key.Length != 32)
            {
                key = key + "0";
            }
            return key;
        }

        private static byte[] XorBytesAndUInt(byte[] byteArray, uint[] uintArray)
        {
            byte[] uintAsBytes = new byte[uintArray.Length * 4];
            Buffer.BlockCopy(uintArray, 0, uintAsBytes, 0, uintAsBytes.Length);

            // Crear un nuevo arreglo de bytes para el resultado
            byte[] result = new byte[byteArray.Length];

            // Hacer el XOR entre byteArray y los bytes de uintArray
            for (int i = 0; i < byteArray.Length; i++)
            {
                result[i] = (byte)(byteArray[i] ^ uintAsBytes[i % uintAsBytes.Length]);
            }

            return result;
        }

        private static byte[] HexStringToByteArray(string hex)
        {

            // Crear un arreglo de bytes del tamaño de la cadena hexadecimal dividida por 2
            byte[] bytes = new byte[hex.Length / 2];

            // Convertir cada par de caracteres hexadecimales en un byte
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

    }
}
