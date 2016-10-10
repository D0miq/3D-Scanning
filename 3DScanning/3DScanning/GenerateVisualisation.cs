using System.Collections.Generic;
using Microsoft.Kinect;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace _3DScanning
{
    class GenerateVisualisation : APointCloudVisualisation
    {
        /// <summary>
        /// Limit for diference between maximal and minimal depth of the pixel
        /// </summary>
        private const int LIMIT = 200;

        /// <summary>
        /// List of frames
        /// </summary>
        private List<ushort[]> framesList = new List<ushort[]>();

        /// <summary>
        /// Counter of scanned frames
        /// </summary>
        private int framesCounter = 0;

        private ProgressBar progressBar;

        private Label statusText;

        public GenerateVisualisation(ProgressBar progressBar, Label statusText)
        {
            this.progressBar = progressBar;
            this.statusText = statusText;
        }

        public override void Reader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    using (KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                    {
                        ushort[] depthArray = GetDepthFromBuffer(depthBuffer);
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

                            this.statusText.Text = "Mesh byl vygenerován a uložen!";
                            this.progressBar.Hide();
                            this.progressBar.Value = 0;
                            this.framesCounter = 0;
                            //this.DisableControls(false);
                            this.kinect.Stop();

                        }
                    }
                }
            }
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
                int zeroCounter = 0;
                foreach (ushort[] depthArray in framesList)
                {
                    if (depthArray[i] == 0)
                    {
                        zeroCounter++;
                    }else
                    {
                        averageDepth = averageDepth + depthArray[i];
                    }
                }
                if(averageDepth != 0)
                {
                    averageDepth = averageDepth / (framesList.Count - zeroCounter);
                }
                finalDepthArray[i] = (ushort)averageDepth;
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
    }
}
