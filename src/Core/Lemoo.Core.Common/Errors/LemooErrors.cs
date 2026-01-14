namespace Lemoo.Core.Common.Errors;

/// <summary>
/// Lemoo 框架错误码定义
/// </summary>
public static class LemooErrors
{
    #region 模块相关错误 (LEM001-LEM099)

    /// <summary>
    /// 模块未找到
    /// </summary>
    public const string MODULE_NOT_FOUND = "LEM001";

    /// <summary>
    /// 模块加载失败
    /// </summary>
    public const string MODULE_LOAD_FAILED = "LEM002";

    /// <summary>
    /// 检测到循环依赖
    /// </summary>
    public const string CIRCULAR_DEPENDENCY = "LEM003";

    /// <summary>
    /// 缺少依赖模块
    /// </summary>
    public const string MISSING_DEPENDENCY = "LEM004";

    /// <summary>
    /// 模块初始化失败
    /// </summary>
    public const string MODULE_INITIALIZATION_FAILED = "LEM005";

    /// <summary>
    /// 模块配置无效
    /// </summary>
    public const string MODULE_CONFIGURATION_INVALID = "LEM006";

    /// <summary>
    /// 模块版本不兼容
    /// </summary>
    public const string MODULE_VERSION_INCOMPATIBLE = "LEM007";

    #endregion

    #region 验证相关错误 (LEM100-LEM199)

    /// <summary>
    /// 验证失败
    /// </summary>
    public const string VALIDATION_FAILED = "LEM100";

    /// <summary>
    /// 必填字段为空
    /// </summary>
    public const string REQUIRED_FIELD_MISSING = "LEM101";

    /// <summary>
    /// 字段长度超出限制
    /// </summary>
    public const string FIELD_TOO_LONG = "LEM102";

    /// <summary>
    /// 字段格式无效
    /// </summary>
    public const string INVALID_FORMAT = "LEM103";

    #endregion

    #region 领域相关错误 (LEM200-LEM299)

    /// <summary>
    /// 实体未找到
    /// </summary>
    public const string ENTITY_NOT_FOUND = "LEM200";

    /// <summary>
    /// 实体已存在
    /// </summary>
    public const string ENTITY_ALREADY_EXISTS = "LEM201";

    /// <summary>
    /// 实体状态无效
    /// </summary>
    public const string INVALID_ENTITY_STATE = "LEM202";

    /// <summary>
    /// 操作不允许
    /// </summary>
    public const string OPERATION_NOT_ALLOWED = "LEM203";

    #endregion

    #region 持久化相关错误 (LEM300-LEM399)

    /// <summary>
    /// 数据库连接失败
    /// </summary>
    public const string DATABASE_CONNECTION_FAILED = "LEM300";

    /// <summary>
    /// 数据库操作失败
    /// </summary>
    public const string DATABASE_OPERATION_FAILED = "LEM301";

    /// <summary>
    /// 并发冲突
    /// </summary>
    public const string CONCURRENCY_CONFLICT = "LEM302";

    /// <summary>
    /// 事务失败
    /// </summary>
    public const string TRANSACTION_FAILED = "LEM303";

    #endregion

    #region 服务相关错误 (LEM400-LEM499)

    /// <summary>
    /// 服务不可用
    /// </summary>
    public const string SERVICE_UNAVAILABLE = "LEM400";

    /// <summary>
    /// 服务超时
    /// </summary>
    public const string SERVICE_TIMEOUT = "LEM401";

    /// <summary>
    /// 缓存操作失败
    /// </summary>
    public const string CACHE_OPERATION_FAILED = "LEM402";

    #endregion

    #region 安全相关错误 (LEM500-LEM599)

    /// <summary>
    /// 未授权访问
    /// </summary>
    public const string UNAUTHORIZED_ACCESS = "LEM500";

    /// <summary>
    /// 权限不足
    /// </summary>
    public const string INSUFFICIENT_PERMISSIONS = "LEM501";

    /// <summary>
    /// 认证失败
    /// </summary>
    public const string AUTHENTICATION_FAILED = "LEM502";

    #endregion
}
