using System.ComponentModel;

namespace _3DScanning
{
    public class KinectAttributes : INotifyPropertyChanged
    {
        public const int MIN_DEPTH = 500;

        public const int MAX_DEPTH = 8000;

        public const int MIN_INTERPOLATION = 1;

        public const int MAX_INTERPOLATION = 10000;

        /// <summary>
        /// Minimal depth
        /// </summary>
        private int minDepth;

        /// <summary>
        /// Maximal depth
        /// </summary>
        private int maxDepth;

        /// <summary>
        /// Number of interpolated frames
        /// </summary>
        private int interpolation;

        /// <summary>
        /// Inicializes attributes
        /// </summary>
        public KinectAttributes()
        {
            this.minDepth = MIN_DEPTH;
            this.maxDepth = MAX_DEPTH;
            this.interpolation = 100;
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the minimum depth
        /// </summary>
        public int MinDepth
        {
            get
            {
                return this.minDepth;
            }

            set
            {
                if (value < this.maxDepth && value >= MIN_DEPTH)
                {
                    this.minDepth = value;
                    if (null != this.PropertyChanged)
                    {
                        this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("MinDepth"));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum depth
        /// </summary>
        public int MaxDepth
        {
            get
            {
                return this.maxDepth;
            }

            set
            {
                if (value > this.minDepth && value <= MAX_DEPTH)
                {
                    this.maxDepth = value;
                    if (null != this.PropertyChanged)
                    {
                        this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("MaxDepth"));
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the number of interpolated frames
        /// </summary>
        public int Interpolation
        {
            get
            {
                return this.interpolation;
            }

            set
            {
                if (value > MIN_INTERPOLATION && value <= MAX_INTERPOLATION)
                {
                    this.interpolation = value;
                    if (null != this.PropertyChanged)
                    {
                        this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Interpolation"));
                    }
                }                  
            }
        }
    }
}
