using S7.Net;

namespace MotorControlSystem.Services
{
    public class PlcService : IDisposable
    {
        private readonly Plc _plc;
        private readonly object _plcLock = new();

        public bool IsConnected => _plc.IsConnected;

        public PlcService()
        {
            _plc = new Plc(
                CpuType.S71200,
                "192.168.1.4",
                0,
                1);
        }

        public bool Connect()
        {
            try
            {
                lock (_plcLock)
                {
                    if (!_plc.IsConnected)
                        _plc.Open();

                    return _plc.IsConnected;
                }
            }
            catch
            {
                return false;
            }
        }

        public void Disconnect()
        {
            lock (_plcLock)
            {
                if (_plc.IsConnected)
                    _plc.Close();
            }
        }

        // =========================
        // C# → PLC：启动命令
        // =========================
        public async Task StartAsync()
        {
            lock (_plcLock)
            {
                _plc.Write("DB3.DBX0.0", true);
            }

            await Task.Delay(200);

            lock (_plcLock)
            {
                _plc.Write("DB3.DBX0.0", false);
            }
        }

        // =========================
        // C# → PLC：停止命令
        // =========================
        public async Task StopAsync()
        {
            lock (_plcLock)
            {
                _plc.Write("DB3.DBX0.1", true);
            }

            await Task.Delay(200);

            lock (_plcLock)
            {
                _plc.Write("DB3.DBX0.1", false);
            }
        }

        // =========================
        // PLC → C#：读取状态
        // =========================
        public (
            bool Ready,
            bool Running,
            bool Fault,
            bool FeedbackFault,
            short State,
            bool EmergencyOk,
            bool OverloadOk,
            bool MotorFeedback
        ) ReadStatus()
        {
            lock (_plcLock)
            {
                bool ready = (bool)_plc.Read("DB3.DBX0.2");
                bool running = (bool)_plc.Read("DB3.DBX0.3");
                bool fault = (bool)_plc.Read("DB3.DBX0.4");
                bool feedbackFault = (bool)_plc.Read("DB3.DBX0.5");
                short state = unchecked((short)(ushort)_plc.Read("DB3.DBW2"));
                bool emergencyOk = (bool)_plc.Read("DB3.DBX4.0");
                bool overloadOk = (bool)_plc.Read("DB3.DBX4.1");
                bool motorFeedback = (bool)_plc.Read("DB3.DBX4.2");

                return (
                    ready,
                    running,
                    fault,
                    feedbackFault,
                    state,
                    emergencyOk,
                    overloadOk,
                    motorFeedback);
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}