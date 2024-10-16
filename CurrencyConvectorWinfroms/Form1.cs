using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace CurrencyConverterApp
{
    public partial class MainForm : Form
    {
        private const string ApiUrl = "https://api.exchangerate-api.com/v4/latest/";

        private Dictionary<string, decimal> exchangeRates;

        public MainForm()
        {
            InitializeComponent();
            LoadCurrencies();
        }

        private async void LoadCurrencies()
        {
            exchangeRates = new Dictionary<string, decimal>();
            var baseCurrency = "USD"; 
            var rates = await GetExchangeRates(baseCurrency);

            foreach (var rate in rates)
            {
                exchangeRates[rate.Key] = rate.Value;
                comboBoxFrom.Items.Add(rate.Key);
                comboBoxTo.Items.Add(rate.Key);
            }

            comboBoxFrom.SelectedItem = baseCurrency;
            comboBoxTo.SelectedItem = "EUR";
        }

        private async Task<Dictionary<string, decimal>> GetExchangeRates(string baseCurrency)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(ApiUrl + baseCurrency);
                var json = JObject.Parse(response);
                var rates = json["rates"].ToObject<Dictionary<string, decimal>>();
                return rates;
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxAmount.Text, out decimal amount) &&
                comboBoxFrom.SelectedItem is string fromCurrency &&
                comboBoxTo.SelectedItem is string toCurrency)
            {
                var convertedAmount = ConvertCurrency(amount, fromCurrency, toCurrency);
                labelResult.Text = $"{amount} {fromCurrency} = {convertedAmount} {toCurrency}";
            }
            else
            {
                MessageBox.Show("¬ведите корректное значение дл€ конвертации.");
            }
        }

        private decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
        {
            if (exchangeRates.TryGetValue(fromCurrency, out decimal fromRate) &&
                exchangeRates.TryGetValue(toCurrency, out decimal toRate))
            {
                return amount / fromRate * toRate;
            }
            return 0;
        }
    }
}
