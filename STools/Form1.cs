using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using log4net;

using STools.Core;
using STools.CommonLibrary;

namespace STools
{
    public partial class Form1 : Form
    {
        protected readonly static ILog _logger =LogManager.GetLogger(typeof(Program));
        private STool _sTools = null;

        public Form1()
        {
            SToolSettings settings = new SToolSettings();
            settings.StartupPath = Application.StartupPath;
            settings.Logger = _logger;

            _sTools = new STool(settings);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /// STool Initialize
            loadProgress.Style = ProgressBarStyle.Continuous;
            lbMessage.Text = "S-Tool Started.";

            lbMessage.Text = "Initialize S-Tools";
            if (!_sTools.Initialize())
            {
                /// Initialize Failed.
                MessageBox.Show("S-Tools Initialize Failed\nPlease Check System Log Files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AppDomain domain = AppDomain.CurrentDomain;

            BaseObject aaa = _sTools.Applications["AAA"];
            BaseObject bbb = _sTools.Applications["BBB"];

            BaseObject ccc = bbb;
       

            Type t = Type.GetType("STools.CommonLibrary.BaseObject");
        }
    }
}
