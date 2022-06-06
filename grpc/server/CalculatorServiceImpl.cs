using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator;
using Grpc.Core;
using static Calculator.CalculatorService;

namespace server
{
    public class CalculatorServiceImpl : CalculatorServiceBase
    {
        public override Task<CalculatorSumResponse> Calculator(CalculatorSumRequest request, ServerCallContext context)
        {
            int result = request.A + request.B;

            return Task.FromResult(new CalculatorSumResponse() { Result = result });
        }

        public override async Task PrimeNumberDecomposition(PrimeNumberDecompositionRequest request, IServerStreamWriter<PrimeNumberDecompositionResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server received the request :");
            Console.WriteLine(request.ToString());

            int number = request.Number;
            int divisor = 2;

            while (number > 1)
            {
                if (number % divisor == 0)
                {
                    number /= divisor;
                    await responseStream.WriteAsync(new PrimeNumberDecompositionResponse() { Result = divisor });
                }
                else
                    divisor++;
            }
        }
    }
}