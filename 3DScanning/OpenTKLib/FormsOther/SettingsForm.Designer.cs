namespace OpenTKLib
{
    partial class SettingsForm
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPointSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPointSizeAxis = new System.Windows.Forms.TextBox();
            this.buttonColorModels = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonColorModel = new System.Windows.Forms.Button();
            this.buttonBackColor = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBoxPointCloudCentered = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxShowModelAxes = new System.Windows.Forms.CheckBox();
            this.checkBoxShowAxesLabels = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxShowXYZAxes = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxCameraFOV = new System.Windows.Forms.CheckBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(206, 372);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(176, 23);
            this.buttonOK.TabIndex = 27;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Point Size for point cloud";
            // 
            // textBoxPointSize
            // 
            this.textBoxPointSize.Location = new System.Drawing.Point(217, 68);
            this.textBoxPointSize.Name = "textBoxPointSize";
            this.textBoxPointSize.Size = new System.Drawing.Size(100, 20);
            this.textBoxPointSize.TabIndex = 30;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Point Size for axis";
            // 
            // textBoxPointSizeAxis
            // 
            this.textBoxPointSizeAxis.Location = new System.Drawing.Point(217, 101);
            this.textBoxPointSizeAxis.Name = "textBoxPointSizeAxis";
            this.textBoxPointSizeAxis.Size = new System.Drawing.Size(100, 20);
            this.textBoxPointSizeAxis.TabIndex = 32;
            // 
            // buttonColorModels
            // 
            this.buttonColorModels.Location = new System.Drawing.Point(217, 129);
            this.buttonColorModels.Name = "buttonColorModels";
            this.buttonColorModels.Size = new System.Drawing.Size(123, 23);
            this.buttonColorModels.TabIndex = 33;
            this.buttonColorModels.Text = "Change";
            this.buttonColorModels.UseVisualStyleBackColor = true;
            this.buttonColorModels.Click += new System.EventHandler(this.buttonColorModels_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(66, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Color of all Models";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Color of current model";
            // 
            // buttonColorModel
            // 
            this.buttonColorModel.Location = new System.Drawing.Point(217, 162);
            this.buttonColorModel.Name = "buttonColorModel";
            this.buttonColorModel.Size = new System.Drawing.Size(123, 23);
            this.buttonColorModel.TabIndex = 36;
            this.buttonColorModel.Text = "Change";
            this.buttonColorModel.UseVisualStyleBackColor = true;
            this.buttonColorModel.Click += new System.EventHandler(this.buttonColorModel_Click);
            // 
            // buttonBackColor
            // 
            this.buttonBackColor.Location = new System.Drawing.Point(217, 191);
            this.buttonBackColor.Name = "buttonBackColor";
            this.buttonBackColor.Size = new System.Drawing.Size(123, 23);
            this.buttonBackColor.TabIndex = 37;
            this.buttonBackColor.Text = "Change";
            this.buttonBackColor.UseVisualStyleBackColor = true;
            this.buttonBackColor.Click += new System.EventHandler(this.buttonBackColor_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(66, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "Color of background";
            // 
            // checkBoxPointCloudCentered
            // 
            this.checkBoxPointCloudCentered.AutoSize = true;
            this.checkBoxPointCloudCentered.Checked = true;
            this.checkBoxPointCloudCentered.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPointCloudCentered.Location = new System.Drawing.Point(217, 237);
            this.checkBoxPointCloudCentered.Name = "checkBoxPointCloudCentered";
            this.checkBoxPointCloudCentered.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPointCloudCentered.TabIndex = 39;
            this.checkBoxPointCloudCentered.UseVisualStyleBackColor = true;
            this.checkBoxPointCloudCentered.CheckedChanged += new System.EventHandler(this.checkBoxPointCloudCentered_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(66, 241);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "Point Cloud is centered ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(66, 266);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 41;
            this.label7.Text = "Show Model Axes";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(67, 290);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 13);
            this.label8.TabIndex = 42;
            this.label8.Text = "Show Axes Labels";
            // 
            // checkBoxShowModelAxes
            // 
            this.checkBoxShowModelAxes.AutoSize = true;
            this.checkBoxShowModelAxes.Checked = true;
            this.checkBoxShowModelAxes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowModelAxes.Location = new System.Drawing.Point(217, 266);
            this.checkBoxShowModelAxes.Name = "checkBoxShowModelAxes";
            this.checkBoxShowModelAxes.Size = new System.Drawing.Size(15, 14);
            this.checkBoxShowModelAxes.TabIndex = 43;
            this.checkBoxShowModelAxes.UseVisualStyleBackColor = true;
            this.checkBoxShowModelAxes.CheckedChanged += new System.EventHandler(this.checkBoxShowModelAxes_CheckedChanged);
            // 
            // checkBoxShowAxesLabels
            // 
            this.checkBoxShowAxesLabels.AutoSize = true;
            this.checkBoxShowAxesLabels.Checked = true;
            this.checkBoxShowAxesLabels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowAxesLabels.Location = new System.Drawing.Point(217, 290);
            this.checkBoxShowAxesLabels.Name = "checkBoxShowAxesLabels";
            this.checkBoxShowAxesLabels.Size = new System.Drawing.Size(15, 14);
            this.checkBoxShowAxesLabels.TabIndex = 44;
            this.checkBoxShowAxesLabels.UseVisualStyleBackColor = true;
            this.checkBoxShowAxesLabels.CheckedChanged += new System.EventHandler(this.checkBoxShowAxesLabels_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(66, 341);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 13);
            this.label9.TabIndex = 45;
            this.label9.Text = "Show xyz Axes ";
            // 
            // checkBoxShowXYZAxes
            // 
            this.checkBoxShowXYZAxes.AutoSize = true;
            this.checkBoxShowXYZAxes.Checked = true;
            this.checkBoxShowXYZAxes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowXYZAxes.Location = new System.Drawing.Point(217, 341);
            this.checkBoxShowXYZAxes.Name = "checkBoxShowXYZAxes";
            this.checkBoxShowXYZAxes.Size = new System.Drawing.Size(15, 14);
            this.checkBoxShowXYZAxes.TabIndex = 46;
            this.checkBoxShowXYZAxes.UseVisualStyleBackColor = true;
            this.checkBoxShowXYZAxes.CheckedChanged += new System.EventHandler(this.checkBoxShowXYZAxes_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(67, 313);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 13);
            this.label10.TabIndex = 47;
            this.label10.Text = "Show Camera FOV";
            // 
            // checkBoxCameraFOV
            // 
            this.checkBoxCameraFOV.AutoSize = true;
            this.checkBoxCameraFOV.Checked = true;
            this.checkBoxCameraFOV.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCameraFOV.Location = new System.Drawing.Point(217, 313);
            this.checkBoxCameraFOV.Name = "checkBoxCameraFOV";
            this.checkBoxCameraFOV.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCameraFOV.TabIndex = 48;
            this.checkBoxCameraFOV.UseVisualStyleBackColor = true;
            this.checkBoxCameraFOV.CheckedChanged += new System.EventHandler(this.checkBoxCameraFOV_CheckedChanged);
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(343, 80);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(66, 23);
            this.buttonApply.TabIndex = 49;
            this.buttonApply.Text = "Apply change";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 486);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.checkBoxCameraFOV);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.checkBoxShowXYZAxes);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.checkBoxShowAxesLabels);
            this.Controls.Add(this.checkBoxShowModelAxes);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBoxPointCloudCentered);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonBackColor);
            this.Controls.Add(this.buttonColorModel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonColorModels);
            this.Controls.Add(this.textBoxPointSizeAxis);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPointSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPointSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPointSizeAxis;
        private System.Windows.Forms.Button buttonColorModels;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonColorModel;
        private System.Windows.Forms.Button buttonBackColor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxPointCloudCentered;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBoxShowModelAxes;
        private System.Windows.Forms.CheckBox checkBoxShowAxesLabels;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBoxShowXYZAxes;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox checkBoxCameraFOV;
        private System.Windows.Forms.Button buttonApply;

    }
}