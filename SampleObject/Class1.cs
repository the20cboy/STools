using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STools.CommonLibrary;
using System.Windows;

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
            AddServices("ServiceFunction");
            return true;
        }
        public override bool DefineChannels()
        {
            AddChannels(new Channel<double>("doubleChannel", 0));
            AddChannels(new Channel<string>("stringChannel", "sss"));
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


        public bool ServiceFunction()
        {
            System.Windows.Forms.MessageBox.Show("Service Function Run");
            return true;
        }

    }
}
