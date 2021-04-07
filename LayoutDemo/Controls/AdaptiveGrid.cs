using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;

namespace LayoutDemo.Controls
{
    public class AdaptiveGrid : Grid
    {
        private static string ToString(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public AdaptiveGrid()
        {
            this.GetObservable(BoundsProperty).Subscribe(InitializeGrid);
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

        private void InitializeGrid(Rect rect)
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
                var rowDefinitionsStr = $"{ToString(itemHeight)},{ToString(itemHeight)}";

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
                var rowDefinitionsStr = $"{ToString(itemHeight)},{ToString(itemHeight)}";

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
}