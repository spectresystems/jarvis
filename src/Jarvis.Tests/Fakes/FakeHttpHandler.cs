using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.Tests.Fakes
{
    public class FakeHttpHandler : HttpMessageHandler
    {
        public virtual HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("");
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request, cancellationToken));
        }
    }
}