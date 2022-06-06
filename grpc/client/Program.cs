using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Calculator;
using Dummy;
using Greet;
using Grpc.Core;

namespace client
{
    class Program
    {
        const string target = "127.0.0.1:50052";

        static void Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The client connected successfully");
            });

            //var client = new DummyService.DummyServiceClient(channel);

            /*
            var client = new GreetingService.GreetingServiceClient(channel);
            var greeting = new Greeting() { FirstName = "Önder", LastName = "Genç" };
            var request = new GreetingRequest() { Greeting = greeting };
            var response = client.Greet(request);
            Console.WriteLine(response.Result);
            */

            var client = new CalculatorService.CalculatorServiceClient(channel);

            var request = new CalculatorSumRequest()
            {
                A = 3,
                B = 10
            };

            var response = client.Calculator(request);

            Console.WriteLine(response.Result);
            Console.WriteLine("The sum of {0} and {1} is {2}", request.A, request.B, response.Result);
            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}

