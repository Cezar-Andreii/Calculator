using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

public class CalculatorViewModel : INotifyPropertyChanged
{
    private string _displayText = "0";
    private double _currentValue = 0;
    private double _memory = 0;
    private List<double> _memoryStack = new List<double>();
    private string _operation;

    public string DisplayText
    {
        get => _displayText;
        set
        {
            _displayText = value;
            OnPropertyChanged(nameof(DisplayText));
        }
    }

    public ICommand DigitCommand { get; }
    public ICommand OperationCommand { get; }
    public ICommand EqualsCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ClearEntryCommand { get; }
    public ICommand BackspaceCommand { get; }
    public ICommand NegateCommand { get; }
    public ICommand DecimalPointCommand { get; }
    public ICommand MemoryClearCommand { get; }
    public ICommand MemoryRecallCommand { get; }
    public ICommand MemoryStoreCommand { get; }
    public ICommand MemoryAddCommand { get; }
    public ICommand MemorySubtractCommand { get; }
    public ICommand MemoryStackCommand { get; }

    public CalculatorViewModel()
    {
        DigitCommand = new RelayCommand(DigitPressed);
        OperationCommand = new RelayCommand(OperationPressed);
        EqualsCommand = new RelayCommand(EqualsPressed);
        ClearCommand = new RelayCommand(Clear);
        ClearEntryCommand = new RelayCommand(ClearEntry);
        BackspaceCommand = new RelayCommand(Backspace);
        NegateCommand = new RelayCommand(Negate);
        DecimalPointCommand = new RelayCommand(AddDecimalPoint);
        MemoryClearCommand = new RelayCommand(MemoryClear);
        MemoryRecallCommand = new RelayCommand(MemoryRecall);
        MemoryStoreCommand = new RelayCommand(MemoryStore);
        MemoryAddCommand = new RelayCommand(MemoryAdd);
        MemorySubtractCommand = new RelayCommand(MemorySubtract);
        MemoryStackCommand = new RelayCommand(ViewMemoryStack);
    }

    private void DigitPressed(object parameter)
    {
        string digit = parameter.ToString();
        if (DisplayText == "0") DisplayText = digit;
        else DisplayText += digit;
    }

    private void OperationPressed(object parameter)
    {
        _operation = parameter.ToString();
        _currentValue = double.Parse(DisplayText);
        DisplayText = "0";
    }

    private void EqualsPressed(object parameter)
    {
        double secondValue = double.Parse(DisplayText);
        double result = 0;

        switch (_operation)
        {
            case "+": result = _currentValue + secondValue; break;
            case "-": result = _currentValue - secondValue; break;
            case "*": result = _currentValue * secondValue; break;
            case "/": result = secondValue != 0 ? _currentValue / secondValue : double.NaN; break;
            case "%": result = _currentValue % secondValue; break;
            case "sqrt": result = Math.Sqrt(_currentValue); break;
            case "square": result = Math.Pow(_currentValue, 2); break;
            case "reciprocal": result = _currentValue != 0 ? 1 / _currentValue : double.NaN; break;
        }

        DisplayText = result.ToString();
    }

    private void Clear(object parameter) => DisplayText = "0";
    private void ClearEntry(object parameter) => DisplayText = "0";
    private void Backspace(object parameter) =>
        DisplayText = DisplayText.Length > 1 ? DisplayText.Substring(0, DisplayText.Length - 1) : "0";
    private void Negate(object parameter) => DisplayText = (-double.Parse(DisplayText)).ToString();
    private void AddDecimalPoint(object parameter) => DisplayText += ".";

    private void MemoryClear(object parameter) => _memory = 0;
    private void MemoryRecall(object parameter) => DisplayText = _memory.ToString();
    private void MemoryStore(object parameter) => _memory = double.Parse(DisplayText);
    private void MemoryAdd(object parameter) => _memory += double.Parse(DisplayText);
    private void MemorySubtract(object parameter) => _memory -= double.Parse(DisplayText);
    private void ViewMemoryStack(object parameter)
    {
        _memoryStack.Add(_memory);
        DisplayText = string.Join(", ", _memoryStack);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
