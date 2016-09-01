using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace DotnetPatch
{
    public partial class MainForm : Form
    {
        public ToolStripProgressBar ProgressBar
        {
            get
            {
                return progressBar;
            }
        }
        public ToolStripStatusLabel StatusBar
        {
            get
            {
                return statusLabel;
            }
        }
        public TextBox ResultCtrl
        {
            get
            {
                return resultCtrl;
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            resultCtrl.Text = @"";
            ScriptProcessor.Init();
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            Dictionary<string, bool> existFiles = new Dictionary<string, bool>();
            foreach (string s in assemblyList.Items)
            {
                existFiles[s] = true;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "exe";
            ofd.Filter = "exe文件|*.exe|dll文件|*.dll||";
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            ofd.Multiselect = true;
            ofd.Title = "请指定要添加的.net程序文件";
            if (DialogResult.OK == ofd.ShowDialog())
            {
                foreach (string s in ofd.FileNames)
                {
                    if (!existFiles.ContainsKey(s))
                        assemblyList.Items.Add(s);
                }
                if (ofd.FileNames.Length > 0 && exportDir.Text.Trim().Length <= 0)
                {
                    string as0 = ofd.FileNames[0];
                    string path = Path.GetDirectoryName(as0);
                    exportDir.Text = Path.GetDirectoryName(path);
                }
                statusLabel.Text = "OK.";
            }
            ofd.Dispose();
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            assemblyList.Items.Clear();
            statusLabel.Text = "OK.";
        }

        private void folderBtn_Click(object sender, EventArgs e)
        {
            string path = exportDir.Text.Trim();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "请指定一个输出目录（注意不要使用原文件所在目录，否则会覆盖原文件！）";
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = path;
            if (DialogResult.OK == fbd.ShowDialog())
            {
                exportDir.Text = fbd.SelectedPath;
                statusLabel.Text = "OK.";
            }
            else
            {
                return;
            }
        }
        
        private void replaceMethodBody_Click(object sender, EventArgs e)
        {
            if (assemblyList.Items.Count <= 0)
                return;
            string path = exportDir.Text.Trim();
            if (path.Length <= 0) {
                MessageBox.Show("请先选择一个输出目录！");
                return;
            }
            
            int curNum = 0;
            int totalNum = assemblyList.Items.Count;
            progressBar.Value = 0;
            statusLabel.Text = "开始方法体替换......";

            MethodBodyModifier.ErrorTxts.Clear();
            Dictionary<string, MethodBodyModifier> methodBodyModifiers = new Dictionary<string, MethodBodyModifier>();
            foreach (string s in assemblyList.Items) {
                statusLabel.Text = "对文件 " + s + " 进行方法体替换中......";
                Application.DoEvents();

                if (!methodBodyModifiers.ContainsKey(s)) {
                    MethodBodyModifier methodBodyModifier = new MethodBodyModifier(s, path);
                    methodBodyModifiers[s] = methodBodyModifier;
                }
                methodBodyModifiers[s].BeginReplace();
                methodBodyModifiers[s].Replace(srcClassName.Text, targetClassName.Text);
                methodBodyModifiers[s].EndReplace();

                curNum++;
                progressBar.Value = curNum * 100 / totalNum;
                Application.DoEvents();
            }
            progressBar.Value = 0;
            statusLabel.Text = "方法体替换完毕.";
            resultCtrl.Text = string.Join("\r\n", MethodBodyModifier.ErrorTxts.ToArray());
            MethodBodyModifier.ErrorTxts.Clear();
        }

        private void execScript_Click(object sender, EventArgs e)
        {
            if (assemblyList.Items.Count <= 0)
                return;
            string path = exportDir.Text.Trim();
            if (path.Length <= 0) {
                MessageBox.Show("请先选择一个输出目录！");
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "scp";
            ofd.Filter = "脚本文件|*.scp||";
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.Title = "请指定脚本文件";
            if (DialogResult.OK == ofd.ShowDialog()) {
                string file = ofd.FileName;

                List<string> files = new List<string>();
                foreach (string s in assemblyList.Items) {
                    files.Add(s);
                }
                ScriptProcessor.Start(files, path, file);
            }
        }
    }
}