using System;
using Microsoft.Kinect;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Threading;

namespace _3DScanning
{
    class DepthVisualisation : AVisualisation
    {
        /// <summary>
        /// Map depth range to byte range
        /// </summary>
        private const int MAP_DEPTH_TO_BYTE = 8000 / 256;

        private PictureBox box;

        private Bitmap image;

        private ushort[] depthPixels;

        public DepthVisualisation(PictureBox box)
        {
            this.image = new Bitmap(this.kinect.Description.Width, this.kinect.Description.Height);
            this.box = box;
            this.box.Image = this.image;
            this.depthPixels = new ushort[this.depthFrameLength];
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

            for(int y = 0; y < this.kinect.Description.Height; y++)
            {
                for(int x = 0; x < this.kinect.Description.Width; x++)
                {
                    this.image.SetPixel(x,y, Color.FromArgb(this.depthPixels[y* this.kinect.Description.Width+x],Color.Red));                    
                }
            }
            this.box.Refresh();
        }
    }
}
