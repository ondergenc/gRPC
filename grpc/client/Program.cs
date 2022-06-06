using System;
using System.Linq;
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

        static async Task Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The client connected successfully");
            });

            /*
             * Unary
            //var client = new DummyService.DummyServiceClient(channel);
            var client = new GreetingService.GreetingServiceClient(channel);
            var greeting = new Greeting() { FirstName = "Önder", LastName = "Genç" };
            var request = new GreetingRequest() { Greeting = greeting };
            var response = client.Greet(request);
            Console.WriteLine(response.Result);
            */

            /*
             * Calculator sum
            var client = new CalculatorService.CalculatorServiceClient(channel);

            var request = new CalculatorSumRequest()
            {
                A = 3,
                B = 10
            };

            var response = client.Calculator(request);
            Console.WriteLine(response.Result);
            Console.WriteLine("The sum of {0} and {1} is {2}", request.A, request.B, response.Result);
            */

            /*
             * Server streaming Greeting 
            var client = new GreetingService.GreetingServiceClient(channel);
            var greeting = new Greeting() { FirstName = "Önder", LastName = "Genç" };
            var request = new GreetManyTimesRequest() { Greeting = greeting };
            var response = client.GreetManyTimes(request);
            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Result);
                await Task.Delay(200);
            }            
            */

            /*
             * Server streaming PrimeNumbers
            var client = new CalculatorService.CalculatorServiceClient(channel);

            var request = new PrimeNumberDecompositionRequest()
            {
                Number = 120
            };

            var response = client.PrimeNumberDecomposition(request);

            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
            */
            var client = new GreetingService.GreetingServiceClient(channel);
            var greeting = new Greeting() { FirstName = "Önder", LastName = "Genç" };
            var request = new LongGreetRequest() { Greeting = greeting };
            var stream = client.LongGreet();
            foreach (int i in Enumerable.Range(1, 10))
            {
                await stream.RequestStream.WriteAsync(request);
            }

            await stream.RequestStream.CompleteAsync();
            var response = await stream.ResponseAsync;

            Console.WriteLine(response.Result);

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}

