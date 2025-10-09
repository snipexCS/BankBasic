using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using API_Classes;
using Newtonsoft.Json;
using RestSharp;

namespace BankClientGUI
{
    public partial class MainWindow : Window
    {
        private readonly string _businessBase = "https://localhost:7077/api/business/values";
        private readonly RestClient _client;

        public MainWindow()
        {
            InitializeComponent();
            _client = new RestClient(_businessBase);
        }

        private async void GoButton_Click(object sender, RoutedEventArgs e)
        {
            await GetByIndexAsync();
        }

        

       



        private async Task GetByIndexAsync()
        {
            if (!int.TryParse(IndexNum.Text, out int index))
            {
                MessageBox.Show("Index must be a number.");
                return;
            }

            try
            {
                var request = new RestRequest($"{index}", Method.Get);
                var response = await _client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    await HandleApiError(response.Content);
                    return;
                }

                var data = await Task.Run(() => JsonConvert.DeserializeObject<DataIntermedDTO>(response.Content!));
                if (data == null)
                {
                    MessageBox.Show("Failed to parse server response.");
                    return;
                }

                DisplayData(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

     


        private async Task HandleApiError(string? content)
        {
            try
            {
                var error = JsonConvert.DeserializeObject<ApiError>(content!);
                MessageBox.Show($"API Error: {error?.Message}");
            }
            catch
            {
                MessageBox.Show("Unknown server error.");
            }
        }

        private void DisplayData(DataIntermedDTO d)
        {
            FNameBox.Text = d.fname;
            LNameBox.Text = d.lname;
            BalanceBox.Text = d.bal.ToString("C");
            AcctNoBox.Text = d.acct.ToString();
            PinBox.Text = d.pin.ToString("D4");

            if (!string.IsNullOrEmpty(d.imageBase64))
            {
                byte[] bytes = Convert.FromBase64String(d.imageBase64);
                using var ms = new MemoryStream(bytes);
                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                ProfileImage.Source = bitmap;
            }
            else
            {
                ProfileImage.Source = null;
            }
        }

    }
}
