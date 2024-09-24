using ChatCifrado_Servidor.Services;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ClientgRPC
{
    public class Servicio
    {

        private static GrpcChannel _channel;
        private static ClientsConexion.ClientsConexionClient _client;
        private static Chat.ChatClient _chat;
        private readonly CifrarService _cifradoService = new CifrarService("0123456789abcdef0123456789abcdef");

        private string _UserName;

        public string UserName { get => _UserName; }

        private static void InitializeChannel()
        {
            if (_channel == null)
            {
                // Solo creamos el canal una vez
                _channel = GrpcChannel.ForAddress("https://localhost:5003");
                _client = new ClientsConexion.ClientsConexionClient(_channel);
                _chat = new Chat.ChatClient(_channel);
            }
        }

        public static void DisposeChannel()
        {
            if (_channel != null)
            {
                _channel.ShutdownAsync().Wait(); // Cerrar el canal de forma asíncrona
                _channel = null; // Limpiar la referencia al canal
                _client = null;  // Limpiar la referencia al cliente
            }
        }

        public async Task<bool> LogginAsync()
        {
            InitializeChannel();
            Console.Write("Por favor, introduce tu nombre de usuario: ");
            _UserName = Console.ReadLine();

            using var call = _client.loggear();

            // Obtener el stream de solicitud y respuesta
            var requestStream = call.RequestStream;
            var responseStream = call.ResponseStream;

            await requestStream.WriteAsync(new LogInRequest
            {
                User = _UserName
            });

            await requestStream.CompleteAsync();

            await foreach (var response in responseStream.ReadAllAsync())
            {
                Console.WriteLine($"Received message: {response.Message}");

                if (response.Success)
                {
                    Console.WriteLine("Login successful!");
                    StartSendingHeartbeats(_UserName);
                    return true;
                }
                else
                {
                    Console.WriteLine("Login failed: " + response.Message);
                    DisposeChannel();
                    return false;
                }
            }
            return false;
        }

        public async Task MostrarMenuAsync()
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("=== Menú Principal ===");
                Console.WriteLine("1. Opción 1: Ver contactos en linea.");
                Console.WriteLine("2. Opción 2: Conectar con un contacto");
                Console.WriteLine("3. Opción 3: Salir");
                Console.Write("Selecciona una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Contactos Registrados");
                        List<string> contacts = await ListContactsAsync();
                        foreach (string contact in contacts)
                        {
                            string userDecifrado = _cifradoService.DecifrarSalsa20(contact);
                            Console.WriteLine("- " + userDecifrado);
                        }
                        break;
                    case "2":
                        Console.WriteLine("Has seleccionado 'Enviar mensaje'.");
                        // Lógica para enviar mensaje
                        break;
                    case "3":
                        Console.WriteLine("Saliendo...");
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida, intenta de nuevo.");
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        public async Task<List<string>> ListContactsAsync()
        {
            ContactsOnLineRequest request = new ContactsOnLineRequest()
            {
                User = _UserName
            };

            ContactsOnLineResponse response = await _chat.ContactsOnLineAsync(request);

            return response.ContactsOnLine.ToList();
        }

        public async Task StartSendingHeartbeats(string userId)
        {
            bool connection = true;
            while (connection)
            {
                await Task.Delay(TimeSpan.FromSeconds(30)); // Enviar cada 30 segundos

                var heartbeat = new Heartbeat { UserId = userId };
                var response = await _client.SendHeartbeatAsync(heartbeat);

                connection= response.ServerHealthy; // Mostrar la respuesta del servidor
            }
        }

    }
}
