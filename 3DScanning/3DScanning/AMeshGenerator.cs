using Microsoft.Kinect;
using System.Globalization;
using System.IO;

namespace _3DScanning
{
    abstract class AMeshGenerator : APointCloudVisualisation
    {
        /// <summary>
        /// Counter of scanned frames
        /// </summary>
        protected int framesCounter = 0;

        /// <summary>
        /// 
        /// </summary>
        protected NumberFormatInfo numberFormatInfo;

        /// <summary>
        /// 
        /// </summary>
        protected StreamWriter streamWriter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public AMeshGenerator(string path) {
            this.numberFormatInfo = new NumberFormatInfo();
            this.numberFormatInfo.NumberDecimalSeparator = ".";
            this.streamWriter = new StreamWriter(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        protected abstract void GenerateMesh(CameraSpacePoint[] points);   
    }
}
