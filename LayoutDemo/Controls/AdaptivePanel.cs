using System;
using System.Globalization;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;

namespace LayoutDemo
{
    public class AdaptiveGrid : Grid
    {
        private string D2S(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        
        public AdaptiveGrid()
        {
            this.GetObservable(BoundsProperty).Subscribe(x => Init(x));
        }

        private void SetRowDefinitions(RowDefinitions rowDefinitions)
        {
            RowDefinitions.Clear();

            foreach (var rowDefinition in rowDefinitions)
            {
                RowDefinitions.Add(rowDefinition);
            }
        }
        
        private void SetColumnDefinitions(ColumnDefinitions columnDefinitions)
        {
            ColumnDefinitions.Clear();

            foreach (var columnDefinition in columnDefinitions)
            {
                ColumnDefinitions.Add(columnDefinition);
            }
        }

        private void Init(Rect rect)
        {
            Console.WriteLine($"Size {rect.Size}");
            if (rect.Size.Width <= 0)
            {
                Console.WriteLine($"ColumnDefinitions {ColumnDefinitions}");
                Console.WriteLine($"RowDefinitions {RowDefinitions}");
                return;
            }

            double twoColumnsTriggerWidth = 500;
            double aspectRatio = 0.5;

            if (rect.Size.Width < twoColumnsTriggerWidth)
            {
                var columnDefinitionsStr = "1*,1*";
                
                var columnWidth = rect.Size.Width / 2;
                var itemHeight = columnWidth * aspectRatio;
                var rowDefinitionsStr = $"{D2S(itemHeight)},{D2S(itemHeight)}";
                
                ColumnDefinitions = ColumnDefinitions.Parse(columnDefinitionsStr);
                RowDefinitions = RowDefinitions.Parse(rowDefinitionsStr);
                //var columnDefinitions = ColumnDefinitions.Parse(columnDefinitionsStr);
                //var rowDefinitions = RowDefinitions.Parse(rowDefinitionsStr);
                //SetColumnDefinitions(columnDefinitions);
                //SetRowDefinitions(rowDefinitions);
                //InvalidateMeasure();
                
                Console.WriteLine($"ColumnDefinitions {columnDefinitionsStr}");
                Console.WriteLine($"RowDefinitions {rowDefinitionsStr}");
            }
            else
            {
                var columnDefinitionsStr = "1*,1*,1*";
                
                var columnWidth = rect.Size.Width / 3;
                var itemHeight = columnWidth * aspectRatio;
                var rowDefinitionsStr = $"{D2S(itemHeight)},{D2S(itemHeight)}";
                
                ColumnDefinitions = ColumnDefinitions.Parse(columnDefinitionsStr);
                RowDefinitions = RowDefinitions.Parse(rowDefinitionsStr);
                //var columnDefinitions = ColumnDefinitions.Parse(columnDefinitionsStr);
                //var rowDefinitions = RowDefinitions.Parse(rowDefinitionsStr);
                //SetColumnDefinitions(columnDefinitions);
                //SetRowDefinitions(rowDefinitions);
                //InvalidateMeasure();
                
                Console.WriteLine($"ColumnDefinitions {columnDefinitionsStr}");
                Console.WriteLine($"RowDefinitions {rowDefinitionsStr}");
            }
        }
    }

    public class AdaptivePanel : Panel
    {
        public static readonly StyledProperty<double> AspectRatioProperty =
            AvaloniaProperty.Register<AdaptivePanel, double>(nameof(AspectRatio), 0.5);

        public static readonly StyledProperty<AvaloniaList<int>> ColumnsProperty =
            AvaloniaProperty.Register<AdaptivePanel, AvaloniaList<int>>(nameof(Columns), new AvaloniaList<int>() { 1 });

        public static readonly StyledProperty<AvaloniaList<double>> TriggersProperty =
            AvaloniaProperty.Register<AdaptivePanel, AvaloniaList<double>>(nameof(Triggers), new AvaloniaList<double>() { 0.0 });

        public static readonly AttachedProperty<AvaloniaList<int>> ColumnSpanProperty =
            AvaloniaProperty.RegisterAttached<AdaptivePanel, Control, AvaloniaList<int>>("ColumnSpan", new AvaloniaList<int>() { 1 });

        public static readonly AttachedProperty<AvaloniaList<int>> RowSpanProperty =
            AvaloniaProperty.RegisterAttached<AdaptivePanel, Control, AvaloniaList<int>>("RowSpan", new AvaloniaList<int>() { 1 });

        public static AvaloniaList<int> GetColumnSpan(Control element)
        {
            Contract.Requires<ArgumentNullException>(element != null);
            return element!.GetValue(ColumnSpanProperty);
        }

        public static void SetColumnSpan(Control element, AvaloniaList<int> value)
        {
            Contract.Requires<ArgumentNullException>(element != null);
            element!.SetValue(ColumnSpanProperty, value);
        }

        public static AvaloniaList<int> GetRowSpan(Control element)
        {
            Contract.Requires<ArgumentNullException>(element != null);
            return element!.GetValue(RowSpanProperty);
        }

        public static void SetRowSpan(Control element, AvaloniaList<int> value)
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

        private Size MeasureArrange(Size panelSize, bool isMeasure)
        {
            double aspectRatio = AspectRatio;

            var columnsNum = 1;
            var layoutId = 0;

            for (int i = 0; i < Triggers.Count; i++)
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
            int column = 0;
            int row = 0;
            int rowIncrement = 1;

            for (int index = 0; index < Children.Count; index++)
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
            MeasureArrange(finalSize, false);
            return finalSize;
        }
    }
}