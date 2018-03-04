namespace _3DScanning
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using Microsoft.Kinect;
    using HelixToolkit.Wpf;

    /// <summary>
    /// An instance of the <see cref="Form"/> class represents a main window of the application.
    /// It provides informations about states of the application, controls and shows preview of scanned frames.
    /// </summary>
    public partial class Form : System.Windows.Forms.Form
    {
        /// <summary>
        /// Visualisation algorithms.
        /// </summary>
        private Visualisation visualisation;

        /// <summary>
        /// The bitmap used for 2D visualisation.
        /// </summary>
        private WriteableBitmap writeableBitmap;

        /// <summary>
        /// The 3D point cloud used for 3D visualisation.
        /// </summary>
        private PointsVisual3D pointCloud;

        /// <summary>
        /// The writer saves meshes into a file.
        /// </summary>
        private IMeshWriter meshWriter;

        /// <summary>
        /// The target path meshes will be stored into.
        /// </summary>
        private string path = string.Empty;

        /// <summary>
        /// The indicator of generating state. True if generation is in progress, false otherwise.
        /// </summary>
        private bool isGenerating = false;

        /// <summary>
        /// The indicator of previewing state. True if previewing is in progress, false otherwise.
        /// </summary>
        private bool isPreviewing = false;

        /// <summary>
        /// The index of a selected tab.
        /// </summary>
        private int tabIndex = 0;

        /// <summary>
        /// The kinect.
        /// </summary>
        private Kinect kinect;

        /// <summary>
        /// Depth data storage.
        /// </summary>
        private DepthData depthData;

        /// <summary>
        /// Color data storage.
        /// </summary>
        private ColorData colorData;

        /// <summary>
        /// Frames counter.
        /// </summary>
        private int counter = 0;

        /// <summary>
        /// Records a progress.
        /// </summary>
        private IProgress<int> progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class.
        /// </summary>
        public Form()
        {
            this.InitializeComponent();
            this.kinect = Kinect.INSTANCE;
            this.kinect.AddEventHandler(this.Reader_FrameArrived);
            this.depthData = new DepthData();
            this.colorData = new ColorData();
            this.visualisation = new Visualisation(this.depthData);
            this.meshWriter = new ObjMeshWriter();
            this.writeableBitmap = new WriteableBitmap(this.kinect.DepthFrameDescription.Width, this.kinect.DepthFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray8, null);
            this.pointCloud = new PointsVisual3D
            {
                Color = Colors.White
            };

            this.progress = new Progress<int>(v =>
            {
                this.progressBar.Increment(v);
            });

            this.DataBinding();
            this.kinect.Start();
        }

        /// <summary>
        /// Handles incoming frames from <see cref="MultiSourceFrameReader"/>.
        /// </summary>
        /// <param name="sender">The sender of an event.</param>
        /// <param name="e">Arguments of an event.</param>
        /// <seealso cref="Kinect"/>
        public async void Reader_FrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            try
            {
                MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
                if (multiSourceFrame != null)
                {
                    this.depthData.SetLastFrame(multiSourceFrame);

                    // Visualise in 2D or 3D.
                    if (this.tabIndex == 1)
                    {
                        this.visualisation.VisualiseDepth(this.writeableBitmap);
                    }
                    else if (this.isPreviewing)
                    {
                        this.visualisation.VisualisePointCloud(this.pointCloud);

                        this.isPreviewing = false;
                        this.statusLB.Text = "Náhled byl zobrazen.";
                    }

                    // Adds frames to depth and color data and start generate them.
                    if (this.isGenerating)
                    {
                        this.depthData.AddLastFrame();
                        this.colorData.AddFrame(multiSourceFrame);
                        this.progress.Report(1);

                        if (this.generateAllCB.Checked)
                        {
                            ushort[] depthDataCopy = new ushort[this.depthData.LastFrame.Length];
                            this.depthData.LastFrame.CopyTo(depthDataCopy, 0);
                            Task.Run(() => this.GenerateAllFrames(depthDataCopy, this.depthData.FramesCount, this.counter));
                        }

                        if (this.kinect.KinectAttributes.Interpolation <= this.depthData.FramesCount && this.kinect.KinectAttributes.Interpolation <= this.colorData.FramesCount)
                        {
                            this.isGenerating = false;
                            this.stopBT.Enabled = false;

                            await Task.Run(() => this.GenerateSingleMesh());

                            this.DisableControls(false);
                            this.statusLB.Text = "Mesh byl vygenerován a uložen.";
                            this.progressBar.Value = 0;
                            this.progressBar.Hide();
                            this.counter++;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                this.statusLB.Text = "Vyskytla se chyba. Restartujte aplikaci.";
            }
        }

        /// <summary>
        /// Generates a single mesh with the given settings from a user.
        /// </summary>
        private void GenerateSingleMesh()
        {
            this.progress.Report(1);
            this.depthData.ReduceRange();
            var meshBuilder = new Mesh.Builder(this.depthData.Data);
            this.progress.Report(1);
            if (this.dispersionCB.Checked)
            {
                this.depthData.FinalizeDispersion();
                meshBuilder.AddDispersions(this.depthData.Dispersion);
            }

            this.progress.Report(1);
            if (this.coloredMeshRB.Checked)
            {
                meshBuilder.AddRealColors(this.colorData.Data);
            }
            else if (this.scaledMeshRB.Checked)
            {
                meshBuilder.AddComputedColors(this.depthData.Dispersion);
            }

            this.progress.Report(1);
            this.meshWriter.WriteMesh(meshBuilder.Build(), this.path + "\\3Dscan" + this.counter + ".obj", false, true, string.Empty);
            this.progress.Report(1);
        }

        /// <summary>
        /// Generates mesh and appends it to a single file.
        /// </summary>
        /// <param name="data">Depth data.</param>
        /// <param name="framesCounter">ID of the read frame.</param>
        /// <param name="filesCounter">ID of the file.</param>
        private void GenerateAllFrames(ushort[] data, int framesCounter, int filesCounter)
        {
            var meshBuilder = new Mesh.Builder(data);
            lock (this)
            {
                this.meshWriter.WriteMesh(meshBuilder.Build(), this.path + "\\AllFrames" + filesCounter + ".obj", true, true, "Frame " + framesCounter);
            }
        }

        /// <summary>
        /// Disables or enables controls.
        /// </summary>
        /// <param name="state">Determine if controls should be enabled or disabled.</param>
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
        /// Binds controls to kinect attributes.
        /// </summary>
        private void DataBinding()
        {
            this.minDepthTB.DataBindings.Add("Value", this.kinect.KinectAttributes, "MinDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthTB.DataBindings.Add("Value", this.kinect.KinectAttributes, "MaxDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.minDepthValueTB.DataBindings.Add("Text", this.kinect.KinectAttributes, "MinDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthValueTB.DataBindings.Add("Text", this.kinect.KinectAttributes, "MaxDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationValueTB.DataBindings.Add("Text", this.kinect.KinectAttributes, "Interpolation", false, DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationTB.DataBindings.Add("Value", this.kinect.KinectAttributes, "Interpolation", false, DataSourceUpdateMode.OnPropertyChanged);
            this.scaledMeshRB.CheckedChanged += (sender, e) =>
            {
                if (this.scaledMeshRB.Checked)
                {
                    this.dispersionCB.Checked = true;
                    this.dispersionCB.Enabled = false;
                }
                else
                {
                    this.dispersionCB.Checked = false;
                    this.dispersionCB.Enabled = true;
                }
            };

            this.imageControl.image.Source = this.writeableBitmap;
            this.viewport.AddPointCloud(this.pointCloud);
        }

        /// <summary>
        /// Event that triggers when generate button is clicked. Shows folder dialog and clears data.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event arguments.</param>
        private void GenerateBT_Click(object sender, EventArgs e)
        {
            if (this.path.Equals(string.Empty))
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.ShowDialog();
                this.path = folderBrowserDialog.SelectedPath;
            }

            if (!this.path.Equals(string.Empty))
            {
                this.depthData.ClearData();
                this.colorData.ClearData();
                this.progressBar.Value = 0;
                this.progressBar.Maximum = this.kinect.KinectAttributes.Interpolation + 5;

                this.progressBar.Show();
                this.stopBT.Enabled = true;
                this.statusLB.Text = "Probíhá generování meshe!";
                this.DisableControls(true);
                this.isGenerating = true;
            }
        }

        /// <summary>
        /// Event that triggers when preview button is clicked. Starts previewing.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PreviewBT_Click(object sender, EventArgs e)
        {
            this.statusLB.Text = "Probíhá vytvoření náhledu!";
            this.isPreviewing = true;
        }

        /// <summary>
        /// Event that triggers when tabs are changed. Sets tab index.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tabIndex = (sender as TabControl).SelectedIndex;
        }

        /// <summary>
        /// Event that triggers when stop button is clicked. Stops the generation of a mesh.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event arguments.</param>
        private void StopBT_Click(object sender, EventArgs e)
        {
            this.isGenerating = false;
            this.statusLB.Text = "Generování zastaveno!";
            this.DisableControls(false);
            this.progressBar.Hide();
            this.stopBT.Enabled = false;
        }
    }
}
