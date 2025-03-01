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
    public class ServerServices
    {
        public int Port { get; set; } = 6001;
        public bool IsRunning { get; set; }
        public QuestionModel[] questionModels { get; set; }
        public UdpClient Server { get; set; }
        public ServerServices(int port)

        {
            Server = new(Port);
            Port = port;
            var thread = new Thread(new ThreadStart(Start))
            {
                IsBackground = true
            };
            thread.Start();
        }

        public EventHandler<ServerMessageDTO> MessageReived;
        void Start()
        {

            while (true)
            {

                IPEndPoint remote = new(IPAddress.Any, Port);
                byte[] buffer = Server.Receive(ref remote);
                ServerMessageDTO? message = JsonSerializer.Deserialize<ServerMessageDTO>(Encoding.UTF8.GetString(buffer));

                if (message != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageReived?.Invoke(this, message);
                    });
                }
            };
        }
    }
}
