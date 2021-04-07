using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LayoutDemo.Views
{
    public class AdaptiveGridView : UserControl
    {
        public AdaptiveGridView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}