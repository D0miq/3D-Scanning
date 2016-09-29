using System.ComponentModel;


namespace _3DScanning
{
    class KinectAttributes : INotifyPropertyChanged
    {
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
            this.minDepth = 500;
            this.maxDepth = 8000;
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
                if(value < this.maxDepth && value >= 500)
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
                if(value > this.minDepth && value <=8000)
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
                this.interpolation = value;
                if (null != this.PropertyChanged)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Interpolation"));
                }
            }
        }
    }
}
