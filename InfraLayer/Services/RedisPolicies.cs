using Polly;
using Polly.Retry;
using System.Net.Sockets;
using Polly.Contrib.WaitAndRetry;
using Microsoft.Extensions.Logging;

namespace InfraLayer.Services
{
    public static class RedisPolicies
    {
        public static AsyncRetryPolicy RetryPolicy => Policy.Handle<SocketException>() 
          .Or<TimeoutException>()  
          .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3)); 
        public static AsyncPolicy FallbackPolicy(ILogger logger) => Policy.Handle<Exception>() 
            .FallbackAsync(async (ct) =>
            {
                logger.LogError("Fallback to DB");
            });
    }
}
