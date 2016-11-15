using Microsoft.Kinect;
using OpenTK;
using OpenTKLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
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

        private Mapper mapper;

        private CameraSpacePoint[] reorderedPoints;

        private Color[] reorderedColors;

        private float[] reorderedDispersion;

        private NumberFormatInfo numberFormatInfo;

        private DepthData depthData;

        private ColorData colorData;

        /// <summary>
        /// 
        /// </summary>
        public Visualisation(DepthData depthData, ColorData colorData)
        {
            this.kinect = Kinect.GetInstance();
            this.depthFrameDescription = this.kinect.DepthFrameDescription;
            this.colorFrameDescription = this.kinect.ColorFrameDescription;
            this.cameraSpacePoints = new CameraSpacePoint[this.depthFrameDescription.LengthInPixels];
            this.colorSpacePoints = new ColorSpacePoint[this.depthFrameDescription.LengthInPixels];
            this.model = new PointCloudRenderable();
            this.mapper = new Mapper();
            this.depthData = depthData;
            this.colorData = colorData;
            this.numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            numberFormatInfo.NumberDecimalDigits = 10;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        /// <param name="depthValues"></param>
        /// <param name="writeableBitmap"></param>
        public void VisualiseDepth(WriteableBitmap writeableBitmap)
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
        public void VisualisePointCloud(OGLControl viewport)
        {
            CameraSpacePoint[] reorederedPoints = this.CreateCleanMesh(depthData.LastData);
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
        public CameraSpacePoint[] CreateCleanMesh(ushort[] depthValues)
        {
            ushort[] reducedValues = depthData.ReduceDepthRange(depthValues);
            this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(reducedValues, this.cameraSpacePoints);
            this.mapper.CameraToWorldTransfer(this.cameraSpacePoints);
            return this.mapper.Reorder<CameraSpacePoint>(cameraSpacePoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthData"></param>
        /// <returns></returns>
        public ushort[] CreateCleanInterpolatedMesh()
        {
            ushort[] interpolatedDepths = depthData.InterpolateDepths();
            this.reorderedPoints = CreateCleanMesh(interpolatedDepths);
            return interpolatedDepths;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="numberOfFrames"></param>
        public void CreatePointsFromAllFrames(String path, int numberOfFrames)
        {              
            foreach(ushort[] frame in depthData.Data.GetRange(numberOfFrames).IterableData)
            {
                CameraSpacePoint[] reorderedPoints = CreateCleanMesh(frame);
                GenerateMesh(path + "\\allFrames.obj", reorderedPoints, true, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispersion"></param>
        public void CreateScaledMesh()
        {
            Color[] colors = new Color[this.reorderedDispersion.Length];
            float min = this.reorderedDispersion.Min();
            float max = this.reorderedDispersion.Max();
            for (int i = 0; i< this.reorderedDispersion.Length; i++)
            {
                colors[i] = Utility.GetScaledColor(this.reorderedDispersion[i], min , max);
            }
            this.reorderedColors = colors;      
        }

        public void AddDispersion(Dispersion dispersion)
        {
            this.reorderedDispersion = this.mapper.Reorder<float>(dispersion.PixelsDispersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorData"></param>
        public void CreateColorMesh()
        {
            byte[] interpolatedColors = colorData.InterpolateColors();
            this.kinect.CoordinateMapper.MapCameraPointsToColorSpace(this.cameraSpacePoints, this.colorSpacePoints);
            byte[] mappedColors = Mapper.MapColorToDepth(interpolatedColors, this.colorSpacePoints, this.colorFrameDescription.Width, this.colorFrameDescription.Height, 4);
            Color[] colors = Utility.GetColorsFromRGBA(mappedColors, 4);
            this.reorderedColors = this.mapper.Reorder<Color>(colors);
        }

        public void GenerateMesh(String path, bool append, bool triangles)
        {
            GenerateMesh(path, this.reorderedPoints, append, triangles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void GenerateMesh(String path, CameraSpacePoint[] reorderedPoints ,bool append, bool triangles)
        {
            using(StreamWriter streamWriter = new StreamWriter(path, append))
            {               
                if (reorderedPoints != null)
                {
                    streamWriter.WriteLine("Zde zacina novy frame.");
                    for (int i = 0; i < reorderedPoints.Length; i++)
                    {
                        CameraSpacePoint point = reorderedPoints[i];
                        if (reorderedColors != null && reorderedDispersion != null)
                        {
                            Color color = reorderedColors[i];
                            streamWriter.WriteLine("v {0} {1} {2} {3} {4} {5} {6}", point.X.ToString("N", this.numberFormatInfo),
                            point.Y.ToString("N", this.numberFormatInfo), point.Z.ToString("N", this.numberFormatInfo), (color.R / 255.0).ToString("N", this.numberFormatInfo),
                            (color.G / 255.0).ToString("N", this.numberFormatInfo), (color.B / 255.0).ToString("N", this.numberFormatInfo), reorderedDispersion[i].ToString("N", this.numberFormatInfo));
                        }
                        else if (reorderedDispersion != null)
                        {
                            streamWriter.WriteLine("v {0} {1} {2} {3}", point.X.ToString("N", this.numberFormatInfo),
                            point.Y.ToString("N", this.numberFormatInfo), point.Z.ToString("N", this.numberFormatInfo), reorderedDispersion[i].ToString("N", this.numberFormatInfo));
                        }
                        else if (reorderedColors != null)
                        {
                            Color color = reorderedColors[i];
                            streamWriter.WriteLine("v {0} {1} {2} {3} {4} {5}", point.X.ToString("N", this.numberFormatInfo),
                            point.Y.ToString("N", this.numberFormatInfo), point.Z.ToString("N", this.numberFormatInfo), (color.R / 255.0).ToString("N", this.numberFormatInfo),
                            (color.G / 255.0).ToString("N", this.numberFormatInfo), (color.B / 255.0).ToString("N", this.numberFormatInfo));
                        }
                        else
                        {
                            streamWriter.WriteLine("v {0} {1} {2}", point.X.ToString("N", this.numberFormatInfo),
                            point.Y.ToString("N", this.numberFormatInfo), point.Z.ToString("N", this.numberFormatInfo));
                        }
                    }

                    if (triangles)
                    {
                        foreach (int[] triangle in this.mapper.TriangleList)
                        {
                            streamWriter.WriteLine("f {0} {1} {2}", triangle[0], triangle[1], triangle[2]);
                        }
                    }
                }
            }            
        }

        public void Clear()
        {
            reorderedPoints = null;
            reorderedColors = null;
            reorderedDispersion = null;
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
                    this.mapper = null;
                    this.depthData = null;
                    this.colorData = null;
                    this.numberFormatInfo = null;
                    Clear();
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
