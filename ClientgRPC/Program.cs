using System;
using System.Threading.Tasks;

namespace ClientgRPC
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Servicio servicio = new Servicio();
            while(!await servicio.LogginAsync())
            {
                Console.WriteLine("try later.");           
                Console.ReadKey();
                Console.Clear();
            }
            await servicio.MostrarMenuAsync();
        }
    }
}

