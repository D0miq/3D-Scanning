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
using System.Globalization;
using System.IO;
using System.Reflection;
using OpenTK;

namespace OpenTKLib
{
    /// <summary>
    /// An interface for a structure with indexer
    /// </summary>
    public interface IVector
    {
        /// <summary>
        /// Position of the vertex.
        /// </summary>
        //double[] Position { get; set; }
        // Indexer declaration:
        double this[int index]
        {
            get;
            set;
        }
        double[] PositionArray { get; set; }
    }

    public class Vertex : IVector
    {

        private List<int> indexTriangles;
        private List<int> indexParts;
        private List<int> indexNormals;
        private KeyValueList kDTreeSearch;

        //public List<int> NeighboursIndex = new List<int>();
        //public List<double> NeighboursDistance = new List<double>();

        //public Vector3d VectorOrigin;
        //if this vector is not set, the origin is in [0,0,0]
        public Vector3d Vector;

        public System.Drawing.Color Color;


        //necessary for triangulation
        public int IndexInModel;
        //necessary for KDTree Search
        public int IndexKDTreeTarget;

        public bool TakenInTree;

        public bool Marked;
    
        public Vertex()
        {

        
            Vector = new Vector3d();
           // System.Diagnostics.Debug.WriteLine("sizeof: " + System.Runtime.InteropServices.Marshal.SizeOf(Vector).ToString());

        }
        public Vertex(Vector3d v)
        {
            Vector = v;
        }
        public Vertex(Vertex v)
        {

            Vector = new Vector3d(v.Vector.X, v.Vector.Y, v.Vector.Z);

            this.IndexInModel = v.IndexInModel;
            this.Color = v.Color;
            //LengthSquared = v.LengthSquared;

        }

        public Vertex(double x, double y, double z)
        {
            Vector = new Vector3d(x, y, z);
            //LengthSquared = Vector.LengthSquared;
        }
        public Vertex(Vector3d v, byte[] color)
        {
            
            Vector = v;
            this.Color = Color.FromArgb(color[0], color[1], color[2]);
            
        }
        public Vertex(Vector3d v, Color color)
        {
            //Vector = new Vector3d(v.X, v.Y, v.Z);
            Vector = v;
            this.Color = color;
            
        }
        public Vertex(int ind, Vector3d v, byte[] color)
        {
            this.IndexInModel = ind;
            Vector = v;
            this.Color = Color.FromArgb(color[0], color[1], color[2]);
           
        }
        public Vertex(int ind, Vector3d v, Color color)
        {
            this.IndexInModel = ind;
            Vector = v;
            this.Color = color;

        }
        public Vertex(int myindexInModel, double x, double y, double z)
        {
            IndexInModel = myindexInModel;
            Vector = new Vector3d(x, y, z);
            //LengthSquared = Vector.LengthSquared;

        }
        public Vertex(int indexInModel, Vertex v)
        {
            IndexInModel = indexInModel;
            Vector = v.Vector;
            Color = v.Color;
            //Vector = new Vector3d(v.Vector.X, v.Vector.Y, v.Vector.Z);
            //LengthSquared = v.LengthSquared;
        }
   
        public Vertex(int indexInModel, Vector3d v)
        {
            IndexInModel = indexInModel;
            //Vector = new Vector3d(v.X, v.Y, v.Z);
            Vector = v;

        }

        #region public properties
        public double LengthSquared
        {
            get
            {
                return Vector.LengthSquared;
            }

        }
        public List<int> IndexTriangles
        {
            get
            {
                if (indexTriangles == null)
                    indexTriangles = new List<int>();
                return indexTriangles;
            }
            set
            {
                indexTriangles = value;

            }
        }


        public List<int> IndexParts
        {
            get
            {
                if (indexParts == null)
                    indexParts = new List<int>();
                return indexParts;
            }
        }

        public List<int> IndexNormals
        {
            get
            {
                if (indexNormals == null)
                    indexNormals = new List<int>();
                return indexNormals;
            }
        }
        public KeyValueList KDTreeSearch
        {
            get
            {
                if (kDTreeSearch == null)
                    kDTreeSearch = new KeyValueList();
                return kDTreeSearch;
            }
        }


        #endregion



        public override string ToString()
        {

            string returnString = Vector.X.ToString("F2") + "  " + Vector.Y.ToString("F2") + "  " + Vector.Z.ToString("F2");

            
            returnString += " :Color: " + Color.ToString();
            
            return returnString;
        }
        public static bool operator <(Vertex v1, Vertex v2)
        {
            if (v1.Vector.X < v2.Vector.X && v1.Vector.Y < v2.Vector.Y && v1.Vector.Z < v2.Vector.Z)
                return true;
            return false;

        }
        public static bool operator >(Vertex v1, Vertex v2)
        {
            if (v1.Vector.X > v2.Vector.X && v1.Vector.Y > v2.Vector.Y && v1.Vector.Z > v2.Vector.Z)
                return true;
            return false;

        }
        public static bool operator >=(Vertex v1, Vertex v2)
        {
            if (v1.Vector.X >= v2.Vector.X && v1.Vector.Y >= v2.Vector.Y && v1.Vector.Z >= v2.Vector.Z)
                return true;
            return false;

        }
        public static bool operator <=(Vertex v1, Vertex v2)
        {
            if (v1.Vector.X <= v2.Vector.X && v1.Vector.Y <= v2.Vector.Y && v1.Vector.Z <= v2.Vector.Z)
                return true;
            return false;

        }

        public double this[int index]
        {
            get
            {
                return this.Vector[index];
            }
            set
            {
                this.Vector[index] = value;
            }
        }
        public double[] PositionArray
        {
            get
            {

                //only important for Delaunay 2D
                double[] position;
                if (this.Vector.Z == 0)
                {
                    position = new double[2];
                    position[0] = this.Vector.X;
                    position[1] = this.Vector.Y;
                }
                else
                {
                    position = new double[3];
                    position[0] = this.Vector.X;
                    position[1] = this.Vector.Y;
                    position[2] = this.Vector.Z;
                }

                return position; //double[2] { this.Vector.X, this.Vector.Y, this.Vector.Z };

            }
            set
            {

                for (int i = 0; i < value.Length; i++)
                {
                    this.Vector[i] = value[i];
                }
            }
        }

     

    }


}
