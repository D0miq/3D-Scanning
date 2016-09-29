using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows;
using OpenTK;


namespace OpenTKLib
{
    public class UtilsPointCloudIO
    {
        public static int BYTES_PER_PIXEL = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        public static System.Globalization.CultureInfo CultureInfo = new System.Globalization.CultureInfo("en-US");

        /// <summary>
        /// reads the OBJ file ONLY with the special format used also in the write_OBJ method
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileNameShort"></param>
        /// <param name="depthData"></param>
        /// <returns></returns>
        public static void FromPLYFile(string path, string fileNameShort, int width, int height, ref ushort[,] depth, ref byte[,] colorInfoR, ref byte[,] colorInfoG, ref byte[,] colorInfoB, ref byte[,] colorInfoA)
        {
            string fileName = path + "\\" + fileNameShort;
            if (!System.IO.File.Exists(fileName))
            {
                System.Diagnostics.Debug.WriteLine("File does not exist: ");
                return;

            }

            ushort[] lineData = new ushort[7];
            System.Collections.ArrayList lineList = new System.Collections.ArrayList();

            depth = new ushort[width, height];
            colorInfoR = new byte[width, height];
            colorInfoG = new byte[width, height];
            colorInfoB = new byte[width, height];
            colorInfoA = new byte[width, height];


            int numberOfDepthPointsNonZero = -1;

       

            try
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);
                //the first 12 lines are meta data information, which I already know , see also comment on this method
                int nCount =  lines.GetLength(0);
                for (int i = 13; i < nCount; i++)
                {
                    string[] arrStr1 = lines[i].Split(new Char[] { ' ' });
                    if (arrStr1.Length < 6)
                    {
                        System.Windows.Forms.MessageBox.Show("Error reading file " + fileName + " in line i");
                    }
                    else
                    {

                        for (int j = 0; j < 6; j++)
                        {
                            lineData[j] = Convert.ToUInt16(arrStr1[j]);
                        }
                        depth[lineData[0], lineData[1]] = lineData[2];
                        colorInfoR[lineData[0], lineData[1]] = Convert.ToByte(lineData[3]);
                        colorInfoG[lineData[0], lineData[1]] = Convert.ToByte(lineData[4]);
                        colorInfoB[lineData[0], lineData[1]] = Convert.ToByte(lineData[5]);
                        colorInfoA[lineData[0], lineData[1]] = 255;

                        if (depth[lineData[0], lineData[1]] > 0)
                        {
                            numberOfDepthPointsNonZero++;

                        }

                    }
                }
            }
            catch(Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Read_PLY: Read error - e.g. cannot be found:  " + fileName + ": " + err.Message);
            }

            
          
        }
        /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ToPLYFile(byte[] colorInfoPixels, ushort[] depthData, int width, int height, string path, string fileName)
        {
            StringBuilder sb = new StringBuilder();

            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

            List<string> lines = new List<string>();
            lines.Add("ply");
            lines.Add("format ascii 1.0");
            lines.Add("comment VCGLIB generated");
            lines.Add("element vertex HAVE TO REPLACE THIS LATER!!");

            lines.Add("property double x");
            lines.Add("property double y");
            lines.Add("property double z");
            lines.Add("property uchar red");
            lines.Add("property uchar green");
            lines.Add("property uchar blue");
            //lines.Add("property uchar alpha");
            lines.Add("element face 0");
            lines.Add("property list uchar int vertex_indices");
            lines.Add("end_header");


            int iIndex = 0;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int depthIndex = (y * width) + x;
                    int displayIndex = depthIndex * BYTES_PER_PIXEL;
                    if (colorInfoPixels[displayIndex + 3] != 0 && depthData[depthIndex] != 0)
                    {

                        iIndex++;
                        string coordinate = x.ToString() + " " + y.ToString(CultureInfo) + " " + depthData[depthIndex].ToString(CultureInfo);
                        string color = colorInfoPixels[displayIndex].ToString(CultureInfo) + " " + colorInfoPixels[displayIndex + 1].ToString(CultureInfo) + " " + colorInfoPixels[displayIndex + 2].ToString(CultureInfo) + " ";

                        //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                        lines.Add(coordinate + " " + color);

                    }

                }
            }

            lines[3] = "element vertex " + iIndex.ToString();

            System.IO.File.WriteAllLines(path + "\\" + fileName, lines);
            
            return true;

        }
        public static bool ToXYZFile(List<Vector3d> listOfPoints, string fileNameShort, string path)
        {
            string[] lines = new string[listOfPoints.Count];
            for (int i = 0; i < listOfPoints.Count; i++)
            {
                lines[i] = listOfPoints[i].X.ToString("0.0", CultureInfo) + " " + listOfPoints[i].Y.ToString("0.0", CultureInfo) + " " + listOfPoints[i].Z.ToString("0.0", CultureInfo);

            }
                 
            System.IO.File.WriteAllLines(path + "\\" + fileNameShort, lines);

            return true;

        }
        public static bool ToXYZFile(int width, int height, ushort[] uArray, string fileName, string path)
        {
            string[] lines = new string[uArray.GetLength(0)];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int depthIndex = (j * width) + i;
                    lines[depthIndex] = i.ToString() + " " + j.ToString() + " " + uArray[depthIndex].ToString();

                }
            }
            System.IO.File.WriteAllLines(path + "\\" + fileName, lines);

            return true;

        }
    
        public static List<Vector3> FromXYZ_Vectors(string fileName)
        {
            List<Vector3> listOfPoints = new List<Vector3>();
            int i = 0;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);

                for (i = 0; i < lines.GetLength(0); i++)
                {
                    string[] arrStr1 = lines[i].Split(new Char[] { ' ' });
                    Vector3 p3D = new Vector3(Convert.ToSingle(arrStr1[0], CultureInfo), Convert.ToSingle(arrStr1[1], CultureInfo), Convert.ToSingle(arrStr1[2], CultureInfo));

                    listOfPoints.Add(p3D);


                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Read_XYZ: read error in file :  " + fileName + " : at line: " + i.ToString() + " ; " + err.Message);
            }

            return listOfPoints;
        }
        public static List<Vector3> Read_XYZ_Vectors(string path, string fileName)
        {
            string fileNameLong = path + "\\" + fileName;
            return FromXYZ_Vectors(fileNameLong);

        }

        /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ToObjFile(byte[] colorInfoPixels, ushort[] depthData, int width, int height, string path, string fileName)
        {
            StringBuilder sb = new StringBuilder();



            List<string> lines = new List<string>();
            lines.Add("####");
            lines.Add("#");
            lines.Add("# OBJ File Generated by ScannerWPF");
            lines.Add("#");
            lines.Add("####");
            lines.Add("#");
            lines.Add("# Vertices: " + depthData.Length.ToString());
            lines.Add("#");
            lines.Add("####");


            int iIndex = 0;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int depthIndex = (y * width) + x;
                    int displayIndex = depthIndex * BYTES_PER_PIXEL;
                    if (colorInfoPixels[displayIndex + 3] != 0 && depthData[depthIndex] != 0)
                    {

                        iIndex++;
                        string coordinate = x.ToString(CultureInfo) + " " + y.ToString(CultureInfo) + " " + depthData[depthIndex].ToString(CultureInfo);
                        string color = (colorInfoPixels[displayIndex] / 255F).ToString(CultureInfo) + " " + (colorInfoPixels[displayIndex + 1] / 255F).ToString(CultureInfo) + " " + (colorInfoPixels[displayIndex + 2] / 255F).ToString(CultureInfo) + " ";

                        //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                        lines.Add("v " + coordinate + " " + color);

                    }

                }
            }


            System.IO.File.WriteAllLines(path + "\\" + fileName, lines);

            return true;

        }
        /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ToObjFile(List<Vector3> listVector3, string path, string fileName)
        {
            StringBuilder sb = new StringBuilder();



            List<string> lines = new List<string>();
            lines.Add("####");
            lines.Add("#");
            lines.Add("# OBJ File Generated by OpenTKLib");
            lines.Add("#");
            lines.Add("####");
            lines.Add("#");
            lines.Add("# Vertices: " + listVector3.Count.ToString());
            lines.Add("#");
            lines.Add("####");

            for (int i = 0; i < listVector3.Count; ++i)
            {

                Vector3 v = listVector3[i];
              
                string coordinate = v.X.ToString(CultureInfo) + " " + v.Y.ToString(CultureInfo) + " " + v.Z.ToString(CultureInfo);
                string color = string.Empty;
              
                //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                lines.Add("v " + coordinate + " " + color);

            }


            System.IO.File.WriteAllLines(path + "\\" + fileName, lines);

            return true;

        }
        /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ToObjFile(PointCloudVertices pc, string path, string fileName)
        {
            StringBuilder sb = new StringBuilder();



            List<string> lines = new List<string>();
            lines.Add("####");
            lines.Add("#");
            lines.Add("# OBJ File Generated by OpenTKLib");
            lines.Add("#");
            lines.Add("####");
            lines.Add("#");
            lines.Add("# Vertices: " + pc.Count.ToString());
            lines.Add("#");
            lines.Add("####");

            for (int i = 0; i < pc.Count; ++i)
            {

                Vector3d v = pc[i].Vector;
                float[] colorVal = pc[i].Color.ToFloats();
                string coordinate = v.X.ToString(CultureInfo) + " " + v.Y.ToString(CultureInfo) + " " + v.Z.ToString(CultureInfo);
                string color = string.Empty;
                if (colorVal != null)
                {
                    // 255F
                    color = colorVal[0].ToString(CultureInfo) + " " + colorVal[1].ToString(CultureInfo) + " " + colorVal[2].ToString(CultureInfo) + " ";
                }
                //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                lines.Add("v " + coordinate + " " + color);

            }


            System.IO.File.WriteAllLines(path + "\\" + fileName, lines);

            return true;

        }

          /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ToObjFile(PointCloud pc, string fileNameWithPath)
        {
            StringBuilder sb = new StringBuilder();



            List<string> lines = new List<string>();
            lines.Add("####");
            lines.Add("#");
            lines.Add("# OBJ File Generated by OpenTKLib");
            lines.Add("#");
            lines.Add("####");
            lines.Add("#");
            lines.Add("# Vertices: " + pc.Vectors.GetLength(0).ToString());
            lines.Add("#");
            lines.Add("####");

            if (pc.Colors != null && pc.Colors.GetLength(0) == pc.Vectors.GetLength(0))
            {


                for (int i = 0; i < pc.Vectors.GetLength(0); ++i)
                {

                    Vector3 v = pc.Vectors[i];
                    Vector3 colorVal = pc.Colors[i];
                    string coordinate = v.X.ToString(CultureInfo) + " " + v.Y.ToString(CultureInfo) + " " + v.Z.ToString(CultureInfo);
                    string color = string.Empty;
                    if (colorVal != null)
                    {
                        // 255F
                        color = colorVal[0].ToString(CultureInfo) + " " + colorVal[1].ToString(CultureInfo) + " " + colorVal[2].ToString(CultureInfo) + " ";
                    }
                    //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                    lines.Add("v " + coordinate + " " + color);

                }
            }
            else
                for (int i = 0; i < pc.Vectors.GetLength(0); ++i)
                {

                    Vector3 v = pc.Vectors[i];

                    string coordinate = v.X.ToString(CultureInfo) + " " + v.Y.ToString(CultureInfo) + " " + v.Z.ToString(CultureInfo);

                    //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                    lines.Add("v " + coordinate);

                }


            System.IO.File.WriteAllLines(fileNameWithPath, lines);
            return true;
        }
        /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ToObjFile(Vector2[] pc, string fileNameWithPath)
        {
            StringBuilder sb = new StringBuilder();



            List<string> lines = new List<string>();
            lines.Add("####");
            lines.Add("#");
            lines.Add("# OBJ File Generated by OpenTKLib");
            lines.Add("#");
            lines.Add("####");
            lines.Add("#");
            lines.Add("# Vertices: " + pc.Length.ToString());
            lines.Add("#");
            lines.Add("####");


            for (int i = 0; i < pc.Length; ++i)
            {

                Vector2 v = pc[i];

                string coordinate = v.X.ToString(CultureInfo) + " " + v.Y.ToString(CultureInfo) + " 1.";

                //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                lines.Add("v " + coordinate);

            }


            System.IO.File.WriteAllLines(fileNameWithPath, lines);
            return true;
        }
        /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ToObjFile(PointCloud pc, string path, string fileName)
        {
            
            ToObjFile(pc, path + "\\" + fileName);
            return true;

        }
        /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool Write_OBJ_Test(PointCloud pc, PointCloud pcTest, string path, string fileName)
        {
            StringBuilder sb = new StringBuilder();



            List<string> lines = new List<string>();
            lines.Add("####");
            lines.Add("#");
            lines.Add("# OBJ File Generated by OpenTKLib");
            lines.Add("#");
            lines.Add("####");
            lines.Add("#");
            lines.Add("# Vertices: " + pc.Vectors.GetLength(0).ToString());
            lines.Add("#");
            lines.Add("####");

            if (pc.Colors != null && pc.Colors.GetLength(0) == pc.Vectors.GetLength(0))
            {


                for (int i = 0; i < pc.Vectors.GetLength(0); ++i)
                {

                    Vector3 v = pc.Vectors[i];
                    Vector3 vTest = pcTest.Vectors[i];
                    Vector3 colorVal = pc.Colors[i];
                    string coordinate = v.X.ToString(CultureInfo) + " " + v.Y.ToString(CultureInfo) + " " + v.Z.ToString(CultureInfo);
                    string color = string.Empty;
                    if (colorVal != null)
                    {
                        // 255F
                        color = colorVal[0].ToString(CultureInfo) + " " + colorVal[1].ToString(CultureInfo) + " " + colorVal[2].ToString(CultureInfo) + " ";
                    }
                    //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                    lines.Add("v " + coordinate + " " + color);

                    string coordinate1 = vTest.X.ToString(CultureInfo) + " " + vTest.Y.ToString(CultureInfo) + " " + vTest.Z.ToString(CultureInfo);
                    lines.Add("- " + coordinate1);


                }
            }
            else
                for (int i = 0; i < pc.Vectors.GetLength(0); ++i)
                {

                    Vector3 v = pc.Vectors[i];
                    Vector3 vTest = pcTest.Vectors[i];
                    

                    string coordinate = v.X.ToString(CultureInfo) + " " + v.Y.ToString(CultureInfo) + " " + v.Z.ToString(CultureInfo);
                    string coordinate1 = vTest.X.ToString(CultureInfo) + " " + vTest.Y.ToString(CultureInfo) + " " + vTest.Z.ToString(CultureInfo);

                    //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                    lines.Add("v " + coordinate);
                    lines.Add("-" + coordinate1);


                }


            System.IO.File.WriteAllLines(path + "\\" + fileName, lines);

            return true;

        }
        /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ToObjFile(byte[] colorInfoPixels, double[] depthData, int width, int height, string path, string fileName)
        {
            StringBuilder sb = new StringBuilder();



            List<string> lines = new List<string>();
            lines.Add("####");
            lines.Add("#");
            lines.Add("# OBJ File Generated by OpenTKLib");
            lines.Add("#");
            lines.Add("####");
            lines.Add("#");
            lines.Add("# Vertices: " + depthData.Length.ToString());
            lines.Add("#");
            lines.Add("####");


            int iIndex = 0;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int depthIndex = (y * width) + x;
                    int displayIndex = depthIndex * BYTES_PER_PIXEL;
                    if (colorInfoPixels[displayIndex + 3] != 0 && depthData[depthIndex] != 0)
                    {

                        iIndex++;
                        string coordinate = x.ToString(CultureInfo) + " " + y.ToString(CultureInfo) + " " + depthData[depthIndex].ToString(CultureInfo);
                        string color = (colorInfoPixels[displayIndex] / 255F).ToString(CultureInfo) + " " + (colorInfoPixels[displayIndex + 1] / 255F).ToString(CultureInfo) + " " + (colorInfoPixels[displayIndex + 2] / 255F).ToString(CultureInfo) + " ";

                        //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                        lines.Add("v " + coordinate + " " + color);

                    }

                }
            }


            System.IO.File.WriteAllLines(path + "\\" + fileName, lines);

            return true;

        }
        /// <summary>
        /// reads the OBJ file ONLY with the special format used also in the write_OBJ method
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileNameShort"></param>
        /// <param name="depthData"></param>
        /// <returns></returns>
        public static void FromObbjFile(string path, string fileNameShort, int width, int height, ref ushort[,] depth, ref byte[,] colorInfoR, ref byte[,] colorInfoG, ref byte[,] colorInfoB, ref byte[,] colorInfoA)
        {
            string fileName = path + "\\" + fileNameShort;
            if (!System.IO.File.Exists(fileName))
            {
                System.Diagnostics.Debug.WriteLine("File does not exist: ");
                return ;

            }

            ushort[] p3D = new ushort[3];
            System.Collections.ArrayList lineList = new System.Collections.ArrayList();

            depth = new ushort[width, height];
            colorInfoR = new byte[width, height];
            colorInfoG = new byte[width, height];
            colorInfoB = new byte[width, height];
            colorInfoA = new byte[width, height];



            int numberOfDepthPointsNonZero = -1;
            int i = 0;
            int startIndex = 0;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);

                //ignore the comment lines
                
                for (i = 0; i < lines.Length; i++)
                {
                    if (!lines[i].Contains('#'))
                    {
                        startIndex = i;
                        break;
                    }

                }

                for ( i = startIndex + 1; i < lines.GetLength(0); i++)
                {
                    
                    string[] arrStr1 = lines[i].Split(new Char[] { ' ' });
                    if (lines[i].Contains('#') || arrStr1.Length < 4)
                    {
                        //ignore empty and comment lines
                        continue;
                    }
                    //if (arrStr1.Length < 4)
                    //{
                    //    MessageBox.Show("Error reading file " + fileName + " in line : " + i.ToString() + " : " + lines[i]);
                    //}
                    if (arrStr1[0] == "vn")
                    {
                        //ignore vector normals for now
                    }
                    if (arrStr1[0] == "v")
                    {
                        if (arrStr1.Length < 7)
                        {
                            System.Windows.Forms.MessageBox.Show("Error reading file " + fileName + " in line i");
                        }
                        else
                        {

                            for (int j = 0; j < 3; j++)
                            {
                                double f = Convert.ToSingle(arrStr1[j + 1], CultureInfo);
                                p3D[j] = Convert.ToUInt16(f);
                            }
                            depth[p3D[0], p3D[1]] = p3D[2];


                            colorInfoR[p3D[0], p3D[1]] = Convert.ToByte(255 * Convert.ToSingle(arrStr1[4], CultureInfo));
                            colorInfoG[p3D[0], p3D[1]] = Convert.ToByte(255 * Convert.ToSingle(arrStr1[5], CultureInfo));
                            colorInfoB[p3D[0], p3D[1]] = Convert.ToByte(255 * Convert.ToSingle(arrStr1[6], CultureInfo));

                            colorInfoA[p3D[0], p3D[1]] = 255;

                            if (depth[p3D[0], p3D[1]] > 0)
                            {
                                numberOfDepthPointsNonZero++;

                            }

                        }
                    }                    
                   
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Read_OBJ: read error in file :  " + fileName + " : at line: " + i.ToString() + " ; " + err.Message);
            }


        }
        
        /// <summary>
        /// reads the OBJ file ONLY with the special format used also in the write_OBJ method
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileNameShort"></param>
        /// <param name="depthData"></param>
        /// <returns></returns>
        public static PointCloudVertices FromObjFile(string path, string fileNameShort)
        {
            string fileName = path + "\\" + fileNameShort;
            if (!System.IO.File.Exists(fileName))
            {
                System.Diagnostics.Debug.WriteLine("File does not exist: ");
                return null;

            }
            double[] p3D = new double[3];
            System.Collections.ArrayList lineList = new System.Collections.ArrayList();


            PointCloudVertices pointCloud = new PointCloudVertices();


            //int numberOfDepthPointsNonZero = -1;
            int i = 0;
            int startIndex = 0;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);

                //ignore the comment lines

                for (i = 0; i < lines.Length; i++)
                {
                    if (!lines[i].Contains('#'))
                    {
                        startIndex = i;
                        break;
                    }

                }

                for (i = startIndex; i < lines.GetLength(0); i++)
                //for (i = lines.Length -1 ; i >= startIndex + 1; i--)
                {

                    string[] arrStr1 = lines[i].Split(new Char[] { ' ' });
                    if (lines[i].Contains('#') || arrStr1.Length < 4)
                    {
                        //ignore empty and comment lines
                        continue;
                    }
                  
                    if (arrStr1[0] == "vn")
                    {
                        //ignore vector normals for now
                    }
                    if (arrStr1[0] == "v")
                    {
                        if (arrStr1.Length < 7)
                        {
                            System.Windows.Forms.MessageBox.Show("Error reading file " + fileNameShort + " in line i");
                        }
                        else
                        {

                            for (int j = 0; j < 3; j++)
                            {
                                p3D[j] = Convert.ToSingle(arrStr1[j + 1], CultureInfo);
                            }
                            Vector3d vec = new Vector3d(p3D[0], p3D[1], p3D[2]);
                            float[] color = new float[4] {Convert.ToSingle(arrStr1[4], CultureInfo), Convert.ToSingle(arrStr1[5], CultureInfo), Convert.ToSingle(arrStr1[6], CultureInfo), 1f};
                            System.Drawing.Color c = System.Drawing.Color.White;
                            c = c.FromFloatsARGB(color[3], color[0], color[1], color[2]);

                            Vertex v = new Vertex(vec, c);
                            pointCloud.Add(v);
                           

                            //if (vec.Z > 0)
                            //{
                            //    numberOfDepthPointsNonZero++;

                            //}

                        }
                    }

                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Read_OBJ: read error in file :  " + fileNameShort + " : at line: " + i.ToString() + " ; " + err.Message);
            }

            return pointCloud;

        }
        /// <summary>
        /// write ply file from depth data and colorInfoPixels (color info)
        /// </summary>
        /// <param name="colorInfoPixels"></param>
        /// <param name="depthData"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ToObjFile(List<byte[]> depthData, string path, string fileName)
        {
            StringBuilder sb = new StringBuilder();



            List<string> lines = new List<string>();
          
            lines.Add("# Number of vectors: " + depthData.Count.ToString());
        

            int iIndex = 0;

            for (int i = 0; i < depthData.Count; ++i)
            {

                iIndex++;
                string coordinate = depthData[i][0].ToString(CultureInfo) + " " + depthData[i][1].ToString(CultureInfo) + " " + depthData[i][2].ToString(CultureInfo);

                //string color = pixels[displayIndex].ToString() + " " + pixels[displayIndex + 1].ToString() + " " + pixels[displayIndex + 2].ToString() + " " + pixels[displayIndex + 3].ToString();
                lines.Add(coordinate + "   ");
               // lines.Add("v " + coordinate + " ");


            }





            System.IO.File.WriteAllLines(path + "\\" + fileName, lines);

            return true;

        }
    }
}
