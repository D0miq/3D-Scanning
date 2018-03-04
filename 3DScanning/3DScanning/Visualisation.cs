namespace _3DScanning
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using Microsoft.Kinect;
    using HelixToolkit.Wpf;

    /// <summary>
    /// An instance of the <see cref="Visualisation"/> class reppresents a different methods of visualisation. 
    /// Black and white 2D visualisation that shows depths from black(nearest) to white(furthest) and 3D visualisation.
    /// </summary>
    public class Visualisation
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Depth data for visualisation.
        /// </summary>
        /// <seealso cref="DepthData"/>
        private DepthData depthData;

        /// <summary>
        /// Initializes a new instance of the <see cref="Visualisation"/> class.
        /// </summary>
        /// <param name="depthData">DEpth data for visualisation.</param>
        public Visualisation(DepthData depthData)
        {
            this.depthData = depthData;
        }

        /// <summary>
        /// Visualises depth data as a black and white 2D image.
        /// </summary>
        /// <param name="writeableBitmap">Bitmap that holds the visualised image.</param>
        public void VisualiseDepth(WriteableBitmap writeableBitmap)
        {
            byte[] depthColors = Utility.GetColorFromDepth(this.depthData.LastFrame);
            writeableBitmap.WritePixels(
                new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight),
                depthColors,
                writeableBitmap.PixelWidth,
                0);
        }

        /// <summary>
        /// Visualises depth data as 3D model.
        /// </summary>
        /// <param name="pointsVisual">3D point cloud that stores data and shows them in a viewer.</param>
        public void VisualisePointCloud(PointsVisual3D pointsVisual)
        {
            Mesh mesh = new Mesh.Builder(this.depthData.LastFrame).Build();
            Point3DCollection vectorList = new Point3DCollection();

            foreach (CameraSpacePoint point in mesh.Vertexes)
            {
                vectorList.Add(new Point3D(point.X, point.Y, point.Z));
            }

            pointsVisual.Points = vectorList;
        }
    }
}
