// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Caliburn.Micro;
using Jarvis.Core;
using Jarvis.Services;

namespace Jarvis.Infrastructure.Presentation.Controls
{
    public partial class IconControl
    {
        private static QueryProviderService _service;

        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register(
            nameof(Result), typeof(IQueryResult), typeof(IconControl), new PropertyMetadata(OnStringValueChanged));

        public IQueryResult Result
        {
            get => (IQueryResult)GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }

        private static async void OnStringValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is IconControl control)
            {
                if (_service == null)
                {
                    _service = IoC.Get<QueryProviderService>();
                }

                var icon = await _service.LoadIcon(e.NewValue as IQueryResult);
                if (icon != null)
                {
                    control.Image.Source = icon;
                }
            }
        }

        public IconControl()
        {
            InitializeComponent();
        }
    }
}
