
using Autofac;
using Jarvis.Core;

namespace Jarvis.Addin.Windows
{
    public class WindowsAddin : IAddin
    {
        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<WindowsProvider>().As<IQueryProvider>().SingleInstance();
        }
    }
}