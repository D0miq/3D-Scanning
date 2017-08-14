using Microsoft.Kinect;
using System;
using System.Threading.Tasks;

namespace _3DScanning
{
    class DepthData
    {
        /// <summary>
        /// Total depth data
        /// </summary>
        private ushort[] depthData;

        /// <summary>
        /// Last available depth frame
        /// </summary>
        private ushort[] lastDepthData;

        /// <summary>
        /// Dispersion of depth data
        /// </summary>
        private float[] depthDataDispersion;

        /// <summary>
        /// Kinect attributes
        /// </summary>
        private KinectAttributes kinectAttributes;

        /// <summary>
        /// Description of depth frames
        /// </summary>
        private FrameDescription depthFrameDescription;

        /// <summary>
        /// Number of processed frames
        /// </summary>
        private int framesCounter;

        /// <summary>
        /// Getter for total depth data
        /// </summary>
        public ushort[] Data
        {
            get
            {
                return depthData;
            }
        }

        /// <summary>
        /// Getter for last depth frame
        /// </summary>
        public ushort[] LastFrame
        {
            get
            {
                return lastDepthData;
            }
        }

        /// <summary>
        /// Getter for depth dispersion
        /// </summary>
        public float[] Dispersion
        {
            get
            {
                return depthDataDispersion;
            }
        }

        /// <summary>
        /// Getter for frames counter
        /// </summary>
        public int FramesCount
        {
            get
            {
                return framesCounter;
            }
        }

        /// <summary>
        /// Initialize atributes and creates new instance
        /// </summary>
        public DepthData()
        {
            Kinect kinect = Kinect.GetInstance();
            this.kinectAttributes = kinect.KinectAttributes;
            this.depthFrameDescription = kinect.DepthFrameDescription;
            this.ClearData();
        }

        /// <summary>
        /// Gets depth frame from multiSourceFrame and sets its data into lastDepthData
        /// </summary>
        /// <param name="multiSourceFrame">Holds frames acquired from kinnect sensor</param>
        public void SetLastFrame(MultiSourceFrame multiSourceFrame)
        {
            DepthFrameReference depthFrameReference = multiSourceFrame.DepthFrameReference;
            using (DepthFrame depthFrame = depthFrameReference.AcquireFrame())
            {
                if (depthFrame == null)
                {
                    return;
                }

                this.lastDepthData = new ushort[this.depthFrameDescription.LengthInPixels];
                depthFrame.CopyFrameDataToArray(lastDepthData);
            }
        }

        /// <summary>
        /// Clears total data and dispersion
        /// </summary>
        public void ClearData()
        {
            this.depthData = new ushort[this.depthFrameDescription.LengthInPixels];
            this.depthDataDispersion = new float[this.depthFrameDescription.LengthInPixels];
            this.framesCounter = 0;
        }

        /// <summary>
        /// Reduces range of total depth 
        /// </summary>
        public void ReduceRange()
        {
            Parallel.For(0, this.depthData.Length, index =>
            {
                if (this.depthData[index] > this.kinectAttributes.MaxDepth || this.depthData[index] < this.kinectAttributes.MinDepth)
                {
                    this.depthData[index] = 0;
                }
            });
        }

        /// <summary>
        /// Finalyzes counting of dispersion
        /// </summary>
        public void FinalizeDispersion()
        {
            Parallel.For(0, this.depthDataDispersion.Length, index =>
            {
                this.depthDataDispersion[index] = this.depthDataDispersion[index] / this.framesCounter - (float)Math.Pow(this.depthData[index], 2);
            });
        }

        /// <summary>
        /// Adds last frame into total and dispersion
        /// </summary>
        public void AddLastFrame()
        {
            this.framesCounter++;
            Parallel.For(0, this.lastDepthData.Length, index =>
            {
                this.depthDataDispersion[index] += (float)Math.Pow(this.lastDepthData[index], 2);
                if (this.lastDepthData[index] != 0)
                {
                    if (this.depthData[index] == 0)
                    {
                        this.depthData[index] = this.lastDepthData[index];
                    }
                    else
                    {
                        this.depthData[index] = (ushort)((this.depthData[index] + this.lastDepthData[index]) / 2);
                    }
                }
            });
        }
    }
}