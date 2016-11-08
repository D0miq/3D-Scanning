using System.Windows.Forms;
using Microsoft.Kinect;

namespace _3DScanning
{
    class ColorlessMeshGenerator : ASingleMeshGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressBar"></param>
        /// <param name="statusText"></param>
        /// <param name="path"></param>
        public ColorlessMeshGenerator(ProgressBar progressBar, Label statusText, string path) : base(progressBar, statusText, path) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        protected override void GenerateMesh(CameraSpacePoint[] points)
        {
            foreach (CameraSpacePoint point in points)
            {
                this.streamWriter.WriteLine("v {0} {1} {2}", point.X.ToString(this.numberFormatInfo), point.Y.ToString(this.numberFormatInfo),
                    point.Z.ToString(this.numberFormatInfo));
            }

            foreach (int[] triangle in this.triangleList)
            {
                this.streamWriter.WriteLine("f {0} {1} {2}", triangle[0], triangle[1], triangle[2]);
            }
            this.streamWriter.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InterpolateFrames()
        {
            for (int i = 0; i < this.interpolatedDepths.Length; i++)
            {
                int averageDepth = this.GetAverageValue(i, this.depthFramesList);
                this.interpolatedDepths[i] = (ushort)averageDepth;
            }
        }
    }
}
