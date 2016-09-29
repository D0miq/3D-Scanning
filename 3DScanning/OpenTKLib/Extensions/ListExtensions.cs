using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;


namespace OpenTKLib
{
    public static class ListExtensions
    {
      
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
                return listToClone.Select(item => (T)item.Clone()).ToList();
        }
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
  
        public static double GetMedian(this List<double> source)
        {
            // Create a copy of the input, and sort the copy
            double[] temp = source.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                double a = temp[count / 2 - 1];
                double b = temp[count / 2];
                return (a + b) / 2;
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }
        public static void Shuffle<T>(this List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
         public static List<Vector3d> Subtract(this List<Vector3d> myList, List<Vector3d> otherList)
        {

            for (int i = 0; i < myList.Count; i++)
            {

                otherList[i] = myList[i]- otherList[i];

            }
            return otherList;
        }
        public static Vector3d GetMax(this List<Vector3d> myList)
         {
             Vector3d v = new Vector3d(double.MinValue, double.MinValue, double.MinValue);
             
             double dMax = double.MinValue;
             Vector3d vMax = new Vector3d(double.MinValue, double.MinValue, double.MinValue);

             for (int i = 0; i < myList.Count; i++)
             {
                 //double d = myList[i].Length;
                 double d = myList[i].LengthSquared;

                 if (d > dMax)
                 {
                     dMax = d;
                     vMax = myList[i];
                 }
                 //if (myList[i].X > v.X)
                 //    v.X = myList[i].X;
                 //if (myList[i].Y > v.Y)
                 //    v.Y = myList[i].Y;
                 //if (myList[i].Z > v.Z)
                 //    v.Z = myList[i].Z;

             }
             return vMax;
         }


        public static ushort[] Vectors3dListToUshortArray(this List<Vector3d> listVectors, int width, int height)
        {
            ushort[,] points = ToUshortArray(listVectors, width, height);
            ushort[] pointArr = PointCloudVertices.Ushort1DFrom2D(points, width, height);


            return pointArr;


        }
        public static ushort[,] ToUshortArray(this List<Vector3d> listVectors, int width, int height)
        {
            ushort[,] points = new ushort[width, height];


            for (int i = 0; i < listVectors.Count; i++)
            {

                Vector3d p3D = listVectors[i];
                points[Convert.ToInt32(p3D.X), Convert.ToInt32(p3D.Y)] = Convert.ToUInt16(p3D.Z);

            }
            return points;


        }
        public static Vector3d CalculateCentroid(this List<Vector3d> pointsTarget)
        {


            Vector3d centroid = new Vector3d();
            for (int i = 0; i < pointsTarget.Count; i++)
            {
                Vector3d v = pointsTarget[i];
                centroid.X += v.X;
                centroid.Y += v.Y;
                centroid.Z += v.Z;


            }
            centroid.X /= pointsTarget.Count;
            centroid.Y /= pointsTarget.Count;
            centroid.Z /= pointsTarget.Count;

            return centroid;

        }
        public static void CalculatePointsShiftedByCentroid(this List<Vector3d> a)
        {
            Vector3d centroid = a.CalculateCentroid();
            SubtractVector(a, centroid);
           
        }
        public static void SubtractVector(this List<Vector3d> aList, Vector3d centroid)
        {

            //List<Vector3d> b = new List<Vector3d>();
            for (int i = 0; i < aList.Count; i++)
            {
                aList[i] -= centroid;
               
            }
            //return b;

        }
        public static void AddVector(this List<Vector3d> aList, Vector3d centroid)
        {

            List<Vector3d> b = new List<Vector3d>();
            for (int i = 0; i < aList.Count; i++)
            {
                Vector3d v = aList[i];
                Vector3d vNew = new Vector3d(v.X + centroid.X, v.Y + centroid.Y, v.Z + centroid.Z);
                b.Add(vNew);
                //System.Diagnostics.Debug.WriteLine(vNew.X.ToString() + " , " + vNew.Y.ToString());
            }
            aList = b;


        }
        public static List<Vector3d> Clone(this List<Vector3d> aList)
        {

            List<Vector3d> newList = new List<Vector3d>();
            for (int i = 0; i < aList.Count; i++)
            {
                newList.Add(new Vector3d (aList[i]));
                
            }
            return newList;


        }
    
      
    }
}
