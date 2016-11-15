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
            CircularStack<ushort[]> interpolatingFrames = this.depthFramesStack.GetRange(kinectAttributes.Interpolation);
            Parallel.For(0, interpolatedDepths.Length, index => interpolatedDepths[index] = (ushort) Utility.GetAverageValue(index, interpolatingFrames));
            return interpolatedDepths;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        public ushort[] ReduceDepthRange(ushort[] depthData)
        {
            ushort[] reducedDepth = new ushort[depthData.Length];
            Parallel.For(0, depthData.Length, index =>
            {
                if (depthData[index] < this.kinectAttributes.MaxDepth && depthData[index] > this.kinectAttributes.MinDepth)
                {
                    reducedDepth[index] = depthData[index];
                }
            });
            return reducedDepth;
        }
    }
}
