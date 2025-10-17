using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // ✅ Needed for Select() and ToList()
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
                if (error != null)
                {

                    MessageBox.Show($"API Error: {error.Message}\n\nStack Trace:\n{error.StackTrace}");

                   

                }
                else
                {
                    MessageBox.Show("Unknown server error.");
                }
            }
            catch
            {
                MessageBox.Show("Failed to read server error response.");
            }
        }

     
        private async void DisplayData(DataIntermedDTO d)
        {
            FNameBox.Text = d.fname;
            LNameBox.Text = d.lname;
            BalanceBox.Text = d.bal.ToString("C");
            AcctNoBox.Text = d.acct.ToString();
            PinBox.Text = d.pin.ToString("D4");

            if (!string.IsNullOrEmpty(d.imageBase64))
            {
                try
                {
                    byte[] bytes = await Task.Run(() => Convert.FromBase64String(d.imageBase64));

                    await Task.Run(() =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            using var ms = new MemoryStream(bytes);
                            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                            ProfileImage.Source = bitmap;
                        });
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error decoding image: {ex.Message}");
                }
            }
            else
            {
                ProfileImage.Source = null;
            }
        }


        private async void GetCountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                GetCountButton.IsEnabled = false;
                TotalCountText.Text = "Loading total accounts...";
                AccountsListView.ItemsSource = null;

              
                int total = await Task.Run(async () =>
                {
                    var countRequest = new RestRequest("", Method.Get);
                    var countResponse = await _client.ExecuteAsync(countRequest);
                    if (!countResponse.IsSuccessful)
                        throw new Exception("Failed to retrieve total count from API.");

                    return JsonConvert.DeserializeObject<int>(countResponse.Content!);
                });

              
                TotalCountText.Text = $"Total Accounts: {total}";

                
                var allAccounts = await Task.Run(async () => await GetAllAccountsAsync());

                
                var displayList = await Task.Run(() =>
                    allAccounts.Select(a => new
                    {
                        FName = a.fname,
                        LName = a.lname,
                        AcctNo = a.acct,
                        Balance = a.bal.ToString("C")
                    }).ToList()
                );

               
                AccountsListView.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                
                GetCountButton.IsEnabled = true;
            }
        }


        private async Task<List<DataIntermedDTO>> GetAllAccountsAsync()
        {
            var request = new RestRequest("all", Method.Get);
            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                await HandleApiError(response.Content);
                return new List<DataIntermedDTO>();
            }

            var list = await Task.Run(() =>
                JsonConvert.DeserializeObject<List<DataIntermedDTO>>(response.Content!)
            );

            return list ?? new List<DataIntermedDTO>();
        }
    }
}
