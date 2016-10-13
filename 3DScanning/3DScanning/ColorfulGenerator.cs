using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DScanning
{
    class ColorfulGenerator : APointCloudGenerator
    {

        private float maxDispersion = float.MinValue;

        private float minDispersion = float.MaxValue;

        private float[] depthPixelsDispersion;

        private Color[] pixelsColor;

        public ColorfulGenerator(ProgressBar progressBar, Label statusText) : base(progressBar, statusText)
        {
            this.depthPixelsDispersion = new float[this.depthFrameLength];
        }

        protected new CameraSpacePoint[] Reordering(int freeIndex, int depthWidth, int depthHeight, int[] indices)
        {
            CameraSpacePoint[] reordered = new CameraSpacePoint[freeIndex];
            this.pixelsColor = new Color[freeIndex];
            for (int i = 0; i < (depthWidth * depthHeight); i++)
            {
                if (indices[i] > 0)
                {
                    reordered[indices[i]] = csPoints[i];
                    this.pixelsColor[indices[i]] = this.GetColor(this.depthPixelsDispersion[i], this.minDispersion, this.maxDispersion);
                }
            }
            return reordered;
        }

        /// <summary>
        /// Interpolates depth of every pixels in given frames
        /// </summary>
        /// <param name="framesList"> List of depth arrays </param>
        /// <returns> Array of interpolated depths </returns>
        protected override void InterpolateFrames()
        {
            for (int i = 0; i < this.interpolatedDepths.Length; i++)
            {
                int averageDepth = this.GetAverageDepth(i);
                this.interpolatedDepths[i] = (ushort)averageDepth;
                float dispersion = GetPointDispersion(averageDepth, i);
                dispersion = (float)Math.Sqrt(dispersion);
                depthPixelsDispersion[i] = dispersion;
                if (dispersion > this.maxDispersion) { this.maxDispersion = dispersion; }
                if (dispersion < this.minDispersion) { this.minDispersion = dispersion; }
            }
        }

        private float GetPointDispersion(int averageDepth, int index)
        {
            float dispersion = 0;
            foreach (ushort[] depthArray in this.framesList)
            {
                dispersion += (float)Math.Pow((depthArray[index] - averageDepth), 2);
            }
            return dispersion /= this.framesList.Count;
        }

        /**
        * Vypocet barvy pixelu pro mapu.
        * @param h Nadmorska vyska
        * @return Barva
        */
        private Color GetColor(float value, float min, float max)
        {
            float t = (value - min) / (max - min);
            return Color.FromArgb(
            (byte)((1 - t) * Color.Green.R + t * Color.Red.R),
            (byte)((1 - t) * Color.Green.G + t * Color.Red.G),
            0);
        }

        /// <summary>
        /// Creates mesh from given points
        /// </summary>
        /// <param name="points"> Array of points </param>
        protected override void GenerateMesh(CameraSpacePoint[] points)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            StreamWriter sw = new StreamWriter("points.obj");
            for (int i = 0; i < points.Length; i++)
            {
                CameraSpacePoint point = points[i];
                Color color = this.pixelsColor[i];
                sw.WriteLine("v {0} {1} {2} {3} {4} {5}", point.X.ToString(nfi), point.Y.ToString(nfi), point.Z.ToString(nfi), (color.R / 255.0).ToString(nfi), (color.G / 255.0).ToString(nfi), (color.B / 255.0).ToString(nfi));
            }

            for (int i = 0; i < this.triangles.Count; i++)
            {
                sw.WriteLine("f {0} {1} {2}", this.triangles[i][0], this.triangles[i][1], this.triangles[i][2]);
            }
            sw.Close();
        }
    }
}
