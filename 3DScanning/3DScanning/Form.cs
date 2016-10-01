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
        /// <summary>
        /// Limit for diference between maximal and minimal depth of the pixel
        /// </summary>
        private const int LIMIT = 200;

        /// <summary>
        /// Model what is rendered on screen
        /// </summary>
        private RenderableObject model = new PointCloudRenderable();

        /// <summary>
        /// List of frames
        /// </summary>
        private List<ushort[]> framesList = new List<ushort[]>();

        /// <summary>
        /// Counter of scanned frames
        /// </summary>
        private int framesCounter = 0;

        /// <summary>
        /// Rendering state
        /// </summary>
        private bool rendering = false;

        /// <summary>
        /// Generating state
        /// </summary>
        private bool generating = false;

        /// <summary>
        /// Kinect sensor
        /// </summary>
        private KinectDepthSensor kinect;

        /// <summary>
        /// Transformed points on camera view
        /// </summary>
        private CameraSpacePoint[] csPoints;

        /// <summary>
        /// Triangles of the rendered model
        /// </summary>
        private List<int[]> triangles;

        /// <summary>
        /// 
        /// </summary>
        private CameraSpacePoint[] transformedPoints;

        /// <summary>
        /// Kinect attributes
        /// </summary>
        private KinectAttributes kinectAttributes;

        /// <summary>
        /// Number of pixel in depth frames
        /// </summary>
        private uint depthFrameLength;

        /// <summary>
        /// Creates form and inicialize kinect sensor, kinect attributes and camera space points
        /// </summary>
        public Form()
        {
            this.kinect = new KinectDepthSensor(this.Reader_FrameArrived);
            this.kinectAttributes = this.kinect.Attributes;
            this.depthFrameLength = this.kinect.Description.LengthInPixels;
            this.csPoints = new CameraSpacePoint[this.depthFrameLength];
            InitializeComponent();
            this.DataBinding();     
        }

        /// <summary>
        /// Destructor that frees resources
        /// </summary>
        ~Form()
        {
            if (this.kinect != null)
            {
                this.kinect.Dispose();
                this.kinect = null;
            }
            if(this.viewport != null)
            {
                this.viewport.Dispose();
                this.viewport = null;
            }  
            if(this.model != null)
            {
                this.model.Dispose();
                this.model = null;
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
            this.minDepthTB.DataBindings.Add("Value", this.kinectAttributes, "MinDepth", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthTB.DataBindings.Add("Value", this.kinectAttributes, "MaxDepth", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.minDepthValueLB.DataBindings.Add("Text", this.kinectAttributes, "MinDepth", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.maxDepthValueLB.DataBindings.Add("Text", this.kinectAttributes, "MaxDepth", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationValueLB.DataBindings.Add("Text", this.kinectAttributes, "Interpolation", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.interpolationTB.DataBindings.Add("Value", this.kinectAttributes, "Interpolation", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            this.progressBar.DataBindings.Add("Maximum", this.kinectAttributes, "Interpolation", false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
        }

        /// <summary>
        /// Handles the depth frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void Reader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    if (!this.rendering)
                    {
                        using (KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                        {
                            ushort[] depthArray = GetDepthFromBuffer(depthBuffer);

                            if (this.generating)
                            {
                                this.framesCounter++;
                                this.progressBar.PerformStep();
                                this.framesList.Add(depthArray);
                                //When enough frames are stored interpolation begins
                                if (framesCounter == this.kinectAttributes.Interpolation)
                                {                                    
                                    ushort[] interpolatedDepths = this.InterpolateFrames(this.framesList);
                                    this.progressBar.PerformStep();
                                    this.kinect.Mapper.MapDepthFrameToCameraSpace(interpolatedDepths, this.csPoints);                                    
                                    this.transformedPoints = this.CameraToWorldTransfer(this.kinect.Description.Width, this.kinect.Description.Height);
                                    this.progressBar.PerformStep();
                                    this.GenerateMesh(this.transformedPoints);
                                    this.statusLB.Text = "Mesh byl vygenerován a uložen!";
                                    this.progressBar.Hide();
                                    this.progressBar.Value = 0;
                                    this.generating = false;
                                    this.DisableControls(false);
                                    this.kinect.Stop();
                                }
                            }
                            else
                            {
                                this.rendering = true;
                                this.kinect.Mapper.MapDepthFrameToCameraSpace(depthArray,this.csPoints);
                                this.transformedPoints = this.CameraToWorldTransfer(this.kinect.Description.Width, this.kinect.Description.Height);
                                this.RenderPointCloud(this.transformedPoints);
                                this.statusLB.Text = "Náhled byl zobrazen!";
                                this.rendering = false;
                                this.DisableControls(false);
                                this.kinect.Stop();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Getting depths from buffer
        /// </summary>
        /// <param name="depthBuffer">Buffer</param>
        /// <returns>Array with depths</returns>
        private unsafe ushort[] GetDepthFromBuffer(KinectBuffer depthBuffer)
        {
            ushort* depthData = (ushort*)depthBuffer.UnderlyingBuffer;
            ushort[] depthArray = new ushort[this.depthFrameLength];

            for (int i = 0; i < depthArray.Length; i++)
            {
                if (depthData[i] < this.kinectAttributes.MaxDepth && depthData[i] > this.kinectAttributes.MinDepth)
                {
                    depthArray[i] = depthData[i];
                }
            }
            return depthArray;
        }

        /// <summary>
        /// Transfers points from camera view into world coordinates
        /// </summary>
        /// <param name="depthHeight"> Height of the depth frame </param>
        /// <param name="depthWidth"> Width of the depth frame </param>
        /// <returns> Array of transformed points </returns>
        private CameraSpacePoint[] CameraToWorldTransfer(int depthWidth, int depthHeight)
        {
            bool[] used = new bool[depthHeight * depthWidth];

            for (int x = 1; x < depthWidth - 1; x++)
                for (int y = 1; y < depthHeight - 1; y++)
                {
                    if (IsOK(depthWidth * y + x - 1)
                        && IsOK(depthWidth * y + x)
                        && IsOK(depthWidth * y + x + 1)
                        && IsOK(depthWidth * (y + 1) + x - 1)
                        && IsOK(depthWidth * (y + 1) + x)
                        && IsOK(depthWidth * (y + 1) + x + 1)
                        && IsOK(depthWidth * (y - 1) + x - 1)
                        && IsOK(depthWidth * (y - 1) + x)
                        && IsOK(depthWidth * (y - 1) + x + 1)
                        ) used[depthWidth * y + x] = true;
                }

            int[] indices = new int[depthWidth * depthHeight];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = -1;

            int freeIndex = 0;

            this.triangles = new List<int[]>();

            for (int x = 0; x < depthWidth - 1; x++)
                for (int y = 0; y < depthHeight - 1; y++)
                {
                    if (used[depthWidth * y + x]
                        && used[depthWidth * y + x + 1]
                        && used[depthWidth * (y + 1) + x]
                        && used[depthWidth * (y + 1) + x + 1])
                    {
                        int i1 = indices[depthWidth * y + x];
                        if (i1 < 0)
                        {
                            i1 = freeIndex;
                            freeIndex++;
                            indices[depthWidth * y + x] = i1;
                        }
                        int i2 = indices[depthWidth * y + x + 1];
                        if (i2 < 0)
                        {
                            i2 = freeIndex;
                            freeIndex++;
                            indices[depthWidth * y + x + 1] = i2;
                        }
                        int i3 = indices[depthWidth * (y + 1) + x];
                        if (i3 < 0)
                        {
                            i3 = freeIndex;
                            freeIndex++;
                            indices[depthWidth * (y + 1) + x] = i3;
                        }
                        int i4 = indices[depthWidth * (y + 1) + x + 1];
                        if (i4 < 0)
                        {
                            i4 = freeIndex;
                            freeIndex++;
                            indices[depthWidth * (y + 1) + x + 1] = i4;
                        }
                        triangles.Add(new int[] { i1 + 1, i2 + 1, i3 + 1 });
                        triangles.Add(new int[] { i2 + 1, i4 + 1, i3 + 1 });
                    }
                }

            CameraSpacePoint[] reordered = new CameraSpacePoint[freeIndex];
            for (int i = 0; i < (depthWidth * depthHeight); i++)
            {
                if (indices[i] > 0)
                    reordered[indices[i]] = csPoints[i];
            }

            return reordered;
        }

        /// <summary>
        /// Checks coordinates
        /// </summary>
        /// <param name="index"> Index of the pixel </param>
        /// <returns> Data evaluation </returns>
        private bool IsOK(int index)
        {
            if (!double.IsInfinity(csPoints[index].X))
                if (!double.IsInfinity(csPoints[index].Y))
                    if (!double.IsInfinity(csPoints[index].Z))
                        return (true);
            return (false);
        }

        /// <summary>
        /// Interpolates depth of every pixels in given frames
        /// </summary>
        /// <param name="framesList"> List of depth arrays </param>
        /// <returns> Array of interpolated depths </returns>
        private ushort[] InterpolateFrames(List<ushort[]> framesList)
        {
            ushort[] finalDepthArray = new ushort[framesList[0].Length];
            for (int i = 0; i < finalDepthArray.Length; i++)
            {
                int averageDepth = 0;
                int max = 0;
                int min = int.MaxValue;
                foreach (ushort[] depthArray in framesList)
                {
                    if(max < depthArray[i]) { max = depthArray[i]; }
                    if(min > depthArray[i]) { min = depthArray[i]; }
                    averageDepth = averageDepth + depthArray[i];
                    
                }
                //Checks diference between max and min depth values
                if ((max - min) < LIMIT)
                {
                    averageDepth = averageDepth / framesList.Count;
                    finalDepthArray[i] = (ushort) averageDepth;
                }
                else
                {
                    finalDepthArray[i] = (ushort) min;
                }         
            }
            return finalDepthArray;
        }

        /// <summary>
        /// Creates mesh from given points
        /// </summary>
        /// <param name="points"> Array of points </param>
        private void GenerateMesh(CameraSpacePoint[] points)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            StreamWriter sw = new StreamWriter("points.obj");
            for (int i = 0; i < points.Length; i++)
            {
                CameraSpacePoint point = points[i];
                sw.WriteLine("v {0} {1} {2}", point.X.ToString(nfi), point.Y.ToString(nfi), point.Z.ToString(nfi));
            }

            for (int i = 0; i < this.triangles.Count; i++)
            {
                sw.WriteLine("f {0} {1} {2}", this.triangles[i][0], this.triangles[i][1], this.triangles[i][2]);
            }
            sw.Close();
        }

        /// <summary>
        /// Renders point cloud from given points
        /// </summary>
        /// <param name="points"> Array of points </param>
        private void RenderPointCloud(CameraSpacePoint[] points)
        {
            List<Vector3> pointList = new List<Vector3>();
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 v = new Vector3();
                v.X = points[i].X;
                v.Y = points[i].Y;
                v.Z = points[i].Z;
                pointList.Add(v);
            }
            PointCloud pc = PointCloud.FromVector3List(pointList);
            this.model.PointCloud = pc;
            this.viewport.GLrender.ReplaceRenderableObject(this.model, false);
        }

        /// <summary>
        /// Event that triggers when generate button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void GenerateBT_Click(object sender, EventArgs e)
        {
            this.statusLB.Text = "Probíhá generování meshe!";
            this.DisableControls(true);
            this.generating = true;
            this.progressBar.Show();
            this.framesCounter = 0;
            this.framesList.Clear();
            this.kinect.Start();
            
        }

        /// <summary>
        /// Event that triggers when preview button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void PreviewBT_Click(object sender, EventArgs e)
        {
            this.statusLB.Text = "Probíhá vytvoření náhledu!";
            this.DisableControls(true);
            this.kinect.Start();          
        }
    }
}
