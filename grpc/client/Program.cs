using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Calculator;
using Dummy;
using Greet;
using Grpc.Core;
using Sqrt;

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

            /*
             * Client Streaming Greeting 
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
            */

            /*
             * Client Streaming ComputeAverage
            var client = new CalculatorService.CalculatorServiceClient(channel);
            var stream = client.ComputeAverage();
            foreach (var i in Enumerable.Range(1,4))
            {
                await stream.RequestStream.WriteAsync(new ComputeAverageRequest() { Number = i });
            }
            await stream.RequestStream.CompleteAsync();
            var response = await stream.ResponseAsync;
            Console.WriteLine(response.Result);
            */
            /*
             * Bi Directional Streaming
            var client = new GreetingService.GreetingServiceClient(channel);
            var stream = client.GreetEveryone();
            var responseReaderTask = Task.Run(async () =>
            {
                while(await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Received : " + stream.ResponseStream.Current.Result);
                }
            });

            Greeting[] greetings =
            {
                new Greeting() { FirstName = "Önder", LastName = "Genç" },
                new Greeting() { FirstName = "Ali", LastName = "Veli" },
                new Greeting() { FirstName = "Ahmet", LastName = "Mehmet" }
            };

            foreach (var greeting in greetings)
            {
                Console.WriteLine("Sending : " + greeting.ToString());
                await stream.RequestStream.WriteAsync(new GreetEveryoneRequest()
                {
                    Greeting = greeting
                });
            }

            await stream.RequestStream.CompleteAsync();
            await responseReaderTask;
            */

            /*
             * Bi directional streamig
            var client = new CalculatorService.CalculatorServiceClient(channel);
            var stream = client.FindMaximum();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                    Console.WriteLine(stream.ResponseStream.Current.Max);
            });

            int[] numbers = { 1, 5, 3, 6, 2, 20 };
            foreach (var number in numbers)
            {
                await stream.RequestStream.WriteAsync(new FindMaxRequest() { Number = number });
            }

            await stream.RequestStream.CompleteAsync();
            await responseReaderTask;
            */

            /* gRPC Error Example
            var client = new SqrtService.SqrtServiceClient(channel);
            int number = -1;
            try
            {
                var response = client.sqrt(new SqrtRequest() { Number = number });
                Console.WriteLine(response.SquraRoot);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Error : " + e.Status.Detail);
            }
            */

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}

