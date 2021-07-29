using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ReadFromStreamAsync(GetCancellationToken());
        }

        static async Task ReadFromStreamAsync(CancellationToken token)
        {
            using var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync("https://localhost:7001/streams/300", token);
            try
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        Console.Out.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        static CancellationToken GetCancellationToken()
        {
            var tokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => 
            {
                e.Cancel = false;
                tokenSource.Cancel();
            };
            return tokenSource.Token;
        }
    }
}
