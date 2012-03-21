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
            return false;
        }

        public override bool DefineServices()
        {
            return false;
        }
        public override bool DefineChannels()
        {
            return false;
        }
        public override bool DefineAlarms()
        {
            return false;
        }
        public override bool ThreadStarter()
        {
            return false;
        }

    }
}
