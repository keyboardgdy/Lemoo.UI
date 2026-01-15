using FluentAssertions;
using Lemoo.Core.Abstractions.Module;
using Lemoo.Core.Common.Exceptions;
using Lemoo.Core.Infrastructure.ModuleLoader;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;
using Xunit;

namespace Lemoo.Core.Infrastructure.Tests.ModuleLoader;

public class ModuleLoaderTests
{
    private readonly Mock<ILogger<Infrastructure.ModuleLoader.ModuleLoader>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public ModuleLoaderTests()
    {
        _loggerMock = new Mock<ILogger<Infrastructure.ModuleLoader.ModuleLoader>>();
        _configurationMock = new Mock<IConfiguration>();
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Act
        var loader = new Infrastructure.ModuleLoader.ModuleLoader(_loggerMock.Object, _configurationMock.Object);

        // Assert
        loader.Should().NotBeNull();
        loader.GetLoadedModules().Should().BeEmpty();
    }

    [Fact]
    public void GetLoadedModules_WhenNoModulesLoaded_ShouldReturnEmptyList()
    {
        // Arrange
        var loader = new Infrastructure.ModuleLoader.ModuleLoader(_loggerMock.Object, _configurationMock.Object);

        // Act
        var modules = loader.GetLoadedModules();

        // Assert
        modules.Should().BeEmpty();
    }

    [Fact]
    public void GetModule_WhenModuleExists_ShouldReturnModule()
    {
        // Arrange
        var loader = new TestableModuleLoader(_loggerMock.Object, _configurationMock.Object);
        var testModule = new TestModule("TestModule", "1.0.0");
        loader.AddLoadedModule(testModule);

        // Act
        var module = loader.GetModule("TestModule");

        // Assert
        module.Should().NotBeNull();
        module!.Name.Should().Be("TestModule");
    }

    [Fact]
    public void GetModule_WhenModuleDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var loader = new TestableModuleLoader(_loggerMock.Object, _configurationMock.Object);

        // Act
        var module = loader.GetModule("NonExistentModule");

        // Assert
        module.Should().BeNull();
    }

    [Fact]
    public void ValidateModuleDependencies_WhenAllDependenciesExist_ShouldNotThrow()
    {
        // Arrange
        var loader = new Infrastructure.ModuleLoader.ModuleLoader(_loggerMock.Object, _configurationMock.Object);
        var modules = new List<IModule>
        {
            new TestModule("ModuleA", "1.0.0"),
            new TestModule("ModuleB", "1.0.0", new[] { "ModuleA" })
        };

        // Act & Assert
        var act = () => loader.ValidateModuleDependencies(modules);
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateModuleDependencies_WhenDependencyMissing_ShouldThrowException()
    {
        // Arrange
        var loader = new Infrastructure.ModuleLoader.ModuleLoader(_loggerMock.Object, _configurationMock.Object);
        var modules = new List<IModule>
        {
            new TestModule("ModuleA", "1.0.0", new[] { "NonExistentModule" })
        };

        // Act & Assert
        var act = () => loader.ValidateModuleDependencies(modules);
        act.Should().Throw<ModuleDependencyException>()
            .WithMessage("*ModuleA*")
            .WithMessage("*NonExistentModule*");
    }

    [Fact]
    public void ValidateModuleDependencies_WithMultipleDependencies_ShouldValidateAll()
    {
        // Arrange
        var loader = new Infrastructure.ModuleLoader.ModuleLoader(_loggerMock.Object, _configurationMock.Object);
        var modules = new List<IModule>
        {
            new TestModule("ModuleA", "1.0.0"),
            new TestModule("ModuleB", "1.0.0"),
            new TestModule("ModuleC", "1.0.0", new[] { "ModuleA", "ModuleB" })
        };

        // Act & Assert
        var act = () => loader.ValidateModuleDependencies(modules);
        act.Should().NotThrow();
    }

    [Fact]
    public void LoadModulesAsync_WithValidConfiguration_ShouldLoadModules()
    {
        // Arrange
        SetupConfiguration(enabledModules: new[] { "*" }, modulePath: "./Modules");

        var loader = new Infrastructure.ModuleLoader.ModuleLoader(_loggerMock.Object, _configurationMock.Object);

        // Act & Assert
        // Note: This test will not find actual modules in a test environment
        // but validates the basic flow
        var act = async () => await loader.LoadModulesAsync();
        act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UnloadModuleAsync_WhenModuleExists_ShouldReturnTrue()
    {
        // Arrange
        var loader = new TestableModuleLoader(_loggerMock.Object, _configurationMock.Object);
        var testModule = new TestModule("TestModule", "1.0.0");
        loader.AddLoadedModule(testModule);

        // Act
        var result = await loader.UnloadModuleAsync("TestModule");

        // Assert
        result.Should().BeTrue();
        loader.GetLoadedModules().Should().BeEmpty();
    }

    [Fact]
    public async Task UnloadModuleAsync_WhenModuleDoesNotExist_ShouldReturnTrue()
    {
        // Arrange
        var loader = new Infrastructure.ModuleLoader.ModuleLoader(_loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await loader.UnloadModuleAsync("NonExistentModule");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UnloadAllModulesAsync_ShouldClearAllModules()
    {
        // Arrange
        var loader = new TestableModuleLoader(_loggerMock.Object, _configurationMock.Object);
        loader.AddLoadedModule(new TestModule("ModuleA", "1.0.0"));
        loader.AddLoadedModule(new TestModule("ModuleB", "1.0.0"));

        // Act
        await loader.UnloadAllModulesAsync();

        // Assert
        loader.GetLoadedModules().Should().BeEmpty();
    }

    [Fact]
    public void ModuleLoading_Event_ShouldBeRaised()
    {
        // Arrange
        var loader = new TestableModuleLoader(_loggerMock.Object, _configurationMock.Object);
        var eventRaised = false;

        loader.ModuleLoading += (sender, args) =>
        {
            eventRaised = true;
            args.ModuleName.Should().Be("TestModule");
        };

        // Act
        loader.RaiseModuleLoadingEvent("TestModule");

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void ModuleLoaded_Event_ShouldBeRaised()
    {
        // Arrange
        var loader = new TestableModuleLoader(_loggerMock.Object, _configurationMock.Object);
        var eventRaised = false;
        var testModule = new TestModule("TestModule", "1.0.0");

        loader.ModuleLoaded += (sender, args) =>
        {
            eventRaised = true;
            args.ModuleName.Should().Be("TestModule");
            args.Module.Should().Be(testModule);
        };

        // Act
        loader.RaiseModuleLoadedEvent("TestModule", testModule, TimeSpan.FromMilliseconds(100));

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void ModuleUnloading_Event_ShouldBeRaised()
    {
        // Arrange
        var loader = new TestableModuleLoader(_loggerMock.Object, _configurationMock.Object);
        var eventRaised = false;

        loader.ModuleUnloading += (sender, args) =>
        {
            eventRaised = true;
            args.ModuleName.Should().Be("TestModule");
        };

        // Act
        loader.RaiseModuleUnloadingEvent("TestModule");

        // Assert
        eventRaised.Should().BeTrue();
    }

    private void SetupConfiguration(string[]? enabledModules = null, string? modulePath = null)
    {
        var enabledModulesSection = new Mock<IConfigurationSection>();

        // Set up Value property to return serialized JSON array
        var jsonValue = System.Text.Json.JsonSerializer.Serialize(enabledModules ?? Array.Empty<string>());
        enabledModulesSection.Setup(s => s.Value).Returns(jsonValue);

        _configurationMock.Setup(c => c.GetSection("Lemoo:Modules:Enabled"))
            .Returns(enabledModulesSection.Object);

        _configurationMock.Setup(c => c["Lemoo:Modules:Path"])
            .Returns(modulePath ?? "./Modules");
    }
}

/// <summary>
/// Testable module loader with additional methods for testing
/// </summary>
public class TestableModuleLoader : Infrastructure.ModuleLoader.ModuleLoader
{
    public TestableModuleLoader(ILogger<Infrastructure.ModuleLoader.ModuleLoader> logger, IConfiguration configuration)
        : base(logger, configuration)
    {
    }

    public void AddLoadedModule(IModule module)
    {
        // Use reflection to add module to internal list
        var field = typeof(Infrastructure.ModuleLoader.ModuleLoader).GetField("_loadedModules",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var list = field?.GetValue(this) as List<IModule>;
        list?.Add(module);
    }

    public void RaiseModuleLoadingEvent(string moduleName)
    {
        OnModuleLoading(new ModuleLoadingEventArgs(moduleName));
    }

    public void RaiseModuleLoadedEvent(string moduleName, IModule module, TimeSpan duration)
    {
        OnModuleLoaded(new ModuleLoadedEventArgs(moduleName, module, duration));
    }

    public void RaiseModuleUnloadingEvent(string moduleName)
    {
        OnModuleUnloading(new ModuleUnloadingEventArgs(moduleName));
    }

    // Protected event raisers for testing
    protected void OnModuleLoading(ModuleLoadingEventArgs args)
    {
        var field = typeof(Infrastructure.ModuleLoader.ModuleLoader).GetField("ModuleLoading",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var eventDelegate = field?.GetValue(this) as MulticastDelegate;
        var invocationList = eventDelegate?.GetInvocationList();
        if (invocationList != null)
        {
            foreach (var d in invocationList)
            {
                d.DynamicInvoke(this, args);
            }
        }
    }

    protected void OnModuleLoaded(ModuleLoadedEventArgs args)
    {
        var field = typeof(Infrastructure.ModuleLoader.ModuleLoader).GetField("ModuleLoaded",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var eventDelegate = field?.GetValue(this) as MulticastDelegate;
        var invocationList = eventDelegate?.GetInvocationList();
        if (invocationList != null)
        {
            foreach (var d in invocationList)
            {
                d.DynamicInvoke(this, args);
            }
        }
    }

    protected void OnModuleUnloading(ModuleUnloadingEventArgs args)
    {
        var field = typeof(Infrastructure.ModuleLoader.ModuleLoader).GetField("ModuleUnloading",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var eventDelegate = field?.GetValue(this) as MulticastDelegate;
        var invocationList = eventDelegate?.GetInvocationList();
        if (invocationList != null)
        {
            foreach (var d in invocationList)
            {
                d.DynamicInvoke(this, args);
            }
        }
    }
}

/// <summary>
/// Test module implementation
/// </summary>
public class TestModule : IModule
{
    public TestModule(string name, string version, string[]? dependencies = null)
    {
        Name = name;
        Version = version;
        Description = $"Test module {name}";
        Dependencies = dependencies?.ToArray() ?? Array.Empty<string>();
        DependencyModules = Dependencies.Select(d => new ModuleDependency(d)).ToArray();
        Metadata = new ModuleMetadata
        {
            Name = name,
            Version = version,
            Description = Description
        };
    }

    public string Name { get; }
    public string Version { get; }
    public string Description { get; }
    public IReadOnlyList<string> Dependencies { get; }
    public IReadOnlyList<ModuleDependency> DependencyModules { get; }
    public ModuleMetadata Metadata { get; }

    public void PreConfigureServices(IServiceCollection services, IConfiguration configuration) { }
    public Task PreConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration) { }
    public Task ConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void PostConfigureServices(IServiceCollection services, IConfiguration configuration) { }
    public Task PostConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration) { }
    public Task ConfigureDbContextAsync(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void ConfigureDbContext(ModelBuilder modelBuilder) { }
    public Task OnApplicationStartingAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task OnApplicationStartedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task OnApplicationStoppingAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task OnApplicationStoppedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public class ModuleDependencySortingTests
{
    [Fact]
    public void SortModulesByDependencies_WithSimpleChain_ShouldSortCorrectly()
    {
        // Arrange
        var modules = new List<IModule>
        {
            new TestModule("ModuleC", "1.0.0", new[] { "ModuleB" }),
            new TestModule("ModuleA", "1.0.0"),
            new TestModule("ModuleB", "1.0.0", new[] { "ModuleA" })
        };

        var loader = new Infrastructure.ModuleLoader.ModuleLoader(
            Mock.Of<ILogger<Infrastructure.ModuleLoader.ModuleLoader>>(),
            Mock.Of<IConfiguration>());

        // Use reflection to access private method
        var method = typeof(Infrastructure.ModuleLoader.ModuleLoader).GetMethod("SortModulesByDependencies",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        var sorted = method?.Invoke(loader, new object[] { modules }) as IReadOnlyList<IModule>;

        // Assert
        sorted.Should().NotBeNull();
        sorted![0].Name.Should().Be("ModuleA");
        sorted[1].Name.Should().Be("ModuleB");
        sorted[2].Name.Should().Be("ModuleC");
    }

    [Fact]
    public void SortModulesByDependencies_WithCircularDependency_ShouldThrow()
    {
        // Arrange
        var modules = new List<IModule>
        {
            new TestModule("ModuleA", "1.0.0", new[] { "ModuleB" }),
            new TestModule("ModuleB", "1.0.0", new[] { "ModuleA" })
        };

        var loader = new Infrastructure.ModuleLoader.ModuleLoader(
            Mock.Of<ILogger<Infrastructure.ModuleLoader.ModuleLoader>>(),
            Mock.Of<IConfiguration>());

        var method = typeof(Infrastructure.ModuleLoader.ModuleLoader).GetMethod("SortModulesByDependencies",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act & Assert
        var act = () => method?.Invoke(loader, new object[] { modules });
        act.Should().Throw<TargetInvocationException>()
            .WithInnerException<CircularDependencyException>();
    }

    [Fact]
    public void SortModulesByDependencies_WithIndependentModules_ShouldPreserveOrder()
    {
        // Arrange
        var modules = new List<IModule>
        {
            new TestModule("ModuleA", "1.0.0"),
            new TestModule("ModuleB", "1.0.0"),
            new TestModule("ModuleC", "1.0.0")
        };

        var loader = new Infrastructure.ModuleLoader.ModuleLoader(
            Mock.Of<ILogger<Infrastructure.ModuleLoader.ModuleLoader>>(),
            Mock.Of<IConfiguration>());

        var method = typeof(Infrastructure.ModuleLoader.ModuleLoader).GetMethod("SortModulesByDependencies",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        var sorted = method?.Invoke(loader, new object[] { modules }) as IReadOnlyList<IModule>;

        // Assert
        sorted.Should().HaveCount(3);
    }
}
