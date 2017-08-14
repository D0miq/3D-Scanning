using System;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Kinect;
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

        /// <summary>
        /// 
        /// </summary>
        private string path = "";

        /// <summary>
        /// 
        /// </summary>
        private bool generating = false;

        /// <summary>
        /// 
        /// </summary>
        private bool previewing = false;

        /// <summary>
        /// 
        /// </summary>
        private int tabIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        private Kinect kinect;

        /// <summary>
        /// 
        /// </summary>
        private DepthData depthData;

        /// <summary>
        /// 
        /// </summary>
        private ColorData colorData;

        /// <summary>
        /// 
        /// </summary>
        private int counter = 0;

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
            this.visualisation = new Visualisation(this.depthData);
            this.writeableBitmap = new WriteableBitmap(kinect.DepthFrameDescription.Width, kinect.DepthFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray8, null);
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
            this.minDepthValueTB.DataBindings.Add("Text", this.kinect.KinectAttributes, "MinDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthValueTB.DataBindings.Add("Text", this.kinect.KinectAttributes, "MaxDepth", false, DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationValueTB.DataBindings.Add("Text", this.kinect.KinectAttributes, "Interpolation", false, DataSourceUpdateMode.OnPropertyChanged);
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
        public async void Reader_FrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            try
            {
                MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
                if (multiSourceFrame != null)
                {
                    this.depthData.SetLastFrame(multiSourceFrame);

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
                    if (this.generating)
                    {
                        this.depthData.AddLastFrame();
                        this.colorData.AddFrame(multiSourceFrame);
                        if (this.kinect.KinectAttributes.Interpolation <= this.depthData.FramesCount && this.kinect.KinectAttributes.Interpolation <= this.colorData.FramesCount)
                        {
                            this.generating = false;
                            this.stopBT.Enabled = false;
                            var progress = new Progress<int>(v =>
                            {
                                progressBar.Increment(v);
                            });

                            await Task.Run(() => DoWork(progress));

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
        /// 
        /// </summary>
        /// <param name="progress"></param>
        public void DoWork(IProgress<int> progress)
        {
            Parallel.Invoke(() =>
            {
                if (this.generateAllCB.Checked)
                {
                    progress.Report(1);
            //        this.visualisation.CreatePointsFromAllFrames(path, this.kinect.KinectAttributes.Interpolation, this.counter);
                    progress.Report(1);
                }
            },() =>
            {
                progress.Report(1);
                this.depthData.ReduceRange();
                var meshBuilder = new Mesh.Builder(this.depthData.Data);
                progress.Report(1);
                if (dispersionCB.Checked)
                {
                    this.depthData.FinalizeDispersion();
                    meshBuilder.AddDispersions(this.depthData.Dispersion);
                }
                progress.Report(1);
                if (this.coloredMeshRB.Checked)
                {
                    meshBuilder.AddRealColors(this.colorData.Data);
                } else if (this.scaledMeshRB.Checked)
                {
                    meshBuilder.AddComputedColors(this.depthData.Dispersion);
                }
                progress.Report(1);
                meshBuilder.Build().GenerateMesh(path + "\\3Dscan" + counter + ".obj", false, true, "");
                progress.Report(1);
            });
        }

        /// <summary>
        /// Event that triggers when generate button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void GenerateBT_Click(object sender, EventArgs e)
        {
            if (path.Equals(""))
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.ShowDialog();
                this.path = folderBrowserDialog.SelectedPath;
            }
            if (!path.Equals(""))
            {
                this.depthData.ClearData();
                this.colorData.ClearData();
                this.progressBar.Maximum = 7;
                if (generateAllCB.Checked)
                {
//                    this.visualisation.GenerateMesh(path + "\\allFrames.obj", null , false, false, "");
                    this.progressBar.Maximum = 9;
                }
                this.progressBar.Show();
                this.stopBT.Enabled = true;
                this.statusLB.Text = "Probíhá generování meshe!";
                this.DisableControls(true);
                this.generating = true;
            }
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
        /// Event that triggers when tabs are changed
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tabIndex = (sender as TabControl).SelectedIndex;
        }


        private void StopBT_Click(object sender, EventArgs e)
        {
            this.generating = false;
            this.statusLB.Text = "Generování zastaveno!";
            this.DisableControls(false);
            this.progressBar.Hide();
            this.stopBT.Enabled = false;
        }
    }
}
