// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Pen = System.Windows.Media.Pen;

namespace Jarvis.Addin.Files.Drawing
{
    internal static class UwpIconLoader
    {
        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/587e6164-65d7-4589-82e6-d1e5548e88a1/wpf-draw-graphics-to-imagesource?forum=wpf
        public static ImageSource DrawIcon(string path, string backgroundColor)
        {
            var source = new BitmapImage(new Uri(path.TrimStart('/')));
            var group = new DrawingGroup();

            // Draw the background.
            var color = GetColor(backgroundColor);
            var brush = new SolidColorBrush(color);
            var pen = new Pen(brush, 1);
            group.Children.Add(new GeometryDrawing(brush, pen,
                new RectangleGeometry(new Rect(0, 0, source.Width, source.Height))));

            // Add the icon to the group.
            var imageDrawing = new ImageDrawing(source, new Rect(0, 0, source.Width, source.Height));
            group.Children.Add(imageDrawing);

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                context.DrawDrawing(group);
            }

            var target = new RenderTargetBitmap((int)source.Width, (int)source.Height, 96, 96, PixelFormats.Pbgra32);
            target.Render(visual);
            return target;
        }

        private static Color GetColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color) ||
                color.Equals("Transparent", StringComparison.OrdinalIgnoreCase))
            {
                return SystemParameters.WindowGlassColor;
            }
            // ReSharper disable once PossibleNullReferenceException
            return (Color)ColorConverter.ConvertFromString(color);
        }
    }
}
