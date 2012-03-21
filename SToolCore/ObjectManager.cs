using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace STools.Core
{
    public class ObjectManager
    {
    }

    [XmlRoot("Applications")]
    public class ApplicationsList
    {
        private List<ApplicationCreationInfo> _appList = new List<ApplicationCreationInfo>();

        [XmlElement("AppInfo")]
        public List<ApplicationCreationInfo> ApplicationCreateInfo
        {
            get { return _appList; }
            set { _appList = value; }
        }
    }

    [XmlRoot("AppInfo")]
    public class ApplicationCreationInfo
    {
        private string _objectName = string.Empty;
        private string _className = string.Empty;
        private string _fileName = string.Empty;

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _objectName; }
            set { _objectName = value; }
        }

        [XmlAttribute("ClassName")]
        public string ClassName
        {
            get { return _className; }
            set { _className = value; }
        }

        [XmlAttribute("FileName")]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
    }


    public interface IOjbectControlInterface
    {

    }
}
