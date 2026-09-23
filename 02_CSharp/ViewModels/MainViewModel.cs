using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using MotorControlSystem.Models;
using MotorControlSystem.Services;
using System.Diagnostics;
using System.Windows.Threading;


namespace MotorControlSystem.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly PlcService _plcService = new();

        private readonly DispatcherTimer _plcTimer;

        public void ReadPlcStatus()
        {
            try
            {
                if (!_plcService.IsConnected)
                {
                    _plcService.Connect();
                }

                var status = _plcService.ReadStatus();

                Debug.WriteLine(
                    $"PLC状态：Ready={status.Ready}, " +
                    $"Running={status.Running}, " +
                    $"Fault={status.Fault}, " +
                    $"FeedbackFault={status.FeedbackFault}, " +
                    $"State={status.State}, " +
                    $"EmergencyOK={status.EmergencyOk}, " +
                    $"OverloadOK={status.OverloadOk}, " +
                    $"MotorFeedback={status.MotorFeedback}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PLC读取失败：{ex.Message}");
            }
        }

        private MotorState _motorState = new MotorState
        {
            State = 1,
            Ready = true,
            Running = false,
            Fault = false,
            EmergencyOk = true,
            OverloadOk = true,
            FeedbackFault = false
        };
        public MotorState MotorState
        {
            get { return _motorState; }
            set
            {
                _motorState = value;
                OnPropertyChanged();
            }
        }


        private bool _isRunning;

        private bool _isReady = true;

        private bool _isFault;

        private bool _emergencyOk = true;

        private bool _overloadOk = true;

        private bool _feedbackFault = false;

        private bool _feedbackOk = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _plcConnected;
        public bool PlcConnected
        {
            get => _plcConnected;
            private set
            {
                if (_plcConnected != value)
                {
                    _plcConnected = value;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PlcConnectionText));
                    OnPropertyChanged(nameof(PlcStateText));

                    OnPropertyChanged(nameof(ReadyStatusColor));
                    OnPropertyChanged(nameof(RunningStatusColor));
                    OnPropertyChanged(nameof(FaultStatusColor));
                    OnPropertyChanged(nameof(FeedbackFaultStatusColor));

                    OnPropertyChanged(nameof(EmergencyStatusColor));
                    OnPropertyChanged(nameof(OverloadStatusColor));
                    OnPropertyChanged(nameof(FeedbackStatusColor));

                    OnPropertyChanged(nameof(PlcConnectionColor));

                    StartPlcCommand?.RaiseCanExecuteChanged();
                    StopPlcCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public string PlcConnectionText => PlcConnected ? "PLC：已连接" : "PLC：未连接";

        public string PlcStateText => !PlcConnected ? "PLC中断" : StateText;

        public string FaultStatusColor => !PlcConnected ? "Gray" : IsFault ? "Red" : "LimeGreen";

        public string ReadyStatusColor => !PlcConnected ? "Gray" : IsReady ? "LimeGreen" : "Gray";

        public string RunningStatusColor => !PlcConnected ? "Gray" :  IsRunning ? "DodgerBlue" : "Gray";

        public string FeedbackFaultStatusColor => !PlcConnected ? "Gray" : FeedbackFault ? "Red" : "LimeGreen";

        public string EmergencyStatusColor => !PlcConnected ? "Gray" : EmergencyOk ? "LimeGreen" : "Red";

        public string OverloadStatusColor => !PlcConnected ? "Gray" : OverloadOk ? "LimeGreen" : "Red";

        public string FeedbackStatusColor => !PlcConnected ? "Gray" : FeedbackOk ? "LimeGreen" : "Gray";

        public string PlcConnectionColor => PlcConnected ? "LimeGreen" : "Red";

        // PLC里的状态码 0停止 1就绪 2运行 3故障 -1等待PLC
        private int _state = -1;
        public int State
        {
            get => _state;
            private set
            {
                if (_state != value)
                {
                    _state = value;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StateText));
                    OnPropertyChanged(nameof(PlcStateText));
                }
            }
        }


        public bool FeedbackOk
        {
            get
            {
                return _feedbackOk;
            }
            set
            {
                _feedbackOk = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FeedbackStatusColor));
            }
        }

        // ==========================
        // 运行状态
        // ==========================

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }

            set
            {
                _isRunning = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(RunningStatusColor));
            }
        }

        // ==========================
        // 就绪状态
        // ==========================

        public bool IsReady
        {
            get
            {
                return _isReady;
            }

            set
            {
                _isReady = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ReadyStatusColor));
            }
        }



        // ==========================
        // 故障状态
        // ==========================

        public bool IsFault
        {
            get
            {
                return _isFault;
            }

            set
            {
                _isFault = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FaultStatusColor));
            }
        }



        // ==========================
        // 急停
        // ==========================

        public bool EmergencyOk
        {
            get
            {
                return _emergencyOk;
            }

            set
            {
                _emergencyOk = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EmergencyStatusColor));
            }
        }



        // ==========================
        // 过载
        // ==========================

        public bool OverloadOk
        {
            get
            {
                return _overloadOk;
            }

            set
            {
                _overloadOk = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OverloadStatusColor));
            }
        }


        // ==========================
        // 反馈故障
        // ==========================

        public bool FeedbackFault
        {
            get
            {
                return _feedbackFault;
            }

            set
            {
                _feedbackFault = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FeedbackFaultStatusColor));
            }
        }


        // ==========================
        // 状态文字
        // ==========================

        public string StateText
        {
            get
            {
                return State switch
                {
                    0 => "停止",
                    1 => "准备就绪",
                    2 => "运行中",
                    3 => "故障",
                    _ => "等待PLC"
                };
            }
        }

        // ==========================
        // 模拟故障命令
        // ==========================
        public ICommand FaultCommand { get; }


        // ==========================
        // 故障复位命令
        // ==========================
        public ICommand ResetFaultCommand { get; }


        // ==========================
        // 模拟运行反馈到位
        // ==========================
        public ICommand FeedbackOkCommand { get; }

        // ==========================
        // 模拟急停
        // ==========================
        public ICommand EmergencyCommand { get; }


        // ==========================
        // 模拟过载
        // ==========================
        public ICommand OverloadCommand { get; }

        public RelayCommand ReadPlcStatusCommand { get; }
        public RelayCommand StartPlcCommand { get; }
        public RelayCommand StopPlcCommand { get; }

        private bool CanStartPlc()
        {
            return PlcConnected
                   && IsReady
                   && !IsRunning
                   && !IsFault
                   && EmergencyOk
                   && OverloadOk
                   && !FeedbackFault;
        }

        private bool CanStopPlc()
        {
            return PlcConnected && IsRunning;
        }

        public MainViewModel()
        {
            // 正式控制：由 PLC 执行
            StartPlcCommand = new RelayCommand(async () => await StartPlcAsync(), CanStartPlc);
            StopPlcCommand = new RelayCommand(async () => await StopPlcAsync(), CanStopPlc);

            // 手动读取 PLC，暂时保留
            ReadPlcStatusCommand = new RelayCommand(ReadPlcStatus);

            // PLC 状态自动轮询
            _plcTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };

            _plcTimer.Tick += PlcTimer_Tick;
            _plcTimer.Start();
        }


        private async Task StartPlcAsync()
        {
            try
            {
                if (!_plcService.IsConnected)
                {
                    if (!_plcService.Connect())
                    {
                        Debug.WriteLine("PLC连接失败");
                        return;
                    }
                }

                // C# 只发送启动脉冲
                await _plcService.StartAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PLC启动失败：{ex.Message}");
            }
        }

        private async Task StopPlcAsync()
        {
            try
            {
                if (!_plcService.IsConnected)
                {
                    if (!_plcService.Connect())
                    {
                        Debug.WriteLine("PLC连接失败");
                        return;
                    }
                }

                // C# 只发送停止脉冲
                await _plcService.StopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PLC停止失败：{ex.Message}");
            }
        }

        private void PlcTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!_plcService.IsConnected)
                {
                    _plcService.Connect();
                }

                PlcConnected = _plcService.IsConnected;

                if (!PlcConnected)
                {
                    IsReady = false;
                    IsRunning = false;
                    IsFault = false;
                    FeedbackFault = false;
                    FeedbackOk = false;

                    State = -1;

                    StartPlcCommand?.RaiseCanExecuteChanged();
                    StopPlcCommand?.RaiseCanExecuteChanged();

                    return;
                }

                var status = _plcService.ReadStatus();

                IsReady = status.Ready;
                IsRunning = status.Running;
                IsFault = status.Fault;
                FeedbackFault = status.FeedbackFault;

                EmergencyOk = status.EmergencyOk;
                OverloadOk = status.OverloadOk;
                FeedbackOk = status.MotorFeedback;

                State = status.State;

                MotorState = new MotorState
                {
                    State = status.State,
                    Ready = status.Ready,
                    Running = status.Running,
                    Fault = status.Fault,
                    EmergencyOk = status.EmergencyOk,
                    OverloadOk = status.OverloadOk,
                    FeedbackFault = status.FeedbackFault
                };

                StartPlcCommand?.RaiseCanExecuteChanged();
                StopPlcCommand?.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                PlcConnected = false;

                IsReady = false;
                IsRunning = false;
                IsFault = false;
                FeedbackFault = false;
                FeedbackOk = false;

                State = -1;

                StartPlcCommand?.RaiseCanExecuteChanged();
                StopPlcCommand?.RaiseCanExecuteChanged();

                Debug.WriteLine($"PLC状态读取失败：{ex.Message}");
            }
        }

        protected void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));

        }


    }
}