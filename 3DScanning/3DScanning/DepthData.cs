using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DScanning
{
    class DepthData
    {
        /// <summary>
        /// 
        /// </summary>
        private CircularStack<ushort[]> depthFramesStack;

        /// <summary>
        /// Kinect attributes
        /// </summary>
        protected KinectAttributes kinectAttributes;

        /// <summary>
        /// 
        /// </summary>
        protected FrameDescription depthFrameDescription;

        /// <summary>
        /// 
        /// </summary>
        public CircularStack<ushort[]> Data
        {
            get
            {
                return depthFramesStack;
            }
        }

        public ushort[] LastData
        {
            get
            {
                return depthFramesStack.Last();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DepthData()
        {
            Kinect kinect = Kinect.GetInstance();
            this.depthFramesStack = new CircularStack<ushort[]>();
            this.kinectAttributes = kinect.KinectAttributes;
            this.depthFrameDescription = kinect.DepthFrameDescription;
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddDepthData(MultiSourceFrame multiSourceFrame)
        {
            ushort[] depthData;
            DepthFrameReference depthFrameReference = multiSourceFrame.DepthFrameReference;
            using (DepthFrame depthFrame = depthFrameReference.AcquireFrame())
            {
                if (depthFrame == null)
                    return;

                depthData = new ushort[this.depthFrameDescription.LengthInPixels];
                depthFrame.CopyFrameDataToArray(depthData);
                this.depthFramesStack.Push(depthData);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public ushort[] InterpolateDepths()
        {
            ushort[] interpolatedDepths = new ushort[this.depthFrameDescription.LengthInPixels];
            for (int i = 0; i < interpolatedDepths.Length; i++)
            {
                int averageDepth = Utility.GetAverageValue(i, this.depthFramesStack.GetRange(kinectAttributes.Interpolation));
                interpolatedDepths[i] = (ushort)averageDepth;
            }

            return interpolatedDepths;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        public ushort[] ReduceDepthRange(ushort[] depthData)
        {
            ushort[] reducedDepth = new ushort[depthData.Length];
            for (int i = 0; i < depthData.Length; i++)
            {
                if (depthData[i] < this.kinectAttributes.MaxDepth && depthData[i] > this.kinectAttributes.MinDepth)
                {
                    reducedDepth[i] = depthData[i];
                }
            }
            return reducedDepth;
        }
    }
}
