using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Addin.StackExchange.Common;
using Jarvis.Addin.StackExchange.Common.QueryLanguage;

namespace Jarvis.Addin.StackExchange.StackOverflow
{
    public class StackOverflowProvider : StackExchangeProvider<StackOverflowResult>
    {
        private readonly Lazy<ImageSource> _icon;
        public override string Command => "so";

        public StackOverflowProvider(IStackExchangeClient stackExchangeClient, IQueryParser<SearchQuery> queryParser, IQuestionDescriptionFactory descriptionFactory)
            : base(stackExchangeClient, queryParser, descriptionFactory)
        {
            _icon = new Lazy<ImageSource>(() =>
                    new BitmapImage(new Uri("pack://application:,,,/Jarvis.Addin.StackExchange;component/Resources/StackOverflow.png"))
            );
        }

        protected override Task<ImageSource> GetIconAsync(StackOverflowResult result)
        {
            return Task.FromResult(_icon.Value);
        }

        protected override string Site => StackExchangeSites.StackOverflow;
    }
}
