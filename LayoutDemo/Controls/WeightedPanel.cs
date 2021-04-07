using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace LayoutDemo.Controls
{
    public class WeightedPanel : Panel
    {
        private double[] _normalWeights;

        public static readonly AttachedProperty<double> WeightProperty =
            AvaloniaProperty.RegisterAttached<WeightedPanel, IControl, double>("Weight", 1.0);

        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<WeightedPanel, Orientation>("Orientation");

        public static double GetWeight(IControl control)
        {
            return control.GetValue(WeightProperty);
        }

        public static void SetWeight(IControl control, double value)
        {
            control.SetValue(WeightProperty, value);
        }

        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        static WeightedPanel()
        {
            AffectsParentMeasure<WeightedPanel>(WeightProperty);
            AffectsParentArrange<WeightedPanel>(WeightProperty);
            AffectsMeasure<WeightedPanel>(WeightProperty);
            AffectsArrange<WeightedPanel>(WeightProperty);
        }

        private void NormalizeWeights()
        {
            // Calculate total weight

            double weightSum = 0;

            foreach (var child in Children)
            {
                weightSum += child.GetValue(WeightProperty);
            }

            // Normalize each weight

            _normalWeights = new double[Children.Count];

            for (var i = 0; i < Children.Count; i++)
            {
                _normalWeights[i] = Children[i].GetValue(WeightProperty) / weightSum;
            }
        }

        private Rect[] CalculateItemRects(Size panelSize)
        {
            NormalizeWeights();

            var rects = new Rect[Children.Count];
            double offset = 0;

            for (var i = 0; i < Children.Count; i++)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    var width = panelSize.Width * _normalWeights[i];
                    rects[i] = new Rect(offset, 0, width, panelSize.Height);
                    offset += width;
                }
                else if (Orientation == Orientation.Vertical)
                {
                    var height = panelSize.Height * _normalWeights[i];
                    rects[i] = new Rect(0, offset, panelSize.Width, height);
                    offset += height;
                }
            }

            return rects;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var rects = CalculateItemRects(availableSize);

            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].Measure(rects[i].Size);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var rects = CalculateItemRects(finalSize);

            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].Arrange(rects[i]);
            }

            return finalSize;
        }
    }
}