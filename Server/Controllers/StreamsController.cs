using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StreamsController : ControllerBase
    {
        private readonly ILogger<StreamsController> logger;

        public StreamsController(ILogger<StreamsController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("{count}")]
        public async Task GetAsync(int count, CancellationToken cancellationToken = default)
        {
            try
            {
                this.Response.StatusCode = (int)HttpStatusCode.Accepted;
                this.Response.Headers.Add(HeaderNames.ContentType, "text/event-stream");
                
                using var outputStreamWriter = new StreamWriter(this.Response.Body);

                await foreach (var item in GetData(count))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    logger.LogInformation($"Streaming {item}");

                    await outputStreamWriter.WriteLineAsync(item);
                    await outputStreamWriter.FlushAsync();
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning($"Operation Cancelled");
            }
        }

        private async IAsyncEnumerable<string> GetData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                await Task.Delay(new Random().Next(100, 1500));
                yield return await Task.FromResult(Guid.NewGuid().ToString());
            }
        }
    }
}
