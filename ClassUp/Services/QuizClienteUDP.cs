using ClassUp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassUp.Services
{
    public class QuizClienteUDP
    {
        UdpClient client;

        public QuizClienteUDP()
        {
            client = new();
            client.EnableBroadcast = true;
        }


        public void Enviar(QuizClienteModel q)
        {
            IPEndPoint server = new(IPAddress.Broadcast, 6000);
            var json = JsonSerializer.Serialize(q);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            client.Send(buffer, buffer.Length, server);
        }
    }
}
