using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace STools.CommonLibrary
{
    public enum ServiceResult
    {
        Available = 0,
        Aborted = 1,
        Failed = 2,
        CannotFindService = 3,
    };

    public abstract class BaseObject : IDisposable
    {
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
        }

        private List<string> _serviceNames = new List<string>();
        private List<Channel<double>> _doubleChannels = new List<Channel<double>>();
        private List<Channel<string>> _stringChannels = new List<Channel<string>>();
        private List<Alarm> _alarmList = new List<Alarm>();

        public List<string> GetChannelNames()
        {
            List<string> channelNames = new List<string>();
            foreach (Channel<double> channel in _doubleChannels)
            {
                channelNames.Add(channel.Name);
            }

            foreach (Channel<string> channel in _stringChannels)
            {
                channelNames.Add(channel.Name);
            }

            return channelNames;
        }

        public bool AddChannels(Channel<double> newChannel)
        {
            foreach (Channel<double> channel in _doubleChannels)
            {
                if (newChannel.Name.Equals(channel.Name))
                {
                    return false;
                }
            }

            _doubleChannels.Add(newChannel);

            return true;
        }

        public bool AddChannels(Channel<string> newChannel)
        {
            foreach (Channel<string> channel in _stringChannels)
            {
                if (newChannel.Name.Equals(channel.Name))
                {
                    return false;
                }
            }

            _stringChannels.Add(newChannel);

            return true;
        }

        public List<string> GetServiceNames()
        {
            return _serviceNames;
        }

        public bool GetDoubleData(string name, out double data)
        {
            foreach (Channel<double> channel in _doubleChannels)
            {
                if (channel.Name.Equals(name))
                {
                    data = channel.Data;
                    return true;
                }
            }

            data = 0;
            return false;
        }

        public bool GetStringData(string name, out string data)
        {
            foreach (Channel<string> channel in _stringChannels)
            {
                if (channel.Name.Equals(name))
                {
                    data = channel.Data;
                    return true;
                }
            }

            data = string.Empty;
            return false;
        }

        public bool AddServices(string name)
        {
            if (_serviceNames.Contains(name))
            {
                return false;
            }

            _serviceNames.Add(name);

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

        }

        ~BaseObject()
        {
            Dispose(false);
        }

        public void SetObjectName(string name)
        {
            _name = name;
        }

        public abstract bool CreateObject();
        public abstract bool DefineServices();
        public abstract bool DefineChannels();
        public abstract bool DefineAlarms();
        public abstract bool InitializeObject();
        public abstract bool ThreadStarter();
        public bool RunService(string name, params object[] param)
        {
            MethodInfo info = this.GetType().GetMethod(name);
            if (info != null)
            {
                if (info.ReturnType != typeof(bool))
                {
                    return false;
                }

                ParameterInfo[] paramInfos = info.GetParameters();
                if (param.Length != paramInfos.Length)
                {
                    return false;
                }

                for (int i = 0; i < param.Length; i++)
                {
                    if (param[i].GetType() != paramInfos[i].ParameterType)
                    {
                        return false;
                    }
                }

                return (bool)info.Invoke(this, param);
            }

            return false;
        }
    }

    public abstract class ApplicationObject : BaseObject
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }
    }

    public abstract class IOObject : BaseObject
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }
    }
}
