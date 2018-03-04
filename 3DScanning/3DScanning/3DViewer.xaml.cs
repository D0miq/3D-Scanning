namespace _3DScanning
{
    using System.Windows.Controls;
    using System.Windows.Media.Media3D;
    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for _3DViewer.xaml
    /// </summary>
    public partial class _3DViewer : UserControl
    {
        /// <summary>
        /// 3D viewport.
        /// </summary>
        private HelixViewport3D helixViewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="_3DViewer"/> class.
        /// </summary>
        public _3DViewer()
        {
            this.InitializeComponent();
            this.Create3DViewport();
        }

        /// <summary>
        /// Adds a new point cloud to the <see cref="helixViewport"/>.
        /// </summary>
        /// <param name="pointsVisual">Point cloud that will be added.</param>
        public void AddPointCloud(PointsVisual3D pointsVisual)
        {
            this.helixViewport.Children.Add(pointsVisual);
        }

        /// <summary>
        /// Creates viewport and sets lighting and camera.
        /// </summary>
        private void Create3DViewport()
        {
            this.helixViewport = new HelixViewport3D();
            var lights = new DefaultLights();
            var camera = new PerspectiveCamera()
            {
                Position = new Point3D(0, 0, 0),
                LookDirection = new Vector3D(0, 0, 1)
            };

            this.helixViewport.Children.Add(lights);
            this.helixViewport.Camera = camera;
            this.AddChild(this.helixViewport);
        }
    }
}