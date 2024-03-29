﻿using System;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;

namespace LayoutDemo.Controls
{
    public class ResponsivePanel : Panel
    {
        public static readonly StyledProperty<double> AspectRatioProperty =
            AvaloniaProperty.Register<ResponsivePanel, double>(nameof(AspectRatio), double.NaN);

        public static readonly StyledProperty<AvaloniaList<int>> ColumnHintsProperty =
            AvaloniaProperty.Register<ResponsivePanel, AvaloniaList<int>>(nameof(ColumnHints), new AvaloniaList<int>() { 1 });

        public static readonly StyledProperty<AvaloniaList<double>> WidthTriggersProperty =
            AvaloniaProperty.Register<ResponsivePanel, AvaloniaList<double>>(nameof(WidthTriggers), new AvaloniaList<double>() { 0.0 });

        public static readonly AttachedProperty<AvaloniaList<int>> ColumnSpanProperty =
            AvaloniaProperty.RegisterAttached<ResponsivePanel, Control, AvaloniaList<int>>("ColumnSpan", new AvaloniaList<int>() { 1 });

        public static readonly AttachedProperty<AvaloniaList<int>> RowSpanProperty =
            AvaloniaProperty.RegisterAttached<ResponsivePanel, Control, AvaloniaList<int>>("RowSpan", new AvaloniaList<int>() { 1 });

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

        public AvaloniaList<int> ColumnHints
        {
            get => GetValue(ColumnHintsProperty);
            set => SetValue(ColumnHintsProperty, value);
        }

        public AvaloniaList<double> WidthTriggers
        {
            get => GetValue(WidthTriggersProperty);
            set => SetValue(WidthTriggersProperty, value);
        }

        static ResponsivePanel()
        {
            AffectsParentMeasure<ResponsivePanel>(AspectRatioProperty, ColumnHintsProperty, WidthTriggersProperty, ColumnSpanProperty, RowSpanProperty);
            AffectsParentArrange<ResponsivePanel>(AspectRatioProperty, ColumnHintsProperty, WidthTriggersProperty, ColumnSpanProperty, RowSpanProperty);
            AffectsMeasure<ResponsivePanel>(AspectRatioProperty, ColumnHintsProperty, WidthTriggersProperty, ColumnSpanProperty, RowSpanProperty);
            AffectsArrange<ResponsivePanel>(AspectRatioProperty, ColumnHintsProperty, WidthTriggersProperty, ColumnSpanProperty, RowSpanProperty);
        }

        private struct Item
        {
            public int Column;
            public int Row;
            public int ColumnSpan;
            public int RowSpan;
        }

        private Size MeasureArrange(Size panelSize, bool isMeasure)
        {
            var children = Children;
            var widthTriggers = WidthTriggers;
            var columnHints = ColumnHints;
            var aspectRatio = AspectRatio;
            var width = panelSize.Width;
            var height = panelSize.Height;

            if (widthTriggers.Count <= 0)
            {
                throw new Exception($"No width trigger specified in {nameof(WidthTriggers)} property.");
            }

            if (columnHints.Count <= 0)
            {
                throw new Exception($"No column hints specified in {nameof(ColumnHints)} property.");
            }

            if (widthTriggers.Count != columnHints.Count)
            {
                throw new Exception($"Number of width triggers must be equal to the number of column triggers.");
            }

            if (double.IsNaN(aspectRatio))
            {
                if (height == 0 || double.IsInfinity(height))
                {
                    aspectRatio = 1.0;
                }
                // else
                // {
                //     aspectRatio = Math.Min(height, width) / Math.Max(height, width);
                // }
            }

            var totalColumns = 1;
            var layoutIndex = 0;

            for (var i = 0; i < widthTriggers.Count; i++)
            {
                if (width > widthTriggers[i])
                {
                    totalColumns = columnHints[i];
                    layoutIndex = i;
                }
            }

            var currentColumn = 0;
            var totalRows = 0;
            var rowIncrement = 1;
            var items = new Item[children.Count];

            for (var i = 0; i < children.Count; i++)
            {
                var element = children[i];
                var columnSpan = GetColumnSpan((Control) element)[layoutIndex];
                var rowSpan = GetRowSpan((Control) element)[layoutIndex];

                items[i] = new Item()
                {
                    Column = currentColumn,
                    Row = totalRows,
                    ColumnSpan = columnSpan,
                    RowSpan = rowSpan
                };

                rowIncrement = Math.Max(rowSpan, rowIncrement);
                currentColumn += columnSpan;

                if (currentColumn >= totalColumns)
                {
                    currentColumn = 0;
                    totalRows += rowIncrement;
                    rowIncrement = 1;
                }
            }

            var itemWidth = width / totalColumns;
            var itemHeight = double.IsNaN(aspectRatio) ? height/ totalRows : itemWidth * aspectRatio;

            for (var i = 0; i < children.Count; i++)
            {
                var element = children[i];
                var size = new Size(itemWidth * items[i].ColumnSpan, itemHeight * items[i].RowSpan);
                var position = new Point(items[i].Column * itemWidth, items[i].Row * itemHeight);
                var rect = new Rect(position, size);

                if (isMeasure)
                {
                    element.Measure(size);
                }
                else
                {
                    element.Arrange(rect);
                }
            }

            return new Size(width, itemHeight * totalRows);
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