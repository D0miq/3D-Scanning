using Microsoft.Kinect;
using System.Collections.Generic;

namespace _3DScanning
{
    /// <summary>
    /// 
    /// </summary>
    abstract class APointCloudVisualisation : AVisualisation
    {
        /// <summary>
        /// Triangles of the rendered model
        /// </summary>
        protected List<int[]> triangleList;

        /// <summary>
        /// Transformed points on camera view
        /// </summary>
        protected CameraSpacePoint[] csPoints;

        /// <summary>
        /// 
        /// </summary>
        public APointCloudVisualisation()
        {
            this.triangleList = new List<int[]>();
            this.csPoints = new CameraSpacePoint[this.depthFrameDescription.LengthInPixels];
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

            return this.Reorder(freeIndex, depthWidth,depthHeight, indices);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="freeIndex"></param>
        /// <param name="depthWidth"></param>
        /// <param name="depthHeight"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        protected virtual CameraSpacePoint[] Reorder(int freeIndex, int depthWidth, int depthHeight, int[] indices)
        {
            CameraSpacePoint[] reordered = new CameraSpacePoint[freeIndex];
            for (int i = 0; i < (depthWidth * depthHeight); i++)
            {
                if (indices[i] > 0)
                    reordered[indices[i]] = this.csPoints[i];
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
    }
}
