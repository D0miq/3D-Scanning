using System;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Kinect;
using System.Threading;
using System.Threading.Tasks;

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
        private Visualisation visualisation;

        /// <summary>
        /// 
        /// </summary>
        private WriteableBitmap writeableBitmap;

        private string path;

        private object locker;

        private bool generating = false;

        private bool previewing = false;

        private int tabIndex = 0;

        private Kinect kinect;

        private DepthData depthData;

        private ColorData colorData;

        /// <summary>
        /// Creates form and inicialize kinect sensor, kinect attributes and camera space points
        /// </summary>
        public Form()
        {
            InitializeComponent();
            this.kinect = Kinect.GetInstance();
            this.kinect.AddEventHandler(this.Reader_FrameArrived);
            this.depthData = new DepthData();
            this.colorData = new ColorData();
            this.visualisation = new Visualisation(this.depthData, this.colorData);
            this.writeableBitmap = new WriteableBitmap(kinect.DepthFrameDescription.Width, kinect.DepthFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray8, null);
            this.locker = new object();
            this.DataBinding();
            this.kinect.Start();
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
            this.dispersionCB.Enabled = !state;
        }

        /// <summary>
        /// Binds controls to kinect attributes
        /// </summary>
        private void DataBinding()
        {
            this.minDepthTB.DataBindings.Add("Value", this.kinect.KinectAttributes, "MinDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthTB.DataBindings.Add("Value", this.kinect.KinectAttributes, "MaxDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.minDepthValueLB.DataBindings.Add("Text", this.kinect.KinectAttributes, "MinDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthValueLB.DataBindings.Add("Text", this.kinect.KinectAttributes, "MaxDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationValueLB.DataBindings.Add("Text", this.kinect.KinectAttributes, "Interpolation", false, DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationTB.DataBindings.Add("Value", this.kinect.KinectAttributes, "Interpolation", false, DataSourceUpdateMode.OnPropertyChanged);
            this.scaledMeshRB.CheckedChanged += delegate (object sender, EventArgs e)
            {
                if (scaledMeshRB.Checked)
                {
                    dispersionCB.Checked = true;
                    dispersionCB.Enabled = false;

                }
                else
                {
                    dispersionCB.Checked = false;
                    dispersionCB.Enabled = true;
                }
            };
            this.imageControl.image.Source = this.writeableBitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Reader_FrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            try
            {
                MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
                if (multiSourceFrame != null)
                {
                    this.depthData.AddDepthData(multiSourceFrame);
                    this.colorData.AddColorData(multiSourceFrame);
                    //
                    if (this.tabIndex == 1)
                    {
                        this.visualisation.VisualiseDepth(this.writeableBitmap);
                    }
                    else if (this.previewing)
                    {
                        this.visualisation.VisualisePointCloud(this.viewport);
                        this.previewing = false;
                        this.statusLB.Text = "Náhled byl zobrazen.";
                    }


                    //
                    if (this.generating && this.kinect.KinectAttributes.Interpolation <= this.depthData.Data.Count && this.kinect.KinectAttributes.Interpolation <= this.colorData.Data.Count)
                    {

                        if (this.generateAllCB.Checked)
                        {
                            this.visualisation.CreatePointsFromAllFrames(path, this.kinect.KinectAttributes.Interpolation);
                        }

                        Dispersion dispersion = new Dispersion(depthData.Data.GetRange(this.kinect.KinectAttributes.Interpolation));
                        ushort[] interpolatedMesh = this.visualisation.CreateCleanInterpolatedMesh();
                        if (dispersionCB.Checked)
                        {
                            dispersion.CreateDispersions(interpolatedMesh);
                            this.visualisation.AddDispersion(dispersion);
                        }
                        if (this.coloredMeshRB.Checked) { this.visualisation.CreateColorMesh(); }
                        else if (this.scaledMeshRB.Checked) { this.visualisation.CreateScaledMesh(); }
                        this.visualisation.GenerateMesh(path + "\\points.obj", false, true);

                        this.generating = false;
                        this.DisableControls(false);
                        this.statusLB.Text = "Mesh byl vygenerován a uložen.";
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                this.statusLB.Text = "Vyskytla se chyba. Prosím restartujte aplikaci.";
            }
        }

        /// <summary>
        /// Event that triggers when generate button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void GenerateBT_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.ShowDialog();
                this.path = folderBrowserDialog.SelectedPath;
            }
            this.visualisation.Clear();
            if (generateAllCB.Checked) { this.visualisation.GenerateMesh(path + "\\allFrames.obj", false, false); }
            this.statusLB.Text = "Probíhá generování meshe!";
            this.DisableControls(true);
            this.generating = true;
        }

        /// <summary>
        /// Event that triggers when preview button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void PreviewBT_Click(object sender, EventArgs e)
        {
            this.statusLB.Text = "Probíhá vytvoření náhledu!";
            this.previewing = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tabIndex = (sender as TabControl).SelectedIndex;
        }
    }
}
