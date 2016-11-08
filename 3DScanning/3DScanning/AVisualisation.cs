

using Microsoft.Kinect;
using System.Threading.Tasks;

namespace _3DScanning
{
    /// <summary>
    /// 
    /// </summary>
    abstract class AVisualisation
    {

        public delegate void FinishedHandler(bool state);

        public event FinishedHandler FinishedEvent;

        /// <summary>
        /// Kinect sensor
        /// </summary>
        protected Kinect kinect;

        /// <summary>
        /// Kinect attributes
        /// </summary>
        protected KinectAttributes kinectAttributes;

        protected FrameDescription colorFrameDescription;

        protected FrameDescription depthFrameDescription;

        private object locker = new object();

        /// <summary>
        /// 
        /// </summary>
        public AVisualisation()
        {
            this.kinect = Kinect.GetInstance();
            this.kinectAttributes = this.kinect.KinectAttributes;
            this.colorFrameDescription = this.kinect.ColorFrameDescription;
            this.depthFrameDescription = this.kinect.DepthFrameDescription;
            this.kinect.AddEventHandler(this.Reader_FrameArrived);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Reader_FrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            try {
                MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
            
                if (multiSourceFrame != null)
                {
                    lock (this.locker)
                    {
                        this.ProcessFrame(multiSourceFrame);
                    }
                }
            }catch
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        protected void OnFinishedChanged(bool state)
        {
            if(this.FinishedEvent != null)
            {
                this.FinishedEvent(state);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiSourceFrame"></param>
        protected abstract void ProcessFrame(MultiSourceFrame multiSourceFrame);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        protected void ReduceDepthRange(ushort[] depthData)
        {
            this.ReduceDepthRange(depthData, new byte[depthData.Length]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        /// <param name="colorData"></param>
        protected void ReduceDepthRange(ushort[] depthData, byte[] colorData)
        {
            for (int i = 0; i < depthData.Length; i++)
            {
                if (depthData[i] >this.kinectAttributes.MaxDepth || depthData[i] < this.kinectAttributes.MinDepth)
                {
                    depthData[i] = 0;
                    colorData[i] = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Kinect Kinect
        {
            get
            {
                return this.kinect;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public KinectAttributes KinectAttributes
        {
            get
            {
                return this.kinectAttributes;
            }
        }
    }
}
