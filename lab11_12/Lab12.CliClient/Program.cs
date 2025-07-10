using Lab12.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

TcpClient? client = null;

try
{
    client = new TcpClient();
    await client.ConnectAsync(IPAddress.Loopback, 1234);
    Console.WriteLine($"Connected to {client.Client.RemoteEndPoint}");
}
catch (SocketException)
{
    Console.WriteLine("❌ Nie udało się połączyć z serwerem. Spróbuj ponownie później.");
    return;
}




var i = 1;
var data = new Data
{
    NumberA = 0,
    NumberB = 1,
    Content = $"Message {i}. does uppcarcase work?",
    ClientsName = "Client " + (client.Client.RemoteEndPoint as IPEndPoint)?.Port.ToString() ?? "without port",
    MessageId = i
};

var running = true;
Console.CancelKeyPress += (_, _) => running = false;

var writingStream = new StreamWriter(client.GetStream()) { AutoFlush = true };
var readingStream = new StreamReader(client.GetStream());

try
{
    while (running)
    {
        data.SentOnUtc = DateTime.UtcNow;
        var serialized = JsonSerializer.Serialize(data);
        Console.WriteLine($"Sending {serialized}");
        await writingStream.WriteLineAsync(serialized);

        var line = await readingStream.ReadLineAsync();
        if (line is null || string.IsNullOrWhiteSpace(line))
        {
            Console.WriteLine("⚠️ Połączenie zakończone przez serwer.");
            break;
        }

        Console.WriteLine($"Received {line}");

        var deserialized = JsonSerializer.Deserialize<Data>(line);
        if (deserialized is null)
        {
            Console.WriteLine("⚠️ Otrzymano nieprawidłowy JSON.");
            continue;
        }

        data.NumberA = deserialized.NumberB;
        data.NumberB = deserialized.Result!.Value;
        data.Content = "message " + i;

        Console.WriteLine();
        i++;
        await Task.Delay(Random.Shared.Next(500, 3500));
    }
}
catch (IOException)
{
    Console.WriteLine("⚠️ Utracono połączenie z serwerem.");
}
catch (Exception ex)
{
    Console.WriteLine($"[ERR] Nieoczekiwany błąd: {ex.Message}");
}
finally
{
    client.Close();
}



