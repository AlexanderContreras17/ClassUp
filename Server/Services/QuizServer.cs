using Server.Models;
using Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Server.Services
{
    public class QuizServer
    {
        UdpClient server;



        public QuizServer()
        {
            server = new(6001);
            server.EnableBroadcast = true;
            Thread hilo = new(RecibirRespuesta);
            hilo.IsBackground = true;
            hilo.Start();
        }

        public event Action<QuizModel>? RespuestaRecibida;

        void RecibirRespuesta()
        {
            while (true)
            {
                IPEndPoint cliente = new(IPAddress.Any, 0);
                byte[] buffer = server.Receive(ref cliente);
                var json = Encoding.UTF8.GetString(buffer);
                var resp = JsonSerializer.Deserialize<QuizModel>(json);
                if (resp!=null)
                {
                    RespuestaRecibida?.Invoke(resp);
                }
            }
        }





        //public int Port { get; set; } = 6001;
        //public bool IsRunning { get; set; }
        //public QuestionModel[] questionModels { get; set; }
        //public UdpClient Server { get; set; }
        //public QuizServer(int port)

        //{
        //    Server = new(Port);
        //    Port = port;
        //    var thread = new Thread(new ThreadStart(Start))
        //    {
        //        IsBackground = true
        //    };
        //    thread.Start();
        //}

        //public EventHandler<ServerMessageDTO> MessageReived;
        //void Start()
        //{

        //    while (true)
        //    {

        //        IPEndPoint remote = new(IPAddress.Any, Port);
        //        byte[] buffer = Server.Receive(ref remote);
        //        ServerMessageDTO? message = JsonSerializer.Deserialize<ServerMessageDTO>(Encoding.UTF8.GetString(buffer));

        //        if (message != null)
        //        {
        //            Application.Current.Dispatcher.Invoke(() =>
        //            {
        //                MessageReived?.Invoke(this, message);
        //            });
        //        }
        //    };
        //}
    }
}
