using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenTKLib
{
  public  class Rectangular
    {

        public Random Random
        {
            get { return rand; }
            set { rand = value; }


        }

        protected Random rand = new Random();

        public Matrix3d Randomdouble(int m)
        {
            return Randomdouble(m, m, 0, 1);

        }

        public Matrix3d Randomdouble(int m, int n)
        {
            return Randomdouble(m, n, 0, 1);

        }


        public Matrix3d Randomdouble(int m, int n, double min, double max)
        {
            Matrix3d A = new Matrix3d();
            double[,] X = A.ToArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    X[i,j] = min + Convert.ToSingle(rand.NextDouble()) * (max - min);
                }
            }
            return A;
        }


        public Matrix3d RandomInt(int m, int n, int min, int max)
        {
            Matrix3d A = new Matrix3d();
            double[,] X = A.ToArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    X[i,j] = rand.Next(min, max);
                }
            }
            return A;

        }

        public Matrix3d RandomInt(int m, int n)
        {
            return RandomInt(m, n, 0, 9);
        }

        public Matrix3d RandomInt(int m )
        {
            return RandomInt(m, m, 0, 9);
        }
    }
}
