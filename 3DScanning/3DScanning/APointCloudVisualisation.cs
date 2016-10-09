using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DScanning
{
    abstract class APointCloudVisualisation : AVisualisation
    {

        /// <summary>
        /// Triangles of the rendered model
        /// </summary>
        protected List<int[]> triangles = new List<int[]>();

        /// <summary>
        /// 
        /// </summary>
        protected CameraSpacePoint[] transformedPoints;

        public APointCloudVisualisation()
        {

        }

        /// <summary>
        /// Transfers points from camera view into world coordinates
        /// </summary>
        /// <param name="depthHeight"> Height of the depth frame </param>
        /// <param name="depthWidth"> Width of the depth frame </param>
        /// <returns> Array of transformed points </returns>
        protected CameraSpacePoint[] CameraToWorldTransfer(int depthWidth, int depthHeight)
        {
            bool[] used = new bool[depthHeight * depthWidth];

            for (int x = 1; x < depthWidth - 1; x++)
                for (int y = 1; y < depthHeight - 1; y++)
                {
                    if (IsOK(depthWidth * y + x - 1)
                        && IsOK(depthWidth * y + x)
                        && IsOK(depthWidth * y + x + 1)
                        && IsOK(depthWidth * (y + 1) + x - 1)
                        && IsOK(depthWidth * (y + 1) + x)
                        && IsOK(depthWidth * (y + 1) + x + 1)
                        && IsOK(depthWidth * (y - 1) + x - 1)
                        && IsOK(depthWidth * (y - 1) + x)
                        && IsOK(depthWidth * (y - 1) + x + 1)
                        ) used[depthWidth * y + x] = true;
                }

            int[] indices = new int[depthWidth * depthHeight];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = -1;

            int freeIndex = 0;

            this.triangles.Clear();

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
                        triangles.Add(new int[] { i1 + 1, i2 + 1, i3 + 1 });
                        triangles.Add(new int[] { i2 + 1, i4 + 1, i3 + 1 });
                    }
                }

            CameraSpacePoint[] reordered = new CameraSpacePoint[freeIndex];
            for (int i = 0; i < (depthWidth * depthHeight); i++)
            {
                if (indices[i] > 0)
                    reordered[indices[i]] = csPoints[i];
            }

            return reordered;
        }

        /// <summary>
        /// Checks coordinates
        /// </summary>
        /// <param name="index"> Index of the pixel </param>
        /// <returns> Data evaluation </returns>
        private bool IsOK(int index)
        {
            if (!double.IsInfinity(csPoints[index].X))
                if (!double.IsInfinity(csPoints[index].Y))
                    if (!double.IsInfinity(csPoints[index].Z))
                        return (true);
            return (false);
        }

        /// <summary>
        /// Getting depths from buffer
        /// </summary>
        /// <param name="depthBuffer">Buffer</param>
        /// <returns>Array with depths</returns>
        protected unsafe ushort[] GetDepthFromBuffer(KinectBuffer depthBuffer)
        {
            ushort* depthData = (ushort*)depthBuffer.UnderlyingBuffer;
            ushort[] depthArray = new ushort[this.depthFrameLength];

            for (int i = 0; i < depthArray.Length; i++)
            {
                if (depthData[i] < this.kinectAttributes.MaxDepth && depthData[i] > this.kinectAttributes.MinDepth)
                {
                    depthArray[i] = depthData[i];
                }
            }
            return depthArray;
        }
    }
}
