namespace _3DScanning
{
    using System;
    using Microsoft.Kinect;

    /// <summary>
    /// An instance of the <see cref="Kinect"/> class represents a wrapper for the <see cref="KinectSensor"/> class.
    /// The <see cref="KinectSensor"/> class is wrapped to avoid possible problems with other connected Kinect sensors.
    /// The <see cref="Kinect"/> class has private constructor and is made as a singleton to guarantee usage of only one Kinect sensor for whole run time.
    /// </summary>
    /// <seealso cref="KinectSensor"/>
    public class Kinect
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The only instance of the <see cref="Kinect"/> class.
        /// </summary>
        public static readonly Kinect INSTANCE = new Kinect();

        /// <summary>
        /// The instance of the <see cref="KinectSensor"/> class.
        /// </summary>
        private KinectSensor kinectSensor;

        /// <summary>
        /// The instance of the <see cref="MultiSourceFrameReader"/> class.
        /// </summary>
        private MultiSourceFrameReader multiSourceFrameReader;

        /// <summary>
        /// The description of data contained in a depth frame.
        /// </summary>
        private FrameDescription depthFrameDescription;

        /// <summary>
        /// The description of data contained in a color frame.
        /// </summary>
        private FrameDescription colorFrameDescription;

        /// <summary>
        /// The coordinate mapper that maps one type of points to another.
        /// </summary>
        private CoordinateMapper coordinateMapper;

        /// <summary>
        /// Adjustable attributes of the kinect sensor.
        /// </summary>
        private KinectAttributes kinectAttributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Kinect"/> class.
        /// </summary>
        private Kinect()
        {
            this.kinectSensor = KinectSensor.GetDefault();
            Log.Info("Opening a frame reader.");
            this.multiSourceFrameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color);
            this.depthFrameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
            this.colorFrameDescription = this.kinectSensor.ColorFrameSource.FrameDescription;
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;
            this.kinectAttributes = new KinectAttributes();
        }

        /// <summary>
        /// Gets the description of a depth frame.
        /// </summary>
        public FrameDescription DepthFrameDescription
        {
            get
            {
                return this.depthFrameDescription;
            }
        }

        /// <summary>
        /// Gets the description of a color frame.
        /// </summary>
        public FrameDescription ColorFrameDescription
        {
            get
            {
                return this.colorFrameDescription;
            }
        }

        /// <summary>
        /// Gets the coordinate mapper.
        /// </summary>
        public CoordinateMapper CoordinateMapper
        {
            get
            {
                return this.coordinateMapper;
            }
        }

        /// <summary>
        /// Gets the adjustable attributes of the kinect sensor.
        /// </summary>
        public KinectAttributes KinectAttributes
        {
            get
            {
                return this.kinectAttributes;
            }
        }

        /// <summary>
        /// Adds new <see cref="EventHandler"/> to <see cref="multiSourceFrameReader"/>.
        /// </summary>
        /// <param name="eventHandler">Called whenever a new frame appears.</param>
        public void AddEventHandler(EventHandler<MultiSourceFrameArrivedEventArgs> eventHandler)
        {
            this.multiSourceFrameReader.MultiSourceFrameArrived += eventHandler;
        }

        /// <summary>
        /// Stops the <see cref="kinectSensor"/>.
        /// </summary>
        public void Stop()
        {
            if (this.kinectSensor.IsAvailable)
            {
                Log.Info("Stoping the Kinect.");
                this.kinectSensor.Close();
            }
        }

        /// <summary>
        /// Starts the <see cref="kinectSensor"/>.
        /// </summary>
        public void Start()
        {
            if (!this.kinectSensor.IsAvailable)
            {
                Log.Info("Starting the Kinect.");
                this.kinectSensor.Open();
            }
        }
    }
}
