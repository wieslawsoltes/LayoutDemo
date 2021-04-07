using System;
using Avalonia;
using Avalonia.Controls;

namespace LayoutDemo.Controls
{
    public class VanishingPointPanel : Panel
    {
        public static readonly StyledProperty<double> ZFactorProperty =
            AvaloniaProperty.Register<VanishingPointPanel, double>(nameof(ZFactor), 1.0);

        public static readonly StyledProperty<double> ItemHeightProperty =
            AvaloniaProperty.Register<VanishingPointPanel, double>(nameof(ItemHeight));

        public double ZFactor
        {
            get => GetValue(ZFactorProperty);
            set => SetValue(ZFactorProperty, value);
        }

        public double ItemHeight
        {
            get => GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        static VanishingPointPanel()
        {
            AffectsMeasure<VanishingPointPanel>(ZFactorProperty, ItemHeightProperty);
        }
        
        private Rect CalculateRect(Size panelSize, int index)
        {
            var zFactor = Math.Pow(ZFactor, index);
            var itemSize = new Size(panelSize.Width * zFactor, ItemHeight * zFactor);
            var left = (panelSize.Width - itemSize.Width) * 0.5;
            var top = panelSize.Height;

            for (var i = 0; i <= index; i++)
            {
                top -= Math.Pow(ZFactor, i) * ItemHeight;
            }

            var rect = new Rect(new Point(left, top), itemSize);
            return rect;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                var childSize = new Size(availableSize.Width, ItemHeight);
                child.Measure(childSize);
            }

            return new Size(availableSize.Width, ItemHeight * Children.Count);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var currentIndex = 0;
            
            for (var index = Children.Count - 1; index >= 0; index--)
            {
                var rect = CalculateRect(finalSize, currentIndex);
                Children[index].Arrange(rect);
                currentIndex++;
            }

            return finalSize;
        }
    }
}