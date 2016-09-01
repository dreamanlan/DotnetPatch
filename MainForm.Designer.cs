namespace DotnetPatch
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.infoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.assemblyList = new System.Windows.Forms.ListBox();
            this.resultCtrl = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addBtn = new System.Windows.Forms.ToolStripButton();
            this.clearBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.exportDir = new System.Windows.Forms.ToolStripTextBox();
            this.folderBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.srcClassName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.targetClassName = new System.Windows.Forms.ToolStripTextBox();
            this.replaceMethodBody = new System.Windows.Forms.ToolStripButton();
            this.extendMethodBody = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(881, 447);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(881, 494);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.statusLabel,
            this.infoLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(881, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(617, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "OK.";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // infoLabel
            // 
            this.infoLabel.IsLink = true;
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(147, 17);
            this.infoLabel.Text = "dreaman_163@163.com";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.assemblyList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.resultCtrl);
            this.splitContainer1.Size = new System.Drawing.Size(881, 447);
            this.splitContainer1.SplitterDistance = 320;
            this.splitContainer1.TabIndex = 0;
            // 
            // assemblyList
            // 
            this.assemblyList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assemblyList.FormattingEnabled = true;
            this.assemblyList.ItemHeight = 12;
            this.assemblyList.Location = new System.Drawing.Point(0, 0);
            this.assemblyList.Name = "assemblyList";
            this.assemblyList.Size = new System.Drawing.Size(320, 447);
            this.assemblyList.TabIndex = 1;
            // 
            // resultCtrl
            // 
            this.resultCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultCtrl.Location = new System.Drawing.Point(0, 0);
            this.resultCtrl.Multiline = true;
            this.resultCtrl.Name = "resultCtrl";
            this.resultCtrl.Size = new System.Drawing.Size(557, 447);
            this.resultCtrl.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addBtn,
            this.clearBtn,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.exportDir,
            this.folderBtn,
            this.toolStripSeparator2,
            this.toolStripLabel2,
            this.srcClassName,
            this.toolStripLabel3,
            this.targetClassName,
            this.replaceMethodBody,
            this.extendMethodBody});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(869, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // addBtn
            // 
            this.addBtn.Image = ((System.Drawing.Image)(resources.GetObject("addBtn.Image")));
            this.addBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(52, 22);
            this.addBtn.Text = "添加";
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // clearBtn
            // 
            this.clearBtn.Image = ((System.Drawing.Image)(resources.GetObject("clearBtn.Image")));
            this.clearBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(52, 22);
            this.clearBtn.Text = "清空";
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(68, 22);
            this.toolStripLabel1.Text = "输出目录：";
            // 
            // exportDir
            // 
            this.exportDir.Name = "exportDir";
            this.exportDir.Size = new System.Drawing.Size(200, 25);
            this.exportDir.ToolTipText = "输出目录";
            // 
            // folderBtn
            // 
            this.folderBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.folderBtn.Image = ((System.Drawing.Image)(resources.GetObject("folderBtn.Image")));
            this.folderBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.folderBtn.Name = "folderBtn";
            this.folderBtn.Size = new System.Drawing.Size(23, 22);
            this.folderBtn.Text = "...";
            this.folderBtn.ToolTipText = "选择输出目录";
            this.folderBtn.Click += new System.EventHandler(this.folderBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel2.Text = "被替换类";
            // 
            // srcClassName
            // 
            this.srcClassName.Name = "srcClassName";
            this.srcClassName.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(44, 22);
            this.toolStripLabel3.Text = "替换类";
            // 
            // targetClassName
            // 
            this.targetClassName.Name = "targetClassName";
            this.targetClassName.Size = new System.Drawing.Size(100, 25);
            // 
            // replaceMethodBody
            // 
            this.replaceMethodBody.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.replaceMethodBody.Image = ((System.Drawing.Image)(resources.GetObject("replaceMethodBody.Image")));
            this.replaceMethodBody.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceMethodBody.Name = "replaceMethodBody";
            this.replaceMethodBody.Size = new System.Drawing.Size(84, 22);
            this.replaceMethodBody.Text = "方法实现替换";
            this.replaceMethodBody.Click += new System.EventHandler(this.replaceMethodBody_Click);
            // 
            // extendMethodBody
            // 
            this.extendMethodBody.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.extendMethodBody.Image = ((System.Drawing.Image)(resources.GetObject("extendMethodBody.Image")));
            this.extendMethodBody.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.extendMethodBody.Name = "extendMethodBody";
            this.extendMethodBody.Size = new System.Drawing.Size(60, 22);
            this.extendMethodBody.Text = "脚本处理";
            this.extendMethodBody.Click += new System.EventHandler(this.execScript_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 494);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "MainForm";
            this.Text = "DotNet补丁工具";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addBtn;
        private System.Windows.Forms.ToolStripTextBox exportDir;
        private System.Windows.Forms.ToolStripButton folderBtn;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton clearBtn;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel infoLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox assemblyList;
        private System.Windows.Forms.TextBox resultCtrl;
        private System.Windows.Forms.ToolStripButton replaceMethodBody;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox srcClassName;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox targetClassName;
        private System.Windows.Forms.ToolStripButton extendMethodBody;
    }
}

