namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块依赖描述（支持版本范围）
/// </summary>
public class ModuleDependency
{
    /// <summary>
    /// 依赖的模块名称
    /// </summary>
    public string ModuleName { get; set; } = string.Empty;

    /// <summary>
    /// 版本范围（如 "1.0.0", ">=1.0.0", "1.0.0 - 2.0.0", "^1.0.0"）
    /// </summary>
    public string? VersionRange { get; set; }

    /// <summary>
    /// 是否必需（false 表示可选依赖）
    /// </summary>
    public bool IsRequired { get; set; } = true;

    public ModuleDependency()
    {
    }

    public ModuleDependency(string moduleName, string? versionRange = null, bool isRequired = true)
    {
        ModuleName = moduleName;
        VersionRange = versionRange;
        IsRequired = isRequired;
    }

    /// <summary>
    /// 解析版本范围字符串
    /// </summary>
    public VersionRange ParseVersionRange()
    {
        if (string.IsNullOrWhiteSpace(VersionRange))
        {
            return VersionRange.Any;
        }

        return VersionRange.Parse(VersionRange);
    }
}

/// <summary>
/// 版本范围
/// </summary>
public class VersionRange
{
    /// <summary>
    /// 最小版本（包含）
    /// </summary>
    public Version? MinVersion { get; private set; }

    /// <summary>
    /// 最大版本（包含）
    /// </summary>
    public Version? MaxVersion { get; private set; }

    /// <summary>
    /// 是否包含最小版本
    /// </summary>
    public bool IncludeMinVersion { get; private set; } = true;

    /// <summary>
    /// 是否包含最大版本
    /// </summary>
    public bool IncludeMaxVersion { get; private set; } = true;

    /// <summary>
    /// 任意版本
    /// </summary>
    public static VersionRange Any { get; } = new();

    private VersionRange()
    {
    }

    /// <summary>
    /// 解析版本范围字符串
    /// 支持格式：
    /// - "1.0.0" - 精确版本
    /// - ">=1.0.0" - 大于等于
    /// - ">1.0.0" - 大于
    /// - "<=1.0.0" - 小于等于
    /// - "<1.0.0" - 小于
    /// - "1.0.0 - 2.0.0" - 范围
    /// - "^1.0.0" - 兼容版本（1.x.x）
    /// - "~1.0.0" - 补丁版本（1.0.x）
    /// </summary>
    public static VersionRange Parse(string versionRange)
    {
        var result = new VersionRange();

        if (string.IsNullOrWhiteSpace(versionRange))
        {
            return Any;
        }

        versionRange = versionRange.Trim();

        // 精确版本
        if (!versionRange.StartsWith(">") && !versionRange.StartsWith("<") && !versionRange.StartsWith("^") && !versionRange.StartsWith("~") && !versionRange.Contains("-"))
        {
            result.MinVersion = new Version(versionRange);
            result.MaxVersion = new Version(versionRange);
            return result;
        }

        // 兼容版本 ^1.0.0 (>=1.0.0 && <2.0.0)
        if (versionRange.StartsWith("^"))
        {
            var version = new Version(versionRange.Substring(1));
            result.MinVersion = version;
            result.IncludeMinVersion = true;
            result.MaxVersion = new Version(version.Major + 1, 0, 0);
            result.IncludeMaxVersion = false;
            return result;
        }

        // 补丁版本 ~1.0.0 (>=1.0.0 && <1.1.0)
        if (versionRange.StartsWith("~"))
        {
            var version = new Version(versionRange.Substring(1));
            result.MinVersion = version;
            result.IncludeMinVersion = true;
            result.MaxVersion = new Version(version.Major, version.Minor + 1, 0);
            result.IncludeMaxVersion = false;
            return result;
        }

        // 大于等于 >=1.0.0
        if (versionRange.StartsWith(">="))
        {
            result.MinVersion = new Version(versionRange.Substring(2));
            result.IncludeMinVersion = true;
            return result;
        }

        // 大于 >1.0.0
        if (versionRange.StartsWith(">"))
        {
            result.MinVersion = new Version(versionRange.Substring(1));
            result.IncludeMinVersion = false;
            return result;
        }

        // 小于等于 <=1.0.0
        if (versionRange.StartsWith("<="))
        {
            result.MaxVersion = new Version(versionRange.Substring(2));
            result.IncludeMaxVersion = true;
            return result;
        }

        // 小于 <1.0.0
        if (versionRange.StartsWith("<"))
        {
            result.MaxVersion = new Version(versionRange.Substring(1));
            result.IncludeMaxVersion = false;
            return result;
        }

        // 范围 1.0.0 - 2.0.0
        if (versionRange.Contains("-"))
        {
            var parts = versionRange.Split('-');
            if (parts.Length == 2)
            {
                result.MinVersion = new Version(parts[0].Trim());
                result.MaxVersion = new Version(parts[1].Trim());
                return result;
            }
        }

        // 无法解析，返回任意版本
        return Any;
    }

    /// <summary>
    /// 检查版本是否在范围内
    /// </summary>
    public bool IsSatisfied(Version version)
    {
        if (MinVersion != null)
        {
            var comparison = version.CompareTo(MinVersion);
            if (IncludeMinVersion)
            {
                if (comparison < 0) return false;
            }
            else
            {
                if (comparison <= 0) return false;
            }
        }

        if (MaxVersion != null)
        {
            var comparison = version.CompareTo(MaxVersion);
            if (IncludeMaxVersion)
            {
                if (comparison > 0) return false;
            }
            else
            {
                if (comparison >= 0) return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        if (this == Any) return "*";

        var parts = new List<string>();

        if (MinVersion != null)
        {
            parts.Add(IncludeMinVersion ? $">={MinVersion}" : $">{MinVersion}");
        }

        if (MaxVersion != null)
        {
            parts.Add(IncludeMaxVersion ? $"<={MaxVersion}" : $"<{MaxVersion}");
        }

        if (MinVersion != null && MaxVersion != null && MinVersion.Equals(MaxVersion))
        {
            return MinVersion.ToString();
        }

        return parts.Count > 0 ? string.Join(" ", parts) : "*";
    }
}
