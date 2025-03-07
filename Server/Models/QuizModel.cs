using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class QuizModel
    {

        public string? Pregunta { get; set; }
        public byte Tiempo { get; set; }
        public string Respuesta { get; set; }
        //public byte Puntuacion { get; set; }
    }
}
