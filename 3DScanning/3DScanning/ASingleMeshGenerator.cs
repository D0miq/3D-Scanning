using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Forms;
using System;

namespace _3DScanning
{
    abstract class ASingleMeshGenerator : AMeshGenerator
    {
        /// <summary>
        /// List of frames
        /// </summary>
        protected List<ushort[]> depthFramesList;

        /// <summary>
        /// 
        /// </summary>
        protected ushort[] interpolatedDepths;

        /// <summary>
        /// 
        /// </summary>
        protected ProgressBar progressBar;

        /// <summary>
        /// 
        /// </summary>
        protected Label statusText;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressBar"></param>
        /// <param name="statusText"></param>
        /// <param name="path"></param>
        public ASingleMeshGenerator(ProgressBar progressBar, Label statusText, string path) : base(path+"\\points.obj")
        {
            this.progressBar = progressBar;
            this.statusText = statusText;
            this.depthFramesList = new List<ushort[]>();
            this.interpolatedDepths = new ushort[this.depthFrameDescription.LengthInPixels];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiSourceFrame"></param>
        protected override void ProcessFrame(MultiSourceFrame multiSourceFrame)
        {
            DepthFrameReference depthFrameReference = multiSourceFrame.DepthFrameReference;
            using (DepthFrame depthFrame = depthFrameReference.AcquireFrame())
            {
                if (depthFrame == null)
                    return;
                
                ushort[] depthArray = new ushort[this.depthFrameDescription.LengthInPixels];
                depthFrame.CopyFrameDataToArray(depthArray);
                this.framesCounter++;
                this.progressBar.PerformStep();
                this.depthFramesList.Add(depthArray);
            }
            //When frames are stored interpolation begins
            if (this.framesCounter == this.kinectAttributes.Interpolation)
            {
                this.InterpolateFrames();
                this.ReduceDepthRange(this.interpolatedDepths);
                this.progressBar.PerformStep();
                this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(this.interpolatedDepths, this.csPoints);
                CameraSpacePoint[] transformedPoints = this.CameraToWorldTransfer(this.depthFrameDescription.Width, this.depthFrameDescription.Height);
                this.progressBar.PerformStep();
                this.GenerateMesh(transformedPoints);

                this.statusText.Text = "Mesh byl vygenerován a uložen!";
                this.progressBar.Hide();
                this.progressBar.Value = 0;
                this.depthFramesList.Clear();
                this.framesCounter = 0;
                this.OnFinishedChanged(false);
                this.kinect.RemoveEventHandler(this.Reader_FrameArrived);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void InterpolateFrames();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="framesList"></param>
        /// <returns></returns>
        protected int GetAverageValue(int index, List<ushort[]> framesList)
        {
            int averageValue = 0;
            int zeroCounter = 0;
            foreach (ushort[] array in framesList)
            {
                if (array[index] == 0)
                {
                    zeroCounter++;
                }
                averageValue += array[index];
            }
            if (averageValue != 0)
            {
                averageValue /= (framesList.Count - zeroCounter);
            }
            return averageValue;
        }
    }
}
