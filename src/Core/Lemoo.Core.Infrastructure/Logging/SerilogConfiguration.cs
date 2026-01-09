using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Lemoo.Core.Infrastructure.Logging;

/// <summary>
/// Serilog配置
/// </summary>
public static class SerilogConfiguration
{
    public static LoggerConfiguration ConfigureSerilog(IConfiguration configuration)
    {
        var logLevel = configuration.GetValue<string>("Lemoo:Logging:Level") ?? "Information";
        var seqUrl = configuration["Lemoo:Logging:Seq:ServerUrl"];
        
        var logEventLevel = Enum.TryParse<LogEventLevel>(logLevel, true, out var level)
            ? level
            : LogEventLevel.Information;
        
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(logEventLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "Lemoo")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/lemoo-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
        
        if (!string.IsNullOrWhiteSpace(seqUrl))
        {
            loggerConfiguration.WriteTo.Seq(seqUrl);
        }
        
        return loggerConfiguration;
    }
}

