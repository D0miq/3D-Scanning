using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DScanning
{
    class Mapper
    {
        /// <summary>
        ///
        /// </summary>
        public static byte[] MapColorToDepth(byte[] colorData, ColorSpacePoint[] colorSpacePoint, int width, int height)
        {
            byte[] colorPixels = new byte[colorData.Length];
            Parallel.For(0, colorSpacePoint.Length, index =>
            {
                ColorSpacePoint point = colorSpacePoint[index];

                int colorX = (int)Math.Floor(point.X + 0.5);
                int colorY = (int)Math.Floor(point.Y + 0.5);

                if ((colorX >= 0) && (colorX < width) && (colorY >= 0) && (colorY < height))
                {
                    int colorImageIndex = ((width * colorY) + colorX) * Utility.BYTES_PER_PIXEL;
                    int depthPixel = index * Utility.BYTES_PER_PIXEL;

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
