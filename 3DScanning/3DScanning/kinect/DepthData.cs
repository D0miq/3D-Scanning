namespace _3DScanning
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Kinect;

    /// <summary>
    /// An instance of the <see cref="DepthData"/> class collects depth data, their dispersion
    /// and computes their average.
    /// </summary>
    /// <seealso cref="ColorData"/>
    /// <seealso cref="Kinect"/>
    /// <seealso cref="KinectAttributes"/>
    public class DepthData
    {
        /// <summary>
        /// Average depth data
        /// </summary>
        private ushort[] depthData;

        /// <summary>
        /// The last available depth frame
        /// </summary>
        private ushort[] lastDepthData;

        /// <summary>
        /// The dispersion of depth data
        /// </summary>
        private float[] depthDataDispersion;

        /// <summary>
        /// Kinect attributes
        /// </summary>
        private KinectAttributes kinectAttributes;

        /// <summary>
        /// The description of depth frames
        /// </summary>
        private FrameDescription depthFrameDescription;

        /// <summary>
        /// The number of processed frames
        /// </summary>
        private int framesCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthData"/> class.
        /// </summary>
        public DepthData()
        {
            Kinect kinect = Kinect.INSTANCE;
            this.kinectAttributes = kinect.KinectAttributes;
            this.depthFrameDescription = kinect.DepthFrameDescription;
            this.ClearData();
        }

        /// <summary>
        /// Gets the average depth data <see cref="depthData"/>.
        /// </summary>
        public ushort[] Data
        {
            get
            {
                return this.depthData;
            }
        }

        /// <summary>
        /// Gets the last frame <see cref="lastDepthData"/>.
        /// </summary>
        public ushort[] LastFrame
        {
            get
            {
                return this.lastDepthData;
            }
        }

        /// <summary>
        /// Gets the dispersion of depth data <see cref="depthDataDispersion"/>.
        /// </summary>
        public float[] Dispersion
        {
            get
            {
                return this.depthDataDispersion;
            }
        }

        /// <summary>
        /// Gets the number of processed frames <see cref="framesCounter"/>.
        /// </summary>
        public int FramesCount
        {
            get
            {
                return this.framesCounter;
            }
        }

        /// <summary>
        /// Gets a depth frame from <paramref name="multiSourceFrame"/> and sets its data into <see cref="lastDepthData"/>.
        /// </summary>
        /// <param name="multiSourceFrame">Holds frames acquired from a kinnect sensor</param>
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
                depthFrame.CopyFrameDataToArray(this.lastDepthData);
            }
        }

        /// <summary>
        /// Clears total data and the dispersion.
        /// </summary>
        public void ClearData()
        {
            this.depthData = new ushort[this.depthFrameDescription.LengthInPixels];
            this.depthDataDispersion = new float[this.depthFrameDescription.LengthInPixels];
            this.framesCounter = 0;
        }

        /// <summary>
        /// Reduces range of usable depth.
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
        /// Finalyzes counting of the dispersion.
        /// </summary>
        public void FinalizeDispersion()
        {
            Parallel.For(0, this.depthDataDispersion.Length, index =>
            {
                this.depthDataDispersion[index] = (this.depthDataDispersion[index] / this.framesCounter) - (float)Math.Pow(this.depthData[index], 2);
            });
        }

        /// <summary>
        /// Adds a last frame into <see cref="depthData"/> and dispersion.
        /// </summary>
        public void AddLastFrame()
        {
            if (this.lastDepthData != null)
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
}