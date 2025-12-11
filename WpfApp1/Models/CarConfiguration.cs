using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class CarConfiguration : INotifyPropertyChanged
    {
        private string _selectedModel;
        private string _selectedEngine;
        private string _selectedColor;
        private List<string> _selectedOptions = new List<string>();
        private double _downPaymentPercent = 20;
        private int _loanTerm = 36;
        private string _customerName;
        private string _phone;
        private string _email;

        public event PropertyChangedEventHandler PropertyChanged;

        
        public Dictionary<string, double> ModelPrices = new Dictionary<string, double>
        {
            { "Седан", 1200000 },
            { "Хэтчбэк", 1100000 },
            { "Кроссовер", 1500000 },
            { "Внедорожник", 2000000 }
        };

        
        public Dictionary<string, double> EnginePrices = new Dictionary<string, double>
        {
            { "Бензин 1.6", 0 },
            { "Бензин 2.0", 150000 },
            { "Дизель 2.0", 200000 },
            { "Гибрид", 300000 }
        };

        
        public Dictionary<string, double> ColorPrices = new Dictionary<string, double>
        {
            { "Белый", 0 },
            { "Чёрный", 15000 },
            { "Серый", 10000 },
            { "Синий", 20000 },
            { "Красный", 25000 }
        };

        
        public Dictionary<string, double> OptionPrices = new Dictionary<string, double>
        {
            { "Кожаный салон", 80000 },
            { "Панорамная крыша", 120000 },
            { "Подогрев сидений", 30000 },
            { "Круиз-контроль", 40000 },
            { "Климат-контроль", 50000 },
            { "Парктроники", 35000 }
        };

        public string SelectedModel
        {
            get => _selectedModel;
            set
            {
                _selectedModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BasePrice));
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(DownPaymentAmount));
                OnPropertyChanged(nameof(LoanAmount));
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        public string SelectedEngine
        {
            get => _selectedEngine;
            set
            {
                _selectedEngine = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EnginePrice));
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(DownPaymentAmount));
                OnPropertyChanged(nameof(LoanAmount));
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        public string SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ColorPrice));
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(DownPaymentAmount));
                OnPropertyChanged(nameof(LoanAmount));
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        public List<string> SelectedOptions
        {
            get => _selectedOptions;
            set
            {
                _selectedOptions = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OptionsPrice));
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(DownPaymentAmount));
                OnPropertyChanged(nameof(LoanAmount));
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        public double DownPaymentPercent
        {
            get => _downPaymentPercent;
            set
            {
                _downPaymentPercent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DownPaymentAmount));
                OnPropertyChanged(nameof(LoanAmount));
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        public int LoanTerm
        {
            get => _loanTerm;
            set
            {
                _loanTerm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        
        public double BasePrice
        {
            get
            {
                if (SelectedModel != null && ModelPrices.ContainsKey(SelectedModel))
                    return ModelPrices[SelectedModel];
                return 0;
            }
        }

        public double EnginePrice
        {
            get
            {
                if (SelectedEngine != null && EnginePrices.ContainsKey(SelectedEngine))
                    return EnginePrices[SelectedEngine];
                return 0;
            }
        }

        public double ColorPrice
        {
            get
            {
                if (SelectedColor != null && ColorPrices.ContainsKey(SelectedColor))
                    return ColorPrices[SelectedColor];
                return 0;
            }
        }

        public double OptionsPrice
        {
            get
            {
                double total = 0;
                foreach (var option in SelectedOptions)
                {
                    if (OptionPrices.ContainsKey(option))
                        total += OptionPrices[option];
                }
                return total;
            }
        }

        public double TotalPrice
        {
            get
            {
                return BasePrice + EnginePrice + ColorPrice + OptionsPrice;
            }
        }

        
        public double DownPaymentAmount
        {
            get
            {
                return TotalPrice * (DownPaymentPercent / 100);
            }
        }

        public double LoanAmount
        {
            get
            {
                return TotalPrice - DownPaymentAmount;
            }
        }

        public double MonthlyPayment
        {
            get
            {
                if (LoanAmount <= 0 || LoanTerm <= 0)
                    return 0;

                double annualRate = 8.5; // 8.5% годовых
                double monthlyRate = annualRate / 100 / 12;

                // Формула : A = S * (i * (1 + i)^n) / ((1 + i)^n - 1)
                double numerator = LoanAmount * (monthlyRate * Math.Pow(1 + monthlyRate, LoanTerm));
                double denominator = Math.Pow(1 + monthlyRate, LoanTerm) - 1;

                if (denominator == 0)
                    return 0;

                return numerator / denominator;
            }
        }

        
        public void AddOption(string option)
        {
            if (!SelectedOptions.Contains(option))
            {
                SelectedOptions.Add(option);
                OnPropertyChanged(nameof(SelectedOptions));
                OnPropertyChanged(nameof(OptionsPrice));
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(DownPaymentAmount));
                OnPropertyChanged(nameof(LoanAmount));
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        public void RemoveOption(string option)
        {
            if (SelectedOptions.Contains(option))
            {
                SelectedOptions.Remove(option);
                OnPropertyChanged(nameof(SelectedOptions));
                OnPropertyChanged(nameof(OptionsPrice));
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(DownPaymentAmount));
                OnPropertyChanged(nameof(LoanAmount));
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        public bool HasOption(string option)
        {
            return SelectedOptions.Contains(option);
        }

        
        public List<string> GetAvailableModels()
        {
            return new List<string>(ModelPrices.Keys);
        }

        
        public List<string> GetAvailableEngines()
        {
            return new List<string>(EnginePrices.Keys);
        }

        
        public List<string> GetAvailableColors()
        {
            return new List<string>(ColorPrices.Keys);
        }

        
        public List<string> GetAvailableOptions()
        {
            return new List<string>(OptionPrices.Keys);
        }

        
        public double GetOptionPrice(string option)
        {
            if (OptionPrices.ContainsKey(option))
                return OptionPrices[option];
            return 0;
        }

        
        public string GetConfigurationSummary()
        {
            return $"Модель: {SelectedModel ?? "Не выбрана"}\n" +
                   $"Двигатель: {SelectedEngine ?? "Не выбран"}\n" +
                   $"Цвет: {SelectedColor ?? "Не выбран"}\n" +
                   $"Опции: {(SelectedOptions.Count > 0 ? string.Join(", ", SelectedOptions) : "Нет")}\n" +
                   $"Стоимость: {TotalPrice:N0} ₽";
        }

        
        public string GetLoanSummary()
        {
            return $"Первоначальный взнос: {DownPaymentPercent}% ({DownPaymentAmount:N0} ₽)\n" +
                   $"Сумма кредита: {LoanAmount:N0} ₽\n" +
                   $"Срок кредита: {LoanTerm} месяцев\n" +
                   $"Ежемесячный платеж: {MonthlyPayment:N0} ₽";
        }

        
        public string GetFullApplicationSummary()
        {
            return $"=== КОНФИГУРАЦИЯ АВТОМОБИЛЯ ===\n" +
                   $"{GetConfigurationSummary()}\n\n" +
                   $"=== ПАРАМЕТРЫ КРЕДИТА ===\n" +
                   $"{GetLoanSummary()}\n\n" +
                   $"=== КОНТАКТНЫЕ ДАННЫЕ ===\n" +
                   $"Имя: {CustomerName ?? "Не указано"}\n" +
                   $"Телефон: {Phone ?? "Не указан"}\n" +
                   $"Email: {Email ?? "Не указан"}";
        }

        
        public void Reset()
        {
            SelectedModel = null;
            SelectedEngine = null;
            SelectedColor = null;
            SelectedOptions.Clear();
            DownPaymentPercent = 20;
            LoanTerm = 36;
            CustomerName = null;
            Phone = null;
            Email = null;
        }

        
        public bool IsConfigurationComplete()
        {
            return !string.IsNullOrEmpty(SelectedModel) &&
                   !string.IsNullOrEmpty(SelectedEngine) &&
                   !string.IsNullOrEmpty(SelectedColor);
        }

        
        public bool IsApplicationComplete()
        {
            return IsConfigurationComplete() &&
                   !string.IsNullOrWhiteSpace(CustomerName) &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   !string.IsNullOrWhiteSpace(Email);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
