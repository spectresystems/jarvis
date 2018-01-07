using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Addin.StackExchange.Common;

namespace Jarvis.Addin.StackExchange.StackOverflow
{
    public class StackOverflowProvider : StackExchangeProvider<StackOverflowResult>
    {
        private readonly ImageSource _icon;
        public override string Command => "so";

        public StackOverflowProvider(IStackExchangeClient stackExchangeClient) : base(stackExchangeClient)
        {
            _icon = new BitmapImage(new Uri("pack://application:,,,/Jarvis.Addin.StackExchange;component/Resources/StackOverflow.png"));
        }

        protected override Task<ImageSource> GetIconAsync(StackOverflowResult result)
        {
            return Task.FromResult(_icon);
        }

        protected override string Site => StackExchangeSites.StackOverflow;
    }
}
