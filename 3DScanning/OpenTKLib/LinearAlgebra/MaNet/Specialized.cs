using System;
using OpenTK;

namespace OpenTKLib
{
    public class Specialized
    {

        public static Matrix3d Toeplitz(double[] firstRow)
        {

            Matrix3d A = new Matrix3d();

            for (int i = 1; i < firstRow.Length; i++)
            {
                double[] row = new double[firstRow.Length];
                for (int j = 0; j < row.Length; j++)
                {

                    A[i, j] = firstRow[Math.Abs(j - i)];
                }

            }




            return A;

        }

        public static Matrix3d Stiffness(int dimension)
        {
            if (dimension < 2) throw new Exception("Matrix only defined for dimension 2 and above");
            double[] array = new double[dimension];
            array[0] = 2;
            array[1] = -1;
            return Toeplitz(array);

        }

        public static Matrix3d K(int dimension)
        {
            return Stiffness(dimension);

        }

        public static Matrix3d Circulant(int dimension)
        {
            if (dimension < 2) throw new Exception("Matrix only defined for dimension 3 and above");
            Matrix3d A = K(dimension);
            A[0, dimension - 1] = -1;
            A[dimension - 1, 0] = -1;
            return A;
        }


        public static Matrix3d C(int dimension)
        {
            return Circulant(dimension);
        }


        public static Matrix3d T(int dimension)
        {
            if (dimension < 2) throw new Exception("Matrix only defined for dimension 2 and above");
            double[] array = new double[dimension];
            array[0] = 2;
            array[1] = -1;
            Matrix3d A = Toeplitz(array);
            A[0, 0] = 1;
            return A;

        }

        public static Matrix3d B(int dimension)
        {
            if (dimension < 2) throw new Exception("Matrix only defined for dimension 2 and above");
            double[] array = new double[dimension];
            array[0] = 2;
            array[1] = -1;
            Matrix3d A = Toeplitz(array);
            A[0, 0] = 1;
            A[dimension - 1, dimension - 1] = 1;
            return A;

        }



        /// <summary>
        /// Returns the Rosser matrix, a famous 8 by eight matrix which many algorithms have trouble with.
        /// </summary>
        /// <returns>Rosser Matrix</returns>
        public static Matrix3d Rosser()
        {

            string strRosser = @" 611   196  -192   407    -8   -52   -49    29
                               196   899   113  -192   -71   -43    -8   -44
                              -192   113   899   196    61    49     8    52
                               407  -192   196   611     8    44    59   -23
                                -8   -71    61     8   411  -599   208   208
                               -52   -43    49    44  -599   411   208   208
                               -49    -8     8    59   208   208    99  -911
                                29   -44    52   -23   208   208  -911    99";

            Matrix3d mat = new Matrix3d();
            return mat.Parse(strRosser);

        }


    }
}
