using System;
using System.IO;
using Calculator;
using Greet;
using Grpc.Core;
using Sqrt;

namespace server
{
    class Program
    {
        const int Port = 50052;

        static void Main(string[] args)
        {
            Server server = null;

            try
            {
                server = new Server()
                {
                    Services = { GreetingService.BindService(new GreetingServiceImpl()),
                                 CalculatorService.BindService(new CalculatorServiceImpl()),
                                 SqrtService.BindService(new SqrtServiceImpl())
                    },
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("The server is listening on the port : " + Port);
                Console.ReadKey();
            }
            catch (IOException e)
            {
                Console.WriteLine("The server failed to start : " + e.Message);
                throw;
            }
            finally
            {
                if (server != null)
                    server.ShutdownAsync().Wait();
            }
        }
    }
}

