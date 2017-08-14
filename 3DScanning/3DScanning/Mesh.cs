using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
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
                return vertexes;
            }
        }

        private Mesh()
        {
            this.indices = new int[depthWidth * depthHeight];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = -1;
            this.triangles = new List<int[]>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void GenerateMesh(String path, bool append, bool triangles, String description)
        {
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            numberFormatInfo.NumberGroupSeparator = "";
            numberFormatInfo.NumberDecimalDigits = 10;

            using (StreamWriter streamWriter = new StreamWriter(path, append))
            {
                streamWriter.WriteLine(description);
                for (int i = 0; i < this.vertexes.Length; i++)
                {
                    CameraSpacePoint point = this.vertexes[i];
                    Color color = this.colors[i];
                    streamWriter.WriteLine("v {0} {1} {2} {3} {4} {5} {6}", point.X.ToString("N", numberFormatInfo),
                    point.Y.ToString("N", numberFormatInfo), point.Z.ToString("N", numberFormatInfo), (color.R / 255.0).ToString("N", numberFormatInfo),
                    (color.G / 255.0).ToString("N", numberFormatInfo), (color.B / 255.0).ToString("N", numberFormatInfo), dispersions[i].ToString("N", numberFormatInfo));
                }
                if (triangles)
                {
                    foreach (int[] triangle in this.triangles)
                    {
                        streamWriter.WriteLine("f {0} {1} {2}", triangle[0], triangle[1], triangle[2]);
                    }
                }
            }
        }

        /// <summary>
        /// Transfers points from camera view into world coordinates
        /// </summary>
        /// <param name="depthHeight"> Height of the depth frame </param>
        /// <param name="depthWidth"> Width of the depth frame </param>
        /// <returns> Array of transformed points </returns>
        private void CreateTriangles()
        {
            bool[] used = new bool[depthHeight * depthWidth];

            for (int x = 1; x < depthWidth - 1; x++)
                for (int y = 1; y < depthHeight - 1; y++)
                {
                    if (CheckVertex(depthWidth * y + x - 1)
                        && CheckVertex(depthWidth * y + x)
                        && CheckVertex(depthWidth * y + x + 1)
                        && CheckVertex(depthWidth * (y + 1) + x - 1)
                        && CheckVertex(depthWidth * (y + 1) + x)
                        && CheckVertex(depthWidth * (y + 1) + x + 1)
                        && CheckVertex(depthWidth * (y - 1) + x - 1)
                        && CheckVertex(depthWidth * (y - 1) + x)
                        && CheckVertex(depthWidth * (y - 1) + x + 1)
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
            this.Reorder();
        }

        private void Reorder()
        {
            CameraSpacePoint[] reorderedPoints = new CameraSpacePoint[freeIndex];
            Color[] reorderedColors = new Color[freeIndex];
            float[] reorderedDispersions = new float[freeIndex];
            Parallel.For(0, depthHeight * depthWidth, index =>
            {
                if (indices[index] > 0)
                {
                    reorderedPoints[indices[index]] = vertexes[index];
                    reorderedColors[indices[index]] = colors[index];
                    reorderedDispersions[indices[index]] = dispersions[index];
                }          
            });
            this.vertexes = reorderedPoints;
            this.colors = reorderedColors;
            this.dispersions = reorderedDispersions;
        }

        /// <summary>
        /// Checks coordinates
        /// </summary>
        /// <param name="index"> Index of the pixel </param>
        /// <returns> Data evaluation </returns>
        private bool CheckVertex(int index)
        {
            if (!double.IsInfinity(vertexes[index].X))
                if (!double.IsInfinity(vertexes[index].Y))
                    if (!double.IsInfinity(vertexes[index].Z))
                        return (true);
            return (false);
        }


        public class Builder
        {

            private Color[] colors;

            private float[] dispersions;

            private CameraSpacePoint[] cameraSpacePoints;

            private Kinect kinect;

            public Builder(ushort[] depthData)
            {
                this.kinect = Kinect.GetInstance();
                this.colors = new Color[this.kinect.DepthFrameDescription.LengthInPixels];
                this.dispersions = new float[this.kinect.DepthFrameDescription.LengthInPixels];
                this.cameraSpacePoints = new CameraSpacePoint[this.kinect.DepthFrameDescription.LengthInPixels];
                this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(depthData, this.cameraSpacePoints);
            }

            public Mesh Build()
            {
                Mesh mesh = new Mesh();
                mesh.vertexes = cameraSpacePoints;
                mesh.colors = colors;
                mesh.dispersions = dispersions;
                mesh.CreateTriangles();
                return mesh;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dispersion"></param>
            public Builder AddComputedColors(float[] dispersion)
            {
                Color[] colors = new Color[dispersion.Length];
                float min = dispersion.Min();
                float max = dispersion.Max();
                for (int i = 0; i < dispersion.Length; i++)
                {
                    colors[i] = Utility.GetScaledColor(dispersion[i], min, max);
                }
                this.colors = colors;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="colorData"></param>
            public Builder AddRealColors(byte[] colorsData)
            {
                ColorSpacePoint[] colorSpacePoints = new ColorSpacePoint[this.kinect.DepthFrameDescription.LengthInPixels];
                this.kinect.CoordinateMapper.MapCameraPointsToColorSpace(this.cameraSpacePoints, colorSpacePoints);
                byte[] mappedColors = Mapper.MapColorToDepth(colorsData, colorSpacePoints, this.kinect.ColorFrameDescription.Width, this.kinect.ColorFrameDescription.Height);
                Color[] colors = Utility.GetColorsFromRGBA(mappedColors);
                this.colors = colors;
                return this;
            }

            public Builder AddDispersions(float[] dispersions)
            {
                this.dispersions = dispersions;
                return this;
            }
        }
    }
}
