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
            this.kinect = new KinectDepthSensor(reader_FrameArrived);
            this.kinectAttributes = this.kinect.Attributes;
            this.depthFrameLength = this.kinect.Description.LengthInPixels;
            this.csPoints = new CameraSpacePoint[this.depthFrameLength];
            InitializeComponent();
        }

        /// <summary>
        /// Handles the depth frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private unsafe void reader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    if (!this.rendering)
                    {
                        using (KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                        {
                            if (this.generating)
                            {
                                //Getting depths from ImageBuffer
                                this.framesCounter++;
                                ushort* depthData = (ushort*)depthBuffer.UnderlyingBuffer;
                                ushort[] depthArray = new ushort[this.depthFrameLength];

                                for (int i = 0; i < depthArray.Length; i++)
                                {
                                    depthArray[i] = depthData[i];
                                }

                                this.framesList.Add(depthArray);

                                //When enough frames are stored interpolation begins
                                if (framesCounter == this.kinectAttributes.Interpolation)
                                {
                                    ushort[] interpolatedDepths = this.interpolateFrames(this.framesList);
                                   
                                    //When you scan scenery without sharps changeovers you can use commented line below, that will lower diferences between depth of pixels more than the uncommented one
                                    //this.kinect.Mapper.MapDepthFrameToCameraSpace(this.smoothEntropyValues(interpolatedDepths), this.csPoints);
                                    this.kinect.Mapper.MapDepthFrameToCameraSpace(interpolatedDepths, this.csPoints);

                                    this.transformedPoints = this.cameraToWorldTransfer(this.kinect.Description.Width, this.kinect.Description.Height);

                                    this.generateMesh(this.transformedPoints);
                                    
                                }
                            }
                            else
                            {
                                this.rendering = true;
                                this.kinect.Mapper.MapDepthFrameToCameraSpaceUsingIntPtr(
                                           depthBuffer.UnderlyingBuffer,
                                           depthBuffer.Size,
                                           this.csPoints);
                                this.transformedPoints = this.cameraToWorldTransfer(this.kinect.Description.Width, this.kinect.Description.Height);
                                this.renderPointCloud(this.transformedPoints);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Transfers points from camera view into world coordinates
        /// </summary>
        /// <param name="depthHeight"> Height of the depth frame </param>
        /// <param name="depthWidth"> Width of the depth frame </param>
        /// <returns> Array of transformed points </returns>
        private CameraSpacePoint[] cameraToWorldTransfer(int depthWidth, int depthHeight)
        {
            bool[] used = new bool[depthHeight * depthWidth];

            for (int x = 1; x < depthWidth - 1; x++)
                for (int y = 1; y < depthHeight - 1; y++)
                {
                    if (isOK(depthWidth * y + x - 1)
                        && isOK(depthWidth * y + x)
                        && isOK(depthWidth * y + x + 1)
                        && isOK(depthWidth * (y + 1) + x - 1)
                        && isOK(depthWidth * (y + 1) + x)
                        && isOK(depthWidth * (y + 1) + x + 1)
                        && isOK(depthWidth * (y - 1) + x - 1)
                        && isOK(depthWidth * (y - 1) + x)
                        && isOK(depthWidth * (y - 1) + x + 1)
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
        private bool isOK(int index)
        {
            if (!double.IsInfinity(csPoints[index].X))
                if (!double.IsInfinity(csPoints[index].Y))
                    if (!double.IsInfinity(csPoints[index].Z))
                        return (true);
            return (false);
        }

        /// <summary>
        /// Smooths diference of nearby pixels
        /// </summary>
        /// <param name="originalValues"> Depth array </param>
        ///<returns> Array of smoothed depths </returns>
        private ushort[] smoothEntropyValues(ushort[] originalValues)
        {
            ushort[] smoothedValues = new ushort[originalValues.Length];

            double[] mask = new double[] { 0.25, 0.5, 0.25 };

            for (int bin = 1; bin < originalValues.Length - 1; bin++)
            {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++)
                {
                    smoothedValue += originalValues[bin -1 + i] * mask[i];
                }
                smoothedValues[bin] = (ushort)smoothedValue;
            }

            return smoothedValues;
        }

        /// <summary>
        /// Interpolates depth of every pixels in given frames
        /// </summary>
        /// <param name="framesList"> List of depth arrays </param>
        /// <returns> Array of interpolated depths </returns>
        private ushort[] interpolateFrames(List<ushort[]> framesList)
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
        private void generateMesh(CameraSpacePoint[] points)
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

            this.statusLB.Text = "Mesh byl vygenerován a uložen!";
            this.generating = false;
            this.kinect.stop();
        }

        /// <summary>
        /// Renders point cloud from given points
        /// </summary>
        /// <param name="points"> Array of points </param>
        private void renderPointCloud(CameraSpacePoint[] points)
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
            this.statusLB.Text = "Náhled byl zobrazen!";
            this.rendering = false;
            this.kinect.stop();
        }

        /// <summary>
        /// Event that triggers when generate button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void generateBT_Click(object sender, EventArgs e)
        {
            this.statusLB.Text = "Probíhá generování meshe!";
            this.generating = true;
            this.framesCounter = 0;
            this.framesList.Clear();
            this.kinect.start();
            
        }

        /// <summary>
        /// Event that triggers when preview button is clicked
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void previewBT_Click(object sender, EventArgs e)
        {
            this.statusLB.Text = "Probíhá vytvoření náhledu!";
            this.kinect.start();          
        }
    }
}
