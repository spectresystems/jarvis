using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Autofac;
using Autofac.Core;
using Jarvis.Addin.StackExchange.AskUbuntu;
using Jarvis.Addin.StackExchange.Common;
using Jarvis.Addin.StackExchange.StackOverflow;
using Jarvis.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jarvis.Addin.StackExchange
{
    public class StackExchangeAddin : IAddin
    {
        public void Configure(ContainerBuilder builder)
        {
            builder
                .Register(StackExchangeHttpClient)
                .Named(nameof(StackExchangeHttpClient), typeof(HttpClient))
                .SingleInstance();

            builder
                .RegisterType<QueryStringBuilder>()
                .AsImplementedInterfaces();

            builder
                .Register(StackExchangeJsonDeserializer)
                .Named(nameof(StackExchangeJsonDeserializer), typeof(JsonSerializer))
                .SingleInstance();
            
            builder
                .RegisterType<StackExchangeClient>()
                .WithParameters(new []
                {
                    new ResolvedParameter(
                        (info, context) => info.ParameterType == typeof(HttpClient),
                        (info, context) => context.ResolveNamed<HttpClient>(nameof(StackExchangeHttpClient))),
                    new ResolvedParameter(
                        (info, context) => info.ParameterType == typeof(JsonSerializer),
                        (info, context) => context.ResolveNamed<JsonSerializer>(nameof(StackExchangeJsonDeserializer)))
                })
                .AsImplementedInterfaces();

            builder
                .RegisterType<StackOverflowProvider>()
                .As<IQueryProvider>()
                .SingleInstance();

            builder
                .RegisterType<AskUbuntuProvider>()
                .As<IQueryProvider>()
                .SingleInstance();
        }

        private static HttpClient StackExchangeHttpClient(IComponentContext context)
        {
            var decompressingClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };

            return new HttpClient(decompressingClientHandler)
            {
                BaseAddress = new Uri("https://api.stackexchange.com/2.2/search"),
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")}
                }
            };
        }

        private static JsonSerializer StackExchangeJsonDeserializer(IComponentContext context)
        {
            return new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Converters = { new EpochJsonConverter() }
            };
        }
    }
}
