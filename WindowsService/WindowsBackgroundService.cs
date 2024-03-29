using System.Threading;

namespace WindowsService;

public sealed class WindowsBackgroundService : BackgroundService
{
  private readonly TcpListenerProvider _tcpListenerProvider;
  private readonly ILogger<WindowsBackgroundService> _logger;

  public WindowsBackgroundService(
    TcpListenerProvider tcpListenerProvider,
    ILogger<WindowsBackgroundService> logger)
  {
    _tcpListenerProvider = tcpListenerProvider ?? throw new ArgumentNullException(nameof(tcpListenerProvider));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    try
    {
      _logger.LogInformation("Tcp listener is starting...");
      _tcpListenerProvider.Start();
      _logger.LogInformation("Tcp listener is started.");

      while (!stoppingToken.IsCancellationRequested)
      {
        _logger.LogInformation("Tcp listener is handling connection...");
        await _tcpListenerProvider.HandleConnectionAsync(stoppingToken);
        _logger.LogInformation("Tcp listener has received connection.");
      }      
    }
    catch (OperationCanceledException)
    {
      // When the stopping token is canceled, for example, a call made from services.msc,
      // we shouldn't exit with a non-zero exit code. In other words, this is expected...
      _logger.LogInformation("Shutting down in progress...");
    }    
    catch (Exception ex)
    {
      _logger.LogError(ex, "{Message}", ex.Message);

      // Terminates this process and returns an exit code to the operating system.
      // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
      // performs one of two scenarios:
      // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
      // 2. When set to "StopHost": will cleanly stop the host, and log errors.
      //
      // In order for the Windows Service Management system to leverage configured
      // recovery options, we need to terminate the process with a non-zero exit code.
      Environment.Exit(1);
    }
    finally
    {
      _tcpListenerProvider.Stop();
    }
  }
}
