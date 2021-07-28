using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var command = new RootCommand();
            command.Handler = CommandHandler.Create(async (IConsole console, CancellationToken token) =>
            {
                try
                {
                    await ReadFromStream(console, token);
                    return 0;
                }
                catch (OperationCanceledException)
                {
                    console.Error.WriteLine("The operation was aborted");
                    return 1;
                }
            });

            return await command.InvokeAsync(args);
        }

        private static async Task ReadFromStream(IConsole console, CancellationToken token)
        {
            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync("https://localhost:7001/streams/300", token);
            try
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        console.Out.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
