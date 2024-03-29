// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Try to connect to TcpListener, press any key to do tests... Enter to exit.");

ConsoleKeyInfo keyInfo = Console.ReadKey();
while (keyInfo.Key != ConsoleKey.Enter)
{ 
  using var client = new TcpClient("localhost", 35467);
  using NetworkStream stream = client.GetStream();

  var buffer = new byte[1_024];
  int received = await stream.ReadAsync(buffer);

  var message = Encoding.UTF8.GetString(buffer, 0, received);
  Console.WriteLine($"Message received: \"{message}\"");

  keyInfo = Console.ReadKey();
}

