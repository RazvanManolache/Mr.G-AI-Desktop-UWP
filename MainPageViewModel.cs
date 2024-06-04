using MrG.AI.Client;
using MrG.AI.Client.Data;
using MrG.AI.Client.Data.Action;
using MrG.AI.Client.Enum;
using MrG.AI.Client.Helpers;
using MrG.Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrG.Desktop
{
    public class MainPageViewModel : ObservableObject
    {
        public ISecureStorageHelper secureStorageHelper = new SecureStorageHelper();

        private bool _localServer = true;
        public bool LocalServer
        {
            get { return _localServer; }
            set { this.SetProperty(ref _localServer, value);
                this.OnPropertyChanged(nameof(Server));
            }
        }

        private bool _onlineServer = false;
        public bool OnlineServer
        {
            get { return _onlineServer; }
            set { this.SetProperty(ref _onlineServer, value); }
        }
        private Server _server;

        public Server Server
        {
            get
            {
                if (_server == null)
                {
                    _server = secureStorageHelper.GetServer().Result;
                }
                return _server;
            }
            set { this.SetProperty(ref _server, value);
                secureStorageHelper.WriteServer(value);
                this.OnPropertyChanged(nameof(LoggedIn));
                this.OnPropertyChanged(nameof(LoggedOut));
            }
        }
        ActionApi _selectedAction = null;
        public ActionApi SelectedAction
        {
            get { return _selectedAction; }
            set { this.SetProperty(ref _selectedAction, value); }
        }
        public ObservableCollection<ActionApi> Actions  { get; set; } = new ObservableCollection<ActionApi>();
        
       
        public bool LoggedOut
        {
            get
            {
                return !LoggedIn;
            }
        }
        public bool LoggedIn
        {
            get
            {
                return Server != null;
            }
        }

        private string _ip = "127.0.0.1";

        public string IP
        {
            get { return _ip; }
            set { 
                this.SetProperty(ref _ip,value);
                this.SetProperty(ref _ipChecked, null);
                this.OnPropertyChanged(nameof(IPValid));
                this.OnPropertyChanged(nameof(IPChecked));
                this.OnPropertyChanged(nameof(IPError));
            }
        }

        private bool? _ipChecked = null;
        public bool IPChecked
        {
            get { return _ipChecked.HasValue && _ipChecked.Value; }
            set {
                this.SetProperty(ref _ipChecked, value);
                this.OnPropertyChanged(nameof(IPError));
            }
        }


        public string IPError
        {
            get
            {
                if (!IPValid)
                {
                    return "Invalid IP Address";
                }
                if (!_ipChecked.HasValue)
                {
                    return "Press test to check server";
                }
                if (!IPChecked)
                {
                    return "IP Address does not have a Mr.G server";
                }
                
                return "Connection successful";
            }
        }

        public bool IPValid
        {
            get
            {
                return Server.IsValidIpAddress(IP); 
            }
        }

       
       
    }
}
