using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Calculator
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly SettingsService settingsService;
        private readonly NumberBaseConverter numberBaseConverter;
        private string displayText = "0";
        private decimal currentValue = 0;
        private decimal? storedValue = null;
        private string pendingOperation = null;
        private bool isNewNumber = true;
        private ObservableCollection<decimal> memoryStack;
        private decimal memoryValue = 0;
        private bool digitGroupingEnabled;
        private CalculatorMode currentMode;
        private NumberBase currentBase;
        private string operationDisplay = "";
        private string clipboardContent = "";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CalculatorViewModel()
        {
            settingsService = new SettingsService();
            numberBaseConverter = new NumberBaseConverter();
            memoryStack = new ObservableCollection<decimal>();
            LoadSettings();
            InitializeCommands();
        }

        public string DisplayText
        {
            get => displayText;
            set
            {
                if (displayText != value)
                {
                    displayText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DigitGroupingEnabled
        {
            get => digitGroupingEnabled;
            set
            {
                if (digitGroupingEnabled != value)
                {
                    digitGroupingEnabled = value;
                    OnPropertyChanged();
                    SaveSettings();
                    UpdateDisplay();
                }
            }
        }

        public CalculatorMode CurrentMode
        {
            get => currentMode;
            set
            {
                if (currentMode != value)
                {
                    currentMode = value;
                    SaveSettings();
                    OnPropertyChanged();
                }
            }
        }

        public NumberBase CurrentBase
        {
            get => currentBase;
            set
            {
                if (currentBase != value)
                {
                    currentBase = value;
                    SaveSettings();
                    UpdateDisplay();
                    OnPropertyChanged();
                }
            }
        }

        public string OperationDisplay
        {
            get => operationDisplay;
            set
            {
                if (operationDisplay != value)
                {
                    operationDisplay = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand DigitCommand { get; private set; }
        public ICommand OperationCommand { get; private set; }
        public ICommand EqualsCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand ClearEntryCommand { get; private set; }
        public ICommand BackspaceCommand { get; private set; }
        public ICommand NegateCommand { get; private set; }
        public ICommand DecimalPointCommand { get; private set; }
        public ICommand MemoryClearCommand { get; private set; }
        public ICommand MemoryRecallCommand { get; private set; }
        public ICommand MemoryStoreCommand { get; private set; }
        public ICommand MemoryAddCommand { get; private set; }
        public ICommand MemorySubtractCommand { get; private set; }
        public ICommand MemoryStackCommand { get; private set; }
        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand DigitGroupingCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }
        public ICommand ChangeModeCommand { get; private set; }
        public ICommand ChangeBaseCommand { get; private set; }
        public ICommand KeyPressCommand { get; private set; }

        private void InitializeCommands()
        {
            DigitCommand = new RelayCommand<string>(ExecuteDigit);
            OperationCommand = new RelayCommand<string>(ExecuteOperation);
            EqualsCommand = new RelayCommand(obj => ExecuteEquals());
            ClearCommand = new RelayCommand(obj => ExecuteClear());
            ClearEntryCommand = new RelayCommand(obj => ExecuteClearEntry());
            BackspaceCommand = new RelayCommand(obj => ExecuteBackspace());
            NegateCommand = new RelayCommand(obj => ExecuteNegate());
            DecimalPointCommand = new RelayCommand(obj => ExecuteDecimalPoint());
            MemoryClearCommand = new RelayCommand(obj => ExecuteMemoryClear());
            MemoryRecallCommand = new RelayCommand(obj => ExecuteMemoryRecall());
            MemoryStoreCommand = new RelayCommand(obj => ExecuteMemoryStore());
            MemoryAddCommand = new RelayCommand(obj => ExecuteMemoryAdd());
            MemorySubtractCommand = new RelayCommand(obj => ExecuteMemorySubtract());
            MemoryStackCommand = new RelayCommand(obj => ExecuteMemoryStack());
            CutCommand = new RelayCommand(obj => ExecuteCut());
            CopyCommand = new RelayCommand(obj => ExecuteCopy());
            PasteCommand = new RelayCommand(obj => ExecutePaste());
            DigitGroupingCommand = new RelayCommand(obj => ExecuteDigitGrouping());
            AboutCommand = new RelayCommand(obj => ExecuteAbout());
            ChangeModeCommand = new RelayCommand<string>(ExecuteChangeMode);
            ChangeBaseCommand = new RelayCommand<string>(ExecuteChangeBase);
            KeyPressCommand = new RelayCommand<KeyEventArgs>(ExecuteKeyPress);
        }

        private void ExecuteDigit(string digit)
        {
            if (CurrentMode == CalculatorMode.Programmer)
            {
                if (!numberBaseConverter.IsValidForBase(digit[0], CurrentBase))
                    return;
            }
            else if (!char.IsDigit(digit[0]))
                return;

            if (isNewNumber)
            {
                displayText = digit;
                isNewNumber = false;
            }
            else
            {
                displayText += digit;
            }

            OnPropertyChanged(nameof(DisplayText));

            if (!DigitGroupingEnabled)
            {
                DisplayText = displayText;
            }
            else
            {
                UpdateDisplay();
            }
        }

        private void ExecuteOperation(string operation)
        {
            if (storedValue.HasValue)
            {
                ExecuteEquals();
            }

            decimal currentNumber = numberBaseConverter.ConvertFromBase(DisplayText, CurrentBase);
            storedValue = currentNumber;
            pendingOperation = operation;
            isNewNumber = true;

            OperationDisplay = $"{currentNumber} {operation}";
        }

        private void ExecuteEquals()
        {
            if (!storedValue.HasValue || string.IsNullOrEmpty(pendingOperation))
                return;

            if (!decimal.TryParse(DisplayText, out decimal currentNumber))
                return;

            if (CurrentMode == CalculatorMode.Programmer)
            {
                currentNumber = numberBaseConverter.ConvertFromBase(DisplayText, CurrentBase);
            }

            decimal result = 0;

            try
            {
                switch (pendingOperation)
                {
                    case "+":
                        result = storedValue.Value + currentNumber;
                        break;
                    case "-":
                        result = storedValue.Value - currentNumber;
                        break;
                    case "*":
                        result = storedValue.Value * currentNumber;
                        break;
                    case "/":
                        if (currentNumber == 0)
                            throw new DivideByZeroException();
                        result = storedValue.Value / currentNumber;
                        break;
                    case "%":
                        result = storedValue.Value % currentNumber;
                        break;
                    case "sqrt":
                        result = (decimal)Math.Sqrt((double)storedValue.Value);
                        break;
                    case "square":
                        result = storedValue.Value * storedValue.Value;
                        break;
                    case "reciprocal":
                        if (storedValue.Value == 0)
                            throw new DivideByZeroException();
                        result = 1 / storedValue.Value;
                        break;
                }

                if (CurrentMode == CalculatorMode.Programmer)
                {
                    DisplayText = numberBaseConverter.ConvertToBase(result, CurrentBase);
                }
                else
                {
                    DisplayText = result.ToString(CultureInfo.InvariantCulture);
                    if (DigitGroupingEnabled)
                    {
                        UpdateDisplay();
                    }
                }
            }
            catch (DivideByZeroException)
            {
                DisplayText = "Cannot divide by zero";
                isNewNumber = true;
            }
            catch (Exception)
            {
                DisplayText = "Error";
                isNewNumber = true;
            }

            storedValue = null;
            pendingOperation = null;
            isNewNumber = true;
            OperationDisplay = "";
        }

        private void ExecuteClear()
        {
            DisplayText = "0";
            storedValue = null;
            pendingOperation = null;
            OperationDisplay = "";
            isNewNumber = true;
        }

        private void ExecuteClearEntry()
        {
            DisplayText = "0";
            isNewNumber = true;
        }

        private void ExecuteBackspace()
        {
            if (DisplayText.Length > 1)
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
            else
                DisplayText = "0";
        }

        private void ExecuteNegate()
        {
            if (CurrentMode == CalculatorMode.Programmer)
                return;

            if (DisplayText != "0")
            {
                decimal value = decimal.Parse(DisplayText);
                DisplayText = (-value).ToString();
            }
        }

        private void ExecuteDecimalPoint()
        {
            if (CurrentMode == CalculatorMode.Programmer)
                return;

            if (isNewNumber)
            {
                DisplayText = "0.";
                isNewNumber = false;
            }
            else if (!DisplayText.Contains("."))
            {
                DisplayText += ".";
            }
        }

        private void ExecuteMemoryClear()
        {
            memoryValue = 0;
            memoryStack.Clear();
        }

        private void ExecuteMemoryRecall()
        {
            DisplayText = memoryValue.ToString();
            isNewNumber = true;
        }

        private void ExecuteMemoryStore()
        {
            if (decimal.TryParse(DisplayText, out decimal value))
            {
                memoryValue = value;
                memoryStack.Insert(0, value);
                OnPropertyChanged(nameof(memoryStack));
            }
        }

        private void ExecuteMemoryAdd()
        {
            if (decimal.TryParse(DisplayText, out decimal value))
            {
                memoryStack.Insert(0, value);
                memoryValue = value;
                OnPropertyChanged(nameof(memoryStack));
            }
        }

        private void ExecuteMemorySubtract()
        {
            if (memoryStack.Count > 0)
            {
                memoryStack.RemoveAt(0);
                memoryValue = memoryStack.Count > 0 ? memoryStack[0] : 0;
                OnPropertyChanged(nameof(memoryStack));
            }
        }

        private void ExecuteMemoryStack()
        {
            var memoryWindow = new MemoryStackWindow(memoryStack, memoryValue);
            if (memoryWindow.ShowDialog() == true)
            {
                var viewModel = (MemoryStackViewModel)memoryWindow.DataContext;
                DisplayText = viewModel.SelectedMemoryValue.ToString();
                isNewNumber = true;
            }
        }

        private void ExecuteCut()
        {
            if (!string.IsNullOrEmpty(DisplayText) && DisplayText != "0")
            {
                clipboardContent = DisplayText;
                DisplayText = "0";
                isNewNumber = true;
            }
        }

        private void ExecuteCopy()
        {
            if (!string.IsNullOrEmpty(DisplayText) && DisplayText != "0")
            {
                clipboardContent = DisplayText;
            }
        }

        private void ExecutePaste()
        {
            if (!string.IsNullOrEmpty(clipboardContent))
            {
                if (decimal.TryParse(clipboardContent, out decimal value))
                {
                    DisplayText = value.ToString(CultureInfo.InvariantCulture);
                    isNewNumber = true;
                }
            }
        }

        private void ExecuteDigitGrouping()
        {
            digitGroupingEnabled = !digitGroupingEnabled;
            OnPropertyChanged(nameof(DigitGroupingEnabled));

            if (decimal.TryParse(DisplayText, out decimal value))
            {
                if (digitGroupingEnabled)
                {
                    string[] parts = value.ToString(CultureInfo.InvariantCulture).Split('.');
                    string integerPart = string.Format(CultureInfo.CurrentCulture, "{0:N0}", decimal.Parse(parts[0]));
                    DisplayText = parts.Length > 1 ? $"{integerPart}.{parts[1]}" : integerPart;
                }
                else
                {
                    DisplayText = value.ToString(CultureInfo.InvariantCulture);
                }
            }

            SaveSettings();
        }

        private void ExecuteAbout()
        {
            MessageBox.Show("Calculator creat de: Dragomir Cezar Andrei\nGrupa: 10LF232", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteChangeMode(string mode)
        {
            if (Enum.TryParse<CalculatorMode>(mode, out var newMode))
            {
                CurrentMode = newMode;
                if (newMode == CalculatorMode.Standard)
                {
                    CurrentBase = NumberBase.Decimal;
                }
                UpdateDisplay();
            }
        }

        private void ExecuteChangeBase(string baseStr)
        {
            if (Enum.TryParse<NumberBase>(baseStr, out var newBase))
            {
                CurrentBase = newBase;
                UpdateDisplay();
            }
        }

        private void ExecuteKeyPress(KeyEventArgs e)
        {
            if (e == null) return;

            switch (e.Key)
            {
                case Key.Enter:
                    ExecuteEquals();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    ExecuteClear();
                    e.Handled = true;
                    break;
                case Key.Back:
                    ExecuteBackspace();
                    e.Handled = true;
                    break;
                case Key.Delete:
                    ExecuteClearEntry();
                    e.Handled = true;
                    break;
                default:
                    if (e.Key >= Key.D0 && e.Key <= Key.D9 && !e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        ExecuteDigit((e.Key - Key.D0).ToString());
                        e.Handled = true;
                    }
                    else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                    {
                        ExecuteDigit((e.Key - Key.NumPad0).ToString());
                        e.Handled = true;
                    }
                    else if (CurrentMode == CalculatorMode.Programmer)
                    {
                        if (e.Key >= Key.A && e.Key <= Key.F)
                        {
                            ExecuteDigit(e.Key.ToString());
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        switch (e.Key)
                        {
                            case Key.Add:
                                ExecuteOperation("+");
                                e.Handled = true;
                                break;
                            case Key.Subtract:
                                ExecuteOperation("-");
                                e.Handled = true;
                                break;
                            case Key.Multiply:
                                ExecuteOperation("*");
                                e.Handled = true;
                                break;
                            case Key.Divide:
                                ExecuteOperation("/");
                                e.Handled = true;
                                break;
                            case Key.Decimal:
                                ExecuteDecimalPoint();
                                e.Handled = true;
                                break;
                        }
                    }
                    break;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.C:
                        ExecuteCopy();
                        e.Handled = true;
                        break;
                    case Key.V:
                        ExecutePaste();
                        e.Handled = true;
                        break;
                    case Key.X:
                        ExecuteCut();
                        e.Handled = true;
                        break;
                }
            }
        }


        private void SaveSettings()
        {
            try
            {
                var settings = new CalculatorSettings
                {
                    DigitGroupingEnabled = digitGroupingEnabled,
                    CurrentMode = currentMode,
                    CurrentBase = currentBase
                };
                settingsService.SaveSettings(settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea setărilor: {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            try
            {
                var settings = settingsService.LoadSettings();
                digitGroupingEnabled = settings.DigitGroupingEnabled;
                currentMode = settings.CurrentMode;
                currentBase = settings.CurrentBase;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea setărilor: {ex.Message}");
                digitGroupingEnabled = false;
            }
        }

        private void UpdateDisplay()
        {
            if (string.IsNullOrEmpty(DisplayText) || DisplayText == "-")
                return;

            if (CurrentMode == CalculatorMode.Programmer)
            {
                if (decimal.TryParse(DisplayText, out decimal value))
                {
                    DisplayText = numberBaseConverter.ConvertToBase(value, CurrentBase);
                }
            }
            else if (decimal.TryParse(DisplayText, out decimal value))
            {
                if (DigitGroupingEnabled)
                {
                    string[] parts = value.ToString(CultureInfo.InvariantCulture).Split('.');
                    string integerPart = string.Format(CultureInfo.CurrentCulture, "{0:N0}", decimal.Parse(parts[0]));
                    DisplayText = parts.Length > 1 ? $"{integerPart}.{parts[1]}" : integerPart;
                }
                else
                {
                    DisplayText = value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
    }

    public enum CalculatorMode
    {
        Standard,
        Programmer
    }

    public enum NumberBase
    {
        Binary = 2,
        Octal = 8,
        Decimal = 10,
        Hexadecimal = 16
    }
}
