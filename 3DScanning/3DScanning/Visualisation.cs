using Microsoft.Kinect;
using OpenTK;
using OpenTKLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace _3DScanning
{
    /// <summary>
    /// 
    /// </summary>
    class Visualisation
    { 

        private CameraSpacePoint[] cameraSpacePoints;

        private ColorSpacePoint[] colorSpacePoints;

        private Kinect kinect;

        private FrameDescription depthFrameDescription;

        private FrameDescription colorFrameDescription;

        private PointCloudRenderable model;

        private Mapper mapper;

        private CameraSpacePoint[] reorderedPoints;

        private Color[] reorderedColors;

        private float[] reorderedDispersion;

        /// <summary>
        /// 
        /// </summary>
        public Visualisation()
        {
            this.kinect = Kinect.GetInstance();
            this.depthFrameDescription = this.kinect.DepthFrameDescription;
            this.colorFrameDescription = this.kinect.ColorFrameDescription;
            this.cameraSpacePoints = new CameraSpacePoint[this.depthFrameDescription.LengthInPixels];
            this.colorSpacePoints = new ColorSpacePoint[this.depthFrameDescription.LengthInPixels];
            this.model = new PointCloudRenderable();
            this.mapper = new Mapper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        /// <param name="depthValues"></param>
        /// <param name="writeableBitmap"></param>
        public void VisualiseDepth(DepthData depthData, WriteableBitmap writeableBitmap)
        {
            ushort[] depthValues = depthData.ReduceDepthRange(depthData.LastData);
            byte[] depthColors = Utility.GetColorFromDepth(depthValues);
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
        public void VisualisePointCloud(DepthData depthData, OGLControl viewport)
        {
            CameraSpacePoint[] reorederedPoints = this.CreateCleanMesh(depthData);
            List<Vector3> vectorList = new List<Vector3>();

            foreach (CameraSpacePoint point in reorederedPoints)
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
        /// <param name="depthData"></param>
        /// <param name="depthValues"></param>
        /// <returns></returns>
        public CameraSpacePoint[] CreateCleanMesh(DepthData depthData)
        {
            ushort[] depthValues = depthData.ReduceDepthRange(depthData.LastData);
            this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(depthValues, this.cameraSpacePoints);
            this.mapper.CameraToWorldTransfer(this.depthFrameDescription.Width, this.depthFrameDescription.Height, this.cameraSpacePoints);
            this.reorderedPoints = this.mapper.Reorder<CameraSpacePoint>(cameraSpacePoints, this.depthFrameDescription.Width, this.depthFrameDescription.Height);
            return reorderedPoints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        /// <returns></returns>
        public CameraSpacePoint[] CreateCleanInterpolatedMesh(DepthData depthData)
        {
            ushort[] interpolatedDepths = depthData.InterpolateDepths();
            ushort[] depthValues = depthData.ReduceDepthRange(interpolatedDepths);
            this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(depthValues, this.cameraSpacePoints);
            this.mapper.CameraToWorldTransfer(this.depthFrameDescription.Width, this.depthFrameDescription.Height, this.cameraSpacePoints);
            this.reorderedPoints = this.mapper.Reorder<CameraSpacePoint>(cameraSpacePoints, this.depthFrameDescription.Width, this.depthFrameDescription.Height);
            return this.reorderedPoints;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispersion"></param>
        public void CreateScaledMesh(Dispersion dispersion)
        {
            Color[] colors = new Color[dispersion.PixelsDispersion.Length];

            for(int i = 0; i< dispersion.PixelsDispersion.Length; i++)
            {
                colors[i] = Utility.GetScaledColor(dispersion.PixelsDispersion[i], dispersion.MinDispersion, dispersion.MaxDispersion);
            }
            this.reorderedColors = this.mapper.Reorder<Color>(colors, this.depthFrameDescription.Width, this.depthFrameDescription.Height);
            this.reorderedDispersion = this.mapper.Reorder<float>(dispersion.PixelsDispersion, this.depthFrameDescription.Width, this.depthFrameDescription.Height);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorData"></param>
        public void CreateColorMesh(ColorData colorData)
        {
            byte[] interpolatedColors = colorData.InterpolateColors();
            this.kinect.CoordinateMapper.MapCameraPointsToColorSpace(this.cameraSpacePoints, this.colorSpacePoints);
            byte[] mappedColors = Mapper.MapColorToDepth(interpolatedColors, this.colorSpacePoints, this.colorFrameDescription.Width, this.colorFrameDescription.Height, 4);
            Color[] colors = Utility.GetColorsFromRGBA(mappedColors, 4);
            this.reorderedColors = this.mapper.Reorder<Color>(colors, this.depthFrameDescription.Width, this.depthFrameDescription.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void GenerateMesh()
        {
            for (int i = 0; i < reorderedPoints.Length; i++)
            {
                CameraSpacePoint point = reorderedPoints[i];
                Color color = reorderedColors[i];
                this.streamWriter.WriteLine("v {0} {1} {2} {3} {4} {5}", point.X.ToString(this.numberFormatInfo),
                    point.Y.ToString(this.numberFormatInfo), point.Z.ToString(this.numberFormatInfo), (color.R / 255.0).ToString(this.numberFormatInfo),
                    (color.G / 255.0).ToString(this.numberFormatInfo), (color.B / 255.0).ToString(this.numberFormatInfo), reorderedDispersion[i].ToString(this.numberFormatInfo));
            }

            foreach (int[] triangle in this.mapper.TriangleList)
            {
                this.streamWriter.WriteLine("f {0} {1} {2}", triangle[0], triangle[1], triangle[2]);
            }
            this.streamWriter.Close();
        }
    }
}
