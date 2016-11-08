using System.Windows.Forms;
using Microsoft.Kinect;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace _3DScanning
{
    class ColoredMeshGenerator : ASingleMeshGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        private const int BYTES_PER_PIXEL = 4;

        /// <summary>
        /// 
        /// </summary>
        private byte[] interpolatedColors;

        /// <summary>
        /// 
        /// </summary>
        private List<byte[]> colorFramesList;

        /// <summary>
        /// 
        /// </summary>
        private Color[] colorPoints;

        /// <summary>
        /// 
        /// </summary>
        private Color[] reorderedColors;

        private byte[] colorPixels;

        private ColorSpacePoint[] csColors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressBar"></param>
        /// <param name="statusText"></param>
        /// <param name="path"></param>
        public ColoredMeshGenerator(ProgressBar progressBar, Label statusText, string path) : base(progressBar, statusText, path)
        {
            this.interpolatedColors = new byte[this.colorFrameDescription.LengthInPixels*BYTES_PER_PIXEL];
            this.colorFramesList = new List<byte[]>();
            this.colorPoints = new Color[this.colorFrameDescription.LengthInPixels];
            this.reorderedColors = new Color[this.colorFrameDescription.LengthInPixels];
            this.colorPixels = new byte[this.depthFrameDescription.LengthInPixels * BYTES_PER_PIXEL];
            this.csColors = new ColorSpacePoint[this.depthFrameDescription.LengthInPixels];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiSourceFrame"></param>
        protected override void ProcessFrame(MultiSourceFrame multiSourceFrame)
        {
            ColorFrameReference colorFrameReference = multiSourceFrame.ColorFrameReference;
            DepthFrameReference depthFrameReference = multiSourceFrame.DepthFrameReference;

            using (ColorFrame colorFrame = colorFrameReference.AcquireFrame())
            {
                using (DepthFrame depthFrame = depthFrameReference.AcquireFrame())
                {
                    if (depthFrame == null || colorFrame == null)
                        return;
                    
                    this.progressBar.PerformStep();
                    this.framesCounter++;
                    ushort[] depthArray = new ushort[this.depthFrameDescription.LengthInPixels];
                    depthFrame.CopyFrameDataToArray(depthArray);
                    byte[] colorArray = new byte[this.colorFrameDescription.LengthInPixels*BYTES_PER_PIXEL];
                    colorFrame.CopyConvertedFrameDataToArray(colorArray, ColorImageFormat.Rgba);                   
                    this.depthFramesList.Add(depthArray);
                    this.colorFramesList.Add(colorArray);
                }
            }
            if (this.framesCounter == this.kinectAttributes.Interpolation)
            {
                this.InterpolateFrames();
                this.progressBar.PerformStep();
                this.ReduceDepthRange(this.interpolatedDepths);
                this.progressBar.PerformStep();
                this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(this.interpolatedDepths, this.csPoints);
                this.kinect.CoordinateMapper.MapCameraPointsToColorSpace(this.csPoints, this.csColors);
                this.progressBar.PerformStep();
                this.MapColorToDepth();
                this.CreateColorsFromRGBA();
                this.progressBar.PerformStep();
                CameraSpacePoint[] transformedDepthPoints = this.CameraToWorldTransfer(this.depthFrameDescription.Width, this.depthFrameDescription.Height);
                this.GenerateMesh(transformedDepthPoints);
                this.statusText.Text = "Mesh byl vygenerován a uložen.";
                this.progressBar.Hide();
                this.progressBar.Value = 0;
                this.depthFramesList.Clear();
                this.colorFramesList.Clear();
                this.framesCounter = 0;
                this.OnFinishedChanged(false);
                this.kinect.RemoveEventHandler(this.Reader_FrameArrived);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateColorsFromRGBA()
        {
            int k = 0;
            for(int i = 0; i<colorPixels.Length; i += BYTES_PER_PIXEL)
            {
                colorPoints[k] = Color.FromArgb(colorPixels[i + 3], colorPixels[i], colorPixels[i+1], colorPixels[i+2]);
                k++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="freeIndex"></param>
        /// <param name="depthWidth"></param>
        /// <param name="depthHeight"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        protected override CameraSpacePoint[] Reorder(int freeIndex, int depthWidth, int depthHeight, int[] indices)
        {
            CameraSpacePoint[] reordered = new CameraSpacePoint[freeIndex];
            for (int i = 0; i < (depthWidth * depthHeight); i++)
            {
                if (indices[i] > 0)
                {
                    reordered[indices[i]] = this.csPoints[i];
                    this.reorderedColors[indices[i]] = this.colorPoints[i];
                }
            }
            return reordered;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        protected override void GenerateMesh(CameraSpacePoint[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                CameraSpacePoint point = points[i];
                Color color = this.reorderedColors[i];
                this.streamWriter.WriteLine("v {0} {1} {2} {3} {4} {5}", point.X.ToString(this.numberFormatInfo),
                    point.Y.ToString(this.numberFormatInfo), point.Z.ToString(this.numberFormatInfo), (color.R / 255.0).ToString(this.numberFormatInfo),
                    (color.G / 255.0).ToString(this.numberFormatInfo), (color.B / 255.0).ToString(this.numberFormatInfo));
            }

            foreach (int[] triangle in this.triangleList)
            {
                this.streamWriter.WriteLine("f {0} {1} {2}", triangle[0], triangle[1], triangle[2]);
            }
            this.streamWriter.Close();
        }
       
        /// <summary>
        /// 
        /// </summary>
        protected override void InterpolateFrames()
        {
            Parallel.For(0,interpolatedColors.Length,index => {
                int averageColor = this.GetAverageValue(index, colorFramesList);
                this.interpolatedColors[index] = (byte)averageColor;
            });

            for (int i = 0; i < this.interpolatedDepths.Length; i++)
            {
                int averageDepth = this.GetAverageValue(i,depthFramesList);
                this.interpolatedDepths[i] = (ushort)averageDepth;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="colorFramesList"></param>
        /// <returns></returns>
        private int GetAverageValue(int i, List<byte[]> colorFramesList)
        {
            int averageValue = 0;
            int zeroCounter = 0;
            foreach (byte[] array in colorFramesList)
            {
                if (array[i] == 0)
                {
                    zeroCounter++;
                }
                averageValue += array[i];
            }
            if (averageValue != 0)
            {
                averageValue /= (colorFramesList.Count - zeroCounter);
            }
            return averageValue;

        }

        /// <summary>
        ///
        /// </summary>
        private void MapColorToDepth()
        {
            for (int depthIndex = 0; depthIndex < this.csPoints.Length; ++depthIndex)
            {
                ColorSpacePoint point = csColors[depthIndex];

                int colorX = (int)Math.Floor(point.X + 0.5);
                int colorY = (int)Math.Floor(point.Y + 0.5);
                
                if ((colorX >= 0) && (colorX < this.colorFrameDescription.Width) && (colorY >= 0) && (colorY < this.colorFrameDescription.Height))
                {
                    int colorImageIndex = ((this.colorFrameDescription.Width * colorY) + colorX) * BYTES_PER_PIXEL;
                    int depthPixel = depthIndex * BYTES_PER_PIXEL;

                    this.colorPixels[depthPixel] = this.interpolatedColors[colorImageIndex];
                    this.colorPixels[depthPixel + 1] = this.interpolatedColors[colorImageIndex + 1];
                    this.colorPixels[depthPixel + 2] = this.interpolatedColors[colorImageIndex + 2];
                    this.colorPixels[depthPixel + 3] = 255;
                }
            }
        }
    }
}
