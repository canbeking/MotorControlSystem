using System.Windows;
using MotorControlSystem.ViewModels;
using MotorControlSystem.Services;


namespace MotorControlSystem
{
    public partial class MainWindow : Window
    {
        private readonly PlcService _plcService = new();
        public MainWindow()
        {
            InitializeComponent();


            DataContext = new MainViewModel();

            TestPlcConnection();

        }

        private void TestPlcConnection()
        {
            bool connected = _plcService.Connect();

            //MessageBox.Show(
            //    connected
            //        ? "PLC连接成功！"
            //        : "PLC连接失败！",
            //    "PLC通信测试");
        }

        private async void Click_PLC_Start(object sender, RoutedEventArgs e)
        {
            await _plcService.StartAsync();
        }
    }
}