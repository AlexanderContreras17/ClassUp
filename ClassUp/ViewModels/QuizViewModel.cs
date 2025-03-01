using ClassUp.Helpers;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClassUp.ViewModels
{
    public class QuizViewModel : INotifyPropertyChanged
    {
        private UdpClient _udpClient;
        private DispatcherTimer _timer;
        private int _timeLeft;
        private string _selectedOption;
        private string _questionText;
        private bool _isLocked;
        private const int PORT = 6000;
        private const string SERVER_IP = "127.0.0.1";

        public event PropertyChangedEventHandler PropertyChanged;

        public QuizViewModel()
        {
            _udpClient = new UdpClient(PORT); // Escuchar en el puerto 6000
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += TimerElapsed;
            StartTimer();
            ReceiveQuestionFromServer(); // Iniciar la recepción de preguntas
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

        public ICommand SelectOptionCommand => new GalaSoft.MvvmLight.Command.RelayCommand<string>(SendAnswer);

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
                _timer.Stop();
        }

        private void SendAnswer(string option)
        {
            SelectedOption = option;
        }

        private void Accept()
        {
            IsLocked = true; // Bloquear la interfaz
            _timer.Stop(); // Detener el temporizador
            // Enviar la respuesta seleccionada al servidor
            string message = $"FinalAnswer:{SelectedOption}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            _udpClient.Send(data, data.Length, SERVER_IP, PORT);
        }

        private async void ReceiveQuestionFromServer()
        {
            try
            {
                while (true) // Escuchar continuamente
                {
                    // Recibir datos del servidor
                    UdpReceiveResult result = await _udpClient.ReceiveAsync();
                    string receivedMessage = Encoding.UTF8.GetString(result.Buffer);

                    // Actualizar la pregunta en la vista
                    QuestionText = receivedMessage;
                }
            }
            catch (Exception ex)
            {
                QuestionText = "Error al recibir la pregunta: " + ex.Message;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}



