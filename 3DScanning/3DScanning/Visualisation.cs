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
            this.depthData = depthData;
            this.colorData = colorData;
            this.numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            numberFormatInfo.NumberGroupSeparator = "";
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
            Mesh mesh = this.CreateCleanMesh(depthData.LastData);
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
        /// <param name="depthData"></param>
        /// <param name="depthValues"></param>
        /// <returns></returns>
        public Mesh CreateCleanMesh(ushort[] depthValues)
        {
            ushort[] reducedValues = depthData.ReduceDepthRange(depthValues);
            this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(reducedValues, this.cameraSpacePoints);
            Mesh mesh = new Mesh();
            mesh.CreateTriangles(cameraSpacePoints);
            mesh.Vertexes = mesh.Reorder<CameraSpacePoint>(cameraSpacePoints);
            return mesh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="numberOfFrames"></param>
        public void CreatePointsFromAllFrames(String path, int numberOfFrames, int counter)
        {
            int k = 1;           
            foreach(ushort[] frame in depthData.Data.GetRange(numberOfFrames).IterableData)
            {
                Mesh mesh = CreateCleanMesh(frame);
                GenerateMesh(path + "\\allFrames" + counter + ".obj", mesh, true, false, "Frame cislo: " + k++);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispersion"></param>
        public void AddComputedColors(Mesh mesh)
        {
            Color[] colors = new Color[mesh.Dispersions.Length];
            float min = mesh.Dispersions.Min();
            float max = mesh.Dispersions.Max();
            for (int i = 0; i< mesh.Dispersions.Length; i++)
            {
                colors[i] = Utility.GetScaledColor(mesh.Dispersions[i], min , max);
            }
            mesh.Colors = colors;      
        }

        public void AddDispersion(Dispersion dispersion, Mesh mesh)
        {
            mesh.Dispersions = dispersion.PixelsDispersion;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorData"></param>
        public void AddRealColors(Mesh mesh)
        {
            byte[] interpolatedColors = colorData.InterpolateColors();
            this.kinect.CoordinateMapper.MapCameraPointsToColorSpace(this.cameraSpacePoints, this.colorSpacePoints);
            byte[] mappedColors = Mapper.MapColorToDepth(interpolatedColors, this.colorSpacePoints, this.colorFrameDescription.Width, this.colorFrameDescription.Height);
            Color[] colors = Utility.GetColorsFromRGBA(mappedColors);
            mesh.Colors = mesh.Reorder<Color>(colors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void GenerateMesh(String path, Mesh mesh, bool append, bool triangles, String description)
        {
            using (StreamWriter streamWriter = new StreamWriter(path, append))
            {
                if (mesh != null)
                {
                    streamWriter.WriteLine(description);
                    for (int i = 0; i < mesh.Vertexes.Length; i++)
                    {
                        CameraSpacePoint point = mesh.Vertexes[i];
                        Color color = mesh.Colors[i];
                        streamWriter.WriteLine("v {0} {1} {2} {3} {4} {5} {6}", point.X.ToString("N", this.numberFormatInfo),
                        point.Y.ToString("N", this.numberFormatInfo), point.Z.ToString("N", this.numberFormatInfo), (color.R / 255.0).ToString("N", this.numberFormatInfo),
                        (color.G / 255.0).ToString("N", this.numberFormatInfo), (color.B / 255.0).ToString("N", this.numberFormatInfo), mesh.Dispersions[i].ToString("N", this.numberFormatInfo));
                    }
                        if (triangles)
                        {
                            foreach (int[] triangle in mesh.Triangles)
                            {
                                streamWriter.WriteLine("f {0} {1} {2}", triangle[0], triangle[1], triangle[2]);
                            }
                        }
                    

                }
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
