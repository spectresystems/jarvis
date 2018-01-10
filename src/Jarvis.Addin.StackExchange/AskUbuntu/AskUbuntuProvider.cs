using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Addin.StackExchange.Common;
using Jarvis.Addin.StackExchange.Common.QueryLanguage;

namespace Jarvis.Addin.StackExchange.AskUbuntu
{
    public class AskUbuntuProvider : StackExchangeProvider<AskUbuntuResult>
    {
        private readonly ImageSource _icon;
        public override string Command => "au";
        protected override string Site => StackExchangeSites.AskUbuntu;

        protected override Task<ImageSource> GetIconAsync(AskUbuntuResult result)
        {
            return Task.FromResult(_icon);
        }

        public AskUbuntuProvider(IStackExchangeClient stackExchangeClient, IQueryParser<SearchQuery> queryParser, IQuestionDescriptionFactory descriptionFactory)
            : base(stackExchangeClient, queryParser, descriptionFactory)
        {
            _icon = new BitmapImage(new Uri("pack://application:,,,/Jarvis.Addin.StackExchange;component/Resources/AskUbuntu.png"));
        }
    }
}
