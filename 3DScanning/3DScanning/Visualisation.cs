using Microsoft.Kinect;
using OpenTK;
using OpenTKLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Media.Imaging;

namespace _3DScanning
{
    /// <summary>
    /// 
    /// </summary>
    class Visualisation : IDisposable
    { 

        private CameraSpacePoint[] cameraSpacePoints;

        private ColorSpacePoint[] colorSpacePoints;

        private Kinect kinect;

        private FrameDescription depthFrameDescription;

        private FrameDescription colorFrameDescription;

        private PointCloudRenderable model;

        private NumberFormatInfo numberFormatInfo;

        private DepthData depthData;

        private ColorData colorData;

        /// <summary>
        /// 
        /// </summary>
        public Visualisation(DepthData depthData)
        {
            this.kinect = Kinect.GetInstance();
            this.depthFrameDescription = this.kinect.DepthFrameDescription;
            this.colorFrameDescription = this.kinect.ColorFrameDescription;
            this.cameraSpacePoints = new CameraSpacePoint[this.depthFrameDescription.LengthInPixels];
            this.colorSpacePoints = new ColorSpacePoint[this.depthFrameDescription.LengthInPixels];
            this.model = new PointCloudRenderable();
            this.depthData = depthData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        /// <param name="depthValues"></param>
        /// <param name="writeableBitmap"></param>
        public void VisualiseDepth(WriteableBitmap writeableBitmap)
        {
            byte[] depthColors = Utility.GetColorFromDepth(depthData.LastFrame);
            writeableBitmap.WritePixels(
                new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight),
                depthColors,
                writeableBitmap.PixelWidth,
                0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        /// <param name="depthValues"></param>
        /// <param name="viewport"></param>
        public void VisualisePointCloud(OGLControl viewport)
        {
            Mesh mesh = new Mesh.Builder(depthData.LastFrame).Build();
            List<Vector3> vectorList = new List<Vector3>();

            foreach (CameraSpacePoint point in mesh.Vertexes)
            {
                vectorList.Add(new Vector3(point.X, point.Y, point.Z));
            }
            PointCloud pc = PointCloud.FromVector3List(vectorList);
            this.model.PointCloud = pc;
            viewport.GLrender.AddRenderableObject(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="numberOfFrames"></param>
        public void CreatePointsFromAllFrames(String path, int numberOfFrames, int counter)
        {
            int k = 1;           
//            foreach(ushort[] frame in depthData.Data.GetRange(numberOfFrames).IterableData)
            {
//                Mesh mesh = CreateCleanMesh(frame);
//                GenerateMesh(path + "\\allFrames" + counter + ".obj", mesh, true, false, "Frame cislo: " + k++);
            }
        }

                           

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.depthFrameDescription = null;
                    this.colorFrameDescription = null;
                    this.cameraSpacePoints = null;
                    this.colorSpacePoints = null;
                    this.model = null;
                    this.depthData = null;
                    this.colorData = null;
                    this.numberFormatInfo = null;
                }
                disposedValue = true;
            }
        }
       
         ~Visualisation() {
        
           Dispose(false);
        }
       
        public void Dispose()
        {
            
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
