using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DScanning
{
    class Mesh
    {
        private CameraSpacePoint[] vertexes;

        private Color[] colors;

        private float[] dispersions;

        private List<int[]> triangles;

        private int[] indices;

        private int freeIndex = 0;

        private int depthWidth = Kinect.GetInstance().DepthFrameDescription.Width;

        private int depthHeight = Kinect.GetInstance().DepthFrameDescription.Height;

        public CameraSpacePoint[] Vertexes
        {
            get
            {
                return this.vertexes;
            }
            set
            {
                this.vertexes = value;
            }
        }

        public Color[] Colors
        {
            get
            {
                return this.colors;
            }
            set
            {
                this.colors = value;
            }
        }

        public float[] Dispersions
        {
            get
            {
                return this.dispersions;
            }
            set
            {
                this.dispersions = value;
            }
        }

        public List<int[]> Triangles
        {
            get
            {
                return this.triangles;
            }
        }

        public Mesh()
        {
            this.indices = new int[depthWidth * depthHeight];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = -1;
            this.triangles = new List<int[]>();
            this.vertexes = new CameraSpacePoint[Kinect.GetInstance().DepthFrameDescription.LengthInPixels];
            this.colors  = new Color[Kinect.GetInstance().DepthFrameDescription.LengthInPixels];
            this.dispersions = new float[Kinect.GetInstance().DepthFrameDescription.LengthInPixels];
        }

        /// <summary>
        /// Transfers points from camera view into world coordinates
        /// </summary>
        /// <param name="depthHeight"> Height of the depth frame </param>
        /// <param name="depthWidth"> Width of the depth frame </param>
        /// <returns> Array of transformed points </returns>
        public void CreateTriangles(CameraSpacePoint[] cameraSpacePoints)
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
                        this.triangles.Add(new int[] { i1 + 1, i2 + 1, i3 + 1 });
                        this.triangles.Add(new int[] { i2 + 1, i4 + 1, i3 + 1 });
                    }
                }
        }

        public R[] Reorder<R>(R[] points)
        {
            R[] reordered = new R[freeIndex];
            Parallel.For(0, depthHeight * depthWidth, index =>
            {
                if (indices[index] > 0)
                    reordered[indices[index]] = points[index];
            });
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
    }
}
