namespace _3DScanning
{
    partial class Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.previewBT = new System.Windows.Forms.Button();
            this.minDepthLB = new System.Windows.Forms.Label();
            this.minDepthTB = new System.Windows.Forms.TrackBar();
            this.minDepthValueLB = new System.Windows.Forms.Label();
            this.maxDepthLB = new System.Windows.Forms.Label();
            this.maxDepthTB = new System.Windows.Forms.TrackBar();
            this.interpolationTB = new System.Windows.Forms.TrackBar();
            this.interpolationLB = new System.Windows.Forms.Label();
            this.maxDepthValueLB = new System.Windows.Forms.Label();
            this.interpolationValueLB = new System.Windows.Forms.Label();
            this.statusLB = new System.Windows.Forms.Label();
            this.generateBT = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.generateAllCB = new System.Windows.Forms.CheckBox();
            this.viewport = new OpenTKLib.OGLControl();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPointCloud = new System.Windows.Forms.TabPage();
            this.tabDepthFrame = new System.Windows.Forms.TabPage();
            this.elementHost = new System.Windows.Forms.Integration.ElementHost();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minDepthTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDepthTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.interpolationTB)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPointCloud.SuspendLayout();
            this.tabDepthFrame.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.previewBT, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.minDepthLB, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.minDepthTB, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.minDepthValueLB, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.maxDepthLB, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.maxDepthTB, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.interpolationTB, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.interpolationLB, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.maxDepthValueLB, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.interpolationValueLB, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.statusLB, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.generateBT, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.progressBar, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.generateAllCB, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 13);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(300, 511);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // previewBT
            // 
            this.previewBT.Location = new System.Drawing.Point(78, 3);
            this.previewBT.Name = "previewBT";
            this.previewBT.Size = new System.Drawing.Size(144, 34);
            this.previewBT.TabIndex = 0;
            this.previewBT.Text = "Náhled";
            this.previewBT.UseVisualStyleBackColor = true;
            this.previewBT.Click += new System.EventHandler(this.PreviewBT_Click);
            // 
            // minDepthLB
            // 
            this.minDepthLB.Location = new System.Drawing.Point(3, 43);
            this.minDepthLB.Margin = new System.Windows.Forms.Padding(3);
            this.minDepthLB.Name = "minDepthLB";
            this.minDepthLB.Size = new System.Drawing.Size(69, 34);
            this.minDepthLB.TabIndex = 4;
            this.minDepthLB.Text = "Min hloubka";
            this.minDepthLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // minDepthTB
            // 
            this.minDepthTB.BackColor = System.Drawing.SystemColors.Control;
            this.minDepthTB.LargeChange = 50;
            this.minDepthTB.Location = new System.Drawing.Point(78, 43);
            this.minDepthTB.Maximum = 8000;
            this.minDepthTB.Minimum = 500;
            this.minDepthTB.Name = "minDepthTB";
            this.minDepthTB.Size = new System.Drawing.Size(144, 34);
            this.minDepthTB.SmallChange = 10;
            this.minDepthTB.TabIndex = 8;
            this.minDepthTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.minDepthTB.Value = 500;
            // 
            // minDepthValueLB
            // 
            this.minDepthValueLB.Location = new System.Drawing.Point(228, 43);
            this.minDepthValueLB.Margin = new System.Windows.Forms.Padding(3);
            this.minDepthValueLB.Name = "minDepthValueLB";
            this.minDepthValueLB.Size = new System.Drawing.Size(69, 34);
            this.minDepthValueLB.TabIndex = 12;
            this.minDepthValueLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // maxDepthLB
            // 
            this.maxDepthLB.Location = new System.Drawing.Point(3, 83);
            this.maxDepthLB.Margin = new System.Windows.Forms.Padding(3);
            this.maxDepthLB.Name = "maxDepthLB";
            this.maxDepthLB.Size = new System.Drawing.Size(69, 34);
            this.maxDepthLB.TabIndex = 5;
            this.maxDepthLB.Text = "Max hloubka";
            this.maxDepthLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // maxDepthTB
            // 
            this.maxDepthTB.LargeChange = 50;
            this.maxDepthTB.Location = new System.Drawing.Point(78, 83);
            this.maxDepthTB.Maximum = 8000;
            this.maxDepthTB.Minimum = 500;
            this.maxDepthTB.Name = "maxDepthTB";
            this.maxDepthTB.Size = new System.Drawing.Size(144, 34);
            this.maxDepthTB.SmallChange = 10;
            this.maxDepthTB.TabIndex = 9;
            this.maxDepthTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.maxDepthTB.Value = 500;
            // 
            // interpolationTB
            // 
            this.interpolationTB.LargeChange = 10;
            this.interpolationTB.Location = new System.Drawing.Point(78, 123);
            this.interpolationTB.Maximum = 1000;
            this.interpolationTB.Minimum = 1;
            this.interpolationTB.Name = "interpolationTB";
            this.interpolationTB.Size = new System.Drawing.Size(144, 34);
            this.interpolationTB.TabIndex = 17;
            this.interpolationTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.interpolationTB.Value = 1;
            // 
            // interpolationLB
            // 
            this.interpolationLB.Location = new System.Drawing.Point(3, 123);
            this.interpolationLB.Margin = new System.Windows.Forms.Padding(3);
            this.interpolationLB.Name = "interpolationLB";
            this.interpolationLB.Size = new System.Drawing.Size(69, 34);
            this.interpolationLB.TabIndex = 16;
            this.interpolationLB.Text = "Počet interpolací";
            this.interpolationLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // maxDepthValueLB
            // 
            this.maxDepthValueLB.Location = new System.Drawing.Point(228, 83);
            this.maxDepthValueLB.Margin = new System.Windows.Forms.Padding(3);
            this.maxDepthValueLB.Name = "maxDepthValueLB";
            this.maxDepthValueLB.Size = new System.Drawing.Size(69, 34);
            this.maxDepthValueLB.TabIndex = 13;
            this.maxDepthValueLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // interpolationValueLB
            // 
            this.interpolationValueLB.Location = new System.Drawing.Point(228, 123);
            this.interpolationValueLB.Margin = new System.Windows.Forms.Padding(3);
            this.interpolationValueLB.Name = "interpolationValueLB";
            this.interpolationValueLB.Size = new System.Drawing.Size(69, 34);
            this.interpolationValueLB.TabIndex = 15;
            this.interpolationValueLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusLB
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.statusLB, 3);
            this.statusLB.Location = new System.Drawing.Point(3, 474);
            this.statusLB.Margin = new System.Windows.Forms.Padding(3);
            this.statusLB.Name = "statusLB";
            this.statusLB.Size = new System.Drawing.Size(294, 34);
            this.statusLB.TabIndex = 14;
            this.statusLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // generateBT
            // 
            this.generateBT.Location = new System.Drawing.Point(78, 434);
            this.generateBT.Name = "generateBT";
            this.generateBT.Size = new System.Drawing.Size(144, 34);
            this.generateBT.TabIndex = 1;
            this.generateBT.Text = "Generovat mesh";
            this.generateBT.UseVisualStyleBackColor = true;
            this.generateBT.Click += new System.EventHandler(this.GenerateBT_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.progressBar.Location = new System.Drawing.Point(78, 405);
            this.progressBar.MarqueeAnimationSpeed = 1;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(144, 23);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 19;
            this.progressBar.Visible = false;
            // 
            // generateAllCB
            // 
            this.generateAllCB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.generateAllCB.AutoSize = true;
            this.generateAllCB.Location = new System.Drawing.Point(78, 163);
            this.generateAllCB.Name = "generateAllCB";
            this.generateAllCB.Size = new System.Drawing.Size(144, 34);
            this.generateAllCB.TabIndex = 20;
            this.generateAllCB.Text = "Generovat všechny meshe";
            this.generateAllCB.UseVisualStyleBackColor = true;
            // 
            // viewport
            // 
            this.viewport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(255)))));
            this.viewport.Location = new System.Drawing.Point(0, 0);
            this.viewport.Name = "viewport";
            this.viewport.Size = new System.Drawing.Size(667, 485);
            this.viewport.TabIndex = 1;
            this.viewport.VSync = false;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPointCloud);
            this.tabControl.Controls.Add(this.tabDepthFrame);
            this.tabControl.Location = new System.Drawing.Point(320, 13);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(675, 511);
            this.tabControl.TabIndex = 2;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // tabPointCloud
            // 
            this.tabPointCloud.Controls.Add(this.viewport);
            this.tabPointCloud.Location = new System.Drawing.Point(4, 22);
            this.tabPointCloud.Name = "tabPointCloud";
            this.tabPointCloud.Padding = new System.Windows.Forms.Padding(3);
            this.tabPointCloud.Size = new System.Drawing.Size(667, 485);
            this.tabPointCloud.TabIndex = 0;
            this.tabPointCloud.Text = "Point Cloud";
            this.tabPointCloud.UseVisualStyleBackColor = true;
            // 
            // tabDepthFrame
            // 
            this.tabDepthFrame.Controls.Add(this.elementHost);
            this.tabDepthFrame.Location = new System.Drawing.Point(4, 22);
            this.tabDepthFrame.Name = "tabDepthFrame";
            this.tabDepthFrame.Padding = new System.Windows.Forms.Padding(3);
            this.tabDepthFrame.Size = new System.Drawing.Size(667, 485);
            this.tabDepthFrame.TabIndex = 1;
            this.tabDepthFrame.Text = "Depth Frame";
            this.tabDepthFrame.UseVisualStyleBackColor = true;
            // 
            // elementHost
            // 
            this.elementHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost.Location = new System.Drawing.Point(3, 3);
            this.elementHost.Name = "elementHost";
            this.elementHost.Size = new System.Drawing.Size(661, 479);
            this.elementHost.TabIndex = 0;
            this.elementHost.Text = "elementHost";
            this.elementHost.Child = null;
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 537);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "3D Scanning";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minDepthTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDepthTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.interpolationTB)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPointCloud.ResumeLayout(false);
            this.tabDepthFrame.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button previewBT;
        private System.Windows.Forms.Button generateBT;
        private System.Windows.Forms.Label minDepthLB;
        private System.Windows.Forms.Label maxDepthLB;
        private System.Windows.Forms.TrackBar minDepthTB;
        private System.Windows.Forms.TrackBar maxDepthTB;
        private System.Windows.Forms.Label minDepthValueLB;
        private System.Windows.Forms.Label maxDepthValueLB;
        private System.Windows.Forms.Label statusLB;
        private System.Windows.Forms.Label interpolationValueLB;
        private System.Windows.Forms.Label interpolationLB;
        private System.Windows.Forms.TrackBar interpolationTB;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox generateAllCB;
        private OpenTKLib.OGLControl viewport;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPointCloud;
        private System.Windows.Forms.TabPage tabDepthFrame;
        private System.Windows.Forms.Integration.ElementHost elementHost;
    }
}

