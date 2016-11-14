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
        private List<int[]> triangleList;
        private int[] indices;
        private int freeIndex = 0;
        private int depthWidth = Kinect.GetInstance().DepthFrameDescription.Width;
        private int depthHeight = Kinect.GetInstance().DepthFrameDescription.Height;


        public List<int[]> TriangleList
        {
            get
            {
                return this.triangleList;
            }
        }


        public Mapper()
        {
            this.triangleList = new List<int[]>();
        }

        /// <summary>
        /// Transfers points from camera view into world coordinates
        /// </summary>
        /// <param name="depthHeight"> Height of the depth frame </param>
        /// <param name="depthWidth"> Width of the depth frame </param>
        /// <returns> Array of transformed points </returns>
        public void CameraToWorldTransfer(CameraSpacePoint[] cameraSpacePoints)
        {
            bool[] used = new bool[depthHeight * depthWidth];

            for (int x = 1; x < depthWidth - 1; x++)
                for (int y = 1; y < depthHeight - 1; y++)
                {
                    if (IsOK(depthWidth * y + x - 1, cameraSpacePoints)
                        && IsOK(depthWidth * y + x, cameraSpacePoints)
                        && IsOK(depthWidth * y + x + 1, cameraSpacePoints)
                        && IsOK(depthWidth * (y + 1) + x - 1, cameraSpacePoints)
                        && IsOK(depthWidth * (y + 1) + x, cameraSpacePoints)
                        && IsOK(depthWidth * (y + 1) + x + 1, cameraSpacePoints)
                        && IsOK(depthWidth * (y - 1) + x - 1, cameraSpacePoints)
                        && IsOK(depthWidth * (y - 1) + x, cameraSpacePoints)
                        && IsOK(depthWidth * (y - 1) + x + 1, cameraSpacePoints)
                        ) used[depthWidth * y + x] = true;
                }

            this.indices = new int[depthWidth * depthHeight];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = -1;

            freeIndex = 0;

            this.triangleList.Clear();

            for (int x = 0; x < depthWidth - 1; x++)
                for (int y = 0; y < depthHeight - 1; y++)
                {
                    if (used[depthWidth * y + x]
                        && used[depthWidth * y + x + 1]
                        && used[depthWidth * (y + 1) + x]
                        && used[depthWidth * (y + 1) + x + 1])
                    {
                        int i1 = indices[depthWidth * y + x];
                        if (i1 < 0)
                        {
                            i1 = freeIndex;
                            freeIndex++;
                            indices[depthWidth * y + x] = i1;
                        }
                        int i2 = indices[depthWidth * y + x + 1];
                        if (i2 < 0)
                        {
                            i2 = freeIndex;
                            freeIndex++;
                            indices[depthWidth * y + x + 1] = i2;
                        }
                        int i3 = indices[depthWidth * (y + 1) + x];
                        if (i3 < 0)
                        {
                            i3 = freeIndex;
                            freeIndex++;
                            indices[depthWidth * (y + 1) + x] = i3;
                        }
                        int i4 = indices[depthWidth * (y + 1) + x + 1];
                        if (i4 < 0)
                        {
                            i4 = freeIndex;
                            freeIndex++;
                            indices[depthWidth * (y + 1) + x + 1] = i4;
                        }
                        this.triangleList.Add(new int[] { i1 + 1, i2 + 1, i3 + 1 });
                        this.triangleList.Add(new int[] { i2 + 1, i4 + 1, i3 + 1 });
                    }
                }         
        }

        public R[] Reorder<R>(R[] points)
        {
            R[] reordered = new R[freeIndex];
            for (int i = 0; i < depthHeight * depthWidth; i++)
            {
                if (indices[i] > 0)
                    reordered[indices[i]] = points[i];
            }
            return reordered;
        }

        /// <summary>
        /// Checks coordinates
        /// </summary>
        /// <param name="index"> Index of the pixel </param>
        /// <returns> Data evaluation </returns>
        private bool IsOK(int index, CameraSpacePoint[] cameraSpacePoints)
        {
            if (!double.IsInfinity(cameraSpacePoints[index].X))
                if (!double.IsInfinity(cameraSpacePoints[index].Y))
                    if (!double.IsInfinity(cameraSpacePoints[index].Z))
                        return (true);
            return (false);
        }


        /// <summary>
        ///
        /// </summary>
        public static byte[] MapColorToDepth(byte[] colorData, ColorSpacePoint[] colorSpacePoint, int width, int height, int bytesPerPixel)
        {
            byte[] colorPixels = new byte[colorData.Length];
            for (int depthIndex = 0; depthIndex < colorSpacePoint.Length; ++depthIndex)
            {
                ColorSpacePoint point = colorSpacePoint[depthIndex];

                int colorX = (int)Math.Floor(point.X + 0.5);
                int colorY = (int)Math.Floor(point.Y + 0.5);

                if ((colorX >= 0) && (colorX < width) && (colorY >= 0) && (colorY < height))
                {
                    int colorImageIndex = ((width * colorY) + colorX) * bytesPerPixel;
                    int depthPixel = depthIndex * bytesPerPixel;

                    colorPixels[depthPixel] = colorData[colorImageIndex];
                    colorPixels[depthPixel + 1] = colorData[colorImageIndex + 1];
                    colorPixels[depthPixel + 2] = colorData[colorImageIndex + 2];
                    colorPixels[depthPixel + 3] = colorData[colorImageIndex + 3];
                }
            }
            return colorPixels;
        }
    }
}
