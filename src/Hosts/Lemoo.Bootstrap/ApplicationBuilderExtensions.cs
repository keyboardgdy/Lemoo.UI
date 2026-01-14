using Lemoo.Core.Abstractions.Module;
using Lemoo.Core.Application.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lemoo.Bootstrap;

/// <summary>
/// 应用程序构建器扩展方法
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 配置Lemoo应用程序（用于Web API）
    /// </summary>
    public static async Task<WebApplication> ConfigureLemooApplicationAsync(
        this WebApplicationBuilder builder,
        Action<WebApplication>? configurePipeline = null,
        Action<StartupOptions>? configureOptions = null)
    {
        // 创建引导器
        var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
        var logger = loggerFactory.CreateLogger<Bootstrapper>();

        // 创建并配置启动选项
        var options = new StartupOptions();
        configureOptions?.Invoke(options);

        var bootstrapper = new Bootstrapper(builder.Configuration, logger, options);

        // 注册Bootstrapper为单例，以便后续使用
        builder.Services.AddSingleton(bootstrapper);
        builder.Services.AddSingleton(options);

        // 配置Host
        bootstrapper.ConfigureHost(builder.Host);

        // 注册服务
        builder.Services.AddLemooCore(builder.Configuration);
        await bootstrapper.RegisterServicesAsync(builder.Services, builder.Configuration);
        builder.Services.AddCqrsPipelineBehaviors();

        // 构建应用
        var app = builder.Build();

        // 配置管道
        configurePipeline?.Invoke(app);

        // 引导应用程序
        var bootstrapResult = await bootstrapper.BootstrapAsync();
        if (!bootstrapResult.IsSuccess)
        {
            logger.LogError("应用程序引导失败: {Errors}",
                string.Join(", ", bootstrapResult.Errors.Select(e => e.Message)));
            throw new InvalidOperationException($"应用程序引导失败: {bootstrapResult.Message}");
        }

        // 启动模块生命周期
        await bootstrapper.StartModulesAsync(app.Services);

        return app;
    }

}

