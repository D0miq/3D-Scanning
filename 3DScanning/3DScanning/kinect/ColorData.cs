namespace _3DScanning
{
    using System.Threading.Tasks;
    using Microsoft.Kinect;

    /// <summary>
    /// An instance of the <see cref="ColorData"/> class collects color data
    /// and computes their average.
    /// </summary>
    /// <seealso cref="DepthData"/>
    /// <seealso cref="Kinect"/>
    /// <seealso cref="KinectAttributes"/>
    public class ColorData
    {
        /// <summary>
        /// The description of color frames
        /// </summary>
        private FrameDescription colorFrameDescription;

        /// <summary>
        /// Average color data.
        /// </summary>
        private byte[] colorData;

        /// <summary>
        /// Attributes of a kinnect sensor
        /// </summary>
        private KinectAttributes kinectAttributes;

        /// <summary>
        /// The number of processed frames
        /// </summary>
        private int framesCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorData"/> class.
        /// </summary>
        public ColorData()
        {
            Kinect kinect = Kinect.INSTANCE;
            this.colorFrameDescription = kinect.ColorFrameDescription;
            this.kinectAttributes = kinect.KinectAttributes;
            this.ClearData();
        }

        /// <summary>
        /// Gets the average color data <see cref="colorData"/>.
        /// </summary>
        public byte[] Data
        {
            get
            {
                return this.colorData;
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
        /// Gets a color frame from <paramref name="multiSourceFrame"/> and adds its data into <see cref="colorData"/>.
        /// </summary>
        /// <param name="multiSourceFrame">Holds frames acquired from a kinnect sensor.</param>
        public void AddFrame(MultiSourceFrame multiSourceFrame)
        {
            byte[] tempColorData;
            ColorFrameReference colorFrameReference = multiSourceFrame.ColorFrameReference;
            using (ColorFrame colorFrame = colorFrameReference.AcquireFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                tempColorData = new byte[this.colorFrameDescription.LengthInPixels * Utility.BYTES_PER_PIXEL];
                colorFrame.CopyConvertedFrameDataToArray(tempColorData, ColorImageFormat.Rgba);
                if (this.framesCounter != 0)
                {
                    Parallel.For(0, this.colorData.Length, index => this.colorData[index] = (byte)((this.colorData[index] + tempColorData[index]) / 2));
                }
                else
                {
                    this.colorData = tempColorData;
                }

                this.framesCounter++;
            }
        }

        /// <summary>
        /// Clears color data and number of processed frames.
        /// </summary>
        public void ClearData()
        {
            this.colorData = new byte[this.colorFrameDescription.LengthInPixels * Utility.BYTES_PER_PIXEL];
            this.framesCounter = 0;
        }
    }
}