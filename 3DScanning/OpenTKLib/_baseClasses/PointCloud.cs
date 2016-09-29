using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.IO;
using System.Globalization;
using System.Drawing;

namespace OpenTKLib
{
   
    public class PointCloud 
    {
        public Vector3[] Vectors;
        public Vector3[] Colors;

        public Vector3[] Normals;
        public uint[] IndicesNormals;
        public uint[] IndicesTexture;
        public Bitmap Texture;

        public List<double[]> TextureCoords = new List<double[]>();
       

        //triangle part
        public uint[] Indices;
        
        //privates

        private Vector3 centroid;
        private Vector3 centroidOld;

        Vector3 boundingBoxMax;
        Vector3 boundingBoxMin;

        bool centroidAndBoundingBoxCalculated;

        public string Path;
        public string FileNameLong;
        public string FileNameShort;

        public PointCloud()
        {
        }
        public PointCloud(int dim)
        {
            Vectors = new Vector3[dim];
            Colors = new Vector3[dim];
            Indices = new uint[dim];

        }

        public PointCloud(List<Vector3> vectors, List<Vector3> colors, List<Vector3> normals, List<uint> indices, List<uint> indicesNormals, List<uint> indicesTexture)
        {
            AssignData(vectors, colors, normals, indices, indicesNormals, indicesTexture);

            

        }
        public Vector3 CentroidVector
        {
            get
            {
                if (!centroidAndBoundingBoxCalculated)
                    CalculateCentroidBoundingBox();
                return centroid;

            }

        }
        public void CalculateCentroidBoundingBox()
        {
            this.CalculateCentroid();
            PointCloud.BoundingBox(this, ref boundingBoxMax, ref boundingBoxMin);
            centroidAndBoundingBoxCalculated = true;
        }
        private Vector3 CalculateCentroid()
        {
            centroid = new Vector3();

            int nCount = Vectors.Length;
            for (int i = 0; i < nCount; i++)
                centroid += Vectors[i];
            centroid /= nCount;
            return centroid;
        }
        public Vector3 BoundingBoxMax
        {
            get
            {
                if (!centroidAndBoundingBoxCalculated)
                    CalculateCentroidBoundingBox();
                return boundingBoxMax;
            }

        }
        public float NormSquaredMax
        {
            get
            {
               float normMax = float.MinValue;
               for(int i = 0; i < this.Vectors.Length; i++)
               {
                   float n = this.Vectors[i].NormSquared();
                   if (n > normMax)
                       normMax = n;
               }
               return normMax;

            }
            

        }
        public float BoundingBoxMaxFloat
        {
            get
            {
                if (!centroidAndBoundingBoxCalculated)
                    CalculateCentroidBoundingBox();
                float f = float.MinValue;
                for (int i = 0; i < 2; i++)
                {
                    f = System.Math.Max(f, System.Math.Abs(this.boundingBoxMin[i]));
                }
                for(int i = 0; i < 2; i++)
                {
                    f = System.Math.Max(f, System.Math.Abs(boundingBoxMax[i]));
                }
               
                return f;
            }

        }
        public float BoundingBoxMinFloat
        {
            get
            {
                if (!centroidAndBoundingBoxCalculated)
                    CalculateCentroidBoundingBox();
                float f = float.MaxValue;
                for (int i = 0; i < 2; i++)
                {
                    f = System.Math.Min(f, System.Math.Abs(this.boundingBoxMin[i]));
                }
                for (int i = 0; i < 2; i++)
                {
                    f = System.Math.Min(f, System.Math.Abs(boundingBoxMax[i]));
                }

                return f;
            }

        }
        public Vector3 BoundingBoxMin
        {
            get
            {
                if (!centroidAndBoundingBoxCalculated)
                    CalculateCentroidBoundingBox();
                return boundingBoxMin;
            }

        }
        //public Vector3 BoundingBoxMinFloat
        //{
        //    get
        //    {
        //        if (!centroidAndBoundingBoxCalculated)
        //            CalculateCentroidBoundingBox();
        //        return boundingBoxMin;
        //    }

        //}
        private static void BoundingBox(PointCloud pointCloud, ref Vector3 maxPoint, ref Vector3 minPoint)
        {

            int nDim = pointCloud.Vectors.Length;
            if (nDim < 1)
                return;

            maxPoint = new Vector3();
            minPoint = new Vector3();

            float xMax = pointCloud.Vectors[0].X;
            float yMax = pointCloud.Vectors[0].Y;
            float zMax = pointCloud.Vectors[0].Z;
            float xMin = pointCloud.Vectors[0].X;
            float yMin = pointCloud.Vectors[0].Y;
            float zMin = pointCloud.Vectors[0].Z;
            for (int i = 0; i < nDim; i++ )
            {
                Vector3 ver = pointCloud.Vectors[i];
                if (ver.X > xMax)
                    xMax = ver.X;
                if (ver.Y > yMax)
                    yMax = ver.Y;
                if (ver.Z > zMax)
                    zMax = ver.Z;
                if (ver.X < xMin)
                    xMin = ver.X;
                if (ver.Y < yMin)
                    yMin = ver.Y;
                if (ver.Z < zMin)
                    zMin = ver.Z;
            }
            maxPoint.X = xMax;
            maxPoint.Y = yMax;
            maxPoint.Z = zMax;

            minPoint.X = xMin;
            minPoint.Y = yMin;
            minPoint.Z = zMin;

        }
        public void ResizeTo1()
        {
            this.CalculateCentroidBoundingBox();

            float d = Math.Max(this.boundingBoxMax.X, boundingBoxMax.Y);
            d = Math.Max(d, boundingBoxMax.Z);
            if (d > 0)
            {
                this.centroid.X /= d;
                this.centroid.Y /= d;
                this.centroid.Z /= d;
                for (int i = 0; i < this.Vectors.GetLength(0); i++)
                {
                    this.Vectors[i].X /= d;
                    this.Vectors[i].Y /= d;
                    this.Vectors[i].Z /= d;
                }
            }

            //recalc again 
            this.CalculateCentroidBoundingBox();


        }
        public Vector3 ResetCentroid(bool centered)
        {
            if (centered)
            {
                //center point cloud to centroid
               // this.centroidAndBoundingBoxCalculated = false;
                this.centroidOld = this.CentroidVector;
                SubtractVectorRef(this, this.CentroidVector);
                //centroid - is now origin
                this.centroid = new Vector3(0, 0, 0);

                this.CalculateCentroidBoundingBox();


            }
            else
            {
                //reset to old center
                if (this.centroidOld != Vector3.Zero)
                {
                    AddVector(this, this.centroidOld);
                    this.CalculateCentroid();//recalcs centrooid

                    this.centroidOld = Vector3.Zero;
                 

                }

            }
            return this.centroidOld;


        }
       
        public static void AddVector(PointCloud pointCloud, Vector3 vToAdd)
        {

            for (int i = 0; i < pointCloud.Vectors.Length; i++)
            {

                Vector3 v = pointCloud.Vectors[i];
                Vector3 translatedV = Vector3.Add(v, vToAdd);
                v = translatedV;
                pointCloud.Vectors[i] = v;
            }

        }
        public static void SubtractVectorRef(PointCloud pointCloud, Vector3 centroid)
        {

            for (int i = 0; i < pointCloud.Vectors.Length; i++)
            {

                Vector3 v = pointCloud.Vectors[i];
                Vector3 translatedV = Vector3.Subtract(v, centroid);
                v = translatedV;
                pointCloud.Vectors[i] = v;
            }

        }
     
        public static void SetIndicesForCubeCorners(PointCloud pcl)
        {
            pcl.Indices = new uint[24];


            pcl.Indices[0] = 0;
            pcl.Indices[1] = 1;
            pcl.Indices[2] = 1;
            pcl.Indices[3] = 5;
            pcl.Indices[4] = 5;
            pcl.Indices[5] = 4;
            pcl.Indices[6] = 4;
            pcl.Indices[7] = 0;

            pcl.Indices[8] = 1;
            pcl.Indices[9] = 2;
            pcl.Indices[10] = 2;
            pcl.Indices[11] =3;
            pcl.Indices[12] = 3;
            pcl.Indices[13] = 0;
            pcl.Indices[14] = 3;
            pcl.Indices[15] = 7;
            pcl.Indices[16] = 7;
            pcl.Indices[17] = 6;
            pcl.Indices[18] = 2;
            pcl.Indices[19] = 6;
            pcl.Indices[20] = 4;
            pcl.Indices[21] = 7;
            pcl.Indices[22] = 5;
            pcl.Indices[23] = 6;


        }
        public void SetColors(List<Vector3> colors)
        {
            this.Colors = new Vector3[colors.Count];
            for(int i = 0; i < colors.Count; i++)
            {
                this.Colors[i] = colors[i];
            }
        }
        public void SetColor(Vector3 color)
        {
            this.Colors = new Vector3[this.Vectors.GetLength(0)];
            for (int i = 0; i < Colors.GetLength(0); i++)
            {
                this.Colors[i] = color;
            }
        }
      
        public PointCloud Clone()
        {
            List<Vector3> colors = null;
            List<Vector3> normals = null;
            List<Vector3> vectors = null;

            List<uint> indices = null;
            List<uint> indicesNormals = null;
            List<uint> indicesTexture = null;

            if(this.Colors != null)
                colors = this.Colors.ToList<Vector3>();
            if (this.Normals != null)
                normals = this.Normals.ToList<Vector3>();
            if (this.Vectors != null)
                vectors = this.Vectors.ToList<Vector3>();
            if (this.Indices != null)
                indices = this.Indices.ToList<uint>();
            if (this.IndicesNormals != null)
                 indicesNormals = this.IndicesNormals.ToList<uint>();
            if (this.IndicesTexture != null)
                indicesTexture = this.IndicesTexture.ToList<uint>();



            //int check1 = vectors.Count;

            PointCloud pc = new PointCloud(vectors, colors, normals, indices, indicesNormals, indicesTexture);
            return pc;

        }
        public static PointCloud FromPointCloud(PointCloud pcOld)
        {
            
            List<Vector3> colors = pcOld.Colors.ToList<Vector3>();
            List<Vector3> normals = pcOld.Normals.ToList<Vector3>();
            List<Vector3> vectors = pcOld.Vectors.ToList<Vector3>();

            List<uint> indices = pcOld.Indices.ToList<uint>();
            List<uint> indicesNormals = pcOld.IndicesNormals.ToList<uint>();
            List<uint> indicesTexture = pcOld.IndicesTexture.ToList<uint>();



            int check1 = vectors.Count;

            PointCloud pc = new PointCloud(vectors, colors, normals, indices, indicesNormals, indicesTexture);
            return pc;

        }
        //public static PointCloud FromRenderableObject(RenderableObject o)
        //{

     
          

        //}
        public static PointCloud FromVector3List(List<Vector3> vectors)
        {
            
            
            List<Vector3> colors = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<uint> indices = new List<uint>();
            List<uint> indicesNormals = new List<uint>();
            List<uint> indicesTexture = new List<uint>();

            int check1 = vectors.Count;
            //System.Diagnostics.Debug.WriteLine("Number of vectors: " + vectors.Count.ToString());
            
            for (uint i = 0; i < vectors.Count; i++)
            {
                indices.Add(i);
                colors.Add(new Vector3(1f, 1f, 1f));
            }
            if (vectors.Count != check1)
            {
                //System.Windows.Forms.MessageBox.Show("Stop camera before saving");
                System.Diagnostics.Debug.WriteLine("SW error - vectors overriden during set: " + check1.ToString() + " should be: " + vectors.Count.ToString());
                return null;
            }
            else
            {
                PointCloud pc = new PointCloud(vectors, colors, normals, indices, indicesNormals, indicesTexture);
                return pc;
            }
            

        }
        public void ToObjFile(string path, string fileName)
        {
            UtilsPointCloudIO.ToObjFile(this, path, fileName);

        }
        public void ToObjFile(string fileNameWithPath)
        {
            UtilsPointCloudIO.ToObjFile(this, fileNameWithPath);

        }
        public PointCloudRenderable ToPointCloudRenderable()
        {
            PointCloudRenderable pcr = new PointCloudRenderable();
            pcr.PointCloud = this;
            return pcr;


        }
       
        private void ReadObjFile(string fileOBJ)
        {
            
            this.FileNameLong = fileOBJ;
            IOUtils.ExtractDirectoryAndNameFromFileName(this.FileNameLong, ref this.FileNameShort, ref this.Path);

            string line = string.Empty;

            Vector3 vector;
            Vector3 color;

            List<Vector3> vectors = new List<Vector3>();
            List<Vector3> colors = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<uint> indices = new List<uint>();
            List<uint> indicesNormals = new List<uint>();
            List<uint> indicesTexture = new List<uint>();

            try
            {

                using (StreamReader streamReader = new StreamReader(fileOBJ))
                {

                    while (!streamReader.EndOfStream)
                    {
                        line = streamReader.ReadLine().Trim();

                        if (!line.StartsWith("#"))
                        {
                            while (line.EndsWith("\\"))
                                line = line.Substring(0, line.Length - 1) + streamReader.ReadLine().Trim();
                            string str1 = GlobalVariables.TreatLanguageSpecifics(line);
                            string[] strArrayRead = str1.Split();
                            if (strArrayRead.Length >= 0)
                            {
                                switch (strArrayRead[0].ToLower())
                                {
                                    case "mtllib":
                                        if (strArrayRead.Length < 2)
                                        {
                                            System.Windows.Forms.MessageBox.Show("Error reading obj file (mtllib) in line : " + line);
                                        }

                                        this.Texture = IOUtils.ReadTexture(strArrayRead[1], fileOBJ);
                                        break;
                                    case "v"://Vertex
                                        IOUtils.HelperReadVector3dAndColor(strArrayRead, out vector, out color);
                                        vectors.Add(vector);
                                        colors.Add(color);


                                        break;
                                    case "vt"://Texture
                                        if (strArrayRead.Length < 3)
                                        {
                                            System.Windows.Forms.MessageBox.Show("Error reading obj file (Texture) in line : " + line);
                                        }
                                        Vector3d vector1 = new Vector3d(0, 0, 0);
                                        double.TryParse(strArrayRead[1], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector1.X);
                                        double.TryParse(strArrayRead[2], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector1.Y);
                                        this.TextureCoords.Add(new double[2] { (double)vector1.X, (double)vector1.Y });
                                        break;
                                    case "vn"://Normals
                                        if (strArrayRead.Length < 4)
                                        {
                                            System.Windows.Forms.MessageBox.Show("Error reading obj file (Normals) in line : " + line);
                                        }
                                        Vector3 vector2 = new Vector3(0, 0, 0);
                                        float.TryParse(strArrayRead[1], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector2.X);
                                        float.TryParse(strArrayRead[2], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector2.Y);
                                        float.TryParse(strArrayRead[3], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector2.Z);
                                        //vector2.NormalizeNew();
                                        normals.Add(vector2);
                                        break;

                                    case "f":
                                        IOUtils.ReadIndicesLine(strArrayRead, indices, indicesNormals, indicesTexture);
                                        break;
                                    case "g":
                                        //if (myNewModel.Triangles.Count > 0)
                                        //{
                                        //    if (myNewModel.TextureBitmap != null)
                                        //    {
                                        //        p.ColorOverall = System.Drawing.Color.FromArgb(1, 1, 1);
                                        //    }
                                        //    else
                                        //    {
                                        //        double r = Convert.ToSingle(0.3 * Math.Cos((double)(23 * myNewModel.Parts.Count)) + 0.5);
                                        //        double g = Convert.ToSingle(0.5f * Math.Cos((double)(17 * myNewModel.Parts.Count + 1)) + 0.5);
                                        //        double b = Convert.ToSingle(0.5f * Math.Cos((double)myNewModel.Parts.Count) + 0.5);


                                        //        p.ColorOverall = System.Drawing.Color.FromArgb(Convert.ToInt32(r * byte.MaxValue), Convert.ToInt32(g * byte.MaxValue), Convert.ToInt32(b * byte.MaxValue));
                                        //    }
                                        //    //p.ColorOverall = myNewModel.TextureBitmap != null ? new Vector3d(1, 1, 1) : new Vector3d(0.3 * Math.Cos((double)(23 * myNewModel.Parts.Count)) + 0.5, 0.5f * Math.Cos((double)(17 * myNewModel.Parts.Count + 1)) + 0.5, 0.5f * Math.Cos((double)myNewModel.Parts.Count) + 0.5);
                                        //    myNewModel.Parts.Add(new Part(p));
                                        //}
                                        //if (strArrayRead.Length > 1)
                                        //    p.Name = str1.Replace(strArrayRead[1], "");
                                        //myNewModel.Triangles.Clear();
                                        break;
                                }
                            }
                        }
                    }

                    streamReader.Close();

                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error reading obj file (general): " + line + " ; " + err.Message);
            }
            if (indices.Count != vectors.Count)
            {
                for (uint i = Convert.ToUInt32(indices.Count); i < vectors.Count; i++)
                {
                    indices.Add(i);

                }
            }
            AssignData(vectors, colors, normals, indices, indicesNormals, indicesTexture);


        }
        public static PointCloud FromObjFile(string fileOBJ)
        {
            PointCloud pc = new PointCloud();
            pc.ReadObjFile(fileOBJ);
            return pc;

        }
        public static PointCloud FromXYZFile(string fileOBJ)
        {
            PointCloud pc = new PointCloud();
            pc.ReadXYZFile(fileOBJ);
            return pc;

        }
        private void ReadXYZFile(string fileName)
        {
            this.FileNameLong = fileName;
            IOUtils.ExtractDirectoryAndNameFromFileName(this.FileNameLong, ref this.FileNameShort, ref this.Path);

            List<Vector3> colors = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<uint> indices = new List<uint>();
            List<uint> indicesNormals = new List<uint>();
            List<uint> indicesTexture = new List<uint>();

            List<Vector3> vectors = UtilsPointCloudIO.FromXYZ_Vectors(fileName);
            for (uint i = 0; i < vectors.Count; i++)
            {
                indices.Add(i);
                colors.Add(new Vector3(1f, 1f, 1f));
            }

            AssignData(vectors, colors, normals, indices, indicesNormals, indicesTexture);

        }
        private void AssignData(List<Vector3> vectors, List<Vector3> colors, List<Vector3> normals, List<uint> indices, List<uint> indicesNormals, List<uint> indicesTexture)
        {
            if (vectors != null)
                this.Vectors = vectors.ToArray();
            if (colors != null)
                this.Colors = colors.ToArray();
            if (normals != null)
                this.Normals = normals.ToArray();
            if (indices != null)
                this.Indices = indices.ToArray();
            if (indicesNormals != null)
                this.IndicesNormals = indicesNormals.ToArray();
            if (indicesTexture != null)
                this.IndicesTexture = indicesTexture.ToArray();

        }
        /// <summary>
        /// x,y and z are the angles in degrees
        /// </summary>
        /// <param name="pointCloud"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void RotateDegrees(float x, float y, float z)
        {
            Matrix3 R = new Matrix3();
            
            R = R.RotationXYZDegrees(x, y, z);
            this.Rotate(R);

        }
        public void Rotate(Matrix3 R)
        {
            for (int i = 0; i < this.Vectors.Length; i++)
            {
                Vector3 v = Vectors[i];
                Vectors[i] = R.MultiplyVector(v);
                // Vector3d v1 = Multiply3x3(R, v);

            }
        }
        public void Scale(float scale)
        {
            Vector3 scaleVector = new Vector3(scale, scale, scale);

            Matrix3 R = Matrix3.Identity;
            R[0, 0] = scaleVector[0];
            R[1, 1] = scaleVector[1];
            R[2, 2] = scaleVector[2];

            Rotate(R);

        }
        public void Translate(double x, double y, double z)
        {
            Vector3 translation = new Vector3(x, y, z);
            for (int i = 0; i < this.Vectors.Length; i++)
            {

                Vector3 v = this.Vectors[i];
                Vector3 translatedV = Vector3.Add(v, translation);
                v = translatedV;
                this.Vectors[i] = v;
            }


        }
        public void Sort_DistanceToCenter()
        {

            List<KeyValuePair<Vector3, Vector3>> listNew = new List<KeyValuePair<Vector3, Vector3>>();

            for(int i = 0; i < this.Vectors.Length; i++)
            {
                KeyValuePair<Vector3, Vector3> k = new KeyValuePair<Vector3, Vector3>(this.Vectors[i], this.Colors[i]);
                listNew.Add(k);

            }

            listNew.Sort(new DistanceComparer());

            for (int i = 0; i < listNew.Count; i++)
            {
                KeyValuePair<Vector3, Vector3> k = listNew[i];
                this.Vectors[i] = k.Key;
                this.Colors[i] = k.Value;


            }


        }

        public PointCloudVertices ToPointCloudVertices()
        {
            PointCloudVertices points = new PointCloudVertices();
            for (int i = 0; i < this.Vectors.GetLength(0); i++)
            {
                points.Add(new Vertex(i, this.Vectors[i].X, this.Vectors[i].Y, this.Vectors[i].Z));
            }

            if (this.Colors != null)
            {
                for (int i = 0; i < this.Colors.GetLength(0); i++)
                {
                    Vector3 col = this.Colors[i];
                    points[i].Color = System.Drawing.Color.FromArgb(255, Convert.ToByte(col.X * 255f), Convert.ToByte(col.Y * 255f), Convert.ToByte(col.Z * 255f));

                }

            }
            points.Path = this.Path;
            points.FilaName = this.FileNameLong;

            return points;

        }
        //extract the custom made calibration object - a model consisting of three axes
        public PointCloud ExtractCalibrationObject()
        {
            PointCloud pcResult = null;
            try
            {
                float rLow = 0.5f;
                float rHigh = 0.85f;

                float gLow = 1f;
                float gHigh = 0.5f;

                float bLow = 1f;
                float bHigh = 0.7f;

                List<Vector3> colorList = new List<Vector3>();
                List<Vector3> vectorList = new List<Vector3>();

                for (int i = 0; i < this.Colors.Length; i++)
                {
                    if (this.Colors[i].X > rHigh && this.Colors[i].Y < gLow && this.Colors[i].Z < bLow)
                    {
                        colorList.Add(new Vector3(1f,0f,0f));
                        vectorList.Add(this.Vectors[i]);
                    }
                    else if (this.Colors[i].X < rLow && this.Colors[i].Y < gLow && this.Colors[i].Z > bHigh)
                    {
                        colorList.Add(new Vector3(0f, 0f, 1f));
                        //colorList.Add(this.Colors[i]);
                        vectorList.Add(this.Vectors[i]);
                    }
                    else if (this.Colors[i].X < rLow && this.Colors[i].Y > gHigh && this.Colors[i].Z < bLow)
                    {
                        colorList.Add(new Vector3(0f,1f,0f));
                        //colorList.Add(this.Colors[i]);
                        vectorList.Add(this.Vectors[i]);
                    }
                    

                }

                List<uint> indicesList = new List<uint>();
                for (uint i = 0; i < colorList.Count; i++)
                    indicesList.Add(i);

                pcResult = new PointCloud(vectorList, colorList, null, indicesList, null, null);
            }
            catch(Exception err)
            {

            }
            return pcResult;



        }

        public static PointCloud CreateSphere_RandomPoints(double cubeSize, int numberOfRandomPoints)
        {
            
            var r = new Random();
            /****** Random Vertices ******/
            List<Vector3> points = new List<Vector3>();
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                var radius = cubeSize * r.NextDouble();
                // if (i < NumberOfVertices / 2) radius /= 2;
                double theta = Convert.ToSingle(2 * Math.PI * r.NextDouble());
                double azimuth = Convert.ToSingle(Math.PI * r.NextDouble());
                double x = Convert.ToSingle(radius * Math.Cos(theta) * Math.Sin(azimuth));
                double y = Convert.ToSingle(radius * Math.Sin(theta) * Math.Sin(azimuth));
                double z = Convert.ToSingle(radius * Math.Cos(azimuth));
                Vector3 vi = new Vector3(x, y, z);
                points.Add(vi);
            }
            PointCloud pointCloud = new PointCloud();
            pointCloud.Vectors = points.ToArray();
            return pointCloud;

        }
        public static PointCloud CreateCube_RandomPointsOnPlanes(double cubeSize, int numberOfRandomPoints)
        {
            
            List<Vector3> points = Example3DModels.Cube_Corners(cubeSize, cubeSize, cubeSize);


            var r = new Random();

            
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                
                var vi = new Vector3(cubeSize / 2, cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2, cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2);
                points.Add(vi);

            }
            for (var i = 0; i < numberOfRandomPoints; i++)
            {

                var vi = new Vector3(-cubeSize / 2, cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2, cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2);
                points.Add(vi);

            }


            for (var i = 0; i < numberOfRandomPoints; i++)
            {

                var vi = new Vector3(cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2, cubeSize / 2, cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2);
                points.Add(vi);

            }
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                
                var vi = new Vector3(
                    cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2,
                    -cubeSize / 2,
                    cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2);
                points.Add(vi);

            }


            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                
                var vi = new Vector3(cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2, cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2, -cubeSize / 2);
                points.Add(vi);

            }
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
               
                var vi = new Vector3(cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2, cubeSize * Convert.ToSingle(r.NextDouble()) - cubeSize / 2, cubeSize / 2);
                points.Add(vi);

            }
             PointCloud pointCloud = new PointCloud();
            pointCloud.Vectors = points.ToArray();
            return pointCloud;
        }

     
            
    }
}
