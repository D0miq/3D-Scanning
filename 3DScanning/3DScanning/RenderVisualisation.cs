using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using OpenTKLib;
using OpenTK;
using System.Windows.Forms;

namespace _3DScanning
{
    class RenderVisualisation : APointCloudVisualisation
    {

        /// <summary>
        /// Model what is rendered on screen
        /// </summary>
        private PointCloudRenderable model = new PointCloudRenderable();

        private OGLControl viewport;

        private Label statusText;

        public RenderVisualisation(OGLControl viewport, Label statusText)
        {
            this.viewport = viewport;
            this.statusText = statusText;
        }

        public override void Reader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    using (KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                    {
                        ushort[] depthArray = GetDepthFromBuffer(depthBuffer);
                        this.kinect.Mapper.MapDepthFrameToCameraSpace(depthArray, this.csPoints);
                        this.transformedPoints = this.CameraToWorldTransfer(this.kinect.Description.Width, this.kinect.Description.Height);
                        this.RenderPointCloud(this.transformedPoints);

                        this.statusText.Text = "Náhled byl zobrazen!";
                        //this.DisableControls(false);
                        this.kinect.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Renders point cloud from given points
        /// </summary>
        /// <param name="points"> Array of points </param>
        private void RenderPointCloud(CameraSpacePoint[] points)
        {
            List<Vector3> pointList = new List<Vector3>();
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 v = new Vector3();
                v.X = points[i].X;
                v.Y = points[i].Y;
                v.Z = points[i].Z;
                pointList.Add(v);
            }
            PointCloud pc = PointCloud.FromVector3List(pointList);
            this.model.PointCloud = pc;
            this.viewport.GLrender.AddRenderableObject(model);
        }
    }
}
