using System;
using Avalonia;
using Avalonia.Controls;

namespace LayoutDemo
{
    public class AdaptivePanel : Panel
    {
        private void MeasureArrange(Size panelSize, bool isMeasure)
        {
            double aspectRatio = 0.5;
            double twoColumnsTriggerWidth = 500;

            var columns = panelSize.Width < twoColumnsTriggerWidth ? 2 : 3;
            var columnWidth = panelSize.Width / columns;
            var totalWidth = Children.Count * columnWidth;
            var rows = (int)Math.Ceiling(totalWidth / panelSize.Width);
            var itemHeight = columnWidth * aspectRatio;

            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (index >= Children.Count)
                    {
                        break;
                    }

                    var position = new Point(column * columnWidth, row * itemHeight);
                    var size = new Size(columnWidth, itemHeight);
                    var rect = new Rect(position, size);
                    var child = Children[index];
                    if (isMeasure)
                    {
                        child.Measure(size);
                    }
                    else
                    {
                        child.Arrange(rect);
                    }
                    index++;
                }
                
            }
        }
 
        protected override Size MeasureOverride(Size availableSize)
        {
            MeasureArrange(availableSize, true);

            return new Size(availableSize.Width, availableSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            MeasureArrange(finalSize, false);

            return finalSize;
        }
    }
}