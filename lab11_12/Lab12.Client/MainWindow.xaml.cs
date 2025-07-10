using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lab12.Contracts;

namespace Lab12.Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TcpClient? _client = null;
    private bool stop = false;
    
    private Task? _checkConnectionTask = null;
    
    public MainWindow()
    {
        InitializeComponent();
        _checkConnectionTask = Task.Run(async () =>
        {
            while (!stop)
            {
                if (_client != null && _client.Client.Poll(0, SelectMode.SelectRead) && _client.Client.Available == 0)
                {
                    _client = null;

                    Status.Dispatcher.Invoke(() =>
                    {
                        Status.Text = "Disconnected";
                    });
                    DisconnectButton.Dispatcher.Invoke(() =>
                    {
                        DisconnectButton.IsEnabled = false;
                    });
                    ConnectButton.Dispatcher.Invoke(() =>
                    {
                        ConnectButton.IsEnabled = true;
                    });
                    SendButton.Dispatcher.Invoke(() =>
                    {
                        SendButton.IsEnabled = false;
                    });
                    Log("Disconnected from server");
                }

                await Task.Delay(1000);
            }
        });
    }

    private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
    {
        _client?.Close();
        var ip = IPAddress.Parse("153.19.54.67");
        try
        {
            _client = new TcpClient();
            _client.Connect(ip, 1234);
            Log("Connected to server");

            var stream = _client.GetStream();
            var reader = new StreamReader(stream);
            if (stream.DataAvailable)
            {
                var response = await reader.ReadLineAsync();
                if (response == "SERVER_BUSY")
                {
                    Log("Server is busy");
                    Status.Text = "Server Busy";
                    MessageBox.Show("❌ Nie udało się połączyć z serwerem.", "server busy",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        catch (SocketException)
        {
            for (int i = 0; i < 3; i++)
            {
                Log("Connection failed: server unavailable");
                Status.Text = "Server unavailable";
                await Task.Delay(1000); // poczekaj 2 sekundy
                try
                {
                    _client = new TcpClient();
                    await _client.ConnectAsync(ip, 1234);
                    Log("Connected to server");
                    break;
                }
                catch (SocketException) { continue; }
            }
            if (!_client.Connected)
            {
                Log("Connection failed: server unavailable");
                Status.Text = "Server unavailable";
                MessageBox.Show("❌ Serwer nieosiągalny.", "Błąd połączenia",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        Status.Text = "Connected";
        SendButton.IsEnabled = true;
        DisconnectButton.IsEnabled = true;
        ConnectButton.IsEnabled = false;
    }




    private async void SendButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_client is null)
        {
            MessageBox.Show("Connect to the server first");
            return;
        }

        Log("Sended data to server");

        if (_client.GetStream().DataAvailable)
        {
            _client?.Close();
            _client = null;
            Status.Text = "Disconnected";
            DisconnectButton.IsEnabled = false;
            ConnectButton.IsEnabled = true;
            SendButton.IsEnabled = false;
            return;
        }
        
        var data = GetData();
        var originalData = new Data
        {
            NumberA = data.NumberA,
            NumberB = data.NumberB,
            Content = data.Content,
            ClientsName = data.ClientsName,
            MessageId = data.MessageId,
            SentOnUtc = data.SentOnUtc
        };


        NumberATextBox.IsEnabled = false;
        NumberBTextBox.IsEnabled = false;
        ContentTextBox.IsEnabled = false;
        ConnectButton.IsEnabled = false;
        SendButton.IsEnabled = false;
        DisconnectButton.IsEnabled = false;
        Status.Text = "Sending...";

        var result = await Task.Run(async () =>
        {
            var streamWriter = new StreamWriter(_client.GetStream());
            await streamWriter.WriteLineAsync(JsonSerializer.Serialize(data));
            await streamWriter.FlushAsync();

            var streamReader = new StreamReader(_client.GetStream());
            var newData = JsonSerializer.Deserialize<Data>((await streamReader.ReadLineAsync())!);
            return newData;
        });
        if (result is not null)
        {
            NumberATextBox.Text = result.NumberA.ToString();
            NumberBTextBox.Text = result.NumberB.ToString();
            ContentTextBox.Text = result.Content;
            ResultTextBox.Text = result.Result!.ToString()!;

         
            List<string> changes = new();

            if (result.Content != originalData.Content)
                changes.Add($"Content changed from '{originalData.Content}' to '{result.Content}'");

            if (result.NumberA!=originalData.NumberA )
                changes.Add($"Result changed: expected {originalData.NumberA}, got {result.NumberA}");
         
            if (result.NumberB != originalData.NumberB)
                changes.Add($"Result changed: expected {originalData.NumberB}, got {result.NumberB}");

            if (changes.Count == 0)
                Log("No changes detected from server.");
            else
                foreach (var change in changes)
                    Log($"Server modified: {change}");
        }



        NumberATextBox.IsEnabled = true;
        NumberBTextBox.IsEnabled = true;
        ContentTextBox.IsEnabled = true;
        SendButton.IsEnabled = true;
        DisconnectButton.IsEnabled = true;

        Status.Text = "Done";
    }

    private Data GetData() => new Data
    {
        NumberA = int.Parse(NumberATextBox.Text),
        NumberB = int.Parse(NumberBTextBox.Text),
        Content = ContentTextBox.Text
    };

    private void DisconnectButton_OnClick(object sender, RoutedEventArgs e)
    {
        _client?.Close();
        _client = null;
        Log("Disconnected from server");
        Status.Text = "Disconnected";
        DisconnectButton.IsEnabled = false;
        ConnectButton.IsEnabled = true;
        SendButton.IsEnabled = false;
    }

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        stop = true;
        _checkConnectionTask.Wait();
        _client?.Close();
    }

    private void Log(string message)
    {
        Dispatcher.Invoke(() =>
        {
            LogListBox.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            LogListBox.ScrollIntoView(LogListBox.Items[^1]);
        });
    }


}