using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTKLib;
using OpenTK;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace _3DScanning
{
    public partial class Form : System.Windows.Forms.Form
    {
        private AVisualisation visualisation;

        private WriteableBitmap bitmap;

        /// <summary>
        /// Creates form and inicialize kinect sensor, kinect attributes and camera space points
        /// </summary>
        public Form()
        {
            InitializeComponent();
            this.visualisation = new ColorfulGenerator(this.progressBar, this.statusLB);
            this.bitmap = new WriteableBitmap(this.visualisation.Kinect.Description.Width, this.visualisation.Kinect.Description.Height, 96.0, 96.0, PixelFormats.Gray8, null);
            this.DataBinding();  
        }

        /// <summary>
        /// Destructor that frees resources
        /// </summary>
        ~Form()
        {
            if (this.viewport != null)
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
            this.imageControl.image.Source = this.bitmap;
        }

        /// <summary>
        /// Event that triggers when generate button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void GenerateBT_Click(object sender, EventArgs e)
        {

            if (!(this.visualisation is ColorfulGenerator) && this.colorCB.Checked)
            {
                this.visualisation = new ColorfulGenerator(this.progressBar, this.statusLB);
            }else if(!(this.visualisation is ColorlessGenerator) && !this.colorCB.Checked)
            {
                this.visualisation = new ColorlessGenerator(this.progressBar, this.statusLB);
            }
            this.statusLB.Text = "Probíhá generování meshe!";
            //this.DisableControls(true);
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
            if (!(this.visualisation is PointCloudRenderer))
            {
                this.visualisation = new PointCloudRenderer(this.viewport, this.statusLB);
                this.visualisation.Kinect.EventHandler = this.visualisation.Reader_FrameArrived;
            }
            this.statusLB.Text = "Probíhá vytvoření náhledu!";
            //this.DisableControls(true);
            this.visualisation.Kinect.Start();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tabIndex = (sender as TabControl).SelectedIndex;
            if(tabIndex == 1)
            {
                if (!(this.visualisation is DepthFrameVisualisation))
                {
                    this.visualisation = new DepthFrameVisualisation(this.bitmap);
                    this.visualisation.Kinect.EventHandler = this.visualisation.Reader_FrameArrived;
                }
                this.visualisation.Kinect.Start();
            }
            else
            {
                this.visualisation.Kinect.Stop();
            }
        }
    }
}
