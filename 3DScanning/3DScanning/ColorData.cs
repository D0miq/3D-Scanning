using Microsoft.Kinect;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _3DScanning
{
    class ColorData
    {
        /// <summary>
        /// 
        /// </summary>
        private CircularStack<byte[]> colorFramesStack;

        /// <summary>
        /// 
        /// </summary>
        protected FrameDescription colorFrameDescription;

        private KinectAttributes kinectAttributes;

        /// <summary>
        /// 
        /// </summary>
        public CircularStack<byte[]> Data
        {
            get
            {
                return colorFramesStack;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public ColorData()
        {
            this.colorFramesStack = new CircularStack<byte[]>();
            this.colorFrameDescription = Kinect.GetInstance().ColorFrameDescription;
            this.kinectAttributes = Kinect.GetInstance().KinectAttributes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiSourceFrame"></param>
        /// <returns></returns>
        public void AddColorData(MultiSourceFrame multiSourceFrame)
        {
            byte[] colorData;
            ColorFrameReference colorFrameReference = multiSourceFrame.ColorFrameReference;
            using (ColorFrame colorFrame = colorFrameReference.AcquireFrame())
            {
                if (colorFrame == null)
                    return;

                colorData = new byte[colorFrameDescription.LengthInPixels * Utility.BYTES_PER_PIXEL];
                colorFrame.CopyConvertedFrameDataToArray(colorData, ColorImageFormat.Rgba);
                this.colorFramesStack.Push(colorData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] InterpolateColors()
        {
            byte[] interpolatedColors = new byte[colorFrameDescription.LengthInPixels*Utility.BYTES_PER_PIXEL];
            CircularStack<byte[]> interpolatingFrames = colorFramesStack.GetRange(this.kinectAttributes.Interpolation);
            Parallel.For(0, interpolatedColors.Length, index => 
                interpolatedColors[index] = (byte) Utility.GetAverageValue(index, interpolatingFrames)
            );

            return interpolatedColors;
        }
    }
}
