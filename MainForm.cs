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
using System.Security.Cryptography;

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
            if (!string.IsNullOrEmpty(m_LastInputDir)) {
                ofd.InitialDirectory = m_LastInputDir;
            }
            ofd.Filter = "exe/dll文件|*.exe;*.dll|所有文件|*.*||";
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
                    m_LastInputDir = path;
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
            if (!string.IsNullOrEmpty(m_LastOutputDir)) {
                fbd.SelectedPath = m_LastOutputDir;
            }
            fbd.Description = "请指定一个输出目录（注意不要使用原文件所在目录，否则会覆盖原文件！）";
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = path;
            if (DialogResult.OK == fbd.ShowDialog())
            {
                m_LastOutputDir= fbd.SelectedPath;
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
            if (assemblyList.Items.Count <= 0) {
                MessageBox.Show("请先添加要处理的dotnet exe/dll！");
                return;
            }
            string path = exportDir.Text.Trim();
            if (path.Length <= 0) {
                MessageBox.Show("请先选择一个输出目录！");
                return;
            }
            
            int curNum = 0;
            int totalNum = assemblyList.Items.Count;
            progressBar.Value = 0;
            statusLabel.Text = "开始方法体替换......";

            ClrFileModifier.ResultTexts.Clear();
            Dictionary<string, ClrFileModifier> methodBodyModifiers = new Dictionary<string, ClrFileModifier>();
            foreach (string s in assemblyList.Items) {
                statusLabel.Text = "对文件 " + s + " 进行方法体替换中......";
                Application.DoEvents();

                if (!methodBodyModifiers.ContainsKey(s)) {
                    ClrFileModifier methodBodyModifier = new ClrFileModifier(s, path);
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
            resultCtrl.Text = string.Join("\r\n", ClrFileModifier.ResultTexts.ToArray());
            ClrFileModifier.ResultTexts.Clear();

            MessageBox.Show($"方法体替换完毕，修改文件在{exportDir.Text}目录下");
        }

        private void execScript_Click(object sender, EventArgs e)
        {
            if (assemblyList.Items.Count <= 0) {
                MessageBox.Show("请先添加要处理的dotnet exe/dll！");
                return;
            }
            string path = exportDir.Text.Trim();
            if (path.Length <= 0) {
                MessageBox.Show("请先选择一个输出目录！");
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = m_LastScriptDir;
            ofd.DefaultExt = "scp";
            ofd.Filter = "脚本文件|*.scp|所有文件|*.*||";
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.Title = "请指定脚本文件";
            if (DialogResult.OK == ofd.ShowDialog()) {
                string file = ofd.FileName;
                m_LastScriptDir = Path.GetDirectoryName(file);

                List<string> files = new List<string>();
                foreach (string s in assemblyList.Items) {
                    files.Add(s);
                }
                ScriptProcessor.Start(files, path, file);

                MessageBox.Show($"脚本执行完成，修改文件在{exportDir.Text}目录下");
            }
        }

        private string m_LastInputDir = string.Empty;
        private string m_LastOutputDir = string.Empty;
        private string m_LastScriptDir = Environment.CurrentDirectory;
    }
}