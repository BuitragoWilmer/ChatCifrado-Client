using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CifrarMensaje.Services.Salsa20
{
    internal class Salsa20
    {
        private const int ROUNDS = 20;
        private  int _round = 0;
        private const string filePath = "salsa20_rounds_log.txt";
        private static uint ROTL(uint a, int b)
        {
            return (a << b) | (a >> (32 - b));
        }

        private static void QR(ref uint a, ref uint b, ref uint c, ref uint d)
        {
            b ^= ROTL(a + d, 7);
            c ^= ROTL(b + a, 9);
            d ^= ROTL(c + b, 13);
            a ^= ROTL(d + c, 18);
        }

        public uint[] Salsa20Block(uint[] inBlock)
        {

            uint[] x = new uint[16];
            uint[] outBlock = new uint[16];
            Array.Copy(inBlock, x, 16); // Copy input to x

            // 10 loops × 2 rounds/loop = 20 rounds
            for (int i = 0; i < ROUNDS; i += 2)
            {
                // Odd round
                QR(ref x[0], ref x[4], ref x[8], ref x[12]); // column 1
                QR(ref x[5], ref x[9], ref x[13], ref x[1]); // column 2
                QR(ref x[10], ref x[14], ref x[2], ref x[6]); // column 3
                QR(ref x[15], ref x[3], ref x[7], ref x[11]); // column 4

                // Even round
                QR(ref x[0], ref x[1], ref x[2], ref x[3]); // row 1
                QR(ref x[5], ref x[6], ref x[7], ref x[4]); // row 2
                QR(ref x[10], ref x[11], ref x[8], ref x[9]); // row 3
                QR(ref x[15], ref x[12], ref x[13], ref x[14]); // row 4

                LogRoundsToFile(x);
            }

            for (int i = 0; i < 16; ++i)
            {
                outBlock[i] = x[i] + inBlock[i];
            }
            return outBlock;
        }

        private  void LogRoundsToFile(uint[] block)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {

                    writer.WriteLine($"Ronda {_round + 1}:");


                    // Escribir la matriz en formato de 4 columnas y 4 filas
                    for (int i = 0; i < 16; i += 4)
                    {
                        writer.WriteLine($"{block[i]:x8}  {block[i + 1]:x8}  {block[i + 2]:x8}  {block[i + 3]:x8}");
                }

                writer.WriteLine(); // Espacio entre rondas
                _round++;

            }
        }
    }
}
