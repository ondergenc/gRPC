using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator;
using Grpc.Core;
using Grpc.Core.Utils;
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

        public override async Task<ComputeAverageResponse> ComputeAverage(IAsyncStreamReader<ComputeAverageRequest> requestStream, ServerCallContext context)
        {
            double sum = 0;
            int count = 0;
            while(await requestStream.MoveNext())
            {
                sum += requestStream.Current.Number;
                count++;
            }

            return await Task.FromResult(new ComputeAverageResponse() { Result = sum / count });
        }

        public override async Task FindMaximum(IAsyncStreamReader<FindMaxRequest> requestStream, IServerStreamWriter<FindMaxResponse> responseStream, ServerCallContext context)
        {
            int? max = null;
            while (await requestStream.MoveNext())
            {
                if (max == null || max < requestStream.Current.Number)
                {
                    max = requestStream.Current.Number;
                    await responseStream.WriteAsync(new FindMaxResponse() { Max = max.Value });
                }
            }
        }
    }
}