using CliWrap;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using WindowsService;

const string ServiceName = "TcpListeneterAsNTService";

if (args is { Length: 1 })
{
  string executablePath =
      Path.Combine(AppContext.BaseDirectory, "WindowsService.exe");

  if (args[0] is "/Install")
  {
    await Cli.Wrap("sc")
        .WithArguments(new[] { "create", ServiceName, $"binPath={executablePath}", "start=auto" })
        .ExecuteAsync();
  }
  else if (args[0] is "/Start")
  {
    await Cli.Wrap("sc")
        .WithArguments(new[] { "start", ServiceName })
        .ExecuteAsync();
  }
  else if (args[0] is "/Stop")
  {
    await Cli.Wrap("sc")
        .WithArguments(new[] { "stop", ServiceName })
        .ExecuteAsync();
  }
  else if (args[0] is "/Delete")
  {
    await Cli.Wrap("sc")
        .WithArguments(new[] { "delete", ServiceName })
        .ExecuteAsync();
  }
  else if (args[0] is "/Uninstall")
  {
    try
    {
      await Cli.Wrap("sc")
          .WithArguments(new[] { "stop", ServiceName })
          .ExecuteAsync();
    }
    finally
    {
      await Cli.Wrap("sc")
          .WithArguments(new[] { "delete", ServiceName })
          .ExecuteAsync();
    }
  }

  return;
}

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
  options.ServiceName = ServiceName;
});

LoggerProviderOptions
  .RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

builder.Services.AddSingleton<TcpListenerProvider>();
builder.Services.AddHostedService<WindowsBackgroundService>();

// See: https://github.com/dotnet/runtime/issues/47303
builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));

IHost host = builder.Build();
host.Run();
