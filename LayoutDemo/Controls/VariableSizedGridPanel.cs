// https://github.com/wasteam/waslibs/blob/master/src/AppStudio.Uwp/Controls/VariableSizedGrid/VariableSizedGridPanel.cs
using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace LayoutDemo.Controls
{
    public class VariableSizedGridPanel : Panel
    {
        private List<Rect> _cells;

        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<VariableSizedGridPanel, Orientation>(nameof(Orientation), Orientation.Horizontal);

        public static readonly StyledProperty<int> MaximumRowsOrColumnsProperty =
            AvaloniaProperty.Register<VariableSizedGridPanel, int>(nameof(MaximumRowsOrColumns), 0);

        public static readonly StyledProperty<double> AspectRatioProperty =
            AvaloniaProperty.Register<VariableSizedGridPanel, double>(nameof(AspectRatio), 1.0);

        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public int MaximumRowsOrColumns
        {
            get => GetValue(MaximumRowsOrColumnsProperty);
            set => SetValue(MaximumRowsOrColumnsProperty, value);
        }

        public double AspectRatio
        {
            get => GetValue(AspectRatioProperty);
            set => SetValue(AspectRatioProperty, value);
        }

        static VariableSizedGridPanel()
        {
            AffectsMeasure<VariableSizedGridPanel>(AspectRatioProperty, OrientationProperty, MaximumRowsOrColumnsProperty, AspectRatioProperty);
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count <= 0)
            {
                return base.MeasureOverride(availableSize);
            }
            
            _cells = new List<Rect>();

            double sizeWidth = availableSize.Width;
            double sizeHeight = availableSize.Height;

            if (double.IsInfinity(sizeWidth))
            {
                sizeWidth = Parent.Bounds.Width;
            }
            if (double.IsInfinity(sizeHeight))
            {
                sizeHeight = Parent.Bounds.Height;
            }

            double cw = sizeWidth / MaximumRowsOrColumns;
            double ch = cw * AspectRatio;
            if (Orientation == Orientation.Vertical)
            {
                ch = sizeHeight / MaximumRowsOrColumns;
                cw = ch / AspectRatio;
            }

            cw = Math.Round(cw);
            ch = Math.Round(ch);

            int n = 0;
            foreach (var item in Children)
            {
                int colSpan = 1;
                int rowSpan = 1;
                PrepareItem(n, item, ref colSpan, ref rowSpan);
                double w = cw * colSpan;
                double h = ch * rowSpan;
                GetNextPosition(_cells, new Size(cw, ch), new Size(w, h));
                item.Measure(new Size(w, h));
                n++;
            }

            return MeasureSize(_cells);

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count > 0)
            {
                int n = 0;
                foreach (var item in Children)
                {
                    var rect = _cells[n++];
                    item.Arrange(rect);
                }
                return MeasureSize(_cells);
            }
            return base.ArrangeOverride(finalSize);
        }

        private Rect GetNextPosition(List<Rect> cells, Size cellSize, Size itemSize)
        {
            if (Orientation == Orientation.Horizontal)
            {
                for (int y = 0; ; y++)
                {
                    for (int x = 0; x < MaximumRowsOrColumns; x++)
                    {
                        var rect = new Rect(new Point(x * cellSize.Width, y * cellSize.Height), itemSize);
                        if (RectFitInCells(rect, cells))
                        {
                            cells.Add(rect);
                            return rect;
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; ; x++)
                {
                    for (int y = 0; y < MaximumRowsOrColumns; y++)
                    {
                        var rect = new Rect(new Point(x * cellSize.Width, y * cellSize.Height), itemSize);
                        if (RectFitInCells(rect, cells))
                        {
                            cells.Add(rect);
                            return rect;
                        }
                    }
                }
            }
        }

        private static bool RectFitInCells(Rect rect, List<Rect> cells)
        {
            return !cells.Any(r => !(r.Left >= rect.Right || r.Right <= rect.Left || r.Top >= rect.Bottom || r.Bottom <= rect.Top));
        }

        protected virtual void PrepareItem(int index, IControl element, ref int colSpan, ref int rowSpan)
        {
            colSpan = index % 3 == 0 ? 2 : 1;
            rowSpan = index % 3 == 0 ? 2 : 1;
        }

        private static Size MeasureSize(List<Rect> cells)
        {
            double mx = cells.Max(r => r.Right);
            double my = cells.Max(r => r.Bottom);
            return new Size(mx, my);
        }
    }
}
