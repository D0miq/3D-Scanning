using Microsoft.Kinect;
using System.Globalization;
using System.IO;
using System;

namespace _3DScanning
{
    class MultiMeshesGenerator : AMeshGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public MultiMeshesGenerator(string path) : base(path+"\\allFrames.obj") { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiSourceFrame"></param>
        protected override void ProcessFrame(MultiSourceFrame multiSourceFrame)
        {
            ushort[] depthArray;
            DepthFrameReference depthFrameReference = multiSourceFrame.DepthFrameReference;
            using (DepthFrame depthFrame = depthFrameReference.AcquireFrame())
            {
                if (depthFrame == null)
                    return;

                this.framesCounter++;
                depthArray = new ushort[this.depthFrameDescription.LengthInPixels];
                depthFrame.CopyFrameDataToArray(depthArray);
            }
            
            this.ReduceDepthRange(depthArray);
            this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(depthArray, this.csPoints);
            CameraSpacePoint[] transformedPoints = this.CameraToWorldTransfer(this.depthFrameDescription.Width, this.depthFrameDescription.Height);
            this.GenerateMesh(transformedPoints);
            if (framesCounter == this.kinectAttributes.Interpolation)
            {
                this.streamWriter.Close();
                this.OnFinishedChanged(false);
                this.kinect.RemoveEventHandler(this.Reader_FrameArrived);
            }    
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        protected override void GenerateMesh(CameraSpacePoint[] points)
        {
            this.streamWriter.WriteLine("\n# Frame cislo {0}",this.framesCounter);
            foreach (CameraSpacePoint point in points)
            {
                this.streamWriter.WriteLine("v {0} {1} {2}", point.X.ToString(this.numberFormatInfo), point.Y.ToString(this.numberFormatInfo), point.Z.ToString(this.numberFormatInfo));
            }
        }
    }
}
