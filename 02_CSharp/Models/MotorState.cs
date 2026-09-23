using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MotorControlSystem.Models
{
    public class MotorState : INotifyPropertyChanged
    {
        private int _state;
        private bool _running;
        private bool _ready;
        private bool _fault;
        private bool _emergencyOk;
        private bool _overloadOk;
        private bool _feedbackFault;

        // PLC状态码
        // 0 = 停止
        // 1 = 就绪
        // 2 = 运行
        // 3 = 故障停止

        public int State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public bool Running
        {
            get { return _running; }
            set
            {
                _running = value;
                OnPropertyChanged();
            }
        }

        public bool Ready
        {
            get { return _ready; }
            set
            {
                _ready = value;
                OnPropertyChanged();
            }
        }

        public bool Fault
        {
            get { return _fault; }
            set
            {
                _fault = value;
                OnPropertyChanged();
            }
        }

        public bool EmergencyOk
        {
            get { return _emergencyOk; }
            set
            {
                _emergencyOk = value;
                OnPropertyChanged();
            }
        }

        public bool OverloadOk
        {
            get { return _overloadOk; }
            set
            {
                _overloadOk = value;
                OnPropertyChanged();
            }
        }

        public bool FeedbackFault
        {
            get { return _feedbackFault; }
            set
            {
                _feedbackFault = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}