namespace _3DScanning
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using Microsoft.Kinect;

    /// <summary>
    /// Utility class used for transforming data between color and depth or between diferent color representations.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// The number of bytes needed for a color pixel.
        /// </summary>
        public const int BYTES_PER_PIXEL = 4;

        /// <summary>
        /// Map depth range to byte range.
        /// </summary>
        private const float MAP_DEPTH_TO_BYTE = (float)KinectAttributes.MAX_DEPTH / byte.MaxValue;

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Maps rgba values to colors.
        /// </summary>
        /// <param name="colorValues">Rgba values.</param>
        /// <returns>Colors.</returns>
        public static Color[] GetColorsFromRGBA(byte[] colorValues)
        {
            Color[] colors = new Color[colorValues.Length / Utility.BYTES_PER_PIXEL];
            int k = 0;
            for (int i = 0; i < colorValues.Length; i += Utility.BYTES_PER_PIXEL)
            {
                colors[k] = Color.FromArgb(colorValues[i + 3], colorValues[i], colorValues[i + 1], colorValues[i + 2]);
                Log.Debug("From: [" + colorValues[i + 3] + ", " + colorValues[i] + ", " + colorValues[i + 1] + ", " + colorValues[i + 2] + "] to " + colors[k].Name);
                k++;
            }

            return colors;
        }

        /// <summary>
        /// Scales color value depending on minimum and maximum dispersion.
        /// </summary>
        /// <param name="value">Dispersion value.</param>
        /// <param name="min">Minimal dispersion.</param>
        /// <param name="max">Maximal dispersion.</param>
        /// <returns>Scaled colors.</returns>
        public static Color GetScaledColor(float value, float min, float max)
        {
            Log.Debug("Value: " + value);
            Log.Debug("Min depth: " + min);
            Log.Debug("Max depth: " + max);

            float t = (value - min) / (max - min);
            return Color.FromArgb(
                (byte)(t * Color.Red.R),
                (byte)((1 - t) * Color.Lime.G),
                0);
        }

        /// <summary>
        /// Transforms depths to colors.
        /// </summary>
        /// <param name="depthData">The array of depth data.</param>
        /// <returns>An array of color data.</returns>
        public static byte[] GetColorFromDepth(ushort[] depthData)
        {
            KinectAttributes kinectAttributes = Kinect.INSTANCE.KinectAttributes;
            byte[] depthColors = new byte[depthData.Length];
            Parallel.For(0, depthData.Length, index =>
            {
                ushort depth = depthData[index];
                depthColors[index] = (byte)(depth >= kinectAttributes.MinDepth && depth <= kinectAttributes.MaxDepth ? (depth / MAP_DEPTH_TO_BYTE) : 0);
                Log.Debug("Depth: " + depth + ", Color: " + depthColors[index]);
            });

            return depthColors;
        }

        /// <summary>
        /// Maps colors to depth points.
        /// </summary>
        /// <param name="colorData">Colors.</param>
        /// <param name="colorSpacePoint">Depth points.</param>
        /// <param name="width">Width of the color frame.</param>
        /// <param name="height">Height of the color frame.</param>
        /// <returns>Mapped colors.</returns>
        public static byte[] MapColorToDepth(byte[] colorData, ColorSpacePoint[] colorSpacePoint, int width, int height)
        {
            Log.Debug("Color width: " + width);
            Log.Debug("Color height: " + height);

            byte[] colorPixels = new byte[colorSpacePoint.Length * BYTES_PER_PIXEL];
            Parallel.For(0, colorSpacePoint.Length, index =>
            {
                ColorSpacePoint point = colorSpacePoint[index];

                int colorX = (int)Math.Round(point.X);
                int colorY = (int)Math.Round(point.Y);
                Log.Debug("X: " + colorX + ", Y: " + colorY);

                if ((colorX >= 0) && (colorX < width) && (colorY >= 0) && (colorY < height))
                {
                    int colorImageIndex = ((width * colorY) + colorX) * BYTES_PER_PIXEL;
                    int depthPixel = index * BYTES_PER_PIXEL;

                    Log.Debug("Color image index: " + colorImageIndex);
                    Log.Debug("Depth pixel index: " + depthPixel);

                    colorPixels[depthPixel] = colorData[colorImageIndex];
                    colorPixels[depthPixel + 1] = colorData[colorImageIndex + 1];
                    colorPixels[depthPixel + 2] = colorData[colorImageIndex + 2];
                    colorPixels[depthPixel + 3] = colorData[colorImageIndex + 3];
                }
            });

            return colorPixels;
        }
    }
}
