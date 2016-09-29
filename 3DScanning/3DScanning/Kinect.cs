using Microsoft.Kinect;
using System;

namespace _3DScanning
{
    class KinectDepthSensor
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor;

        /// <summary>
        /// Reader for depth frames
        /// </summary>
        private DepthFrameReader depthFrameReader;

        /// <summary>
        /// Description of the data contained in the depth frame
        /// </summary>
        private FrameDescription depthFrameDescription;

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper;

        /// <summary>
        /// Adjustable attributes of the kinect sensor
        /// </summary>
        private KinectAttributes kinectAtt;

        /// <summary>
        /// Creates the kinect sensor
        /// </summary>
        /// <param name="eventHandler"> Handler for frame reader </param>
        public KinectDepthSensor(EventHandler<DepthFrameArrivedEventArgs> eventHandler)
        {
            this.kinectSensor = KinectSensor.GetDefault();

            this.depthFrameReader = this.kinectSensor.DepthFrameSource.OpenReader();

            this.depthFrameReader.FrameArrived += eventHandler;

            this.depthFrameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            this.kinectAtt = new KinectAttributes();
        }

        /// <summary>
        /// Gets the sensor
        /// </summary>
        public KinectSensor Sensor
        {
            get
            {
                return this.kinectSensor;
            }
        }

        /// <summary>
        /// Gets the reader of the depth frame
        /// </summary>
        public DepthFrameReader Reader
        {
            get
            {
                return this.depthFrameReader;
            }
        }

        /// <summary>
        /// Gets the description of the depth frame
        /// </summary>
        public FrameDescription Description
        {
            get
            {
                return this.depthFrameDescription;
            }
        }

        /// <summary>
        /// Gets the coordinate mapper
        /// </summary>
        public CoordinateMapper Mapper
        {
            get
            {
                return this.coordinateMapper;
            }
        }

        /// <summary>
        /// Gets kinect attributes
        /// </summary>
        public KinectAttributes Attributes
        {
            get
            {
                return this.kinectAtt;
            }
        }

        /// <summary>
        /// Stops the kinect sensor
        /// </summary>
        public void stop()
        {
            if (this.kinectSensor.IsOpen)
            {
                this.kinectSensor.Close();
            }
        }

        /// <summary>
        /// Starts the kinect sensor
        /// </summary>
        public void start()
        {
            if (!this.kinectSensor.IsOpen)
            {
                this.kinectSensor.Open();
            }
        }     
    }
}
