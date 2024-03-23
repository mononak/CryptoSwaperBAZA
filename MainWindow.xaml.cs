using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CryptoCurrencyParserConsole;
using static CryptoCurrencyParserConsole.CryptoCurrencyParser;

namespace CryptoSwaperBAZA
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            // Установка текущего времени при запуске приложения
            UpdateTime("Данные за " + DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss") + " GMT+03:00");
            fromCurrencyComboBox.SelectionChanged += CurrencyComboBox_SelectionChanged;
            toCurrencyComboBox.SelectionChanged += CurrencyComboBox_SelectionChanged;
            inputAmount.TextChanged += TextBox_TextChanged;
            // Загрузка данных о валютах
            LoadCurrencyData();

            // Установка значения по умолчанию для TextBox
            inputAmount.Text = "1";
        }
        // Обработчик нажатия кнопки для обмена выбранных валют местами
        private void SwapCurrenciesButton_Click(object sender, RoutedEventArgs e)
        {
            // Обмен местами выбранных валют
            int fromIndex = fromCurrencyComboBox.SelectedIndex;
            fromCurrencyComboBox.SelectedIndex = toCurrencyComboBox.SelectedIndex;
            toCurrencyComboBox.SelectedIndex = fromIndex;

            // После обмена обновим курс обмена
            UpdateExchangeRate();
        }

        // Метод для обновления отображаемого времени
        private void UpdateTime(string time)
        {
            // Обновление содержимого TextBlock
            TimeTextbox.Text = time;
        }

        // Список объектов CurrencyItem для хранения данных о валютах
        private List<CurrencyItem> _currencyItems;

        // Свойство для доступа к списку валют
        public List<CurrencyItem> CurrencyItems
        {
            get { return _currencyItems; }
            set
            {
                _currencyItems = value;
                OnPropertyChanged("CurrencyItems");
            }
        }

        // Метод для загрузки данных о валютах
        // Метод для загрузки данных о валютах
        private async void LoadCurrencyData()
        {
            // Получение списка криптовалют
            List<CryptoCurrencyParser.CurrencyItem> cryptocurrencies = await CryptoCurrencyParser.ParseCryptoCurrencies();

            // Заполнение комбобоксов данными из списка криптовалют
            fromCurrencyComboBox.ItemsSource = cryptocurrencies;
            toCurrencyComboBox.ItemsSource = cryptocurrencies;

            // Установка Ethereum (ETH) как выбранной валюты по умолчанию
            fromCurrencyComboBox.SelectedIndex = cryptocurrencies.FindIndex(item => item.Symbol == "ETH");
            toCurrencyComboBox.SelectedIndex = 0; // Выберите первую криптовалюту в качестве второй валюты
        }


        // Обработчик изменения выбранной валюты в ComboBox
        private void CurrencyComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateExchangeRate();
        }

        // Метод для обновления курса обмена
        private void UpdateExchangeRate()
        {
            // Получение введенного пользователем количества
            if (double.TryParse(inputAmount.Text, out double amount))
            {
                // Получение выбранной валюты для обмена
                CurrencyItem fromCurrencyItem = (CurrencyItem)fromCurrencyComboBox.SelectedItem;
                CurrencyItem toCurrencyItem = (CurrencyItem)toCurrencyComboBox.SelectedItem;

                // Проверка, выбраны ли обе валюты
                if (fromCurrencyItem != null && toCurrencyItem != null)
                {
                    // Извлечение курса выбранных криптовалют для обмена
                    if (double.TryParse(fromCurrencyItem.PriceUSD.ToString(), out double fromValue) && double.TryParse(toCurrencyItem.PriceUSD.ToString(), out double toValue))
                    {
                        // Рассчитывается курс обмена и обновляется интерфейс
                        double exchangeRate = amount * toValue / fromValue;
                        exchangeRateTextBlock.Text = $"{exchangeRate:F4}";
                    }
                    else
                    {
                        exchangeRateTextBlock.Text = $"???";
                    }
                }
            }
            else
            {
                exchangeRateTextBlock.Text = $"???";
            }
        }
        // Обработчик изменения текста в TextBox
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Обновление курса обмена при изменении текста в TextBox
            UpdateExchangeRate();
        }

        // Событие, вызываемое при изменении свойства объекта
        public event PropertyChangedEventHandler PropertyChanged;

        // Метод для вызова события изменения свойства
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверка ввода пользователя: разрешены только цифры и запятая
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != ',')
                {
                    e.Handled = true;
                    return;
                }
            }
        }

      
    }
}
