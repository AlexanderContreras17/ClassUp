using GalaSoft.MvvmLight.Command;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Server.ViewModels
{
    public enum Vistas { Ver, Agregar, Quiz, Resultados }

    public class QuizVM:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<QuizModel>? Lista { get; private set; }
        public ObservableCollection<string>? RespuestaCliente { get; set; }
        public Vistas Vista { get; set; } = Vistas.Ver;


        public QuizServer server = new();
        public string? Error { get; set; }


        public QuizModel QuizModel { get; set; }
        public void VerAgregar()
        {
            QuizModel = new();
            Vista = Vistas.Agregar;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
        public void Agregar()
        {
            if (string.IsNullOrEmpty(QuizModel?.Pregunta))
            {
                Error="Ingrese la pregunta";
            }
            if (Error == null)
            {
                Lista?.Add(QuizModel!);
                Vista=Vistas.Ver;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }



        }
        public ICommand AgregarCommand { get; set; }
        public ICommand VerAgregarCommand { get; set; }

        public void Cancelar()
        {
            Vista = Vistas.Ver;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
        public ICommand CancelarCommand { get; set; }

        public QuizVM()
        {
            Lista = new ObservableCollection<QuizModel>();
            server.RespuestaRecibida+=Server_RespuestaRecibida;
            VerAgregarCommand = new RelayCommand(VerAgregar);  
            AgregarCommand = new RelayCommand(Agregar);
            CancelarCommand=new RelayCommand(Cancelar);

        }

        private void Server_RespuestaRecibida(QuizModel obj)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                RespuestaCliente.Add(obj.Respuesta);
            });
        }




    }
}
