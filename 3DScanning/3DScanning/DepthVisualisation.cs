using System;
using Microsoft.Kinect;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace _3DScanning
{
    class DepthVisualisation : AVisualisation
    {
        /// <summary>
        /// Map depth range to byte range
        /// </summary>
        private const int MAP_DEPTH_TO_BYTE = 8000 / 256;

        private WriteableBitmap image;

        private byte[] depthPixels;

        public DepthVisualisation(WriteableBitmap image)
        {
            this.image = image;
            this.depthPixels = new byte[this.depthFrameLength];
        }

        public override void Reader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    using (KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                    {
                        this.ProcessDepthFrameData(depthBuffer.UnderlyingBuffer, depthBuffer.Size, this.kinectAttributes.MinDepth, this.kinectAttributes.MaxDepth);
                        this.RenderDepthPixels();
                    }
                }
            }
        }

        /// <summary>
        /// Directly accesses the underlying image buffer of the DepthFrame to 
        /// create a displayable bitmap.
        /// This function requires the /unsafe compiler option as we make use of direct
        /// access to the native memory pointed to by the depthFrameData pointer.
        /// </summary>
        /// <param name="depthFrameData">Pointer to the DepthFrame image data</param>
        /// <param name="depthFrameDataSize">Size of the DepthFrame image data</param>
        /// <param name="minDepth">The minimum reliable depth value for the frame</param>
        /// <param name="maxDepth">The maximum reliable depth value for the frame</param>
        private unsafe void ProcessDepthFrameData(IntPtr depthFrameData, uint depthFrameDataSize, int minDepth, int maxDepth)
        {
            // depth frame data is a 16 bit value
            ushort* frameData = (ushort*)depthFrameData;

            // convert depth to a visual representation
            for (int i = 0; i < (int)(depthFrameDataSize / this.kinect.Description.BytesPerPixel); ++i)
            {
                // Get the depth for this pixel
                ushort depth = frameData[i];

                // To convert to a byte, we're mapping the depth value to the byte range.
                // Values outside the reliable depth range are mapped to 0 (black).
                this.depthPixels[i] = (byte)(depth >= minDepth && depth <= maxDepth ? (depth / MAP_DEPTH_TO_BYTE) : 0);
            }
        }


        /// <summary>
        /// Renders color pixels into the writeableBitmap.
        /// </summary>
        private void RenderDepthPixels()
        {
            this.image.WritePixels(
                new Int32Rect(0, 0, this.image.PixelWidth, this.image.PixelHeight),
                this.depthPixels,
                this.image.PixelWidth,
                0);
        }
    }
}
