#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Lemoo.UI 图标代码生成脚本

.DESCRIPTION
    完整的数据驱动图标生成流程：
    1. 从 TTF 字体文件提取图标元数据
    2. 生成 IconMetadata.json
    3. 生成 IconKind.cs 枚举代码

.PARAMETER SkipParse
    跳过字体解析，使用现有的 IconMetadata.json

.PARAMETER Method
    代码生成方法：T4 或 SourceGenerator

.EXAMPLE
    .\generate-icons.ps1
    完整流程：解析字体 → 生成JSON → 生成代码

.EXAMPLE
    .\generate-icons.ps1 -SkipParse
    仅生成代码（使用现有 JSON）

.EXAMPLE
    .\generate-icons.ps1 -Method SourceGenerator
    使用 Source Generator 方法
#>

[CmdletBinding()]
param(
    [switch]$SkipParse,
    [ValidateSet('T4', 'SourceGenerator')]
    [string]$Method = 'T4'
)

$ErrorActionPreference = 'Stop'
$ToolsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ToolsDir
$FontPath = Join-Path $ProjectRoot "src\UI\Lemoo.UI\Resources\Fonts\Segoe Fluent Icons.ttf"
$MetadataPath = Join-Path $ToolsDir "IconMetadata.json"
$PythonScript = Join-Path $ToolsDir "parse_font.py"

function Write-Step {
    param([string]$Message)
    Write-Host "`n► $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

# 检查 Python 环境
function Test-Python {
    try {
        $null = python --version 2>&1
        return $true
    } catch {
        return $false
    }
}

# 检查 fonttools
function Test-FontTools {
    try {
        $null = python -c "import fontTools" 2>&1
        return $true
    } catch {
        return $false
    }
}

# 安装 fonttools
function Install-FontTools {
    Write-Step "安装 fonttools..."
    pip install fonttools
    if ($LASTEXITCODE -eq 0) {
        Write-Success "fonttools 安装成功"
    } else {
        Write-Error "fonttools 安装失败"
        exit 1
    }
}

# 步骤 1: 解析字体文件
function Parse-Font {
    Write-Step "步骤 1: 解析 Segoe Fluent Icons.ttf"

    if (-not (Test-Path $FontPath)) {
        Write-Error "字体文件不存在: $FontPath"
        exit 1
    }

    Write-Host "  字体文件: $FontPath"
    Write-Host "  输出文件: $MetadataPath"

    & python $PythonScript
    if ($LASTEXITCODE -eq 0) {
        Write-Success "字体解析完成"
    } else {
        Write-Error "字体解析失败"
        exit 1
    }
}

# 步骤 2: 生成代码
function Generate-Code {
    Write-Step "步骤 2: 生成 IconKind.cs"

    switch ($Method) {
        'T4' {
            Write-Host "  方法: T4 模板"
            Write-Host "  请在 Visual Studio 中执行以下操作:"
            Write-Host "    1. 打开 src\UI\Lemoo.UI\Models\Icons\IconKind.tt"
            Write-Host "    2. 右键 → 运行自定义工具 (Ctrl+Shift+B)"
            Write-Host "  或使用命令行:"
            Write-Host "    dotnet tool install -g dotnet-t4"
            Write-Host "    dotnet-t4 IconKind.tt"
        }
        'SourceGenerator' {
            Write-Host "  方法: Source Generator"
            Write-Host "  代码将在编译时自动生成"
            Write-Host "  请执行: dotnet build src\UI\Lemoo.UI\Lemoo.UI.csproj"
        }
    }
}

# 主流程
function Main {
    Write-Host "========================================" -ForegroundColor DarkCyan
    Write-Host "  Lemoo.UI 图标代码生成器" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor DarkCyan

    # 检查依赖
    if (-not $SkipParse) {
        if (-not (Test-Python)) {
            Write-Error "未找到 Python，请先安装 Python 3.7+"
            Write-Host "下载地址: https://www.python.org/downloads/"
            exit 1
        }

        Write-Success "Python 版本: $(python --version 2>&1)"

        if (-not (Test-FontTools)) {
            Write-Host "fonttools 未安装，正在安装..."
            Install-FontTools
        } else {
            Write-Success "fonttools 已安装"
        }
    }

    # 执行流程
    if (-not $SkipParse) {
        Parse-Font
    } else {
        Write-Step "跳过字体解析，使用现有 JSON"
    }

    Generate-Code

    Write-Host "`n========================================" -ForegroundColor DarkCyan
    Write-Success "图标生成完成！"
    Write-Host "========================================" -ForegroundColor DarkCyan

    # 显示统计信息
    if (Test-Path $MetadataPath) {
        Write-Step "统计信息"
        $metadata = Get-Content $MetadataPath | ConvertFrom-Json
        Write-Host "  图标总数: $($metadata.icons.Count)"
        Write-Host "  分类数量: $($metadata.categories.Count)"
        Write-Host "  字体版本: $($metadata.font.version)"
    }
}

# 执行
try {
    Main
} catch {
    Write-Error "错误: $_"
    exit 1
}
