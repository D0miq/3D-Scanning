using System.Collections.Generic;
using Microsoft.Kinect;
using OpenTKLib;
using OpenTK;
using System.Windows.Forms;
using System;

namespace _3DScanning
{
    class PointCloudRenderer : APointCloudVisualisation
    {
        /// <summary>
        /// Model what is rendered on screen
        /// </summary>
        private static PointCloudRenderable model = new PointCloudRenderable();

        /// <summary>
        /// 
        /// </summary>
        private OGLControl viewport;

        /// <summary>
        /// 
        /// </summary>
        private Label statusText;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="statusText"></param>
        public PointCloudRenderer(OGLControl viewport, Label statusText)
        {
            this.viewport = viewport;
            this.statusText = statusText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiSourceFrame"></param>
        protected override void ProcessFrame(MultiSourceFrame multiSourceFrame)
        {
            ushort[] depthArray;
            DepthFrameReference depthFrameReference = multiSourceFrame.DepthFrameReference;
            using (DepthFrame depthFrame = depthFrameReference.AcquireFrame())
            {
                if (depthFrame == null)
                    return;
                
                depthArray = new ushort[this.depthFrameDescription.LengthInPixels];
                depthFrame.CopyFrameDataToArray(depthArray);        
            }
            this.ReduceDepthRange(depthArray);
            this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(depthArray, this.csPoints);
            CameraSpacePoint[] transformedPoints = this.CameraToWorldTransfer(this.depthFrameDescription.Width, this.depthFrameDescription.Height);
            this.RenderPointCloud(transformedPoints);

            this.statusText.Text = "Náhled byl zobrazen!";
            this.kinect.RemoveEventHandler(this.Reader_FrameArrived);
        }

        /// <summary>
        /// Renders point cloud from given points
        /// </summary>
        /// <param name="points"> Array of points </param>
        private void RenderPointCloud(CameraSpacePoint[] points)
        {
            List<Vector3> pointList = new List<Vector3>();
            foreach (CameraSpacePoint point in points)
            {
                Vector3 vector = new Vector3();
                vector.X = point.X;
                vector.Y = point.Y;
                vector.Z = point.Z;
                pointList.Add(vector);
            }
            PointCloud pc = PointCloud.FromVector3List(pointList);
            model.PointCloud = pc;
            this.viewport.GLrender.AddRenderableObject(model);
        }
    }
}
