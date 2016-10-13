using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Kinect;
using System.Globalization;
using System.IO;

namespace _3DScanning
{
    class ColorlessGenerator : APointCloudGenerator
    {
        public ColorlessGenerator(ProgressBar progressBar, Label statusText) : base(progressBar, statusText)
        {

        }

        protected override void GenerateMesh(CameraSpacePoint[] points)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            StreamWriter sw = new StreamWriter("points.obj");
            for (int i = 0; i < points.Length; i++)
            {
                CameraSpacePoint point = points[i];
                sw.WriteLine("v {0} {1} {2} {3} {4} {5}", point.X.ToString(nfi), point.Y.ToString(nfi), point.Z.ToString(nfi));
            }

            for (int i = 0; i < this.triangles.Count; i++)
            {
                sw.WriteLine("f {0} {1} {2}", this.triangles[i][0], this.triangles[i][1], this.triangles[i][2]);
            }
            sw.Close();
        }

        protected override void InterpolateFrames()
        {
            for (int i = 0; i < this.interpolatedDepths.Length; i++)
            {
                int averageDepth = this.GetAverageDepth(i);
                this.interpolatedDepths[i] = (ushort)averageDepth;
            }
        }
    }
}
