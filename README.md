# Calculator WPF

Un calculator modern și funcțional dezvoltat în WPF (Windows Presentation Foundation) cu suport pentru moduri multiple și funcționalități avansate.

## Caracteristici

- 🧮 Mod Standard și Programmer
- 🔢 Suport pentru multiple baze numerice (Binar, Octal, Zecimal, Hexazecimal)
- 💾 Funcționalități de memorie (MC, MR, MS, M+, M-, M>)
- 📋 Operații de bază (+, -, ×, ÷)
- 🔄 Operații avansate (%, √, x², 1/x)
- ⌨️ Suport pentru tastatură
- 📊 Grupare automată a cifrelor
- 💾 Salvarea setărilor între sesiuni
- ✂️ Operații de copiere/decupare/inserare
- 🎨 Interfață modernă și responsivă

## Cerințe de sistem

- Windows 10 sau mai nou
- .NET Framework 4.7.2 sau mai nou
- Visual Studio 2019 sau mai nou (pentru dezvoltare)

## Instalare

1. Clonează repository-ul:
```bash
git clone https://github.com/yourusername/calculator-wpf.git
```

2. Deschide soluția în Visual Studio:
```bash
cd calculator-wpf
Calculator.sln
```

3. Compilează și rulează proiectul

## Utilizare

### Mod Standard
- Folosește butoanele pentru a introduce numere și operații
- Apasă = pentru a calcula rezultatul
- Folosește CE pentru a șterge ultima intrare
- Folosește C pentru a șterge totul

### Mod Programmer
- Selectează baza numerelor (BIN, OCT, DEC, HEX)
- Introdu numere în baza selectată
- Rezultatele sunt afișate în baza curentă

### Funcționalități de memorie
- MC: Șterge memoria
- MR: Recheamă valoarea din memorie
- MS: Salvează valoarea curentă în memorie
- M+: Adaugă valoarea curentă la memoria existentă
- M-: Scade valoarea curentă din memoria existentă
- M>: Deschide fereastra stivei de memorie

### Scurtături tastatură
- Enter: Execută operația curentă
- Escape: Șterge totul
- Backspace: Șterge ultima cifră
- Delete: Șterge ultima intrare
- Ctrl+C: Copiază valoarea curentă
- Ctrl+V: Inserează valoarea din clipboard
- Ctrl+X: Decupează valoarea curentă

## Structura proiectului

- `CalculatorViewModel.cs`: Logica principală a calculatorului
- `MainWindow.xaml`: Interfața principală
- `NumberBaseConverter.cs`: Conversia între baze numerice
- `RelayCommand.cs`: Implementarea comenzilor
- `SettingsService.cs`: Gestionarea setărilor
- `MemoryStackWindow.xaml`: Fereastra stivei de memorie
- `Converters.cs`: Convertori pentru interfața utilizator

## Contribuții

Contribuțiile sunt binevenite! Te rugăm să:

1. Fork repository-ul
2. Creează o branch pentru feature (`git checkout -b feature/AmazingFeature`)
3. Commit modificările (`git commit -m 'Add some AmazingFeature'`)
4. Push la branch (`git push origin feature/AmazingFeature`)
5. Deschide un Pull Request

## Licență

Acest proiect este licențiat sub licența MIT - vezi fișierul [LICENSE](LICENSE) pentru detalii.

## Autor

Dragomir Cezar Andrei
- Grupa: 10LF232

## Recunoașteri

- Design inspirat de calculatorul Windows
- Implementare bazată pe arhitectura MVVM
- Utilizarea WPF pentru interfața utilizator modernă