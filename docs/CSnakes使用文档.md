# CSnakes 使用文档

## 目录
1. [简介](#简介)
2. [核心概念](#核心概念)
3. [环境准备](#环境准备)
4. [快速开始](#快速开始)
5. [类型映射](#类型映射)
6. [高级特性](#高级特性)
7. [最佳实践](#最佳实践)
8. [常见问题](#常见问题)
9. [实战案例](#实战案例)

---

## 简介

### 什么是 CSnakes？

CSnakes 是一个 .NET 源生成器和运行时，允许你将 Python 代码和库嵌入到 C# .NET 解决方案中。它提供了高性能、低级别的集成，无需 REST、HTTP 或微服务等中间层。

### 主要特性

- 🤖 支持 .NET 8 和 9
- 🐍 支持 Python 3.9-3.13
- 📦 支持虚拟环境和 C 扩展
- 💻 支持 Windows、macOS 和 Linux
- 🧮 NumPy ndarrays 与 Spans、2D Spans 和 TensorSpans（.NET 9）的紧密集成
- ⚡ 使用 Python 的 C-API 实现快速调用
- 🧠 使用 Python 类型提示生成具有 .NET 原生类型的函数签名
- 🧵 支持 CPython 3.13 "free-threading" 模式
- 🧩 支持嵌套序列和映射类型（`tuple`、`dict`、`list`）
- 🏷️ 支持默认值
- 🔥 支持热重载
- 🚀 支持 UV 快速安装 Python 包和依赖项

### 为什么选择 CSnakes？

**传统方案的问题：**
- ❌ IronPython：兼容性问题，无法使用 C 扩展库（如 NumPy）
- ❌ Python.NET：需要手动处理 GIL 和数据编组
- ❌ HTTP/RPC：进程间通信开销大
- ❌ 子进程调用：需要解析输出，错误处理复杂

**CSnakes 的优势：**
- ✅ 直接在 .NET 进程内调用 Python
- ✅ 自动类型编组
- ✅ 自动 GIL 管理
- ✅ 零拷贝缓冲区（适用于大数据）
- ✅ 编译时类型检查
- ✅ Visual Studio 智能感知支持

---

## 核心概念

### 架构原理

```
┌─────────────────────────────────────────┐
│           .NET 应用程序                   │
│  ┌───────────────────────────────────┐  │
│  │      生成的 C# 包装类              │  │
│  │  (由源生成器自动生成)              │  │
│  └───────────────┬───────────────────┘  │
│                  │                       │
│  ┌───────────────▼───────────────────┐  │
│  │      CSnakes 运行时                │  │
│  │  (GIL 管理、类型编组、C-API 调用)  │  │
│  └───────────────┬───────────────────┘  │
│                  │                       │
│  ┌───────────────▼───────────────────┐  │
│  │      Python 嵌入式运行时           │  │
│  │      (同一进程空间)                │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

### 工作流程

1. **源生成阶段**：编译时分析 Python 文件
2. **代码生成**：创建 C# 包装类和接口
3. **运行时初始化**：设置 Python 环境
4. **函数调用**：通过 C-API 直接调用 Python
5. **类型转换**：自动处理 .NET ↔ Python 类型转换

### GIL（全局解释器锁）管理

Python 的 GIL 确保只有单个线程执行 Python 字节码。CSnakes 自动处理：

- 自动获取和释放 GIL
- 递归锁避免死锁
- 线程静态跟踪
- 与 .NET GC 的协同

---

## 环境准备

### 系统要求

- **.NET SDK**：8.0 或更高版本（推荐 9.0+）
- **操作系统**：Windows、macOS 或 Linux
- **IDE**：Visual Studio 2022 或 VS Code（支持 C# Dev Kit）

### 安装 NuGet 包

创建项目后安装以下包：

```bash
# 核心运行时
dotnet add package CSnakes.Runtime

# 依赖注入集成（推荐）
dotnet add package CSnakes.Extensions.Microsoft.DI

# 如果需要使用 UV（更快的包管理）
dotnet add package CSnakes.UV
```

### Python 版本支持

| Python 版本 | 支持状态 | 推荐用途              |
| --------- | ---- | ----------------- |
| 3.9       | ✅ 支持 | 稳定版本              |
| 3.10      | ✅ 支持 | 推荐                |
| 3.11      | ✅ 支持 | 性能改进              |
| 3.12      | ✅ 支持 | 最新特性              |
| 3.13      | ✅ 支持 | Free-threading 模式 |

---

## 快速开始

### 步骤 1：创建项目

```bash
# 创建控制台应用
dotnet new console -n CSnakesDemo
cd CSnakesDemo

# 安装包
dotnet add package CSnakes.Runtime
dotnet add package CSnakes.Extensions.Microsoft.DI
```

### 步骤 2：创建 Python 文件

创建 `myfunctions.py`：

```python
def hello_world(name: str, age: int) -> str:
    """向用户问好"""
    return f"Hello {name}, you must be {age} years old!"

def add_numbers(a: int, b: int) -> int:
    """添加两个数字"""
    return a + b

def get_user_info() -> dict:
    """返回用户信息字典"""
    return {
        "name": "Alice",
        "age": 30,
        "email": "alice@example.com"
    }
```

### 步骤 3：配置项目文件

编辑 `.csproj` 文件，添加 Python 文件作为附加文件：

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <!-- CSnakes 包 -->
    <PackageReference Include="CSnakes.Runtime" Version="1.1.0-beta.*" />
    <PackageReference Include="CSnakes.Extensions.Microsoft.DI" Version="1.1.0-beta.*" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.0" />
  </ItemGroup>

  <ItemGroup>
    <!-- Python 文件 -->
    <AdditionalFiles Include="myfunctions.py" />
    <AdditionalFiles Include="requirements.txt" />
  </ItemGroup>

</Project>
```

### 步骤 4：创建 requirements.txt

```
# requirements.txt
# 在这里列出你的 Python 依赖
# 例如：
# numpy
# pandas
```

### 步骤 5：编写 C# 代码

```csharp
using CSnakes.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// 配置 Python 环境
builder.Services.AddPython()
    .WithHomeDirectory("python")  // Python 安装目录
    .WithVirtualEnvironment("venv")  // 虚拟环境名称
    .WithRequirementsFile("requirements.txt");  // 依赖文件

var host = builder.Build();

// 获取 Python 环境和模块
var env = host.Services.GetRequiredService<IPythonEnvironment>();
var myFunctions = env.Myfunctions();

// 调用 Python 函数
string greeting = myFunctions.HelloWorld("Bob", 25);
Console.WriteLine(greeting);
// 输出: Hello Bob, you must be 25 years old!

int sum = myFunctions.AddNumbers(10, 20);
Console.WriteLine($"10 + 20 = {sum}");
// 输出: 10 + 20 = 30

// 获取字典数据
var userInfo = myFunctions.GetUserInfo();
Console.WriteLine($"User: {userInfo["name"]}, Age: {userInfo["age"]}");
```

### 步骤 6：运行

```bash
dotnet run
```

首次运行会自动：
1. 下载 Python 运行时
2. 创建虚拟环境
3. 安装 requirements.txt 中的依赖
4. 生成 C# 包装类
5. 运行应用程序

---

## 类型映射

### 基本类型

| Python 类型 | C# 类型 | 说明 |
|------------|---------|------|
| `int` | `long` | Python int 是任意精度，C# long 可能溢出时使用 BigInteger |
| `float` | `double` | 双精度浮点数 |
| `str` | `string` | 字符串 |
| `bool` | `bool` | 布尔值 |
| `bytes` | `byte[]` | 字节数组 |
| `None` | `null` | 空值 |

### 集合类型

| Python 类型 | C# 类型 | 特性 |
|------------|---------|------|
| `list[T]` | `IReadOnlyList<T>` | 只读列表，延迟加载 |
| `dict[K, V]` | `IReadOnlyDictionary<K, V>` | 只读字典，延迟加载 |
| `tuple[T, ...]` | `IReadOnlyList<T>` | 元组作为不可变列表 |
| `set[T]` | `IReadOnlySet<T>` | 只读集合 |

### NumPy 类型（需要 NumPy 类型提示）

```python
from typing import Annotated
import numpy as np
from numpy.typing import NDArray

# 使用 NumPy 数组
def process_array(data: NDArray[np.float64]) -> NDArray[np.float64]:
    return data * 2

# C# 中会映射为：
# Span<double> process_array(ReadOnlySpan<double> data)
```

### 复杂类型示例

**Python 代码：**
```python
from typing import List, Dict, Optional, Tuple

def complex_example(
    items: List[int],
    mapping: Dict[str, float],
    optional_value: Optional[str] = None
) -> Tuple[str, int]:
    count = len(items)
    total = sum(items)
    return (optional_value or "default", total)
```

**生成的 C# 签名：**
```csharp
public IReadOnlyList<string> ComplexExample(
    IReadOnlyList<long> items,
    IReadOnlyDictionary<string, double> mapping,
    string? optionalValue = null
);
```

---

## 高级特性

### 1. 异步支持

Python 的 `async` 函数会自动转换为 .NET `Task`：

**Python 代码：**
```python
import asyncio

async def fetch_data(url: str) -> str:
    await asyncio.sleep(1)  # 模拟异步操作
    return f"Data from {url}"
```

**C# 调用：**
```csharp
var result = await myFunctions.FetchDataAsync("https://example.com");
Console.WriteLine(result);
```

### 2. 零拷贝缓冲区

处理大型数组时避免数据复制：

**Python 代码：**
```python
import numpy as np

def process_large_array(data: np.ndarray) -> bytes:
    # 处理大型数组
    result = data.tobytes()
    return result
```

**C# 调用：**
```csharp
// 使用 Span 避免拷贝
double[] data = new double[1000000];
// ... 填充数据 ...

var result = myFunctions.ProcessLargeArray(data);
// 数据直接从 Python 内存传递，无需复制
```

### 3. 热重载

在开发过程中，修改 Python 代码会自动重新生成 C# 包装类：

1. 修改 `.py` 文件
2. 保存文件
3. 源生成器自动检测更改
4. 重新生成 C# 类型
5. 立即看到更新的智能感知

### 4. 使用 UV（快速包管理）

UV 是比 pip 更快的包管理器：

```csharp
builder.Services.AddPython()
    .WithHomeDirectory("python")
    .WithVirtualEnvironment("venv")
    .WithRequirementsFile("requirements.txt")
    .UseUV();  // 启用 UV
```

### 5. 自定义 Python 路径

如果需要使用特定的 Python 安装：

```csharp
builder.Services.AddPython()
    .WithPythonVersion("3.11")  // 指定版本
    .WithPythonPath("/usr/bin/python3.11")  // 或指定路径
    .WithVirtualEnvironment("venv");
```

### 6. 环境变量配置

```csharp
builder.Services.AddPython()
    .WithHomeDirectory("python")
    .WithVirtualEnvironment("venv")
    .WithEnvironmentVariable("MY_VAR", "value");
```

---

## 最佳实践

### 1. 函数设计

**✅ 推荐：**
```python
def calculate_sum(numbers: list[int]) -> int:
    """计算列表中数字的总和"""
    return sum(numbers)
```

**❌ 避免：**
```python
# 缺少类型提示
def calculate_sum(numbers):
    return sum(numbers)
```

### 2. 错误处理

**Python 代码：**
```python
def safe_divide(a: int, b: int) -> float:
    """安全除法，处理除零错误"""
    if b == 0:
        raise ValueError("Cannot divide by zero")
    return a / b
```

**C# 调用：**
```csharp
try
{
    double result = myFunctions.SafeDivide(10, 0);
}
catch (PythonException ex)
{
    Console.WriteLine($"Python error: {ex.Message}");
}
```

### 3. 性能优化

**使用 NumPy 而不是 Python 列表：**

```python
import numpy as np

# ✅ 快速
def fast_process(data: np.ndarray) -> np.ndarray:
    return data * 2

# ❌ 慢
def slow_process(data: list) -> list:
    return [x * 2 for x in data]
```

### 4. 依赖管理

**requirements.txt：**
```
# 明确指定版本
numpy==1.24.3
pandas==2.0.2

# 或使用版本范围
requests>=2.28.0,<3.0.0
```

### 5. 项目结构

```
MyProject/
├── Python/               # Python 代码目录
│   ├── __init__.py
│   ├── ml_models.py
│   └── utils.py
├── requirements.txt
├── .csproj
└── Program.cs
```

**配置 .csproj：**
```xml
<ItemGroup>
  <AdditionalFiles Include="Python/**/*.py" />
</ItemGroup>
```

---

## 常见问题

### Q1: 如何调试 Python 代码？

**A:** 目前 CSnakes 不支持直接调试 Python 代码。建议：
1. 在 Python 环境中单独测试代码
2. 使用 Python 的 `logging` 模块
3. 添加详细的错误处理

```python
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

def my_function(value: int) -> int:
    logger.info(f"Processing value: {value}")
    return value * 2
```

### Q2: 如何处理大型对象？

**A:** 使用缓冲区和延迟加载：

```python
from typing import Iterator

def get_large_data() -> Iterator[bytes]:
    """生成大型数据块"""
    for i in range(1000):
        yield f"Chunk {i}".encode()
```

### Q3: 支持哪些 Python 包？

**A:** 几乎所有纯 Python 包和 C 扩展包，包括：
- NumPy, Pandas, SciPy
- PyTorch, TensorFlow
- scikit-learn
- OpenCV
- 等等...

**不支持：**
- 需要特定操作系统功能且与 .NET 不兼容的包
- 某些多进程相关的包

### Q4: 如何部署应用？

**A:** 部署选项：

1. **自包含部署：**
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

2. **依赖框架部署：**
```bash
dotnet publish -c Release -r win-x64
```

Python 运行时会自动包含在发布中。

### Q5: 内存管理如何工作？

**A:** CSnakes 自动处理：
- Python 对象的引用计数
- .NET GC 的协同
- 缓冲区的生命周期

**注意：** 对于大型缓冲区，使用 `using` 语句确保及时释放：

```csharp
using var buffer = myFunctions.GetLargeBuffer();
// 使用 buffer
// 自动释放
```

### Q6: 如何在 WPF/WinForms 中使用？

**A:** 示例（WPF）：

```csharp
public partial class MainWindow : Window
{
    private readonly IPythonEnvironment _env;
    private readonly MyPythonFunctions _functions;

    public MainWindow(IPythonEnvironment env)
    {
        InitializeComponent();
        _env = env;
        _functions = env.MyPythonFunctions();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        // 在后台线程调用 Python
        var result = await Task.Run(() => _functions.ProcessData(InputText.Text));
        OutputText.Text = result;
    }
}
```

---

## 实战案例

### 案例 1：机器学习预测

**Python 代码 (`ml_predictor.py`)：**
```python
from typing import List
import joblib
import numpy as np

class Predictor:
    def __init__(self):
        # 加载预训练模型
        self.model = joblib.load("model.pkl")

    def predict(self, features: List[float]) -> float:
        """预测单个样本"""
        data = np.array(features).reshape(1, -1)
        return float(self.model.predict(data)[0])

    def predict_batch(self, features: List[List[float]]) -> List[float]:
        """批量预测"""
        data = np.array(features)
        return self.model.predict(data).tolist()
```

**C# 调用：**
```csharp
// 初始化
var predictor = env.MlPredictor();

// 单个预测
var features = new List<double> { 1.5, 2.3, 0.8, 1.1 };
double prediction = predictor.Predict(features);
Console.WriteLine($"Prediction: {prediction}");

// 批量预测
var batchFeatures = new List<List<double>>
{
    new() { 1.5, 2.3, 0.8, 1.1 },
    new() { 2.1, 1.8, 1.2, 0.9 },
    new() { 0.5, 1.2, 2.1, 1.5 }
};
var predictions = predictor.PredictBatch(batchFeatures);
```

### 案例 2：数据处理管道

**Python 代码 (`data_processor.py`)：**
```python
import pandas as pd
from typing import Dict, Any

def process_csv(file_path: str) -> Dict[str, Any]:
    """处理 CSV 文件并返回统计信息"""
    df = pd.read_csv(file_path)

    return {
        "rows": len(df),
        "columns": len(df.columns),
        "mean": df.mean(numeric_only=True).to_dict(),
        "null_counts": df.isnull().sum().to_dict()
    }

def filter_data(
    data: List[Dict[str, Any]],
    column: str,
    threshold: float
) -> List[Dict[str, Any]]:
    """过滤数据"""
    df = pd.DataFrame(data)
    filtered = df[df[column] > threshold]
    return filtered.to_dict(orient="records")
```

**C# 调用：**
```csharp
var processor = env.DataProcessor();

// 处理 CSV
var stats = processor.ProcessCsv("data.csv");
Console.WriteLine($"Rows: {stats["rows"]}, Columns: {stats["columns"]}");

// 过滤数据
var data = new List<Dictionary<string, object>>
{
    new() { { "name", "Alice" }, { "score", 85.5 } },
    new() { { "name", "Bob" }, { "score", 92.0 } },
    new() { { "name", "Charlie" }, { "score", 78.5 } }
};
var filtered = processor.FilterData(data, "score", 80.0);
```

### 案例 3：图像处理

**Python 代码 (`image_processor.py`)：**
```python
from PIL import Image
import io
from typing import Optional

def resize_image(image_bytes: bytes, width: int, height: int) -> bytes:
    """调整图像大小"""
    img = Image.open(io.BytesIO(image_bytes))
    resized = img.resize((width, height))

    output = io.BytesIO()
    resized.save(output, format="PNG")
    return output.getvalue()

def apply_filter(
    image_bytes: bytes,
    filter_type: str,
    intensity: float = 1.0
) -> bytes:
    """应用图像滤镜"""
    img = Image.open(io.BytesIO(image_bytes))

    if filter_type == "grayscale":
        filtered = img.convert("L")
    elif filter_type == "blur":
        from PIL import ImageFilter
        filtered = img.filter(ImageFilter.GaussianBlur(radius=intensity))
    else:
        filtered = img

    output = io.BytesIO()
    filtered.save(output, format="PNG")
    return output.getvalue()
```

**C# 调用（WPF）：**
```csharp
public async Task<BitmapImage> ProcessImageAsync(string imagePath)
{
    var imageBytes = await File.ReadAllBytesAsync(imagePath);
    var processor = env.ImageProcessor();

    // 调整大小
    var resizedBytes = processor.ResizeImage(image_bytes, 800, 600);

    // 转换为 BitmapImage
    var image = new BitmapImage();
    image.BeginInit();
    image.StreamSource = new MemoryStream(resizedBytes);
    image.EndInit();
    image.Freeze();

    return image;
}
```

### 案例 4：自然语言处理

**Python 代码 (`nlp_processor.py`)：**
```python
from typing import List, Dict
import re

def extract_keywords(text: str, top_n: int = 5) -> List[str]:
    """提取关键词"""
    # 简单的关键词提取
    words = re.findall(r'\b[a-zA-Z]{4,}\b', text.lower())

    from collections import Counter
    word_counts = Counter(words)

    return [word for word, _ in word_counts.most_common(top_n)]

def analyze_sentiment(text: str) -> Dict[str, float]:
    """简单的情感分析"""
    # 这是一个简化的示例
    positive_words = {"good", "great", "excellent", "amazing", "wonderful"}
    negative_words = {"bad", "terrible", "awful", "horrible", "poor"}

    words = set(text.lower().split())

    positive_score = len(words & positive_words)
    negative_score = len(words & negative_words)

    total = positive_score + negative_score
    if total == 0:
        sentiment = 0.5  # 中性
    else:
        sentiment = positive_score / total

    return {
        "sentiment": sentiment,
        "positive": positive_score,
        "negative": negative_score
    }
```

**C# 调用：**
```csharp
var nlp = env.NlpProcessor();

string text = "This is an amazing product! I really love it.";
var keywords = nlp.ExtractKeywords(text);
Console.WriteLine("Keywords: " + string.Join(", ", keywords));

var sentiment = nlp.AnalyzeSentiment(text);
Console.WriteLine($"Sentiment: {sentiment["sentiment"]:P2}");
Console.WriteLine($"Positive: {sentiment["positive"]}, Negative: {sentiment["negative"]}");
```

### 案例 5：科学计算

**Python 代码 (`scientific_calculator.py`)：**
```python
import numpy as np
from scipy import integrate

def calculate_integral(func: str, a: float, b: float) -> float:
    """计算定积分"""
    # 安全地评估函数
    def f(x):
        return eval(func, {"x": x, "np": np, "math": __import__("math")})

    result, error = integrate.quad(f, a, b)
    return float(result)

def solve_equation(coeffs: List[float]) -> List[complex]:
    """求解多项式方程"""
    # coeffs 是系数列表，从最高次到常数项
    roots = np.roots(coeffs)
    return [complex(r) for r in roots]

def matrix_multiply(matrix_a: List[List[float]], matrix_b: List[List[float]]) -> List[List[float]]:
    """矩阵乘法"""
    a = np.array(matrix_a)
    b = np.array(matrix_b)
    result = np.dot(a, b)
    return result.tolist()
```

**C# 调用：**
```csharp
var calc = env.ScientificCalculator();

// 计算积分
double integral = calc.CalculateIntegral("x**2 + 2*x + 1", 0, 10);
Console.WriteLine($"Integral: {integral}");

// 求解方程 x² - 5x + 6 = 0
var coeffs = new List<double> { 1, -5, 6 };
var roots = calc.SolveEquation(coeffs);
foreach (var root in roots)
{
    Console.WriteLine($"Root: {root}");
}

// 矩阵乘法
var matrixA = new List<List<double>>
{
    new() { 1, 2 },
    new() { 3, 4 }
};
var matrixB = new List<List<double>>
{
    new() { 5, 6 },
    new() { 7, 8 }
};
var result = calc.MatrixMultiply(matrixA, matrixB);
```

---

## 附录

### A. 完整示例项目

```bash
# 创建项目
dotnet new sln -n CSnakesDemo
dotnet new console -n CSnakesDemo.App
dotnet sln add CSnakesDemo.App/CSnakesDemo.App.csproj

# 添加包
cd CSnakesDemo.App
dotnet add package CSnakes.Runtime
dotnet add package CSnakes.Extensions.Microsoft.DI
dotnet add package Microsoft.Extensions.Hosting

cd ..
```

### B. 依赖项版本建议

```xml
<ItemGroup>
  <PackageReference Include="CSnakes.Runtime" Version="1.1.0-beta.*" />
  <PackageReference Include="CSnakes.Extensions.Microsoft.DI" Version="1.1.0-beta.*" />
  <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.0" />
</ItemGroup>
```

### C. 参考资源

**官方文档：**
- [CSnakes GitHub 仓库](https://github.com/tonybaloney/CSnakes)
- [NuGet 包](https://www.nuget.org/packages/CSnakes.Runtime)

**教程和文章：**
- [Embedding Python in .NET with CSnakes](https://atalupadhyay.wordpress.com/2025/12/05/embedding-python-in-net-with-csnakes/)
- [Running Python Code within .NET Projects](https://jaliyaudagedara.blogspot.com/2025/06/running-python-code-within-net-projects.html)
- [Bridging Python and .NET: Hello CSnakes](https://tjgokken.com/bridging-python-and-net-hello-csnakes)

**视频内容：**
- [Using AI Python Libraries in .NET Apps with CSnakes](https://learn.microsoft.com/en-us/shows/on-dotnet/deep-dotnet-using-ai-python-libraries-in-dotnet-apps-with-csnakes)
- [Talk Python to Me Ep.486](https://www.youtube.com/watch?v=Ur3kLHxG3Gc)

---

## 总结

CSnakes 提供了一种强大而优雅的方式在 .NET 应用中集成 Python 代码。它的主要优势包括：

1. **类型安全**：编译时类型检查和智能感知
2. **高性能**：直接调用 Python C-API，零拷贝缓冲区
3. **易用性**：自动类型编组和 GIL 管理
4. **灵活性**：支持几乎所有 Python 包和 C 扩展

通过本文档，你应该能够：
- 理解 CSnakes 的工作原理
- 在 .NET 项目中集成 Python 代码
- 处理类型映射和数据转换
- 实现常见的使用场景
- 遵循最佳实践构建应用

祝你使用 CSnakes 开发愉快！

---

**文档版本：** 1.0
**最后更新：** 2025-01-19
**适用版本：** CSnakes 1.1.0-beta+
