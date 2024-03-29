using System.Net.Sockets;
using System.Text;

namespace WindowsService;

public class TcpListenerProvider
{
  private readonly ILogger<TcpListenerProvider> _logger;
  private readonly TcpListener _tcpListener;

  public TcpListenerProvider(ILogger<TcpListenerProvider> logger, int port = 35467)
  {
    _logger = logger ?? throw new ArgumentException(nameof(logger));
    _tcpListener = TcpListener.Create(port);
  }

  public void Start()
  {
    _tcpListener.Start();
  }

  public async Task HandleConnectionAsync()
  {
    using TcpClient handler = await _tcpListener.AcceptTcpClientAsync();
    await using NetworkStream stream = handler.GetStream();

    var message = $"📅 {DateTime.Now} 🕛";
    var dateTimeBytes = Encoding.UTF8.GetBytes(message);
    await stream.WriteAsync(dateTimeBytes);

    string msg = $"Sent message: \"{message}\"";
    // Sample output:
    //     Sent message: "📅 8/22/2022 9:07:17 AM 🕛"

    _logger.LogInformation(msg);
  }
}
