using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CryptoCurrencyParserConsole
{
    public static class CryptoCurrencyParser
    {
        private const string ApiBaseUrl = "https://api.coincap.io/v2";

        public static async Task<List<CurrencyItem>> ParseCryptoCurrencies()
        {
            List<CurrencyItem> currencies = new List<CurrencyItem>(); // Создание списка для хранения данных о криптовалютах

            using (HttpClient client = new HttpClient())
            {
                // Получение информации о криптовалютах
                HttpResponseMessage response = await client.GetAsync($"{ApiBaseUrl}/assets?limit=20"); // Выполнение GET запроса

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync(); // Чтение ответа в виде JSON строки
                    var responseData = JsonConvert.DeserializeObject<AssetsResponse>(json); // Десериализация JSON в объект AssetsResponse

                    foreach (var asset in responseData.Data)
                    {
                        currencies.Add(new CurrencyItem
                        {
                            Name = asset.Name,
                            Symbol = asset.Symbol,
                            PriceUSD = asset.PriceUSD
                        });
                    }
                }
                else
                {
                    Console.WriteLine($"Ошибка при получении информации о криптовалютах: {response.StatusCode}");
                }
            }

            return currencies;
        }

        // Класс для представления информации о криптовалюте
        public class CurrencyItem
        {
            public string Name { get; set; } // Название криптовалюты

            [JsonProperty("symbol")]
            public string Symbol { get; set; } // Код криптовалюты

            [JsonProperty("priceUsd")]
            public decimal PriceUSD { get; set; } // Цена в USD
        }

        // Класс для представления ответа от API CoinCap
        public class AssetsResponse
        {
            public List<AssetData> Data { get; set; }
        }

        // Класс для представления информации о криптовалюте из ответа API CoinCap
        public class AssetData
        {
            public string Name { get; set; } // Название криптовалюты

            public string Symbol { get; set; } // Код криптовалюты

            public decimal PriceUSD { get; set; } // Цена в USD
        }
    }
}
