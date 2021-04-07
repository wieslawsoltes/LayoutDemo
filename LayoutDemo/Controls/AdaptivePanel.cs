using System;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;

namespace LayoutDemo
{
    public class AdaptivePanel : Panel
    {
        public static readonly StyledProperty<double> AspectRatioProperty =
            AvaloniaProperty.Register<AdaptivePanel, double>(nameof(AspectRatio), double.NaN);

        public static readonly StyledProperty<AvaloniaList<int>> ColumnsProperty =
            AvaloniaProperty.Register<AdaptivePanel, AvaloniaList<int>>(nameof(Columns), new AvaloniaList<int>() { 1 });

        public static readonly StyledProperty<AvaloniaList<double>> TriggersProperty =
            AvaloniaProperty.Register<AdaptivePanel, AvaloniaList<double>>(nameof(Triggers), new AvaloniaList<double>() { 0.0 });

        public static readonly AttachedProperty<AvaloniaList<int>> ColumnSpanProperty =
            AvaloniaProperty.RegisterAttached<AdaptivePanel, Control, AvaloniaList<int>>("ColumnSpan", new AvaloniaList<int>() { 1 });

        public static readonly AttachedProperty<AvaloniaList<int>> RowSpanProperty =
            AvaloniaProperty.RegisterAttached<AdaptivePanel, Control, AvaloniaList<int>>("RowSpan", new AvaloniaList<int>() { 1 });

        public static AvaloniaList<int> GetColumnSpan(Control? element)
        {
            Contract.Requires<ArgumentNullException>(element != null);
            return element!.GetValue(ColumnSpanProperty);
        }

        public static void SetColumnSpan(Control? element, AvaloniaList<int> value)
        {
            Contract.Requires<ArgumentNullException>(element != null);
            element!.SetValue(ColumnSpanProperty, value);
        }

        public static AvaloniaList<int> GetRowSpan(Control? element)
        {
            Contract.Requires<ArgumentNullException>(element != null);
            return element!.GetValue(RowSpanProperty);
        }

        public static void SetRowSpan(Control? element, AvaloniaList<int> value)
        {
            Contract.Requires<ArgumentNullException>(element != null);
            element!.SetValue(RowSpanProperty, value);
        }

        public double AspectRatio
        {
            get => GetValue(AspectRatioProperty);
            set => SetValue(AspectRatioProperty, value);
        }

        public AvaloniaList<int> Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public AvaloniaList<double> Triggers
        {
            get => GetValue(TriggersProperty);
            set => SetValue(TriggersProperty, value);
        }

        static AdaptivePanel()
        {
            AffectsParentMeasure<AdaptivePanel>(AspectRatioProperty, ColumnsProperty, TriggersProperty, ColumnSpanProperty, RowSpanProperty);
            AffectsParentArrange<AdaptivePanel>(AspectRatioProperty, ColumnsProperty, TriggersProperty, ColumnSpanProperty, RowSpanProperty);
            AffectsMeasure<AdaptivePanel>(AspectRatioProperty, ColumnsProperty, TriggersProperty, ColumnSpanProperty, RowSpanProperty);
            AffectsArrange<AdaptivePanel>(AspectRatioProperty, ColumnsProperty, TriggersProperty, ColumnSpanProperty, RowSpanProperty);
        }

        private Size MeasureArrange(Size panelSize, bool isMeasure)
        {
            var aspectRatio = AspectRatio;
            var columnsNum = 1;
            var layoutId = 0;

            if (double.IsNaN(aspectRatio))
            {
                if (panelSize.Height == 0 || double.IsInfinity(panelSize.Height))
                {
                    aspectRatio = 1.0;
                }
                else
                {
                    var min = Math.Min(panelSize.Height, panelSize.Width);
                    var max = Math.Max(panelSize.Height, panelSize.Width);
                    aspectRatio = min / max;
                }
            }

            for (var i = 0; i < Triggers.Count; i++)
            {
                var trigger = Triggers[i];
                var columns = Columns[i];

                if (panelSize.Width > trigger)
                {
                    columnsNum = columns;
                    layoutId = i;
                }
            }

            var columnWidth = panelSize.Width / columnsNum;
            var itemHeight = columnWidth * aspectRatio;
            var column = 0;
            var row = 0;
            var rowIncrement = 1;

            for (var index = 0; index < Children.Count; index++)
            {
                var element = Children[index];
                var columnSpan = GetColumnSpan((Control) element)[layoutId];
                var rowSpan = GetRowSpan((Control) element)[layoutId];
                var position = new Point(column * columnWidth, row * itemHeight);
                var size = new Size(columnWidth * columnSpan, itemHeight * rowSpan);
                var rect = new Rect(position, size);

                rowIncrement = Math.Max(rowSpan, rowIncrement);
                column += columnSpan;

                if (column >= columnsNum)
                {
                    column = 0;
                    row += rowIncrement;
                    rowIncrement = 1;
                }

                if (isMeasure)
                {
                    element.Measure(size);
                }
                else
                {
                    element.Arrange(rect);
                }
            }

            return new Size(panelSize.Width, itemHeight * row);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var measureSize = MeasureArrange(availableSize, true);
            // return new Size(availableSize.Width, availableSize.Height);
            // Console.WriteLine($"MeasureOverride: {measureSize}");
            return measureSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var arrangeSize = MeasureArrange(finalSize, false);
            // return finalSize;
            return arrangeSize;
        }
    }
}