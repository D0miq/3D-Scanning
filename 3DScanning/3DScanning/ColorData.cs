using Microsoft.Kinect;
using System.Collections.Generic;

namespace _3DScanning
{
    class ColorData
    {
        /// <summary>
        /// 
        /// </summary>
        private const int BYTES_PER_PIXEL = 4;

        /// <summary>
        /// 
        /// </summary>
        private CircularStack<byte[]> colorFramesStack;

        /// <summary>
        /// 
        /// </summary>
        protected FrameDescription colorFrameDescription;

        private int index = -1;

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

                colorData = new byte[colorFrameDescription.LengthInPixels * BYTES_PER_PIXEL];
                colorFrame.CopyConvertedFrameDataToArray(colorData, ColorImageFormat.Rgba);
                this.colorFramesStack.Push(colorData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] InterpolateColors()
        {
            byte[] interpolatedColors = new byte[colorFrameDescription.LengthInPixels];
            for (int i = 0; i < interpolatedColors.Length; i++)
            {
                int averageColor = Utility.GetAverageValue(i, colorFramesStack.GetRange(this.kinectAttributes.Interpolation));
                interpolatedColors[i] = (byte)averageColor;
            }

            return interpolatedColors;
        }
    }
}
