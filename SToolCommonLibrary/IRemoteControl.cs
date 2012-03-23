using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SToolCommonLibrary
{
    public interface IRemoteServer
    {
        bool Connect(IRemoteClient client);
        bool Disconnect(string name);
        string GetConnectState(string name);

        List<string> GetClientNameList();
        bool GetClient(string name, out IRemoteClient client);
    }

    public interface IRemoteClient
    {
        string Name { get; }
        ClientConnectState ConnectionState { get; }
        List<string> GetObjectList();
        List<string> GetChannelList();
    }

    public enum ClientConnectState
    {
        Connected,
        Disconnected,
    }

    public class RemoteServices : MarshalByRefObject, IRemoteServer
    {
        private string _name = string.Empty;
        private Dictionary<string, IRemoteClient> _clients = new Dictionary<string, IRemoteClient>();

        private ClientConnectState _connectState = ClientConnectState.Disconnected;

        private static RemoteServices _instance = new RemoteServices();
        private RemoteServices() { }
        public static RemoteServices Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RemoteServices();
                }

                return _instance;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public bool Connect(IRemoteClient client)
        {
            if (!_clients.ContainsKey(client.Name))
            {
                _clients.Add(client.Name, client);

                return true;
            }

            return false;
        }


        public bool Disconnect(string name)
        {
            if (_clients.ContainsKey(name))
            {
                _clients.Remove(name);
                return true;
            }

            return false;
        }

        public string GetConnectState(string name)
        {
            if (_clients.ContainsKey(name))
            {
                return _clients[name].ConnectionState.ToString();
            }

            return "Disconnect";
        }

        public List<string> GetClientNameList()
        {
            List<string> clientNames = new List<string>();
            foreach (string client in _clients.Keys)
            {
                clientNames.Add(client);
            }

            return clientNames;
        }

        public bool GetClient(string name, out IRemoteClient client)
        {
            if (_clients.ContainsKey(name))
            {
                try
                {
                    client = _clients[name];
                    return true;
                }
                catch (System.Exception ex)
                {
                    _clients.Remove(name);
                }
            }

            client = null;
            return false;
        }
    }

    public class RemoteClient : MarshalByRefObject, IRemoteClient
    {
        private string _name = "None";
        public string Name 
        { 
            get 
            { 
                return _name; 
            } 
        }

        private static RemoteClient _instance;
        private RemoteClient() { }
        public static RemoteClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RemoteClient();
                }

                return _instance;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void SetNames(string name)
        {
            _name = name;
        }

        public ClientConnectState ConnectionState
        {
            get
            {
                return ClientConnectState.Connected;
            }
        }

        public List<string> GetObjectList()
        {
            return new List<string>() { "C.AAA", "C.BBB" };
        }

        public List<string> GetChannelList()
        {
            return new List<string>() { "CH.AAA", "CH.BBB" };
        }
    }
}
