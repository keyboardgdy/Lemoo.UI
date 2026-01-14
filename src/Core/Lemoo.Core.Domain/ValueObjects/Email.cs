using System.Text.RegularExpressions;

namespace Lemoo.Core.Domain.ValueObjects;

/// <summary>
/// 邮箱地址值对象 - 确保邮箱格式的有效性
/// </summary>
public sealed partial class Email : ValueObject<string>
{
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public Email(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));

        if (!EmailRegex().IsMatch(value))
            throw new ArgumentException("Invalid email format.", nameof(value));
    }

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new(value);

    public override string ToString() => Value;

    [GeneratedRegex(EmailPattern, RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}

/// <summary>
/// 电话号码值对象
/// </summary>
public sealed partial class PhoneNumber : ValueObject<string>
{
    private const string PhonePattern = @"^[\d\s\-+()]+$";

    public PhoneNumber(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty.", nameof(value));

        if (!PhoneRegex().IsMatch(value))
            throw new ArgumentException("Invalid phone number format.", nameof(value));
    }

    public static implicit operator string(PhoneNumber phone) => phone.Value;
    public static explicit operator PhoneNumber(string value) => new(value);

    public override string ToString() => Value;

    [GeneratedRegex(PhonePattern, RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();
}

/// <summary>
/// 金额值对象 - 确保货币计算的精确性
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies.");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies.");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}

/// <summary>
/// 百分比值对象
/// </summary>
public sealed class Percentage : ValueObject<decimal>
{
    public Percentage(decimal value) : base(value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentException("Percentage must be between 0 and 100.", nameof(value));
    }

    public static implicit operator decimal(Percentage percentage) => percentage.Value;
    public static explicit operator Percentage(decimal value) => new(value);

    public override string ToString() => $"{Value:N2}%";

    public decimal AsFraction() => Value / 100m;
}

/// <summary>
/// 版本号值对象
/// </summary>
public sealed class Version : ValueObject
{
    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }
    public string? PreRelease { get; }
    public string? BuildMetadata { get; }

    public Version(int major, int minor, int patch, string? preRelease = null, string? buildMetadata = null)
    {
        if (major < 0 || minor < 0 || patch < 0)
            throw new ArgumentException("Version numbers cannot be negative.");

        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = preRelease;
        BuildMetadata = buildMetadata;
    }

    public static Version Parse(string versionString)
    {
        var parts = versionString.Split('+');
        var versionPart = parts[0];
        var buildMetadata = parts.Length > 1 ? parts[1] : null;

        var versionParts = versionPart.Split('-');
        var mainVersion = versionParts[0];
        var preRelease = versionParts.Length > 1 ? versionParts[1] : null;

        var numbers = mainVersion.Split('.').Select(int.Parse).ToArray();
        if (numbers.Length != 3)
            throw new FormatException("Invalid version format.");

        return new Version(numbers[0], numbers[1], numbers[2], preRelease, buildMetadata);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Major;
        yield return Minor;
        yield return Patch;
        yield return PreRelease;
    }

    public bool IsGreaterThan(Version other)
    {
        if (Major != other.Major) return Major > other.Major;
        if (Minor != other.Minor) return Minor > other.Minor;
        if (Patch != other.Patch) return Patch > other.Patch;

        // Pre-release versions are considered lower than release versions
        if (string.IsNullOrEmpty(PreRelease) && !string.IsNullOrEmpty(other.PreRelease))
            return true;

        if (!string.IsNullOrEmpty(PreRelease) && string.IsNullOrEmpty(other.PreRelease))
            return false;

        return string.Compare(PreRelease, other.PreRelease, StringComparison.Ordinal) > 0;
    }

    public override string ToString()
    {
        var version = $"{Major}.{Minor}.{Patch}";
        if (!string.IsNullOrEmpty(PreRelease))
            version += $"-{PreRelease}";
        if (!string.IsNullOrEmpty(BuildMetadata))
            version += $"+{BuildMetadata}";
        return version;
    }
}
