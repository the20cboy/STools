using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STools.CommonLibrary;

namespace SampleObject
{
    public class SampleObject : ApplicationObject
    {
        public override bool CreateObject()
        {
            return true;
        }

        public override bool DefineServices()
        {
            AddServices("ServiceFuncation");
            return true;
        }
        public override bool DefineChannels()
        {
            return true;
        }
        public override bool DefineAlarms()
        {
            return true;
        }
        public override bool InitializeObject()
        {
            return true;
        }
        public override bool ThreadStarter()
        {
            return true;
        }


        public bool ServiceFuncation()
        {
            return true;
        }

    }
}
