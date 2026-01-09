# Lemoo 快速开始指南

## 前置要求

- .NET 10 SDK
- Visual Studio 2022 或 VS Code
- SQL Server（可选，用于数据库）

## 项目结构

```
Lemoo/
├── src/                    # 源代码
│   ├── Lemoo.Core/         # 核心层
│   ├── Lemoo.Modules/      # 模块层
│   ├── Lemoo.Bootstrap/    # 启动引导层
│   ├── Lemoo.Host/         # WPF宿主应用
│   ├── Lemoo.UI/           # UI层
│   └── Lemoo.Api/          # API层
├── tests/                  # 测试项目
└── docs/                   # 文档
```

## 快速开始

### 1. 克隆或下载项目

```bash
git clone <repository-url>
cd Lemoo
```

### 2. 还原NuGet包

```bash
dotnet restore
```

### 3. 配置数据库连接

编辑 `src/Lemoo.Host/appsettings.json`，配置数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "Example": "Server=localhost;Database=Lemoo_Example;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 4. 运行WPF应用

```bash
cd src/Lemoo.Host
dotnet run
```

### 5. 运行API服务

```bash
cd src/Lemoo.Api
dotnet run
```

访问 Swagger UI: `https://localhost:5001/swagger`

## 运行测试

```bash
dotnet test
```

## 创建新模块

### 1. 创建模块项目

```bash
cd src/Lemoo.Modules
dotnet new classlib -n Lemoo.Modules.YourModule -f net10.0
```

### 2. 添加项目引用

```bash
cd Lemoo.Modules.YourModule
dotnet add reference ../Lemoo.Modules.Abstractions/Lemoo.Modules.Abstractions.csproj
dotnet add reference ../Lemoo.Core.Application/Lemoo.Core.Application.csproj
dotnet add reference ../Lemoo.Core.Domain/Lemoo.Core.Domain.csproj
```

### 3. 实现模块类

```csharp
public class YourModule : ModuleBase
{
    public override string Name => "YourModule";
    public override string Version => "1.0.0";
    public override string Description => "您的业务模块";
    
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 注册服务
    }
    
    public override void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
    {
        // 配置数据库
    }
}
```

### 4. 配置模块

在 `appsettings.json` 中启用模块：

```json
{
  "Lemoo": {
    "Modules": {
      "Enabled": [ "YourModule" ]
    }
  }
}
```

## 开发工作流

1. **定义领域模型**：在 `Domain/Entities` 中创建实体
2. **定义命令和查询**：在 `Application/Commands` 和 `Application/Queries` 中定义
3. **实现处理器**：在 `Application/Handlers` 中实现业务逻辑
4. **配置数据库**：在 `Infrastructure/Persistence` 中配置DbContext
5. **编写测试**：在 `tests` 目录中编写单元测试

## 常见问题

### Q: 如何添加新的数据库提供程序？

A: 在 `ModuleDbContextFactory` 中添加新的case分支，并安装相应的EF Core提供程序包。

### Q: 如何实现模块间的通信？

A: 通过领域事件或共享内核（Shared Kernel）实现模块间通信。

### Q: 如何部署应用？

A: 使用 `dotnet publish` 发布应用，然后部署到目标环境。

## 更多资源

- [架构文档](./architecture.md)
- [API文档](./api.md)
- [模块开发指南](./modules.md)

