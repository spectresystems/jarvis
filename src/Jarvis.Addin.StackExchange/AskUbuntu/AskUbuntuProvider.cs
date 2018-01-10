using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Addin.StackExchange.Common;
using Jarvis.Addin.StackExchange.Common.QueryLanguage;
using Jarvis.Core;

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

        protected override IEnumerable<AskUbuntuResult> CreateFallbackResult(SearchQuery query)
        {
            var url = new Uri($"https://askubuntu.com/search?q={query.InTitle.Replace(' ', '_')}");
            yield return new AskUbuntuResult
            {
                Title = $"Search Ask Ubuntu for '{query.InTitle}'",
                Uri = url,
                Description = url.ToString(),
                Type = QueryResultType.Other,
                Distance = 0,
                Score = 0
            };
        }
    }
}
