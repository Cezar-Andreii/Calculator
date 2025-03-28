﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CalculatorViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new CalculatorViewModel();
            DataContext = viewModel;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (viewModel.KeyPressCommand.CanExecute(e))
            {
                viewModel.KeyPressCommand.Execute(e);
                e.Handled = true;
            }
        }
    }
}