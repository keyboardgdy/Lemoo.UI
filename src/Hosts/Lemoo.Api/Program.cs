using Lemoo.Api.Extensions;
using Lemoo.Bootstrap;

var builder = WebApplication.CreateBuilder(args);

// 添加API服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
});

// 配置CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 添加健康检查
builder.Services.AddHealthChecks();

// 配置Lemoo应用程序（包括引导、服务注册、模块生命周期）
var app = await builder.ConfigureLemooApplicationAsync(configurePipeline: app =>
{
    // 配置HTTP请求管道
    // 请求ID追踪（应该最早注册）
    app.UseRequestId();

    // 全局异常处理
    app.UseGlobalExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lemoo API v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAuthorization();

    // 健康检查端点
    app.MapHealthChecks("/health");

    app.MapControllers();
});

app.Run();
