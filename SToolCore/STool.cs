using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using STools.CommonLibrary;
using System.IO;
using System.Reflection;
using log4net;
using System.Xml.Serialization;
using SToolCommonLibrary;
using STools.Core;

using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Collections;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting.Channels.Tcp;

namespace STools.Core
{
    public enum ToolTypes
    {
        Server,
        Client,
    }

    public class SToolSettings
    {
        private ToolTypes _toolTypes =  ToolTypes.Server;
        public ToolTypes ToolTypes
        {
            get { return _toolTypes;  }
            set { _toolTypes = value; }
        }

        private string _startUpPath = string.Empty;
        public string StartupPath
        {
            get { return _startUpPath; }
            set { _startUpPath = value; }
        }

        private ILog _logger = null;
        public ILog Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }
    }

    public enum SystemLogLevel
    {
        Debug,
        Error,
        Fatal,
        Info,
        Warn,
    }

    public interface ISTool
    {
        void SystemLog(SystemLogLevel type, string log);
    }
    
    public class STool : ISTool, IDisposable
    {
        TcpChannel    _serverChannel = null;
        TcpChannel    _clientChannel = null;
        IRemoteServer _serverInterface = null;

        private SToolSettings _settings = new SToolSettings();
        private Dictionary<string, BaseObject> _applicationDictionary = new Dictionary<string, BaseObject>();

        public Dictionary<string, BaseObject> Applications
        {
            get { return _applicationDictionary; }
        }
        
        public STool(SToolSettings setting)
        {
            _settings = setting;
            
            string log = 
                 "\n===================================================\n";
            log += "| S-Tools Version: V0.1                           |\n";
            log += "| S-Tools Started   ...                           |\n";
            log += "===================================================\n";


            SystemLog(SystemLogLevel.Info, log);

        }

        public bool Initialize()
        {

            /// Core Load
            ApplicationsList appList = (ApplicationsList)XmlControl.DeSerialize(_settings.StartupPath + @"/Config/App.Info", typeof(ApplicationsList));
            foreach (ApplicationCreationInfo app in appList.ApplicationCreateInfo)
            {
                /// Load Application Files
                try
                {
                    Assembly assem = Assembly.LoadFile(_settings.StartupPath + @"\Applications\" + app.FileName);

                    if (!_applicationDictionary.ContainsKey(app.Name))
                    {
                        BaseObject appObject = (BaseObject)assem.CreateInstance(app.ClassName);
                        appObject.SetObjectName(app.Name);

                        _applicationDictionary.Add(app.Name, appObject);
                        SystemLog(SystemLogLevel.Info, string.Format("Application Loaded - Name: {0}, ClassName: {1}, FileName: {2}", app.Name, app.ClassName, app.FileName));
                    }
                    else
                    {
                        SystemLog(SystemLogLevel.Error, string.Format("Application Name Duplication: {0}", app.Name));
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
                    SystemLog(SystemLogLevel.Error, string.Format("Application Load Failed. - Name: {0}, ClassName: {1}, FileName: {2}", app.Name, app.ClassName, app.FileName));
                    SystemLog(SystemLogLevel.Error, ex.ToString());
                    return false;
                }
            }



            if (_settings.ToolTypes == ToolTypes.Server)
            {
                /// Remote Server Initialize.
                _serverChannel = CreateTCPChannel(TCPChannelType.Server, 1234);
                ChannelServices.RegisterChannel(_serverChannel, true);
                /*
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(RemoteServices),
                    "IRemoteControl.soap",
                    WellKnownObjectMode.Singleton);
                */
                RemotingServices.Marshal(RemoteServices.Instance, "IRemoteControl.soap");
            }
            else
            {
                _clientChannel = CreateTCPChannel(TCPChannelType.Client, 0);
                ChannelServices.RegisterChannel(_clientChannel, true);
                RemoteClient.Instance.SetNames("CLIENT1");

                try
                {
                    _serverInterface = (IRemoteServer)Activator.GetObject(typeof(IRemoteServer),
                          "tcp://localhost:1234/IRemoteControl.soap");

                    if (_serverInterface.Connect(RemoteClient.Instance))
                    {
                        Console.WriteLine("Client Connected");
                    }
                }
                catch (System.Exception ex)
                {

                }
            }
            
            return true;
        }

        public bool Load()
        {

            /// Create
            if (!CreateApplicationObject())
            {
                SystemLog(SystemLogLevel.Error, "Create Application Object Failed. Exit Load.");
                return false;
            }

            /// Define Services.
            if (!DefineApplicationService())
            {
                SystemLog(SystemLogLevel.Error, "Define Application Service Failed. Exit Load.");
                return false;
            }
            /// Define Channels
            if (!DefineApplicationChannels())
            {
                SystemLog(SystemLogLevel.Error, "Define Application Channel Failed. Exit Load.");
                return false;
            }

            /// Define Alarms
            if (!DefineApplicationAlarms())
            {
                SystemLog(SystemLogLevel.Error, "Define Application Alarms Failed. Exit Load.");
                return false;
            }
            
            /// Initialize Objects
            if (!InitializeApplicationObjects())
            {
                SystemLog(SystemLogLevel.Error, "Initialize Application Objects Failed. Exit Load.");
                return false;
            }


            /// Thread Run

            return true;
        }

        private enum TCPChannelType 
        {
            Server,
            Client
        };

        private TcpChannel CreateTCPChannel(TCPChannelType type, int port)
        {
            BinaryClientFormatterSinkProvider clientProvider = null;
            BinaryServerFormatterSinkProvider serverProvider = null;
            IDictionary props = null;

            if (type == TCPChannelType.Server)
            {
                serverProvider = new BinaryServerFormatterSinkProvider();
                serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

                props = new Hashtable();
                props["port"] = port;
                props["typeFilterLevel"] = TypeFilterLevel.Full;
            }
            else
            {
                clientProvider = new BinaryClientFormatterSinkProvider();
                serverProvider = new BinaryServerFormatterSinkProvider();
                serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                props = new Hashtable();
                props["port"] = port;
                string s = System.Guid.NewGuid().ToString();
                props["name"] = s;
                props["typeFilterLevel"] = TypeFilterLevel.Full;
            }

            
            
            return new TcpChannel(props, clientProvider, serverProvider);
        }

        public void PintClientList()
        {
            string log = string.Empty;

            if(_settings.ToolTypes == ToolTypes.Client)
            {
                SystemLog(SystemLogLevel.Info, string.Format("SERVER CLIENT: {0}", _serverInterface.GetClientNameList().Count));
                List<string> _list = _serverInterface.GetClientNameList();
                foreach (string name in _list)
                {
                    log += name + "\n";
                    IRemoteClient client = null;
                    if (_serverInterface.GetClient(name, out client))
                    {
                        List<string> objects = client.GetObjectList();
                        foreach (string objectName in objects)
                        {
                            log += objectName + "\n";
                        }
                    }
                }

                SystemLog(SystemLogLevel.Info, log);
            }
            else
            {
                SystemLog(SystemLogLevel.Info, string.Format("SERVER CLIENT: {0}", RemoteServices.Instance.GetClientNameList().Count));

                List<string> _list = RemoteServices.Instance.GetClientNameList();
                foreach (string name in _list)
                {
                    log += name + "\n";
                    IRemoteClient client = null;
                    if (RemoteServices.Instance.GetClient(name, out client))
                    {
                        List<string> objects = client.GetObjectList();
                        foreach (string objectName in objects)
                        {
                            log += objectName + "\n";
                        }
                    }
                }
                SystemLog(SystemLogLevel.Info, log);
            }
        }

        public bool CreateApplicationObject()
        {
            foreach (BaseObject app in _applicationDictionary.Values)
            {
                if (!app.CreateObject())
                {
                    SystemLog(SystemLogLevel.Error, string.Format("Application: {0}, Create Failed.", app.Name));
                    return false;
                }
                else
                {
                    SystemLog(SystemLogLevel.Info, string.Format("Create Application Object Name: {0}", app.Name));
                }
            }

            SystemLog(SystemLogLevel.Info, 
                      string.Format("Create Application Object Completed.\nTotal Object Count: {0}", 
                      _applicationDictionary.Count));

            return true;
        }

        public bool DefineApplicationService()
        {
            foreach (BaseObject app in _applicationDictionary.Values)
            {
                if (!app.DefineServices())
                {
                    SystemLog(SystemLogLevel.Error, string.Format("Application: {0}, Define Service Failed.", app.Name));
                    return false;
                }
                else
                {
                    SystemLog(SystemLogLevel.Info, string.Format("Define Service. Object Name: {0}", app.Name));
                }
            }

            SystemLog(SystemLogLevel.Info,
                      string.Format("Define Application Service Completed.\n"));

            return true;
        }

        public bool DefineApplicationChannels()
        {
            foreach (BaseObject app in _applicationDictionary.Values)
            {
                if (!app.DefineChannels())
                {
                    SystemLog(SystemLogLevel.Error, string.Format("Application: {0}, Define Channels Failed.", app.Name));
                    return false;
                }
                else
                {
                    SystemLog(SystemLogLevel.Info, string.Format("Define Channels. Object Name: {0}", app.Name));
                }
            }

            SystemLog(SystemLogLevel.Info,
                      string.Format("Define Application Channels Completed.\n"));

            return true;
        }

        public bool DefineApplicationAlarms()
        {
            foreach (BaseObject app in _applicationDictionary.Values)
            {
                if (!app.DefineAlarms())
                {
                    SystemLog(SystemLogLevel.Error, string.Format("Application: {0}, Define Channels Failed.", app.Name));
                    return false;
                }
                else
                {
                    SystemLog(SystemLogLevel.Info, string.Format("Define Channels. Object Name: {0}", app.Name));
                }
            }

            SystemLog(SystemLogLevel.Info,
                      string.Format("Define Application Channels Completed.\n"));

            return true;
        }

        public bool InitializeApplicationObjects()
        {
            foreach (BaseObject app in _applicationDictionary.Values)
            {
                if (!app.InitializeObject())
                {
                    SystemLog(SystemLogLevel.Error, string.Format("Application: {0}, Define Channels Failed.", app.Name));
                    return false;
                }
                else
                {
                    SystemLog(SystemLogLevel.Info, string.Format("Define Channels. Object Name: {0}", app.Name));
                }
            }

            SystemLog(SystemLogLevel.Info,
                      string.Format("Define Application Channels Completed.\n"));

            return true;
        }

        public void Dispose()
        {
            string log =
                 "\n===================================================\n";
            log += "| S-Tools Version: V0.1                           |\n";
            log += "| S-Tools Terminated...                           |\n";
            log += "===================================================\n";

            SystemLog(SystemLogLevel.Info, log);

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (BaseObject obj in _applicationDictionary.Values)
                {
                    obj.Dispose();
                }

                if (_settings.ToolTypes == ToolTypes.Server)
                {
                    ChannelServices.UnregisterChannel(_serverChannel);
                }
                else
                {
                    _serverInterface.Disconnect("CLIENT1");
                    ChannelServices.UnregisterChannel(_clientChannel);
                }
            }
        }

        public void SystemLog(SystemLogLevel type, string log)
        {
            switch (type)
            {
                case SystemLogLevel.Debug:
                    _settings.Logger.Debug(log);
                    break;
                case SystemLogLevel.Error:
                    _settings.Logger.Error(log);
                    break;
                case SystemLogLevel.Fatal:
                    _settings.Logger.Fatal(log);
                    break;
                case SystemLogLevel.Info:
                    _settings.Logger.Info(log);
                    break;
                case SystemLogLevel.Warn:
                    _settings.Logger.Warn(log);
                    break;
            }
        }
    }
}
