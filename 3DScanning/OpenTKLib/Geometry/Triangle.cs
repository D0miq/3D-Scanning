// Pogramming by
//     Douglas Andrade ( http://www.cmsoft.com.br, email: cmsoft@cmsoft.com.br)
//               Implementation of most of the functionality
//     Edgar Maass: (email: maass@logisel.de)
//               Code adaption, changed to user control
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

namespace OpenTKLib
{
 

    public class Triangle
    {
        public List<int> IndVertices;
        public List<int> IndNormals;
        public List<int> IndTextures;
        public Vector3d Normal;
        public int NormalIndex;

        public Triangle()
        {
            IndVertices = new List<int>();
            IndNormals = new List<int>();
            IndTextures = new List<int>();
            //Normal = new Vector3d();

        }
        public Triangle(int i, int j, int k) : this()
        {
            this.IndVertices.Add(i);
            this.IndVertices.Add(j);
            this.IndVertices.Add(k);
            //Normal = new Vector3d();
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Triangle CreateTriangle(int i, int j, int k)
        {
            Triangle a = new Triangle();
           
            a.IndVertices.Add(i);
            a.IndVertices.Add(j);
            a.IndVertices.Add(k);

            return a;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Triangle CreateTriangle(int i, int j, int k, int l)
        {
            Triangle a = CreateTriangle(i, j, k);
            a.IndVertices.Add(l);

            return a;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vi"></param>
        /// <param name="vj"></param>
        /// <param name="vk"></param>
        /// <returns></returns>
        private Triangle CreateTriangle(Vertex vi, Vertex vj, Vertex vk)
        {
            Triangle a = new Triangle();
           
            a.IndVertices.Add(vi.IndexInModel);
            a.IndVertices.Add(vj.IndexInModel);
            a.IndVertices.Add(vk.IndexInModel);

            return a;

        }

        /// <summary>
        /// Helper method
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="listTriangle"></param>
        public static void AddTriangleToList(int i, int j, int k, List<Triangle> listTriangle, Vertex v)
        {
            Triangle a = Triangle.CreateTriangle(i, j, k);
            if(!listTriangle.Contains(a))
            {
                listTriangle.Add(a);
                v.IndexTriangles.Add(listTriangle.Count - 1);
            }
            else
            {

            }
            
            
           
            
        }
   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="areas"></param>
        public static void SortIndexVerticesWithinAllTriangles(List<Triangle> areas)
        {
            for (int i = 0; i < areas.Count; i++)
            {
                Triangle a = areas[i];
                a.IndVertices.Sort();

            }

        }
        /// <summary>
        /// Check by IndVertices. The areas are already sorted  (performing sort here would make the performance be bad)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Equals(Triangle b)
        {
            if (this.IndVertices.Count != b.IndVertices.Count)
                return false;
            for (int i = 0; i < this.IndVertices.Count; i++ )
            {
                if (this.IndVertices[i] != b.IndVertices[i])
                    return false;
            }
            return true;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < this.IndVertices.Count; i++)
            {
                sb.Append(" : " + this.IndVertices[i].ToString() );

            }

            
            return sb.ToString();
        }
        /// <summary>
        /// is a very time consuming method - use only with list size smaller than 10,000
        /// </summary>
        /// <param name="listTriangle"></param>
        public static void CheckForDuplicates(List<Triangle> listTriangle)
        {

            System.Diagnostics.Debug.WriteLine("Number of areas before check: " + listTriangle.Count.ToString());


            for (int i = listTriangle.Count - 1; i >= 0; i--)
            {
                Triangle ai = listTriangle[i];
                for (int j = 0; j < i; j++)
                {
                    if (ai.Equals(listTriangle[j]))
                    {
                        listTriangle.RemoveAt(i);
                        break;
                    }
                }

            }
            System.Diagnostics.Debug.WriteLine("Number of areas AFTER check: " + listTriangle.Count.ToString());
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="myModel"></param>
        ///// <param name="t"></param>
        //public static void CalculateNormal_UpdateNormalList(Model3D myModel, Triangle t)
        //{

        //    Vector3d normal = CalculateNormalForTriangle(myModel.VertexList, t);
            
        //    if (normal != null)
        //    {
        //        myModel.Normals.Add(normal);
        //        int indNewNormal = myModel.Normals.Count - 1;
                
                
        //        t.IndNormals.Add(indNewNormal);
        //        //adds the normal to each of the pointCloud in the triangle
        //        for (int i = 0; i < t.IndVertices.Count; i++ )
        //        {
        //            int indVertex = t.IndVertices[i];
        //            myModel.VertexList[indVertex].IndexNormals.Add(indNewNormal);
        //        }
                   
        //    }


        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointCloud"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3d CalculateNormalForTriangle(PointCloudVertices pointCloud, Triangle t, Vertex v)
        {

            Vector3d a = pointCloud[t.IndVertices[0]].Vector;
            Vector3d b = pointCloud[t.IndVertices[1]].Vector;
            Vector3d c = pointCloud[t.IndVertices[2]].Vector;

            Vector3d normal = Vector3d.Cross(b - a, c - a);
            //important: The direction of the normal
            //I want the normal to be in the direction of the Vertex v
            //compare if v and the normal are in the same direction
            if(Vector3d.Dot(normal, v.Vector) < 0)
            {
                //they are not in the same direction => Flip normal:
                normal = Vector3d.Cross(b - c, a - c);
            }


            //alternative:
            //Vector3d normal = Vector3d.Cross(b - a, c - b);

            Vector3d temp = normal;
            normal = normal.NormalizeV();
            //if (!CheckVector(normal))
            //{
            //    System.Windows.Forms.MessageBox.Show("SW Error calculating normal");
            //    return new Vector3d(0, 0, 0);

            //}
            
            return normal;
        }
       
        //public static Vector3d CalculateNormal(Model myModel, Triangle t)
        //{
        //    if (t.IndNormals == null || t.IndNormals.Count == 0)
        //    {
        //        Vector3d a = myModel.PointCloudVertices[t.IndVertices[0]].Vector;
        //        Vector3d b = myModel.PointCloudVertices[t.IndVertices[1]].Vector;
        //        Vector3d c = myModel.PointCloudVertices[t.IndVertices[2]].Vector;

        //        Vector3d normal = Vector3d.Cross(b - a, c - a);
        //        //alternative:
        //        //Vector3d normal = Vector3d.Cross(b - a, c - b);

        //        Vector3d temp = normal;
        //        normal = normal.NormalizeV();
        //        //if (!CheckVector(normal))
        //        //{
        //        //    System.Windows.Forms.MessageBox.Show("SW Error calculating normal");
        //        //    return new Vector3d(0, 0, 0);

        //        //}
        //        return normal;

        //    }
        //    return myModel.Normals[t.IndNormals[0]];
        //}
        private static bool CheckVector(Vector3d v)
        {
            if (double.IsInfinity(v.X) || double.IsNaN(v.X) || double.IsInfinity(v.Y) || double.IsNaN(v.Y) || double.IsInfinity(v.Z) || double.IsNaN(v.Z))
                return false;

       

            return true;


        }
        /// <summary>
        /// Calculate the Tangent array based on the Vertex, Face, Normal and UV data.
        /// </summary>
        public static Vector3d[] CalculateTangents(Vector3d[] vertices, Vector3d[] normals, int[] triangles, Vector2[] uvs)
        {
            Vector3d[] tangents = new Vector3d[vertices.Length];
            Vector3d[] tangentData = new Vector3d[vertices.Length];

            for (int i = 0; i < triangles.Length / 3; i++)
            {
                Vector3d v1 = vertices[triangles[i * 3]];
                Vector3d v2 = vertices[triangles[i * 3 + 1]];
                Vector3d v3 = vertices[triangles[i * 3 + 2]];

                Vector2 w1 = uvs[triangles[i * 3]];
                Vector2 w2 = uvs[triangles[i * 3] + 1];
                Vector2 w3 = uvs[triangles[i * 3] + 2];

                double x1 = v2.X - v1.X;
                double x2 = v3.X - v1.X;
                double y1 = v2.Y - v1.Y;
                double y2 = v3.Y - v1.Y;
                double z1 = v2.Z - v1.Z;
                double z2 = v3.Z - v1.Z;

                double s1 = w2.X - w1.X;
                double s2 = w3.X - w1.X;
                double t1 = w2.Y - w1.Y;
                double t2 = w3.Y - w1.Y;
                double r = 1.0f / (s1 * t2 - s2 * t1);
                Vector3d sdir = new Vector3d((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);

                tangents[triangles[i * 3]] += sdir;
                tangents[triangles[i * 3 + 1]] += sdir;
                tangents[triangles[i * 3 + 2]] += sdir;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                double d = Vector3d.Dot(normals[i], tangents[i]);//.Normalize();
                Vector3d tempV = normals[i] * d;
                tempV = tangents[i] - tempV;
                tempV.Normalize();
                tangentData[i] = tempV;
                //tangentData[i] = (tangents[i] - normals[i] * Vector3d.Dot(normals[i], tangents[i])).Normalize();

            }
            return tangentData;
        }
    }

    /// <summary>
    /// compares according to INDEX of first, second, third vertex
    /// </summary>
    public class TriangleComparerVertices : IComparer<Triangle>
    {

        public int Compare(Triangle a, Triangle b)
        {
            if (a.IndVertices.Count != b.IndVertices.Count)
                return 0;

            for (int i = 0; i < a.IndVertices.Count; i++)
            {
                int ai = a.IndVertices[i];
                int bi = b.IndVertices[i];
                if (ai < bi)
                    return -1;
                else if (ai > bi)
                    return 1;

            }
            return 0;

        }
    }
    //}

 
}
