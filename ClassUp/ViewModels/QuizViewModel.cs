using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClassUp.ViewModels
{
    public class QuizViewModel : INotifyPropertyChanged
    {
        private UdpClient _udpClient;
        private DispatcherTimer _timer;
        private CancellationTokenSource _cts;
        private int _timeLeft;
        private string _selectedOption;
        private string _questionText;
        private bool _isLocked;

        // Variables privadas para IP y Puerto (corrige el error)
        private string _serverIP;
        private int _serverPort;

        public event PropertyChangedEventHandler PropertyChanged;

        public QuizViewModel()
        {
            _serverIP = "127.0.0.1";  // Valor predeterminado, configurable
            _serverPort = 6000;       // Valor predeterminado, configurable

            _udpClient = new UdpClient(0); // Puerto dinámico para evitar conflictos
            _cts = new CancellationTokenSource();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += TimerElapsed;

            StartListening(); // Iniciar recepción en un hilo separado
        }

        public string ServerIP
        {
            get => _serverIP;
            set { _serverIP = value; OnPropertyChanged(); }
        }

        public int ServerPort
        {
            get => _serverPort;
            set { _serverPort = value; OnPropertyChanged(); }
        }

        public int TimeLeft
        {
            get => _timeLeft;
            set { _timeLeft = value; OnPropertyChanged(); }
        }

        public string SelectedOption
        {
            get => _selectedOption;
            set { _selectedOption = value; OnPropertyChanged(); }
        }

        public string QuestionText
        {
            get => _questionText;
            set { _questionText = value; OnPropertyChanged(); }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set { _isLocked = value; OnPropertyChanged(); }
        }

        public ICommand SelectOptionCommand => new RelayCommand<string>(option => SelectedOption = option);
        public ICommand AcceptCommand => new RelayCommand(Accept);

        private void StartTimer()
        {
            TimeLeft = 30;
            _timer.Start();
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            if (TimeLeft > 0)
                TimeLeft--;
            else
            {
                _timer.Stop();
                IsLocked = true; // Bloquear la interfaz cuando el tiempo termine
            }
        }

        private void Accept()
        {
            if (string.IsNullOrEmpty(SelectedOption)) return;

            IsLocked = true;
            _timer.Stop();

            string message = $"FinalAnswer:{SelectedOption}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            _udpClient.Send(data, data.Length, ServerIP, ServerPort);
        }

        private async void StartListening()
        {
            try
            {
                _cts = new CancellationTokenSource();
                while (!_cts.Token.IsCancellationRequested)
                {
                    UdpReceiveResult result = await _udpClient.ReceiveAsync();
                    string receivedMessage = Encoding.UTF8.GetString(result.Buffer);

                    if (!string.IsNullOrEmpty(receivedMessage))
                    {
                        QuestionText = receivedMessage;
                        IsLocked = false;
                        StartTimer(); // Reiniciar temporizador cuando llega una nueva pregunta
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Se cerró el cliente, no hacer nada.
            }
            catch (Exception ex)
            {
                QuestionText = $"Error: {ex.Message}";
            }
        }

        public void StopListening()
        {
            _cts.Cancel();
            _udpClient?.Close();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}




