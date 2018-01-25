using Autofac;
using Jarvis.Core;

namespace Jarvis.Addin.Processes
{
    public sealed class ProcessAddin : IAddin
    {
        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<ProcessProvider>().As<IQueryProvider>().SingleInstance();
        }
    }
}