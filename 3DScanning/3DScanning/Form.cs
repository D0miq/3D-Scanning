using System;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Kinect;

namespace _3DScanning
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Form : System.Windows.Forms.Form
    {
        /// <summary>
        /// 
        /// </summary>
        private AVisualisation visualisation;

        /// <summary>
        /// 
        /// </summary>
        private WriteableBitmap writeableBitmap;

        /// <summary>
        /// 
        /// </summary>
        private EventHandler<MultiSourceFrameArrivedEventArgs> eventHandler;

        private string path;

        /// <summary>
        /// Creates form and inicialize kinect sensor, kinect attributes and camera space points
        /// </summary>
        public Form()
        {
            InitializeComponent();
            this.visualisation = new PointCloudRenderer(this.viewport, this.statusLB);
            this.writeableBitmap = new WriteableBitmap(this.visualisation.Kinect.DepthFrameDescription.Width, this.visualisation.Kinect.DepthFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray8, null);
            this.DataBinding();
            this.visualisation.Kinect.Start();
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
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void DisableControls(bool state)
        {
            this.minDepthTB.Enabled = !state;
            this.maxDepthTB.Enabled = !state;
            this.interpolationTB.Enabled = !state;
            this.colorlessMeshRB.Enabled = !state;
            this.coloredMeshRB.Enabled = !state;
            this.scaledMeshRB.Enabled = !state;
            this.generateAllCB.Enabled = !state;
            this.generateBT.Enabled = !state;
        }

        /// <summary>
        /// Binds controls to kinect attributes
        /// </summary>
        private void DataBinding()
        {
            this.minDepthTB.DataBindings.Add("Value", this.visualisation.KinectAttributes, "MinDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthTB.DataBindings.Add("Value", this.visualisation.KinectAttributes, "MaxDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.minDepthValueLB.DataBindings.Add("Text", this.visualisation.KinectAttributes, "MinDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthValueLB.DataBindings.Add("Text", this.visualisation.KinectAttributes, "MaxDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationValueLB.DataBindings.Add("Text", this.visualisation.KinectAttributes, "Interpolation", false, DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationTB.DataBindings.Add("Value", this.visualisation.KinectAttributes, "Interpolation", false, DataSourceUpdateMode.OnPropertyChanged);
            this.progressBar.DataBindings.Add("Maximum", this.visualisation.KinectAttributes, "Interpolation", false, DataSourceUpdateMode.OnPropertyChanged);
            this.imageControl.image.Source = this.writeableBitmap;
        }

        /// <summary>
        /// Event that triggers when generate button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void GenerateBT_Click(object sender, EventArgs e)
        {
            if(path == null)
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.ShowDialog();
                this.path = folderBrowserDialog.SelectedPath;
            }

            this.DisableControls(true);
            this.statusLB.Text = "Probíhá generování meshe!";

            if (this.coloredMeshRB.Checked)
            {
                this.visualisation = new ColoredMeshGenerator(this.progressBar, this.statusLB, this.path);
                this.visualisation.FinishedEvent += this.DisableControls;
            }
            else if (this.colorlessMeshRB.Checked)
            { 
                this.visualisation = new ColorlessMeshGenerator(this.progressBar, this.statusLB, this.path);
                this.visualisation.FinishedEvent += this.DisableControls;
            }
            else
            {
                this.visualisation = new ScaledMeshGenerator(this.progressBar, this.statusLB, this.path);
                this.visualisation.FinishedEvent += this.DisableControls;
            }

            if (this.generateAllCB.Checked)
            {
                this.visualisation = new MultiMeshesGenerator(this.path);
            }

            this.progressBar.Show();          
        }

        /// <summary>
        /// Event that triggers when preview button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void PreviewBT_Click(object sender, EventArgs e)
        {
            this.visualisation = new PointCloudRenderer(this.viewport, this.statusLB);      
            this.statusLB.Text = "Probíhá vytvoření náhledu!";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tabIndex = (sender as TabControl).SelectedIndex;
            if(tabIndex == 1)
            {
                this.visualisation = new DepthFrameVisualisation(this.writeableBitmap);
                this.eventHandler = this.visualisation.Reader_FrameArrived;
            }
            else
            {
                this.visualisation.Kinect.RemoveEventHandler(this.eventHandler);
            }
        }
    }
}
