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
            if (_settings.ToolTypes == ToolTypes.Server)
            {

            }
            else
            {

            }

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

            return true;
        }

        public bool Load()
        {
            /// Create
            /// Define Services.
            /// Define Channels
            /// Define Alarms
            /// Initialize Objects
            /// Thread Run

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
