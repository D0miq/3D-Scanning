using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DScanning
{
    class Dispersion
    {
        private float[] pixelsDispersion;
        private CircularStack<ushort[]> framesStack;

        public float[] PixelsDispersion
        {
            get
            {
                return this.pixelsDispersion;
            }
        }

        public Dispersion(CircularStack<ushort[]> framesStack)
        {
            this.framesStack = framesStack;
        }

        public void CreateDispersions(ushort[] averagePixels)
        {
            this.pixelsDispersion = new float[averagePixels.Length];
            for (int i = 0; i < averagePixels.Length; i++)
            {
                pixelsDispersion[i] = GetPointDispersion(averagePixels[i], i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="averageDepth"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private float GetPointDispersion(ushort averageDepth, int index)
        {
            float dispersion = 0;
            int zeroCounter = 0;
            foreach (ushort[] depthArray in this.framesStack.IterableData)
            {
                if(depthArray[index] == 0) { zeroCounter++; }
                else { dispersion += (float)Math.Pow((depthArray[index] - averageDepth), 2); }
            }
            return dispersion /= this.framesStack.Count - zeroCounter;
        }
    }
}
