using Microsoft.Kinect;
using System.Threading.Tasks;

namespace _3DScanning
{
    class ColorData
    {
        /// <summary>
        /// Total color data
        /// </summary>
        private byte[] colorData;

        /// <summary>
        /// Description of color frames
        /// </summary>
        protected FrameDescription colorFrameDescription;

        /// <summary>
        /// Attributes of kinnect sensor
        /// </summary>
        private KinectAttributes kinectAttributes;

        /// <summary>
        /// Number of processed frames
        /// </summary>
        private int framesCounter;

        /// <summary>
        /// Getter for total data
        /// </summary>
        public byte[] Data
        {
            get
            {
                return colorData;
            }
        }

        /// <summary>
        /// Getter for frames number
        /// </summary>
        public int FramesCount
        {
            get
            {
                return framesCounter;
            }
        }

        /// <summary>
        /// Inicializes attributes and creates new instances
        /// </summary>
        public ColorData()
        {
            Kinect kinect = Kinect.GetInstance();
            this.colorFrameDescription = kinect.ColorFrameDescription;
            this.kinectAttributes = kinect.KinectAttributes;
            this.ClearData();
        }

        /// <summary>
        /// Gets color frame from multiSourceFrame and adds its data into colorData
        /// </summary>
        /// <param name="multiSourceFrame">Holds frames acquired from kinnect sensor</param>
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
                
                tempColorData = new byte[colorFrameDescription.LengthInPixels * Utility.BYTES_PER_PIXEL];
                colorFrame.CopyConvertedFrameDataToArray(tempColorData, ColorImageFormat.Rgba);
                if(this.framesCounter != 0)
                {
                    Parallel.For(0, this.colorData.Length, index => this.colorData[index] = (byte)((this.colorData[index] + tempColorData[index]) / 2));
                } else 
                {
                    this.colorData = tempColorData;
                }
                this.framesCounter++;
            }
        }

        /// <summary>
        /// Clears total data and number of frames
        /// </summary>
        public void ClearData()
        {
            this.colorData = new byte[this.colorFrameDescription.LengthInPixels * Utility.BYTES_PER_PIXEL];
            this.framesCounter = 0;
        }
    }
}