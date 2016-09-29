// Pogramming by
//     Edgar Maass: (email: maass@logisel.de)
//               
//
//Software used: 
//    OpenGL : http://www.opengl.org
//    OpenTK : http://www.opentk.com
//
// DISCLAIMER: Users rely upon this software at their own risk, and assume the responsibility for the results. Should this software or program prove defective, 
// users assume the cost of all losses, including, but not limited to, any necessary servicing, repair or correction. In no event shall the developers or any person 
// be liable for any loss, expense or damage, of any type or nature arising out of the use of, or inability to use this software or program, including, but not
// limited to, claims, suits or causes of action involving alleged infringement of copyrights, patents, trademarks, trade secrets, or unfair competition. 
//


using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using OpenTK;
using System.Windows.Forms;

namespace OpenTKLib
{
    public class IOUtils
    {
        public static PointCloudVertices ReadXYZFile_ToVertices(string fileNameLong, bool rotatePoints)
        {
            List<Vector3d> listVector3d = ReadXYZFile(fileNameLong, rotatePoints);
            PointCloudVertices listVertices = PointCloudVertices.FromVectors(listVector3d);

            return listVertices;

        }
        public static List<Vector3d> ReadXYZFile(string fileNameLong, bool rotatePoints)
        {

            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(fileNameLong);
            }
            catch(Exception err)
            {
                MessageBox.Show("ReadXYZFile: Read error - e.g. cannot be found:  " + fileNameLong + ": " + err.Message);
                return null;
            }
            return ConvertLinesToVector3d(lines, rotatePoints);
        }
        private static List<Vector3d> ConvertLinesToVector3d(string[] lines, bool rotatePoints)
        {
            List<Vector3d> listOfVectors = new List<Vector3d>();

            int nCount = lines.GetLength(0);
            for (int i = 0; i < nCount; i++)
            {
                string[] arrStr1 = lines[i].Split(new Char[] { ' ' });
                try
                {

                    if (arrStr1.GetLength(0) > 2)
                        listOfVectors.Add(new Vector3d(Convert.ToSingle(arrStr1[0], GlobalVariables.CurrentCulture), Convert.ToSingle(arrStr1[1], GlobalVariables.CurrentCulture), Convert.ToSingle(arrStr1[2], GlobalVariables.CurrentCulture)));

                }
                catch(Exception err)
                {
                    MessageBox.Show("Error parsing file at line: " + i.ToString() + " : " + err.Message);
                }

            }

            //if (rotatePoints)
            //{
            //    listOfVectors = RotatePointCloud(listOfVectors);
            //}


            return listOfVectors;
        }

        /// <summary>
        /// Reads only position and color information (No normals, texture, triangles etc. etc)
        /// </summary>
        /// <param name="fileOBJ"></param>
        /// <param name="myNewModel"></param>
        public static PointCloudVertices ReadObjFile_ToPointCloud(string fileOBJ)
        {
            PointCloudVertices myPCL = new PointCloudVertices();
            string line = string.Empty;
            int indexInModel = -1;
            try
            {

                using (StreamReader streamReader = new StreamReader(fileOBJ))
                {
                    //Part p = new Part();
                    Vertex vertex = new Vertex();
                    //myNewModel.Part = new List<Part>();
                    while (!streamReader.EndOfStream)
                    {
                        line = streamReader.ReadLine().Trim();
                        while (line.EndsWith("\\"))
                            line = line.Substring(0, line.Length - 1) + streamReader.ReadLine().Trim();
                        string str1 = GlobalVariables.TreatLanguageSpecifics(line);
                        string[] strArrayRead = str1.Split();
                        if (strArrayRead.Length >= 0)
                        {
                            switch (strArrayRead[0].ToLower())
                            {
                                //case "mtllib":
                                //    if (strArrayRead.Length < 2)
                                //    {
                                //        System.Windows.Forms.MessageBox.Show("Error reading obj file in line : " + line);
                                //    }

                                //    myNewModel.GetTexture(strArrayRead[1], fileOBJ);
                                //    break;
                                case "v"://Vertex
                                    vertex = HelperReadVertex(strArrayRead);
                                    indexInModel++;
                                    vertex.IndexInModel = indexInModel;
                                    myPCL.Add(vertex);
                                    break;
                           
                            }
                        }
                    }
            
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error reading obj file - Vertices: " + line + " ; " + err.Message);
            }
            return myPCL;

        }
        public static Vertex HelperReadVertex(string[] strArrayRead)
        {

            Vertex vertex = new Vertex();
            vertex.Vector = new Vector3d(0, 0, 0);
           
            if (strArrayRead.Length > 3)
            {
                //double dx, dy, dz;
                float f;
                float.TryParse(strArrayRead[1], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out f);
                vertex.Vector.X = f;
                float.TryParse(strArrayRead[2], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out f);
                vertex.Vector.Y = f;
                float.TryParse(strArrayRead[3], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out f);
                vertex.Vector.Z = f;
            
            }

            vertex.Color = System.Drawing.Color.White;
            double fOutValue = 0f;
            byte r,g,b, a;
            a = 255;
            if (strArrayRead.Length > 7)
            {
                double.TryParse(strArrayRead[7], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out fOutValue);
                a = Convert.ToByte(fOutValue * 255);
                
            }

            if (strArrayRead.Length > 6)
            {
                //we have vertex AND color infos
                //double colorR = 
                double.TryParse(strArrayRead[4], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out fOutValue);
                r = Convert.ToByte(fOutValue * 255);
                double.TryParse(strArrayRead[5], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out fOutValue);
                g = Convert.ToByte(fOutValue * 255);
                double.TryParse(strArrayRead[6], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out fOutValue);
                b = Convert.ToByte(fOutValue * 255);
                vertex.Color = System.Drawing.Color.FromArgb(a, r, g, b);
            }



            return vertex;
        }
        public static void HelperReadVector3dAndColor(string[] strArrayRead, out Vector3 vector, out Vector3 color)
        {

            vector = new Vector3();
            color = new Vector3(1f, 1f, 1f);
           
            if (strArrayRead.Length > 3)
            {
                //double dx, dy, dz;
                float f;
                float.TryParse(strArrayRead[1], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out f);
                vector.X = f;
                float.TryParse(strArrayRead[2], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out f);
                vector.Y = f;
                 float.TryParse(strArrayRead[3], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out f);
                 vector.Z = f;
            }

            
            double fOutValue = 0f;
            double r, g, b;
            
            if (strArrayRead.Length > 7)
            {
                double.TryParse(strArrayRead[7], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out fOutValue);
                //a = Convert.ToByte(fOutValue * 255);

            }

            if (strArrayRead.Length > 6)
            {
                //we have vertex AND color infos
                //double colorR = 
                double.TryParse(strArrayRead[4], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out r);
                double.TryParse(strArrayRead[5], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out g);
                double.TryParse(strArrayRead[6], NumberStyles.Float | NumberStyles.AllowThousands, GlobalVariables.CurrentCulture, out b);
                color = new Vector3(r, g, b);
                
            }


        }
        public static Bitmap ReadTexture(string TexFile, string OBJFile)
        {
            Bitmap textureBitmap = null;
            try
            {
                string[] strArray1 = OBJFile.Split('\\');
                string str = "";
                TexFile = TexFile.Replace(",", ".");
                for (int index = strArray1.Length - 2; index >= 0; --index)
                    str = strArray1[index] + "\\" + str;
                using (StreamReader streamReader = new StreamReader(str + TexFile))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine().Trim();
                        while (line.EndsWith("\\"))
                            line = line.Substring(0, line.Length - 1) + streamReader.ReadLine().Trim();
                        string[] strArray2 = GlobalVariables.TreatLanguageSpecifics(line).Split();
                        if (strArray2[0].ToLower() == "map_kd")
                            textureBitmap = new System.Drawing.Bitmap(str + strArray2[1].Replace(",", "."));
                    }
                    streamReader.Close();
                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Err :  " + err.Message);
            }
            return textureBitmap;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strArrayRead"></param>
        /// <param name="myNewModel"></param>
        /// <returns></returns>
        public static void ReadIndicesLine(string[] strArrayRead, List<uint> indices, List<uint> normalIndices, List<uint> textureIndices)
        {
            //indices = new List<uint>();
            //normalIndices = new List<uint>();
            //textureIndices = new List<uint>();

            try
            {
                

                foreach (string strElement in strArrayRead)
                {
                    if (strElement.ToLower() != "f")
                    {
                        try
                        {
                            
                            string[] strSubArr = strElement.Split('/');
                            int result;
                            int.TryParse(strSubArr[0], out result);
                            indices.Add(Convert.ToUInt32(result - 1));

                          
                            if (strSubArr.Length > 2)
                            {
                                int.TryParse(strSubArr[strSubArr.Length - 1], out result);
                                normalIndices.Add(Convert.ToUInt32(result -1));

                                if (strSubArr[strSubArr.Length - 2] != "")
                                {
                                    int.TryParse(strSubArr[strSubArr.Length - 2], out result);
                                    int num2 = result - 1;
                                    textureIndices.Add(Convert.ToUInt32(result - 1));
                                   
                                }
                            }
                        }
                        catch (Exception err1)
                        {
                            System.Windows.Forms.MessageBox.Show("Error reading obj file (triangles)  " + err1.Message);
                            
                        }
                    }
                }

             
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error reading obj file (triangles)  " + err.Message);
               
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strArrayRead"></param>
        /// <param name="myNewModel"></param>
        /// <returns></returns>
        //public static Triangle helper_ReadTriangle(string[] strArrayRead, Model myNewModel)
        //{
        //    try
        //    {
        //        Triangle a = new Triangle();

        //        foreach (string str2 in strArrayRead)
        //        {
        //            if (str2.ToLower() != "f")
        //            {
        //                try
        //                {
        //                    // the index vertex starts with 1 !!
        //                    string[] strArray2 = str2.Split('/');
        //                    int result;
        //                    int.TryParse(strArray2[0], out result);
        //                    a.IndVertices.Add(result - 1);

        //                    int indVertex = a.IndVertices.Count - 1;
        //                    int indPart = myNewModel.Parts.Count - 1;
        //                    if (indPart < 0)
        //                        indPart = 0;
        //                    myNewModel.PointCloudVertices[a.IndVertices[indVertex]].IndexTriangles.Add(indVertex);
        //                    myNewModel.PointCloudVertices[a.IndVertices[indVertex]].IndexParts.Add(indPart);
        //                    if (strArray2.Length > 2)
        //                    {
        //                        int.TryParse(strArray2[strArray2.Length - 1], out result);
        //                        int index2 = result - 1;
        //                        if (!double.IsNaN(myNewModel.Normals[index2].X))
        //                        {
        //                            a.IndNormals.Add(index2);
        //                            int num2 = result - 1;
        //                            myNewModel.PointCloudVertices[a.IndVertices[indVertex]].IndexNormals.Add(num2);
        //                        }
        //                        if (strArray2[strArray2.Length - 2] != "")
        //                        {
        //                            int.TryParse(strArray2[strArray2.Length - 2], out result);
        //                            int num2 = result - 1;
        //                            a.IndTextures.Add(num2);
        //                        }
        //                    }
        //                }
        //                catch (Exception err1)
        //                {
        //                    System.Windows.Forms.MessageBox.Show("Error reading obj file (triangles)  " + err1.Message);
        //                    return new Triangle();
        //                }
        //            }
        //        }

        //        return a;
        //    }
        //    catch (Exception err)
        //    {
        //        System.Windows.Forms.MessageBox.Show("Error reading obj file (triangles)  " + err.Message);
        //        return new Triangle();
        //    }

        //}
        public static void ExtractDirectoryAndNameFromFileName(string fileNameIn, ref string fileNameShort, ref string dirName)
        {
            string[] arrSplit = fileNameIn.Split(new Char[] { '\\' }, 100);
            fileNameShort = arrSplit[arrSplit.GetLength(0) - 1];


            System.Text.StringBuilder str = new System.Text.StringBuilder();
            for (int i = 0; i < arrSplit.GetLength(0) - 1; i++)
            {

                str.Append(arrSplit[i] + @"\");
              
            }

            dirName = str.ToString();



        }
        public static string ExtractExtension(string fileNameIn)
        {
            string[] arrSplit = fileNameIn.Split(new Char[] { '.' }, 100);
            if (arrSplit.Length == 1)
                return null;

            string ext = arrSplit[arrSplit.GetLength(0) - 1];

            return ext;
            
        }
        public static string ExtractFileNameWithoutExtension(string fileNameIn)
        {
            string[] arrSplit = fileNameIn.Split(new Char[] { '.' }, 100);
            string ext = arrSplit[0];
                      


            return ext;

        }
        public static string ExtractFileNameShort(string fileNameIn)
        {
            string[] arrSplit = fileNameIn.Split(new Char[] { '\\' }, 100);
            string fileNameShort = arrSplit[arrSplit.GetLength(0) - 1];

            return fileNameShort;
            


        }
    }
}
