using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DotnetPatch
{
    static class Program
    {
        public static MainForm MainForm
        {
            get { return s_MainForm; }
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            s_MainForm = new MainForm();
            Application.Run(s_MainForm);
        }

        private static MainForm s_MainForm;
    }
}