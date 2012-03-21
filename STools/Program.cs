using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace STools
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


        public static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains(","))
            {
                string fileName = args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
                string assemblyPath = Application.StartupPath + @"/Core/" + fileName;

                if (System.IO.File.Exists(assemblyPath))
                {
                    return Assembly.LoadFile(assemblyPath);
                }
            }

            return null;
        }
    }
}
