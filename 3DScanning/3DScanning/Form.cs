using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTKLib;
using OpenTK;

namespace _3DScanning
{
    public partial class Form : System.Windows.Forms.Form
    {
        private AVisualisation visualisation;

        /// <summary>
        /// Creates form and inicialize kinect sensor, kinect attributes and camera space points
        /// </summary>
        public Form()
        {
            InitializeComponent();
            this.visualisation = new GenerateVisualisation(this.progressBar, this.statusLB);
            this.DataBinding();  
        }

        /// <summary>
        /// Destructor that frees resources
        /// </summary>
        ~Form()
        {
            if(this.viewport != null)
            {
                this.viewport.Dispose();
                this.viewport = null;
            }  
        }

        /// <summary>
        /// Disables or enables controls that adjust kinect attributes
        /// </summary>
        /// <param name="state">Defines if controls are disable or enable</param>
        private void DisableControls(bool state)
        {
            this.minDepthTB.Enabled = !state;
            this.maxDepthTB.Enabled = !state;
            this.interpolationTB.Enabled = !state;
        }

        /// <summary>
        /// Binds controls to kinect attributes
        /// </summary>
        private void DataBinding()
        {
            this.minDepthTB.DataBindings.Add("Value", this.visualisation.KinectAttributes, "MinDepth", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthTB.DataBindings.Add("Value", this.visualisation.KinectAttributes, "MaxDepth", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.minDepthValueLB.DataBindings.Add("Text", this.visualisation.KinectAttributes, "MinDepth", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthValueLB.DataBindings.Add("Text", this.visualisation.KinectAttributes, "MaxDepth", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationValueLB.DataBindings.Add("Text", this.visualisation.KinectAttributes, "Interpolation", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationTB.DataBindings.Add("Value", this.visualisation.KinectAttributes, "Interpolation", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.progressBar.DataBindings.Add("Maximum", this.visualisation.KinectAttributes, "Interpolation", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.generateAllCB.DataBindings.Add("Checked", this.visualisation.KinectAttributes, "GenerateAll", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
        }

        /// <summary>
        /// Event that triggers when generate button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void GenerateBT_Click(object sender, EventArgs e)
        {
            if(!(this.visualisation is GenerateVisualisation))
            {
                this.visualisation = new GenerateVisualisation(this.progressBar, this.statusLB);
            }
            this.statusLB.Text = "Probíhá generování meshe!";
            this.DisableControls(true);
            this.progressBar.Show();
            this.visualisation.Kinect.Start();
            
        }

        /// <summary>
        /// Event that triggers when preview button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void PreviewBT_Click(object sender, EventArgs e)
        {
            if (!(this.visualisation is RenderVisualisation))
            {
                this.visualisation = new RenderVisualisation(this.viewport, this.statusLB);
            }
            this.statusLB.Text = "Probíhá vytvoření náhledu!";
            this.DisableControls(true);
            this.visualisation.Kinect.Start();
        }
    }
}
