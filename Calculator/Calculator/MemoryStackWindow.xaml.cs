using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Calculator
{
    public partial class MemoryStackWindow : Window
    {
        public MemoryStackWindow(ObservableCollection<decimal> memoryStack, decimal selectedValue = 0)
        {
            InitializeComponent();
            DataContext = new MemoryStackViewModel(memoryStack, selectedValue, this);
        }
    }

    public class MemoryStackViewModel : INotifyPropertyChanged
    {
        private readonly Window _window;
        public ObservableCollection<decimal> MemoryStack { get; }

        private decimal selectedMemoryValue;
        public decimal SelectedMemoryValue
        {
            get => selectedMemoryValue;
            set
            {
                if (selectedMemoryValue != value)
                {
                    selectedMemoryValue = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public RelayCommand UseMemoryValueCommand { get; }
        public RelayCommand ClearMemoryCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MemoryStackViewModel(ObservableCollection<decimal> memoryStack, decimal selectedValue, Window window)
        {
            _window = window;
            MemoryStack = memoryStack;
            SelectedMemoryValue = selectedValue;

            UseMemoryValueCommand = new RelayCommand(UseMemoryValue, CanUseMemoryValue);
            ClearMemoryCommand = new RelayCommand(ClearMemory);
        }

        private bool CanUseMemoryValue(object parameter)
        {
            return SelectedMemoryValue != 0 || (MemoryStack.Count > 0 && MemoryStack[0] == 0);
        }

        private void UseMemoryValue(object parameter)
        {
            _window.DialogResult = true;
            _window.Close();
        }

        private void ClearMemory(object parameter)
        {
            MemoryStack.Clear();
            SelectedMemoryValue = 0;
        }
    }
}