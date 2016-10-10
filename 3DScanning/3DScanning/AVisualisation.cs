using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DScanning
{
    abstract class AVisualisation
    {
        /// <summary>
        /// Kinect sensor
        /// </summary>
        protected KinectDepthSensor kinect;

        /// <summary>
        /// Transformed points on camera view
        /// </summary>
        protected CameraSpacePoint[] csPoints;

        /// <summary>
        /// Kinect attributes
        /// </summary>
        protected KinectAttributes kinectAttributes;

        /// <summary>
        /// Number of pixel in depth frames
        /// </summary>
        protected uint depthFrameLength;

        public AVisualisation()
        {
            this.kinect = KinectDepthSensor.GetInstance();
            this.kinectAttributes = this.kinect.Attributes;
            this.depthFrameLength = this.kinect.Description.LengthInPixels;
            this.csPoints = new CameraSpacePoint[this.depthFrameLength];
        }

        abstract public void Reader_FrameArrived(object sender, DepthFrameArrivedEventArgs e);

        public KinectDepthSensor Kinect
        {
            get
            {
                return this.kinect;
            }
        }

        public KinectAttributes KinectAttributes
        {
            get
            {
                return this.kinectAttributes;
            }
        }
    }
}
