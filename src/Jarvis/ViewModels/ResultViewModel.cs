// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Jarvis.Core;
using Jarvis.Services;
using JetBrains.Annotations;

namespace Jarvis.ViewModels
{
    public sealed class ResultViewModel : Screen
    {
        private readonly QueryProviderService _provider;
        private readonly ApplicationService _application;

        public BindableCollection<IQueryResult> Items { get; }
        public int SelectedResultIndex { get; set; }
        public IQueryResult SelectedResult { get; set; }

        public enum Selection
        {
            Previous = 0,
            Next = 1,
            First = 2,
            Last = 3
        }

        public ResultViewModel(QueryProviderService provider, ApplicationService application)
        {
            _provider = provider;
            _application = application;

            Items = new BindableCollection<IQueryResult>();
        }

        public async Task ExecuteQuery(string queryString)
        {
            // More than one character?
            if (queryString.Length > 0)
            {
                Items.IsNotifying = false;

                var query = new Query(queryString);
                var result = await _provider.Query(query).ConfigureAwait(false);

                // Remove items.
                for (var i = Items.Count - 1; i >= 0; i--)
                {
                    var current = Items[i];
                    if (!result.Contains(current))
                    {
                        Items.Remove(current);
                    }
                }

                // Add new items.
                foreach (var item in result)
                {
                    if (!Items.Contains(item))
                    {
                        Items.Add(item);
                    }
                    else
                    {
                        // Same item but higher score?
                        if (Math.Abs(Items[Items.IndexOf(item)].Score - item.Score) > 0.00001f)
                        {
                            Items.Remove(item);
                            Items.Add(item);
                        }
                    }
                }

                Items.IsNotifying = true;
                SelectedResultIndex = 0;
            }
            else
            {
                Items.IsNotifying = false;
                Items.Clear();
                Items.IsNotifying = true;
            }

            Items.Refresh();

            NotifyOfPropertyChange(nameof(Items));
            NotifyOfPropertyChange(nameof(SelectedResult));
            NotifyOfPropertyChange(nameof(SelectedResultIndex));
        }

        public void MoveSelection(Selection step)
        {
            if (step == Selection.Previous)
            {
                if (SelectedResultIndex > 0)
                {
                    SelectedResultIndex -= 1;
                    NotifyOfPropertyChange(nameof(SelectedResult));
                    NotifyOfPropertyChange(nameof(SelectedResultIndex));
                }
            }
            else if (step == Selection.Next)
            {
                if (SelectedResultIndex < Items.Count - 1)
                {
                    SelectedResultIndex += 1;
                    NotifyOfPropertyChange(nameof(SelectedResult));
                    NotifyOfPropertyChange(nameof(SelectedResultIndex));
                }
            }
            else if (step == Selection.First)
            {
                SelectedResultIndex = 0;
                NotifyOfPropertyChange(nameof(SelectedResult));
                NotifyOfPropertyChange(nameof(SelectedResultIndex));
            }
            else if (step == Selection.Last)
            {
                SelectedResultIndex = Math.Max(Items.Count - 1, 0);
                NotifyOfPropertyChange(nameof(SelectedResult));
                NotifyOfPropertyChange(nameof(SelectedResultIndex));
            }
        }

        [UsedImplicitly]
        public async Task OnItemClicked(IQueryResult item)
        {
            await ExecuteItem(item);
        }

        public async Task ExecuteSelected()
        {
            await ExecuteItem(SelectedResult);
        }

        private async Task ExecuteItem(IQueryResult result)
        {
            if (result != null)
            {
                await _provider.Execute(result);
                _application.Hide();
            }
        }
    }
}
