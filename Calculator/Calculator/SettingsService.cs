using System;
using System.IO;
using System.Text.Json;

namespace Calculator
{
    public class CalculatorSettings
    {
        public bool DigitGroupingEnabled { get; set; }
        public CalculatorMode CurrentMode { get; set; }
        public NumberBase CurrentBase { get; set; }
    }

    public class SettingsService
    {
        private readonly string settingsFilePath;

        public SettingsService()
        {
            settingsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Calculator",
                "settings.json");

            Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));
        }

        public CalculatorSettings LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFilePath))
                {
                    string jsonString = File.ReadAllText(settingsFilePath);
                    return JsonSerializer.Deserialize<CalculatorSettings>(jsonString)
                           ?? CreateDefaultSettings();
                }
            }
            catch (Exception)
            {
                // În caz de eroare, returnăm setările implicite
            }

            return CreateDefaultSettings();
        }

        public void SaveSettings(CalculatorSettings settings)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(settingsFilePath, jsonString);
            }
            catch (Exception)
            {
                
            }
        }

        private CalculatorSettings CreateDefaultSettings()
        {
            return new CalculatorSettings
            {
                DigitGroupingEnabled = false,
                CurrentMode = CalculatorMode.Standard,
                CurrentBase = NumberBase.Decimal
            };
        }
    }
}