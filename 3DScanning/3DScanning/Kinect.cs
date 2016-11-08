using Microsoft.Kinect;
using System;
using System.ComponentModel;

namespace _3DScanning
{
    /// <summary>
    /// 
    /// </summary>
    class Kinect : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private static Kinect INSTANCE = new Kinect();

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor;

        /// <summary>
        /// Reader for depth frames
        /// </summary>
        private MultiSourceFrameReader multiSourceFrameReader;

        /// <summary>
        /// Description of the data contained in the depth frame
        /// </summary>
        private FrameDescription depthFrameDescription;

        /// <summary>
        /// 
        /// </summary>
        private FrameDescription colorFrameDescription;

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper;

        /// <summary>
        /// Adjustable attributes of the kinect sensor
        /// </summary>
        private KinectAttributes kinectAttributes;

        /// <summary>
        /// Creates the kinect sensor
        /// </summary>
        /// <param name="eventHandler"> Handler for frame reader </param>
        private Kinect()
        {
            this.kinectSensor = KinectSensor.GetDefault();
            this.multiSourceFrameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color);
            this.depthFrameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
            this.colorFrameDescription = this.kinectSensor.ColorFrameSource.FrameDescription;
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;
            this.kinectAttributes = new KinectAttributes();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Kinect GetInstance()
        {
            return INSTANCE;
        }

        /// <summary>
        /// Gets the sensor
        /// </summary>
        public KinectSensor KinectSensor
        {
            get
            {
                return this.kinectSensor;
            }
        }

        /// <summary>
        /// Gets the reader of the depth frame
        /// </summary>
        public MultiSourceFrameReader MultiSourceFrameReader
        {
            get
            {
                return this.multiSourceFrameReader;
            }
        }

        /// <summary>
        /// Gets the description of the depth frame
        /// </summary>
        public FrameDescription DepthFrameDescription
        {
            get
            {
                return this.depthFrameDescription;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FrameDescription ColorFrameDescription
        {
            get
            {
                return this.colorFrameDescription;
            }
        }

        /// <summary>
        /// Gets the coordinate mapper
        /// </summary>
        public CoordinateMapper CoordinateMapper
        {
            get
            {
                return this.coordinateMapper;
            }
        }

        /// <summary>
        /// Gets kinect attributes
        /// </summary>
        public KinectAttributes KinectAttributes
        {
            get
            {
                return this.kinectAttributes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        public void AddEventHandler(EventHandler<MultiSourceFrameArrivedEventArgs> eventHandler)
        {
            this.multiSourceFrameReader.MultiSourceFrameArrived += eventHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        public void RemoveEventHandler(EventHandler<MultiSourceFrameArrivedEventArgs> eventHandler)
        {
            this.multiSourceFrameReader.MultiSourceFrameArrived -= eventHandler;
        }

        /// <summary>
        /// Stops the kinect sensor
        /// </summary>
        public void Stop()
        {
            if (this.kinectSensor.IsOpen)
            {
                this.kinectSensor.Close();
            }
        }

        /// <summary>
        /// Starts the kinect sensor
        /// </summary>
        public void Start()
        {
            if (!this.kinectSensor.IsOpen)
            {
                this.kinectSensor.Open();
            }
        }

        /// <summary>
        /// Implementation of the IDisposable interface, it releases resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources
        /// </summary>
        /// <param name="disposing">Indicates which resources should be released, true -> all resources, false -> unmanaged resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
            if (this.multiSourceFrameReader != null)
            {
                this.multiSourceFrameReader.Dispose();
                this.multiSourceFrameReader = null;
            }
        }
    }
}
