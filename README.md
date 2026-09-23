# 电机控制系统V1

基于 **Siemens S7-1200 + KTP700 HMI + C# WPF** 开发的电机控制与监控系统。

项目在传统 PLC + HMI 控制的基础上，引入 C# WPF 上位机，实现 **电机启停、PLC 状态监控、故障状态显示以及 PLC 通信管理**。

## 🏗️ 系统架构

```text
┌──────────────┐
│   C# WPF     │
│  MVVM UI     │
└──────┬───────┘
       │ S7.NetPlus
       ▼
┌──────────────┐
│ Siemens PLC  │
│ S7-1200      │
└──────┬───────┘
       │
       ├── KTP700 HMI
       │
       └── Motor / Sensors
```

PLC 负责实时控制、联锁和故障保护；  
C# WPF 负责上位机操作与状态监控；  
HMI 用于现场操作。

## 🔧 技术栈

- **PLC:** Siemens S7-1200 CPU 1214C DC/DC/DC
- **HMI:** Siemens KTP700 Basic PN
- **PLC软件:** TIA Portal V17
- **桌面应用:** C# / WPF
- **架构:** MVVM
- **PLC通信:** S7.NetPlus 0.20.0
- **IDE:** Visual Studio
- **仿真:** PLCSIM + NetToPLCsim

## ⚙️ 主要功能

- 电机启动 / 停止
- PLC 实时状态读取
- Ready / Running / Fault 状态监控
- 急停状态监控
- 过载保护状态监控
- 电机运行反馈监控
- 运行反馈超时故障检测
- PLC 通信状态监控
- PLC 断线自动重连
- WPF 按钮状态联锁

## 🧠 PLC控制逻辑

系统主要状态：

| State | Description |
|---|---|
| `0` | 停止 |
| `1` | 准备就绪 |
| `2` | 运行中 |
| `3` | 故障停止 |

故障优先级：

```text
Fault
  ↓
Running
  ↓
Ready
  ↓
Stop
```

电机启动后，PLC 对 `Motor_Feedback` 进行监控。

如果电机进入运行状态后约 **3 秒**仍未检测到反馈，则产生 `Feedback_Fault`。

## 🔌 C# ↔ PLC 通信

由于 PLC 主数据块 `DB_Motor` 使用优化访问，本项目没有让 C# 直接依赖其内部偏移地址。

而是设计了独立的非优化通信 DB：

```text
DB_CSharp
```

主要接口：

| Address | Variable | Description |
|---|---|---|
| `DBX0.0` | Start_CMD | C# 启动命令 |
| `DBX0.1` | Stop_CMD | C# 停止命令 |
| `DBX0.2` | Ready | 就绪状态 |
| `DBX0.3` | Running | 运行状态 |
| `DBX0.4` | Fault | 故障状态 |
| `DBX0.5` | Feedback_Fault | 反馈故障 |
| `DBW2` | State | PLC 状态码 |
| `DBX4.0` | Emergency_OK | 急停状态 |
| `DBX4.1` | Overload_OK | 过载状态 |
| `DBX4.2` | Motor_Feedback | 电机反馈 |

C# 通过 `PlcService` 统一管理 PLC 通信。

## 🖥️ C# 架构

```text
WPF View
   │
   ▼
MainViewModel
   │
   ├── RelayCommand
   │
   ▼
PlcService
   │
   ▼
S7.NetPlus
   │
   ▼
S7-1200
```

主要模块：

```text
MotorControlSystem
├── MainWindow.xaml
├── MainViewModel.cs
├── RelayCommand.cs
├── PlcService.cs
└── MotorState.cs
```

## 🚀 Running

### 1. PLC / 仿真

使用 TIA Portal V17 打开 PLC 工程并下载程序。

仿真环境：

```text
TIA Portal
    ↓
PLCSIM
    ↓
NetToPLCsim
    ↓
C# WPF
```

### 2. C# 项目

使用 Visual Studio 打开：

```text
MotorControlSystem.sln
```

确认 `PlcService.cs` 中的 PLC IP 地址：

```csharp
new Plc(
    CpuType.S71200,
    "192.168.1.4",
    0,
    1);
```

实际现场 PLC 使用时，请修改为现场 PLC 的实际 IP。

### 3. 启动

运行 WPF 程序后：

```text
连接 PLC
   ↓
读取状态
   ↓
点击启动
   ↓
PLC 执行控制逻辑
   ↓
WPF 实时显示运行状态
```

## 📦 发布

项目支持通过 Visual Studio 发布 Windows 可执行程序。

推荐：

```text
Configuration : Release
Platform      : win-x64
Deployment    : Self-contained
```

发布后可生成：

```text
MotorControlSystem.exe
```

## 💡 项目亮点

- PLC + HMI + PC 上位机一体化架构
- Siemens S7-1200 工业控制
- C# WPF + MVVM 桌面应用
- S7.NetPlus PLC 通信
- 独立 PLC/C# 通信数据接口
- PLC 状态机与故障保护
- 电机反馈超时检测
- PLC 断线检测与自动重连
- Release / EXE 部署实践

## 📌 项目状态

**Version:** V1.0

当前版本已完成：

- PLC 电机控制
- HMI 控制
- C# WPF 上位机
- PLC 状态读取
- C# 启停控制
- 故障状态监控
- PLC 通信管理
- Release 发布

> 本项目主要用于展示 PLC、工业通信、C# WPF 和自动化控制系统的综合开发能力。