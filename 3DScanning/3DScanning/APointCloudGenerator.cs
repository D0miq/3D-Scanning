using System.Collections.Generic;
using Microsoft.Kinect;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System;
using System.Drawing;

namespace _3DScanning
{
    abstract class APointCloudGenerator : APointCloudVisualisation
    {
        /// <summary>
        /// Limit for diference between maximal and minimal depth of the pixel
        /// </summary>
        private const int LIMIT = 200;

        /// <summary>
        /// Counter of scanned frames
        /// </summary>
        private int framesCounter = 0;

        /// <summary>
        /// List of frames
        /// </summary>
        protected List<ushort[]> framesList;

        protected ushort[] interpolatedDepths;

        private ProgressBar progressBar;

        private Label statusText;

        public APointCloudGenerator(ProgressBar progressBar, Label statusText)
        {
            this.progressBar = progressBar;
            this.statusText = statusText;
            this.framesList = new List<ushort[]>();
            this.interpolatedDepths = new ushort[this.depthFrameLength];
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
                        //When frames are stored interpolation begins
                        if (this.framesCounter == this.kinectAttributes.Interpolation)
                        {
                            this.InterpolateFrames();
                            this.progressBar.PerformStep();
                            this.kinect.Mapper.MapDepthFrameToCameraSpace(this.interpolatedDepths, this.csPoints);
                            CameraSpacePoint[] transformedPoints = this.CameraToWorldTransfer(this.kinect.Description.Width, this.kinect.Description.Height);
                            this.progressBar.PerformStep();
                            this.GenerateMesh(transformedPoints);

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
        protected abstract void InterpolateFrames();

        protected abstract void GenerateMesh(CameraSpacePoint[] points);
        
        protected int GetAverageDepth(int index)
        {
            int averageDepth = 0;
            int zeroCounter = 0;
            foreach (ushort[] depthArray in this.framesList)
            {
                if (depthArray[index] == 0)
                {
                    zeroCounter++;
                }           
                averageDepth += depthArray[index];
            }
            if (averageDepth != 0)
            {
                averageDepth /= (this.framesList.Count - zeroCounter);
            }
            return averageDepth;
        }

        
    }
}
