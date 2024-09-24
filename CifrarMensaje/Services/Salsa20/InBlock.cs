using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CifrarMensaje.Services.Salsa20
{
    public class InBlock
    {
        private readonly uint[] constants = { 0x61707865, 0x3320646e, 0x79622d32, 0x6b206574 };
        private readonly byte[] nonce = new byte[8] {    
            0x00, 0x00, 0x00, 0x00, // Primeros 4 bytes
            0x00, 0x00, 0x00, 0x01  // Últimos 4 bytes 
         };

        /*

        +-----------+-----------+-----------+-----------+
        | Constante |  Clave 1   |  Clave 2   |  Clave 3   |
        +-----------+-----------+-----------+-----------+
        |  Clave 4  | Constante  |  Nonce 1   |  Nonce 2   |
        +-----------+-----------+-----------+-----------+
        | Contador  | Constante  |  Clave 5   |  Clave 6   |
        +-----------+-----------+-----------+-----------+
        |  Clave 7  |  Clave 8   | Constante  |  Clave 9   |
        +-----------+-----------+-----------+-----------+

        */

        // El bloque debe ser 512 bit o lo que es igual 16 bytes
        public uint[] InitializeBlock(byte[] key)
        {
            //La clave secreta corresponde a 256 bits
            //Nonce 64 bits
            //Contador 64 bits

            uint[] block = new uint[16];

            block[0] = constants[0];
            block[1] = BitConverter.ToUInt32(key, 0);
            block[2] = BitConverter.ToUInt32(key, 4);
            block[3] = BitConverter.ToUInt32(key, 8);
            block[4] = BitConverter.ToUInt32(key, 12);
            block[5] = constants[1];
            block[6] = BitConverter.ToUInt32(nonce, 0);
            block[7] = BitConverter.ToUInt32(nonce, 4);
            block[8] = 0; // Contador inicial a 0
            block[9] = 0;
            block[10] = constants[2];
            block[11] = BitConverter.ToUInt32(key, 16);
            block[12] = BitConverter.ToUInt32(key, 20);
            block[13] = BitConverter.ToUInt32(key, 24);
            block[14] = BitConverter.ToUInt32(key, 28);
            block[15] = constants[3];

            return block;
        }
    }
}
