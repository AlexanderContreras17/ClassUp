using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ViewModels
{
    public class ServerViewModel : INotifyPropertyChanged
    {
        public string ip {  get; set; }
        public int port { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
