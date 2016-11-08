using Microsoft.Kinect;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace _3DScanning
{
    class ScaledMeshGenerator : ASingleMeshGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        private float maxDispersion = float.MinValue;

        /// <summary>
        /// 
        /// </summary>
        private float minDispersion = float.MaxValue;

        /// <summary>
        /// 
        /// </summary>
        private float[] depthPixelsDispersion;

        /// <summary>
        /// 
        /// </summary>
        private float[] reorderedDispersion;
        
        /// <summary>
        /// 
        /// </summary>
        private Color[] colorPixels;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressBar"></param>
        /// <param name="statusText"></param>
        /// <param name="path"></param>
        public ScaledMeshGenerator(ProgressBar progressBar, Label statusText, string path) : base(progressBar, statusText, path)
        {
            this.depthPixelsDispersion = new float[this.depthFrameDescription.LengthInPixels];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="freeIndex"></param>
        /// <param name="depthWidth"></param>
        /// <param name="depthHeight"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        protected override CameraSpacePoint[] Reorder(int freeIndex, int depthWidth, int depthHeight, int[] indices)
        {
            CameraSpacePoint[] reordered = new CameraSpacePoint[freeIndex];
            this.reorderedDispersion = new float[freeIndex];
            this.colorPixels = new Color[freeIndex];
            for (int i = 0; i < (depthWidth * depthHeight); i++)
            {
                if (indices[i] > 0)
                {
                    reordered[indices[i]] = this.csPoints[i];
                    this.colorPixels[indices[i]] = this.GetColor(this.depthPixelsDispersion[i], this.minDispersion, this.maxDispersion);
                    this.reorderedDispersion[indices[i]] = this.depthPixelsDispersion[i];
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
                int averageDepth = this.GetAverageValue(i,this.depthFramesList);
                this.interpolatedDepths[i] = (ushort)averageDepth;
                float dispersion = GetPointDispersion(averageDepth, i);
                depthPixelsDispersion[i] = dispersion;
                if (dispersion > this.maxDispersion) { this.maxDispersion = dispersion; }
                if (dispersion < this.minDispersion) { this.minDispersion = dispersion; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="averageDepth"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private float GetPointDispersion(int averageDepth, int index)
        {
            float dispersion = 0;
            foreach (ushort[] depthArray in this.depthFramesList)
            {
                dispersion += (float)Math.Pow((depthArray[index] - averageDepth), 2);
            }
            return dispersion /= this.depthFramesList.Count;
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
            for (int i = 0; i < points.Length; i++)
            {
                CameraSpacePoint point = points[i];
                Color color = this.colorPixels[i];
                this.streamWriter.WriteLine("v {0} {1} {2} {3} {4} {5} {6}", point.X.ToString(this.numberFormatInfo),
                    point.Y.ToString(this.numberFormatInfo), point.Z.ToString(this.numberFormatInfo), (color.R / 255.0).ToString(this.numberFormatInfo),
                    (color.G / 255.0).ToString(this.numberFormatInfo), (color.B / 255.0).ToString(this.numberFormatInfo), reorderedDispersion[i].ToString(this.numberFormatInfo));
            }

            foreach (int[] triangle in this.triangleList)
            {
                this.streamWriter.WriteLine("f {0} {1} {2}", triangle[0], triangle[1], triangle[2]);
            }
            this.streamWriter.Close();
        }
    }
}
