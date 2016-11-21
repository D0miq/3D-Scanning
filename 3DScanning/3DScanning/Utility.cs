using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DScanning
{
    class Utility
    {
        /// <summary>
        /// Map depth range to byte range
        /// </summary>
        private const int MAP_DEPTH_TO_BYTE = 8000 / 256;

        /// <summary>
        /// 
        /// </summary>
        public const int BYTES_PER_PIXEL = 4;

        /// <summary>
        /// 
        /// </summary>
        public static int MAX_INTERPOLATION = Environment.Is64BitProcess ? 500 : 100;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="framesList"></param>
        /// <returns></returns>
        public static int GetAverageValue(int index, CircularStack<ushort[]> framesStack)
        {
            int averageValue = 0;
            int zeroCounter = 0;
            foreach (ushort[] array in framesStack.IterableData)
            {
                if (array[index] == 0)
                {
                    zeroCounter++;
                }
                averageValue += array[index];
            }
            if (averageValue != 0)
            {
                averageValue /= (framesStack.Count - zeroCounter);
            }
            return averageValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="framesList"></param>
        /// <returns></returns>
        public static int GetAverageValue(int index, CircularStack<byte[]> framesStack)
        {
            int averageValue = 0;
            foreach (byte[] array in framesStack.IterableData)
            {
                averageValue += array[index];
            }
            averageValue /= framesStack.Count;
            return averageValue;

        }

        /// <summary>
        /// 
        /// </summary>
        public static Color[] GetColorsFromRGBA(byte[] colorValues)
        {
            Color[] colors = new Color[colorValues.Length/Utility.BYTES_PER_PIXEL];
            int k = 0;
            for (int i = 0; i < colorValues.Length; i += Utility.BYTES_PER_PIXEL)
            {
                colors[k] = Color.FromArgb(colorValues[i + 3], colorValues[i], colorValues[i + 1], colorValues[i + 2]);
                k++;
            }

            return colors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Color GetScaledColor(float value, float min, float max)
        {
            float t = (value - min) / (max - min);
            return Color.FromArgb(
            (byte)((1 - t) * Color.Green.R + t * Color.Red.R),
            (byte)((1 - t) * Color.Green.G + t * Color.Red.G),
            0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        public static byte[] GetColorFromDepth(ushort[] depthData)
        {
            KinectAttributes kinectAttributes = Kinect.GetInstance().KinectAttributes;
            byte[] depthColors = new byte[depthData.Length];
            Parallel.For(0, depthData.Length, index =>
            {
                ushort depth = depthData[index];
                depthColors[index] = (byte)(depth >= kinectAttributes.MinDepth && depth <= kinectAttributes.MaxDepth ? (depth / MAP_DEPTH_TO_BYTE) : 0);
            });
            return depthColors;
        }
    }
}
