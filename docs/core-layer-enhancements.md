# 核心层增强功能总结

本文档总结了为核心层添加的所有新功能，以更好地支持WPF和API应用。

## 一、认证授权抽象

### 1.1 ICurrentUserService
**位置**: `src/Core/Lemoo.Core.Abstractions/Security/ICurrentUserService.cs`

提供当前认证用户信息：
- 用户ID、用户名、邮箱
- 角色列表和声明
- 权限检查方法

### 1.2 IUserContext
**位置**: `src/Core/Lemoo.Core.Abstractions/Security/IUserContext.cs`

用户上下文接口，提供用户信息访问。

### 1.3 IAuthenticationService
**位置**: `src/Core/Lemoo.Core.Abstractions/Security/IAuthenticationService.cs`

认证服务接口：
- 登录/登出
- 令牌刷新
- 令牌验证

### 1.4 IAuthorizationService
**位置**: `src/Core/Lemoo.Core.Abstractions/Security/IAuthorizationService.cs`

授权服务接口：
- 权限检查
- 角色检查
- 资源授权

---

## 二、UI状态管理

### 2.1 OperationState
**位置**: `src/Core/Lemoo.Core.Application/UI/OperationState.cs`

WPF UI状态管理类，支持数据绑定：
- `IsLoading` - 加载状态
- `HasError` - 错误状态
- `ErrorMessage` - 错误消息
- `IsSuccess` - 成功状态
- `SuccessMessage` - 成功消息
- `Progress` - 进度（0-100）
- `StatusMessage` - 状态消息

**使用示例**:
```csharp
var state = new OperationState();
state.SetLoading("正在加载数据...");
// 绑定到UI
// <TextBlock Text="{Binding State.StatusMessage}" />
```

### 2.2 ICommandExecutionTracker
**位置**: `src/Core/Lemoo.Core.Application/UI/ICommandExecutionTracker.cs`

命令执行追踪器接口，用于追踪命令执行状态。

### 2.3 CommandExecutionTracker
**位置**: `src/Core/Lemoo.Core.Application/UI/CommandExecutionTracker.cs`

命令执行追踪器实现，提供命令执行的生命周期追踪。

---

## 三、Result类型增强

### 3.1 新增方法
**位置**: `src/Core/Lemoo.Core.Application/Common/Result.cs`

为 `Result` 和 `Result<T>` 添加了以下方法：

#### Result类
- `OnSuccess(Action)` - 成功时执行操作
- `OnFailure(Action<string?>)` - 失败时执行操作
- `Match<TResult>(Func, Func)` - 模式匹配

#### Result<T>类
- `Map<TResult>(Func<T, TResult>)` - 映射结果值
- `Bind<TResult>(Func<T, Result<TResult>>)` - 绑定结果（链式操作）
- `Match<TResult>(Func<T, TResult>, Func<string?, TResult>)` - 模式匹配
- `OnSuccess(Action<T>)` - 成功时执行操作
- `OnFailure(Action<string?>)` - 失败时执行操作
- `ValueOr(T)` - 失败时返回默认值
- `ValueOr(Func<T>)` - 失败时通过函数计算默认值

**使用示例**:
```csharp
var result = await GetDataAsync();
result
    .OnSuccess(data => Console.WriteLine($"成功: {data}"))
    .OnFailure(error => Console.WriteLine($"失败: {error}"));

var mapped = result.Map(x => x.ToString());
var bound = result.Bind(x => ProcessData(x));
var value = result.ValueOr("默认值");
```

---

## 四、分页结果优化

### 4.1 PagedResult增强
**位置**: `src/Core/Lemoo.Core.Application/Common/PagedResult.cs`

新增功能：
- `AsyncData` - 支持 `IAsyncEnumerable<T>` 用于流式处理
- `CreateAsync()` - 创建异步分页结果
- `Empty()` - 创建空分页结果

### 4.2 PagedQuery基类
**位置**: `src/Core/Lemoo.Core.Application/Common/PagedQuery.cs`

分页查询基类，提供：
- `PageNumber` - 页码
- `PageSize` - 每页大小
- `MaxPageSize` - 最大每页大小
- `SortBy` - 排序字段
- `SortDirection` - 排序方向
- `Normalize()` - 验证并规范化分页参数

**使用示例**:
```csharp
public class GetExamplesQuery : PagedQuery<ExampleDto>
{
    public string? SearchTerm { get; set; }
}

// 在Handler中
query.Normalize();
var data = await repository.GetPagedAsync(query);
return PagedResult<ExampleDto>.Create(data, query.PageNumber, query.PageSize, totalCount);
```

---

## 五、API响应标准化

### 5.1 ApiResponse类
**位置**: `src/Core/Lemoo.Core.Application/Common/ApiResponse.cs`

标准化的API响应格式：
- `Success` - 是否成功
- `Data` - 响应数据
- `Message` - 消息
- `Errors` - 错误信息（字典格式）
- `RequestId` - 请求ID（用于追踪）
- `Timestamp` - 时间戳

**使用示例**:
```csharp
var response = ApiResponse<ExampleDto>.SuccessResponse(data, "操作成功", requestId);
// 或从Result转换
var response = ApiResponse<ExampleDto>.FromResult(result, requestId);
```

---

## 六、WPF验证反馈机制

### 6.1 ValidationExtensions
**位置**: `src/Core/Lemoo.Core.Application/Extensions/ValidationExtensions.cs`

提供WPF友好的验证错误转换：
- `ToWpfErrors()` - 转换为错误字典
- `ToWpfErrorMessages()` - 转换为错误消息列表
- `ToWpfErrorMessage()` - 转换为单个错误消息字符串
- `GetPropertyErrors()` - 获取指定属性的错误
- `GetPropertyError()` - 获取指定属性的第一个错误

**使用示例**:
```csharp
try
{
    await command.ExecuteAsync();
}
catch (ValidationException ex)
{
    var errors = ex.ToWpfErrors();
    // 绑定到UI
    // <TextBlock Text="{Binding Errors[PropertyName]}" />
}
```

---

## 七、文件操作抽象

### 7.1 IFileService
**位置**: `src/Core/Lemoo.Core.Abstractions/Files/IFileService.cs`

文件服务接口，提供：
- `UploadAsync()` - 上传文件
- `DownloadAsync()` - 下载文件
- `DeleteAsync()` - 删除文件
- `ExistsAsync()` - 检查文件是否存在
- `GetFileInfoAsync()` - 获取文件信息
- `GetFileUrlAsync()` - 获取文件URL

---

## 八、消息总线抽象

### 8.1 IMessageBus
**位置**: `src/Core/Lemoo.Core.Abstractions/Messaging/IMessageBus.cs`

消息总线接口，提供发布/订阅模式：
- `PublishAsync<TMessage>()` - 发布消息
- `Subscribe<TMessage>()` - 订阅消息
- `Unsubscribe()` - 取消订阅
- `UnsubscribeAll()` - 取消所有订阅

**使用示例**:
```csharp
// 发布消息
await messageBus.PublishAsync(new UserCreatedEvent { UserId = userId });

// 订阅消息
var subscriptionId = messageBus.Subscribe<UserCreatedEvent>(async evt =>
{
    await HandleUserCreated(evt);
});
```

---

## 九、后台任务抽象

### 9.1 IBackgroundJobService
**位置**: `src/Core/Lemoo.Core.Abstractions/Jobs/IBackgroundJobService.cs`

后台任务服务接口，提供：
- `EnqueueAsync()` - 入队任务（立即执行）
- `ScheduleAsync()` - 调度任务（延迟执行）
- `ScheduleRecurringAsync()` - 调度重复任务（Cron表达式）
- `DeleteAsync()` - 删除任务
- `GetStatusAsync()` - 获取任务状态

### 9.2 BackgroundJob基类
提供后台任务基类，包含：
- `JobId` - 任务ID
- `CreatedAt` - 创建时间
- `MaxRetries` - 最大重试次数
- `Timeout` - 超时时间
- `ExecuteAsync()` - 执行任务（抽象方法）

---

## 十、配置热更新支持

### 10.1 IConfigurationService增强
**位置**: `src/Core/Lemoo.Core.Abstractions/Configuration/IConfigurationService.cs`

新增功能：
- `ConfigurationChanged` 事件 - 配置变更通知
- `GetOptionsMonitor<T>()` - 获取配置选项监视器（支持热更新）
- `Reload()` - 重新加载配置

**使用示例**:
```csharp
configurationService.ConfigurationChanged += (sender, e) =>
{
    Console.WriteLine($"配置 {e.Key} 从 {e.OldValue} 变更为 {e.NewValue}");
};

var monitor = configurationService.GetOptionsMonitor<MyOptions>();
monitor.OnChange(options =>
{
    // 配置变更时自动调用
});
```

---

## 十一、多语言支持抽象

### 11.1 ILocalizationService
**位置**: `src/Core/Lemoo.Core.Abstractions/Localization/ILocalizationService.cs`

本地化服务接口，提供：
- `CurrentCulture` - 当前文化
- `CultureChanged` 事件 - 文化变更通知
- `GetString()` - 获取本地化字符串
- `GetStringOrDefault()` - 获取本地化字符串（带默认值）
- `HasKey()` - 检查资源键是否存在
- `GetSupportedCultures()` - 获取支持的文化列表

**使用示例**:
```csharp
var text = localizationService.GetString("WelcomeMessage", userName);
var textWithDefault = localizationService.GetStringOrDefault("Key", "默认值");
```

---

## 总结

所有功能已实现并经过编译检查，代码结构优雅，遵循了以下原则：

1. **分层清晰** - 抽象在 Abstractions 层，实现在 Application/Infrastructure 层
2. **接口优先** - 所有功能都通过接口定义，便于测试和扩展
3. **异步支持** - 所有I/O操作都支持异步
4. **类型安全** - 充分利用泛型和强类型
5. **可扩展性** - 设计考虑了未来扩展需求

这些增强功能使核心层能够更好地支持WPF和API应用的需求。

