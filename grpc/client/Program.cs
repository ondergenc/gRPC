using System;
using System.Linq;
using System.Threading.Tasks;
using Calculator;
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

            Greet(channel); // Unary example 1
            Sum(channel); // Unary example 2
            _ = GreetManyTimes(channel); // Server streaming example 1
            _ = FindPrimeNumber(channel); // Server streaming example 2
            _ = LongGreet(channel); // Client streaming example 1
            _ = ComputeAvg(channel); // Client streaming example 2
            _ = GreetEveryone(channel); // Bi Directional Streaming example 1
            _ = FindMax(channel); // Bi directional streamig example 2
            FindSqrt(channel); // gRPC Error Handling Example 1
            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
        public static void Greet(Channel channel)
        {
            Console.WriteLine("##### Unary Example 1 #####");
            var client = new GreetingService.GreetingServiceClient(channel);
            var greeting = new Greeting() { FirstName = "Önder", LastName = "Genç" };
            var request = new GreetingRequest() { Greeting = greeting };
            var response = client.Greet(request);
            Console.WriteLine(response.Result);
        }
        public static void Sum(Channel channel)
        {
            Console.WriteLine("##### Unary Example 2 #####");
            var client = new CalculatorService.CalculatorServiceClient(channel);
            var request = new CalculatorSumRequest() { A = 3, B = 10 };
            var response = client.Calculator(request);
            Console.WriteLine(response.Result);
            Console.WriteLine("The sum of {0} and {1} is {2}", request.A, request.B, response.Result);
        }
        public static async Task GreetManyTimes(Channel channel)
        {
            Console.WriteLine("##### Server Streaming Example 1 #####");
            var client = new GreetingService.GreetingServiceClient(channel);
            var greeting = new Greeting() { FirstName = "Önder", LastName = "Genç" };
            var request = new GreetManyTimesRequest() { Greeting = greeting };
            var response = client.GreetManyTimes(request);
            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
        }
        public static async Task FindPrimeNumber(Channel channel)
        {
            Console.WriteLine("##### Server Streaming Example 2 #####");
            var client = new CalculatorService.CalculatorServiceClient(channel);
            var request = new PrimeNumberDecompositionRequest() { Number = 120 };
            var response = client.PrimeNumberDecomposition(request);
            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
        }
        public static async Task LongGreet(Channel channel)
        {
            Console.WriteLine("##### Client Streaming Example 1 #####");
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
        }
        public static async Task ComputeAvg(Channel channel)
        {
            Console.WriteLine("##### Client Streaming Example 2 #####");
            var client = new CalculatorService.CalculatorServiceClient(channel);
            var stream = client.ComputeAverage();
            foreach (var i in Enumerable.Range(1, 4))
            {
                await stream.RequestStream.WriteAsync(new ComputeAverageRequest() { Number = i });
            }
            await stream.RequestStream.CompleteAsync();
            var response = await stream.ResponseAsync;
            Console.WriteLine(response.Result);
        }
        public static async Task GreetEveryone(Channel channel)
        {
            Console.WriteLine("##### Bi Directional Streaming Example 1 #####");
            var client = new GreetingService.GreetingServiceClient(channel);
            var stream = client.GreetEveryone();
            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
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
            //await responseReaderTask;
        }
        public static async Task FindMax(Channel channel)
        {
            Console.WriteLine("##### Bi Directional Streaming Example 2 #####");
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
            //await responseReaderTask;
        }
        public static void FindSqrt(Channel channel)
        {
            Console.WriteLine("##### Error Handling Example 1 #####");
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
        }
    }
}

