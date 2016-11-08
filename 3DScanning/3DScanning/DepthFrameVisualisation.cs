using System;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows;

namespace _3DScanning
{
    class DepthFrameVisualisation : AVisualisation
    {
        /// <summary>
        /// Map depth range to byte range
        /// </summary>
        private const int MAP_DEPTH_TO_BYTE = 8000 / 256;

        /// <summary>
        /// 
        /// </summary>
        private WriteableBitmap writeableBitmap;

        /// <summary>
        /// 
        /// </summary>
        private byte[] depthPixels;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeableBitmap"></param>
        public DepthFrameVisualisation(WriteableBitmap writeableBitmap)
        {
            this.writeableBitmap = writeableBitmap;
            this.depthPixels = new byte[this.depthFrameDescription.LengthInPixels];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiSourceFrame"></param>
        protected override void ProcessFrame(MultiSourceFrame multiSourceFrame)
        {
            ushort[] depthData;
            DepthFrameReference depthFrameReference = multiSourceFrame.DepthFrameReference;
            
            using (DepthFrame depthFrame = depthFrameReference.AcquireFrame())
            {
                if (depthFrame == null)
                    return;

                depthData = new ushort[this.depthFrameDescription.LengthInPixels];
                depthFrame.CopyFrameDataToArray(depthData);
            }
           this.ReduceDepthRange(depthData);
           this.ConvertDepthToColor(depthData);
           this.Render();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        private void ConvertDepthToColor(ushort[] depthData)
        {
            for(int i = 0; i < depthData.Length; i++)
            {
                ushort depth = depthData[i];
                this.depthPixels[i] = (byte)(depth >= this.kinectAttributes.MinDepth && depth <= this.kinectAttributes.MaxDepth ? (depth / MAP_DEPTH_TO_BYTE) : 0);
            }
        }

        /// <summary>
        /// Renders color pixels into the writeableBitmap.
        /// </summary>
        private void Render()
        {
            this.writeableBitmap.WritePixels(
                new Int32Rect(0, 0, this.writeableBitmap.PixelWidth, this.writeableBitmap.PixelHeight),
                this.depthPixels,
                this.writeableBitmap.PixelWidth,
                0);
        }
    }
}
