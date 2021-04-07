﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LayoutDemo.Views
{
    public class WeightedPanelView : UserControl
    {
        public WeightedPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}