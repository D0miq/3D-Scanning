// Copyright (c) 2011 Hamrouni Ghassen. All rights reserved.
//
// Portions of this library are based on Miscellaneous Utility Library by Jon Skeet
// Portions of this library are a port of IlmBase http://www.openexr.com/
//
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLinear
{
    /// <summary>
    /// Represents a 4x4 matrix.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Matrix4d4<T> : IEquatable<Matrix4d4<T>>
        where T : IEquatable<T>
    {
        /// <summary>
        ///  The matrix components.
        /// </summary>
        Numeric<T>[][] x;

        /// <summary>
        /// Get component at index i, j
        /// </summary>
        /// <param name="index"></param>
        /// <returns>M[i][j]</returns>
        public Numeric<T>[] this[int index]
        {
            get
            {
                return x[index];
            }
        }

        /// <summary>
        /// Constructor M[i][j] = a
        /// </summary>
        /// <param name="a"></param>
        public Matrix4d4(T a)
        {
            x = new Numeric<T>[4][];

            for (int i = 0; i < 4; i++)
                x[i] = new Numeric<T>[4];

            x[0][0] = a;
            x[0][1] = a;
            x[0][2] = a;
            x[0][3] = a;
            x[1][0] = a;
            x[1][1] = a;
            x[1][2] = a;
            x[1][3] = a;
            x[2][0] = a;
            x[2][1] = a;
            x[2][2] = a;
            x[2][3] = a;
            x[3][0] = a;
            x[3][1] = a;
            x[3][2] = a;
            x[3][3] = a;
        }

        /// <summary>
        /// Constructor M = a
        /// </summary>
        /// <param name="a"></param>
        public Matrix4d4(T[][] a)
        {
            x = new Numeric<T>[4][];

            for (int i = 0; i < 4; i++)
                x[i] = new Numeric<T>[4];

            if (a.Length != 4)
                return;

            if (a[0].Length != 4) return;
            if (a[1].Length != 4) return;
            if (a[2].Length != 4) return;
            if (a[3].Length != 4) return;

            x[0][0] = a[0][0];
            x[0][1] = a[0][1];
            x[0][2] = a[0][2];
            x[0][3] = a[0][3];
            x[1][0] = a[1][0];
            x[1][1] = a[1][1];
            x[1][2] = a[1][2];
            x[1][3] = a[1][3];
            x[2][0] = a[2][0];
            x[2][1] = a[2][1];
            x[2][2] = a[2][2];
            x[2][3] = a[2][3];
            x[3][0] = a[3][0];
            x[3][1] = a[3][1];
            x[3][2] = a[3][2];
            x[3][3] = a[3][3];
        }

        public Matrix4d4(T a, T b, T c, T d, T e, T f, T g, T h,
               T i, T j, T k, T l, T m, T n, T o, T p)
        {
            x = new Numeric<T>[4][];

            for (int ii = 0; ii < 4; ii++)
                x[ii] = new Numeric<T>[4];

            x[0][0] = a;
            x[0][1] = b;
            x[0][2] = c;
            x[0][3] = d;
            x[1][0] = e;
            x[1][1] = f;
            x[1][2] = g;
            x[1][3] = h;
            x[2][0] = i;
            x[2][1] = j;
            x[2][2] = k;
            x[2][3] = l;
            x[3][0] = m;
            x[3][1] = n;
            x[3][2] = o;
            x[3][3] = p;
        }

        public Matrix4d4(Matrix3d3<T> r, Vector3d<T> t, T unit)
        {
            x = new Numeric<T>[4][];

            for (int i = 0; i < 4; i++)
                x[i] = new Numeric<T>[4];

            x[0][0] = r[0][0];
            x[0][1] = r[0][1];
            x[0][2] = r[0][2];
            x[0][3] = Numeric<T>.Zero();
            x[1][0] = r[1][0];
            x[1][1] = r[1][1];
            x[1][2] = r[1][2];
            x[1][3] = Numeric<T>.Zero();
            x[2][0] = r[2][0];
            x[2][1] = r[2][1];
            x[2][2] = r[2][2];
            x[2][3] = Numeric<T>.Zero();
            x[3][0] = t[0];
            x[3][1] = t[1];
            x[3][2] = t[2];
            x[3][3] = unit;
        }

        public Matrix4d4(Matrix4d4<T> v)
        {
            x = new Numeric<T>[4][];

            for (int i = 0; i < 4; i++)
                x[i] = new Numeric<T>[4];

            x[0][0] = v.x[0][0];
            x[0][1] = v.x[0][1];
            x[0][2] = v.x[0][2];
            x[0][3] = v.x[0][3];
            x[1][0] = v.x[1][0];
            x[1][1] = v.x[1][1];
            x[1][2] = v.x[1][2];
            x[1][3] = v.x[1][3];
            x[2][0] = v.x[2][0];
            x[2][1] = v.x[2][1];
            x[2][2] = v.x[2][2];
            x[2][3] = v.x[2][3];
            x[3][0] = v.x[3][0];
            x[3][1] = v.x[3][1];
            x[3][2] = v.x[3][2];
            x[3][3] = v.x[3][3];
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Matrix4d4<T>)
            {
                Matrix4d4<T> other = (Matrix4d4<T>)obj;
                return Equals(ref this, ref other);
            }
            return base.Equals(obj);
        }

        public bool Equals(Matrix4d4<T> other)
        {
            return Equals(this, other);
        }

        public static bool operator ==(Matrix4d4<T> v1, Matrix4d4<T> v2)
        {
            return (EqualityComparer<T>.Default.Equals(v1.x[0][0], v2.x[0][0]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[0][1], v2.x[0][1]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[0][2], v2.x[0][2]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[0][3], v2.x[0][3]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[1][0], v2.x[1][0]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[1][1], v2.x[1][1]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[1][2], v2.x[1][2]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[1][3], v2.x[1][3]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[2][0], v2.x[2][0]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[2][1], v2.x[2][1]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[2][2], v2.x[2][2]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[2][3], v2.x[2][3]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[3][0], v2.x[3][0]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[3][1], v2.x[3][1]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[3][2], v2.x[3][2]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[3][3], v2.x[3][3]));
        }

        public static bool operator !=(Matrix4d4<T> v1, Matrix4d4<T> v2)
        {
            return !(EqualityComparer<T>.Default.Equals(v1.x[0][0], v2.x[0][0]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[0][1], v2.x[0][1]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[0][2], v2.x[0][2]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[0][3], v2.x[0][3]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[1][0], v2.x[1][0]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[1][1], v2.x[1][1]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[1][2], v2.x[1][2]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[1][3], v2.x[1][3]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[2][0], v2.x[2][0]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[2][1], v2.x[2][1]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[2][2], v2.x[2][2]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[2][3], v2.x[2][3]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[3][0], v2.x[3][0]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[3][1], v2.x[3][1]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[3][2], v2.x[3][2]) &&
                     EqualityComparer<T>.Default.Equals(v1.x[3][3], v2.x[3][3]));
        }

        private static bool Equals(ref Matrix4d4<T> v1, ref Matrix4d4<T> v2)
        {
            return (EqualityComparer<T>.Default.Equals(v1.x[0][0], v2.x[0][0]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[0][1], v2.x[0][1]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[0][2], v2.x[0][2]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[0][3], v2.x[0][3]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[1][0], v2.x[1][0]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[1][1], v2.x[1][1]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[1][2], v2.x[1][2]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[1][3], v2.x[1][3]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[2][0], v2.x[2][0]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[2][1], v2.x[2][1]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[2][2], v2.x[2][2]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[2][3], v2.x[2][3]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[3][0], v2.x[3][0]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[3][1], v2.x[3][1]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[3][2], v2.x[3][2]) &&
                    EqualityComparer<T>.Default.Equals(v1.x[3][3], v2.x[3][3]));
        }

        public override int GetHashCode()
        {
            int hashCode = 67;

            hashCode = hashCode * 71 + x[0][0].GetHashCode();
            hashCode = hashCode * 71 + x[0][1].GetHashCode();
            hashCode = hashCode * 71 + x[0][2].GetHashCode();
            hashCode = hashCode * 71 + x[0][3].GetHashCode();
            hashCode = hashCode * 71 + x[1][0].GetHashCode();
            hashCode = hashCode * 71 + x[1][1].GetHashCode();
            hashCode = hashCode * 71 + x[1][2].GetHashCode();
            hashCode = hashCode * 71 + x[1][3].GetHashCode();
            hashCode = hashCode * 71 + x[2][0].GetHashCode();
            hashCode = hashCode * 71 + x[2][1].GetHashCode();
            hashCode = hashCode * 71 + x[2][2].GetHashCode();
            hashCode = hashCode * 71 + x[2][3].GetHashCode();
            hashCode = hashCode * 71 + x[3][0].GetHashCode();
            hashCode = hashCode * 71 + x[3][1].GetHashCode();
            hashCode = hashCode * 71 + x[3][2].GetHashCode();
            hashCode = hashCode * 71 + x[3][3].GetHashCode();

            return hashCode;
        }

        #endregion

        Matrix4d4<T> SetTheMatrix(Matrix4d4<T> v)
        {
            x[0][0] = v.x[0][0];
            x[0][1] = v.x[0][1];
            x[0][2] = v.x[0][2];
            x[0][3] = v.x[0][3];
            x[1][0] = v.x[1][0];
            x[1][1] = v.x[1][1];
            x[1][2] = v.x[1][2];
            x[1][3] = v.x[1][3];
            x[2][0] = v.x[2][0];
            x[2][1] = v.x[2][1];
            x[2][2] = v.x[2][2];
            x[2][3] = v.x[2][3];
            x[3][0] = v.x[3][0];
            x[3][1] = v.x[3][1];
            x[3][2] = v.x[3][2];
            x[3][3] = v.x[3][3];

            return this;
        }

        public void MakeIdentity(T unit)
        {
            x[0][0] = unit;
            x[0][1] = Numeric<T>.Zero();
            x[0][2] = Numeric<T>.Zero();
            x[0][3] = Numeric<T>.Zero();
            x[1][0] = Numeric<T>.Zero();
            x[1][1] = unit;
            x[1][2] = Numeric<T>.Zero();
            x[1][3] = Numeric<T>.Zero();
            x[2][0] = Numeric<T>.Zero();
            x[2][1] = Numeric<T>.Zero();
            x[2][2] = unit;
            x[2][3] = Numeric<T>.Zero();
            x[3][0] = Numeric<T>.Zero();
            x[3][1] = Numeric<T>.Zero();
            x[3][2] = Numeric<T>.Zero();
            x[3][3] = unit;
        }

        public static Matrix4d4<T> Identity(T unit)
        {
            Matrix4d4<T> m = new Matrix4d4<T>(Numeric<T>.Zero());
            m.MakeIdentity(unit);

            return m;
        }

        /// <summary>
        /// Component-wise addition
        /// </summary>
        /// <param name="v"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Matrix4d4<T> operator +(Matrix4d4<T> v, Matrix4d4<T> v2)
        {
            return new Matrix4d4<T>(v2.x[0][0] + v.x[0][0],
                                 v2.x[0][1] + v.x[0][1],
                                 v2.x[0][2] + v.x[0][2],
                                 v2.x[0][3] + v.x[0][3],
                                 v2.x[1][0] + v.x[1][0],
                                 v2.x[1][1] + v.x[1][1],
                                 v2.x[1][2] + v.x[1][2],
                                 v2.x[1][3] + v.x[1][3],
                                 v2.x[2][0] + v.x[2][0],
                                 v2.x[2][1] + v.x[2][1],
                                 v2.x[2][2] + v.x[2][2],
                                 v2.x[2][3] + v.x[2][3],
                                 v2.x[3][0] + v.x[3][0],
                                 v2.x[3][1] + v.x[3][1],
                                 v2.x[3][2] + v.x[3][2],
                                 v2.x[3][3] + v.x[3][3]);
        }


        /// <summary>
        /// Component-wise subtraction
        /// </summary>
        /// <param name="v2"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix4d4<T> operator -(Matrix4d4<T> v2, Matrix4d4<T> v)
        {
            return new Matrix4d4<T>(v2.x[0][0] - v.x[0][0],
                                 v2.x[0][1] - v.x[0][1],
                                 v2.x[0][2] - v.x[0][2],
                                 v2.x[0][3] - v.x[0][3],
                                 v2.x[1][0] - v.x[1][0],
                                 v2.x[1][1] - v.x[1][1],
                                 v2.x[1][2] - v.x[1][2],
                                 v2.x[1][3] - v.x[1][3],
                                 v2.x[2][0] - v.x[2][0],
                                 v2.x[2][1] - v.x[2][1],
                                 v2.x[2][2] - v.x[2][2],
                                 v2.x[2][3] - v.x[2][3],
                                 v2.x[3][0] - v.x[3][0],
                                 v2.x[3][1] - v.x[3][1],
                                 v2.x[3][2] - v.x[3][2],
                                 v2.x[3][3] - v.x[3][3]);
        }

        /// <summary>
        /// Component-wise inversion
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix4d4<T> operator -(Matrix4d4<T> v)
        {
            return new Matrix4d4<T>(-v.x[0][0],
                                 -v.x[0][1],
                                 -v.x[0][2],
                                 -v.x[0][3],
                                 -v.x[1][0],
                                 -v.x[1][1],
                                 -v.x[1][2],
                                 -v.x[1][3],
                                 -v.x[2][0],
                                 -v.x[2][1],
                                 -v.x[2][2],
                                 -v.x[2][3],
                                 -v.x[3][0],
                                 -v.x[3][1],
                                 -v.x[3][2],
                                 -v.x[3][3]);
        }

        /// <summary>
        /// Component-wise inversion
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Matrix4d4<T> Negate()
        {
            x[0][0] = -x[0][0];
            x[0][1] = -x[0][1];
            x[0][2] = -x[0][2];
            x[0][3] = -x[0][3];
            x[1][0] = -x[1][0];
            x[1][1] = -x[1][1];
            x[1][2] = -x[1][2];
            x[1][3] = -x[1][3];
            x[2][0] = -x[2][0];
            x[2][1] = -x[2][1];
            x[2][2] = -x[2][2];
            x[2][3] = -x[2][3];
            x[3][0] = -x[3][0];
            x[3][1] = -x[3][1];
            x[3][2] = -x[3][2];
            x[3][3] = -x[3][3];

            return this;
        }

        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix4d4<T> operator *(Matrix4d4<T> v2, T a)
        {
            return new Matrix4d4<T>(v2.x[0][0] * a,
                                 v2.x[0][1] * a,
                                 v2.x[0][2] * a,
                                 v2.x[0][3] * a,
                                 v2.x[1][0] * a,
                                 v2.x[1][1] * a,
                                 v2.x[1][2] * a,
                                 v2.x[1][3] * a,
                                 v2.x[2][0] * a,
                                 v2.x[2][1] * a,
                                 v2.x[2][2] * a,
                                 v2.x[2][3] * a,
                                 v2.x[3][0] * a,
                                 v2.x[3][1] * a,
                                 v2.x[3][2] * a,
                                 v2.x[3][3] * a);
        }

        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix4d4<T> operator *(T a, Matrix4d4<T> v2)
        {
            return new Matrix4d4<T>(v2.x[0][0] * a,
                                 v2.x[0][1] * a,
                                 v2.x[0][2] * a,
                                 v2.x[0][3] * a,
                                 v2.x[1][0] * a,
                                 v2.x[1][1] * a,
                                 v2.x[1][2] * a,
                                 v2.x[1][3] * a,
                                 v2.x[2][0] * a,
                                 v2.x[2][1] * a,
                                 v2.x[2][2] * a,
                                 v2.x[2][3] * a,
                                 v2.x[3][0] * a,
                                 v2.x[3][1] * a,
                                 v2.x[3][2] * a,
                                 v2.x[3][3] * a);
        }

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix4d4<T> operator *(Matrix4d4<T> v1, Matrix4d4<T> v2)
        {
            Matrix4d4<T> tmp = new Matrix4d4<T>(Numeric<T>.Zero());

            Matrix4d4<T>.Multiply(v1, v2, ref tmp);

            return tmp;
        }

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        static public void Multiply(Matrix4d4<T> a, Matrix4d4<T> b, ref Matrix4d4<T> c)
        {
            T a0, a1, a2, a3;

            a0 = a[0][0];
            a1 = a[0][1];
            a2 = a[0][2];
            a3 = a[0][3];

            c[0][0] = a0 * b[0][0] + a1 * b[1][0] + a2 * b[2][0] + a3 * b[3][0];
            c[0][1] = a0 * b[0][1] + a1 * b[1][1] + a2 * b[2][1] + a3 * b[3][1];
            c[0][2] = a0 * b[0][2] + a1 * b[1][2] + a2 * b[2][2] + a3 * b[3][2];
            c[0][3] = a0 * b[0][3] + a1 * b[1][3] + a2 * b[2][3] + a3 * b[3][3];

            a0 = a[1][0];
            a1 = a[1][1];
            a2 = a[1][2];
            a3 = a[1][3];

            c[1][0] = a0 * b[0][0] + a1 * b[1][0] + a2 * b[2][0] + a3 * b[3][0];
            c[1][1] = a0 * b[0][1] + a1 * b[1][1] + a2 * b[2][1] + a3 * b[3][1];
            c[1][2] = a0 * b[0][2] + a1 * b[1][2] + a2 * b[2][2] + a3 * b[3][2];
            c[1][3] = a0 * b[0][3] + a1 * b[1][3] + a2 * b[2][3] + a3 * b[3][3];

            a0 = a[2][0];
            a1 = a[2][1];
            a2 = a[2][2];
            a3 = a[2][3];

            c[2][0] = a0 * b[0][0] + a1 * b[1][0] + a2 * b[2][0] + a3 * b[3][0];
            c[2][1] = a0 * b[0][1] + a1 * b[1][1] + a2 * b[2][1] + a3 * b[3][1];
            c[2][2] = a0 * b[0][2] + a1 * b[1][2] + a2 * b[2][2] + a3 * b[3][2];
            c[2][3] = a0 * b[0][3] + a1 * b[1][3] + a2 * b[2][3] + a3 * b[3][3];

            a0 = a[3][0];
            a1 = a[3][1];
            a2 = a[3][2];
            a3 = a[3][3];

            c[3][0] = a0 * b[0][0] + a1 * b[1][0] + a2 * b[2][0] + a3 * b[3][0];
            c[3][1] = a0 * b[0][1] + a1 * b[1][1] + a2 * b[2][1] + a3 * b[3][1];
            c[3][2] = a0 * b[0][2] + a1 * b[1][2] + a2 * b[2][2] + a3 * b[3][2];
            c[3][3] = a0 * b[0][3] + a1 * b[1][3] + a2 * b[2][3] + a3 * b[3][3];
        }

        /// <summary>
        /// Vector-matrix multiplication
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public void MultVecMatrix(Vector3d<T> src, ref Vector3d<T> dst)
        {
            Numeric<T> a, b, c, w;

            a = src[0] * x[0][0] + src[1] * x[1][0] + src[2] * x[2][0] + x[3][0];
            b = src[0] * x[0][1] + src[1] * x[1][1] + src[2] * x[2][1] + x[3][1];
            c = src[0] * x[0][2] + src[1] * x[1][2] + src[2] * x[2][2] + x[3][2];
            w = src[0] * x[0][3] + src[1] * x[1][3] + src[2] * x[2][3] + x[3][3];

            dst.X = a / w;
            dst.Y = b / w;
            dst.Z = c / w;
        }

        /// <summary>
        /// Vector-matrix multiplication
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public void MultDirMatrix(Vector3d<T> src, ref Vector3d<T> dst)
        {
            T a, b, c;

            a = src[0] * x[0][0] + src[1] * x[1][0] + src[2] * x[2][0];
            b = src[0] * x[0][1] + src[1] * x[1][1] + src[2] * x[2][1];
            c = src[0] * x[0][2] + src[1] * x[1][2] + src[2] * x[2][2];

            dst.X = a;
            dst.Y = b;
            dst.Z = c;
        }

        /// <summary>
        /// Component-wise division
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix4d4<T> operator /(Matrix4d4<T> v2, T a)
        {
            return new Matrix4d4<T>(v2.x[0][0] / a,
                                v2.x[0][1] / a,
                                v2.x[0][2] / a,
                                v2.x[0][3] / a,
                                v2.x[1][0] / a,
                                v2.x[1][1] / a,
                                v2.x[1][2] / a,
                                v2.x[1][3] / a,
                                v2.x[2][0] / a,
                                v2.x[2][1] / a,
                                v2.x[2][2] / a,
                                v2.x[2][3] / a,
                                v2.x[3][0] / a,
                                v2.x[3][1] / a,
                                v2.x[3][2] / a,
                                v2.x[3][3] / a);
        }

        /// <summary>
        /// Component-wise division
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix4d4<T> operator /(T a, Matrix4d4<T> v2)
        {
            return new Matrix4d4<T>(v2.x[0][0] / a,
                                v2.x[0][1] / a,
                                v2.x[0][2] / a,
                                v2.x[0][3] / a,
                                v2.x[1][0] / a,
                                v2.x[1][1] / a,
                                v2.x[1][2] / a,
                                v2.x[1][3] / a,
                                v2.x[2][0] / a,
                                v2.x[2][1] / a,
                                v2.x[2][2] / a,
                                v2.x[2][3] / a,
                                v2.x[3][0] / a,
                                v2.x[3][1] / a,
                                v2.x[3][2] / a,
                                v2.x[3][3] / a);
        }

        /// <summary>
        /// Transpose the matrix
        /// </summary>
        /// <returns></returns>
        public Matrix4d4<T> Transpose()
        {
            Matrix4d4<T> tmp = new Matrix4d4<T>(x[0][0],
                                          x[1][0],
                                          x[2][0],
                                          x[3][0],
                                          x[0][1],
                                          x[1][1],
                                          x[2][1],
                                          x[3][1],
                                          x[0][2],
                                          x[1][2],
                                          x[2][2],
                                          x[3][2],
                                          x[0][3],
                                          x[1][3],
                                          x[2][3],
                                          x[3][3]);

            this = tmp;

            return this;
        }

        /// <summary>
        /// Transpose the matrix
        /// </summary>
        /// <returns></returns>
        public Matrix4d4<T> Transposed()
        {
            return new Matrix4d4<T>(x[0][0],
                                    x[1][0],
                                    x[2][0],
                                    x[3][0],
                                    x[0][1],
                                    x[1][1],
                                    x[2][1],
                                    x[3][1],
                                    x[0][2],
                                    x[1][2],
                                    x[2][2],
                                    x[3][2],
                                    x[0][3],
                                    x[1][3],
                                    x[2][3],
                                    x[3][3]);
        }

        /// <summary>
        /// Gauss–Jordan elimination
        /// </summary>
        /// <exception cref="MatrixNotInvertibleException">MatrixNotInvertibleException</exception>
        /// <returns></returns>
        public Matrix4d4<T>
        Invert(T unit)
        {
            this = Inverse(unit);
            return this;
        }

        /// <summary>
        /// Gauss–Jordan elimination
        /// </summary>
        /// <exception cref="MatrixNotInvertibleException">MatrixNotInvertibleException</exception>
        /// <returns></returns>
        public Matrix4d4<T> Inverse(T unit)
        {
            int i, j, k;
            Matrix4d4<T> s = Matrix4d4<T>.Identity(unit);
            Matrix4d4<T> t = new Matrix4d4<T>(this);

            // Forward elimination

            for (i = 0; i < 3; i++)
            {
                int pivot = i;

                Numeric<T> pivotsize = (t[i][i]);

                if (pivotsize < Numeric<T>.Zero())
                    pivotsize = -pivotsize;

                for (j = i + 1; j < 4; j++)
                {
                    Numeric<T> tmp = (t[j][i]);

                    if (tmp < Numeric<T>.Zero())
                        tmp = -tmp;

                    if (tmp > pivotsize)
                    {
                        pivot = j;
                        pivotsize = tmp;
                    }
                }

                if (pivotsize.Equals(Numeric<T>.Zero()))
                {
                    throw new Exception("Cannot invert singular matrix.");
                }

                if (pivot != i)
                {
                    for (j = 0; j < 4; j++)
                    {
                        T tmp;

                        tmp = t[i][j];
                        t[i][j] = t[pivot][j];
                        t[pivot][j] = tmp;

                        tmp = s[i][j];
                        s[i][j] = s[pivot][j];
                        s[pivot][j] = tmp;
                    }
                }

                for (j = i + 1; j < 4; j++)
                {
                    T f = t[j][i] / t[i][i];

                    for (k = 0; k < 4; k++)
                    {
                        t[j][k] -= f * t[i][k];
                        s[j][k] -= f * s[i][k];
                    }
                }
            }

            // Backward substitution

            for (i = 3; i >= 0; --i)
            {
                Numeric<T> f;

                if ((f = t[i][i]).Equals(Numeric<T>.Zero()))
                {
                    throw new Exception("Cannot invert singular matrix.");
                }

                for (j = 0; j < 4; j++)
                {
                    t[i][j] /= f;
                    s[i][j] /= f;
                }

                for (j = 0; j < i; j++)
                {
                    f = t[j][i];

                    for (k = 0; k < 4; k++)
                    {
                        t[j][k] -= f * t[i][k];
                        s[j][k] -= f * s[i][k];
                    }
                }
            }

            return s;
        }

        public Matrix4d4<T> SetEulerAngles(Vector3d<T> r, T unit)
        {
            Numeric<T> cosRZ, sinRZ, cosRY, sinRY, cosRX, sinRX;

            T rx = r.X;
            T ry = r.Y;
            T rz = r.Z;

            cosRZ = (GenericMath.Cos(rz, unit));
            cosRY = (GenericMath.Cos(ry, unit));
            cosRX = (GenericMath.Cos(rx, unit));
    
            sinRZ = (GenericMath.Sin(rz, unit));
            sinRY = (GenericMath.Sin(ry, unit));
            sinRX = (GenericMath.Sin(rx, unit));

            x[0][0] = cosRZ * cosRY;
            x[0][1] = sinRZ * cosRY;
            x[0][2] = -sinRY;
            x[0][3] = Numeric<T>.Zero();

            x[1][0] = -sinRZ * cosRX + cosRZ * sinRY * sinRX;
            x[1][1] = cosRZ * cosRX + sinRZ * sinRY * sinRX;
            x[1][2] = cosRY * sinRX;
            x[1][3] = Numeric<T>.Zero();

            x[2][0] = sinRZ * sinRX + cosRZ * sinRY * cosRX;
            x[2][1] = -cosRZ * sinRX + sinRZ * sinRY * cosRX;
            x[2][2] = cosRY * cosRX;
            x[2][3] = Numeric<T>.Zero();

            x[3][0] = Numeric<T>.Zero();
            x[3][1] = Numeric<T>.Zero();
            x[3][2] = Numeric<T>.Zero();
            x[3][3] = unit;

            return this;
        }

        public Matrix4d4<T> SetAxisAngle(Vector3d<T> axis, T angle, T unit)
        {
            Vector3d<T> vunit = axis.Normalized(unit);

            Numeric<T> sine = (GenericMath.Sin(angle, unit));
            Numeric<T> cosine = (GenericMath.Cos(angle, unit));

            Numeric<T> scalarUnit = unit;

            x[0][0] = (Numeric<T>)vunit[0] * vunit[0] * (scalarUnit - cosine) + cosine;
            x[0][1] = (Numeric<T>)vunit[0] * vunit[1] * (scalarUnit - cosine) + vunit[2] * sine;
            x[0][2] = (Numeric<T>)vunit[0] * vunit[2] * (scalarUnit - cosine) - vunit[1] * sine;
            x[0][3] = Numeric<T>.Zero();

            x[1][0] = (Numeric<T>)vunit[0] * vunit[1] * (scalarUnit - cosine) - vunit[2] * sine;
            x[1][1] = (Numeric<T>)vunit[1] * vunit[1] * (scalarUnit - cosine) + cosine;
            x[1][2] = (Numeric<T>)vunit[1] * vunit[2] * (scalarUnit - cosine) + vunit[0] * sine;
            x[1][3] = Numeric<T>.Zero();

            x[2][0] = (Numeric<T>)vunit[0] * vunit[2] * (scalarUnit - cosine) + vunit[1] * sine;
            x[2][1] = (Numeric<T>)vunit[1] * vunit[2] * (scalarUnit - cosine) - vunit[0] * sine;
            x[2][2] = (Numeric<T>)vunit[2] * vunit[2] * (scalarUnit - cosine) + cosine;
            x[2][3] = Numeric<T>.Zero();

            x[3][0] = Numeric<T>.Zero();
            x[3][1] = Numeric<T>.Zero();
            x[3][2] = Numeric<T>.Zero();
            x[3][3] = unit;

            return this;
        }

        /// <summary>
        /// Set matrix to rotation by angle
        /// </summary>
        /// <param name="angle">angle of rotation in radians</param>
        /// <returns></returns>
        public Matrix4d4<T> Rotate(Vector3d<T> angle, T unit)
        {
            Numeric<T> cosRZ, sinRZ, cosRY, sinRY, cosRX, sinRX;
            Numeric<T> m00, m01, m02;
            Numeric<T> m10, m11, m12;
            Numeric<T> m20, m21, m22;

            T rx = angle.X;
            T ry = angle.Y;
            T rz = angle.Z;

            cosRZ = (GenericMath.Cos(rz, unit));
            cosRY = (GenericMath.Cos(ry, unit));
            cosRX = (GenericMath.Cos(rx, unit));
                    
            sinRZ = (GenericMath.Sin(rz, unit));
            sinRY = (GenericMath.Sin(ry, unit));
            sinRX = (GenericMath.Sin(rx, unit));

            m00 = cosRZ * cosRY;
            m01 = sinRZ * cosRY;
            m02 = -sinRY;
            m10 = -sinRZ * cosRX + cosRZ * sinRY * sinRX;
            m11 = cosRZ * cosRX + sinRZ * sinRY * sinRX;
            m12 = cosRY * sinRX;
            m20 = -sinRZ * -sinRX + cosRZ * sinRY * cosRX;
            m21 = cosRZ * -sinRX + sinRZ * sinRY * cosRX;
            m22 = cosRY * cosRX;

            Matrix4d4<T> P = new Matrix4d4<T>(this);

            x[0][0] = P[0][0] * m00 + P[1][0] * m01 + P[2][0] * m02;
            x[0][1] = P[0][1] * m00 + P[1][1] * m01 + P[2][1] * m02;
            x[0][2] = P[0][2] * m00 + P[1][2] * m01 + P[2][2] * m02;
            x[0][3] = P[0][3] * m00 + P[1][3] * m01 + P[2][3] * m02;

            x[1][0] = P[0][0] * m10 + P[1][0] * m11 + P[2][0] * m12;
            x[1][1] = P[0][1] * m10 + P[1][1] * m11 + P[2][1] * m12;
            x[1][2] = P[0][2] * m10 + P[1][2] * m11 + P[2][2] * m12;
            x[1][3] = P[0][3] * m10 + P[1][3] * m11 + P[2][3] * m12;

            x[2][0] = P[0][0] * m20 + P[1][0] * m21 + P[2][0] * m22;
            x[2][1] = P[0][1] * m20 + P[1][1] * m21 + P[2][1] * m22;
            x[2][2] = P[0][2] * m20 + P[1][2] * m21 + P[2][2] * m22;
            x[2][3] = P[0][3] * m20 + P[1][3] * m21 + P[2][3] * m22;

            return this;
        }

        /// <summary>
        /// Set matrix to scale by s
        /// </summary>
        /// <param name="s">The uniform factor</param>
        /// <returns></returns>
        public Matrix4d4<T> SetScale(T s, T unit)
        {
            x[0][0] = s;
            x[0][1] = Numeric<T>.Zero();
            x[0][2] = Numeric<T>.Zero();
            x[0][3] = Numeric<T>.Zero();

            x[1][0] = Numeric<T>.Zero();
            x[1][1] = s;
            x[1][2] = Numeric<T>.Zero();
            x[1][3] = Numeric<T>.Zero();

            x[2][0] = Numeric<T>.Zero();
            x[2][1] = Numeric<T>.Zero();
            x[2][2] = s;
            x[2][3] = Numeric<T>.Zero();

            x[3][0] = Numeric<T>.Zero();
            x[3][1] = Numeric<T>.Zero();
            x[3][2] = Numeric<T>.Zero();
            x[3][3] = unit;

            return this;
        }

        /// <summary>
        /// Set matrix to scale by s
        /// </summary>
        public Matrix4d4<T> SetScale(Vector3d<T> s, T unit)
        {
            x[0][0] = s[0];
            x[0][1] = Numeric<T>.Zero();
            x[0][2] = Numeric<T>.Zero();
            x[0][3] = Numeric<T>.Zero();

            x[1][0] = Numeric<T>.Zero();
            x[1][1] = s[1];
            x[1][2] = Numeric<T>.Zero();
            x[1][3] = Numeric<T>.Zero();

            x[2][0] = Numeric<T>.Zero();
            x[2][1] = Numeric<T>.Zero();
            x[2][2] = s[2];
            x[2][3] = Numeric<T>.Zero();

            x[3][0] = Numeric<T>.Zero();
            x[3][1] = Numeric<T>.Zero();
            x[3][2] = Numeric<T>.Zero();
            x[3][3] = unit;

            return this;
        }

        /// <summary>
        /// Set matrix to scale by s
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Matrix4d4<T> Scale(Vector3d<T> s)
        {
            x[0][0] *= s[0];
            x[0][1] *= s[0];
            x[0][2] *= s[0];
            x[0][3] *= s[0];

            x[1][0] *= s[1];
            x[1][1] *= s[1];
            x[1][2] *= s[1];
            x[1][3] *= s[1];

            x[2][0] *= s[2];
            x[2][1] *= s[2];
            x[2][2] *= s[2];
            x[2][3] *= s[2];

            return this;
        }

        public Matrix4d4<T> SetTranslation(Vector3d<T> t, T unit)
        {
            x[0][0] = unit;
            x[0][1] = Numeric<T>.Zero();
            x[0][2] = Numeric<T>.Zero();
            x[0][3] = Numeric<T>.Zero();

            x[1][0] = Numeric<T>.Zero();
            x[1][1] = unit;
            x[1][2] = Numeric<T>.Zero();
            x[1][3] = Numeric<T>.Zero();

            x[2][0] = Numeric<T>.Zero();
            x[2][1] = Numeric<T>.Zero();
            x[2][2] = unit;
            x[2][3] = Numeric<T>.Zero();

            x[3][0] = t[0];
            x[3][1] = t[1];
            x[3][2] = t[2];
            x[3][3] = unit;

            return this;
        }

        public Vector3d<T> Translation()
        {
            return new Vector3d<T>(x[3][0], x[3][1], x[3][2]);
        }

        public Matrix4d4<T> Translate(Vector3d<T> t)
        {
            x[3][0] += t[0] * x[0][0] + t[1] * x[1][0] + t[2] * x[2][0];
            x[3][1] += t[0] * x[0][1] + t[1] * x[1][1] + t[2] * x[2][1];
            x[3][2] += t[0] * x[0][2] + t[1] * x[1][2] + t[2] * x[2][2];
            x[3][3] += t[0] * x[0][3] + t[1] * x[1][3] + t[2] * x[2][3];

            return this;
        }

        public Matrix4d4<T> SetShear(Vector3d<T> h, T unit)
        {
            x[0][0] = unit;
            x[0][1] = Numeric<T>.Zero();
            x[0][2] = Numeric<T>.Zero();
            x[0][3] = Numeric<T>.Zero();

            x[1][0] = h[0];
            x[1][1] = unit;
            x[1][2] = Numeric<T>.Zero();
            x[1][3] = Numeric<T>.Zero();

            x[2][0] = h[1];
            x[2][1] = h[2];
            x[2][2] = unit;
            x[2][3] = Numeric<T>.Zero();

            x[3][0] = Numeric<T>.Zero();
            x[3][1] = Numeric<T>.Zero();
            x[3][2] = Numeric<T>.Zero();
            x[3][3] = unit;

            return this;
        }

        public Matrix4d4<T> SetShear(Shear6<T> h, T unit)
        {
            x[0][0] = unit;
            x[0][1] = h.YX;
            x[0][2] = h.ZX;
            x[0][3] = Numeric<T>.Zero();

            x[1][0] = h.XY;
            x[1][1] = unit;
            x[1][2] = h.ZY;
            x[1][3] = Numeric<T>.Zero();

            x[2][0] = h.XZ;
            x[2][1] = h.YZ;
            x[2][2] = unit;
            x[2][3] = Numeric<T>.Zero();

            x[3][0] = Numeric<T>.Zero();
            x[3][1] = Numeric<T>.Zero();
            x[3][2] = Numeric<T>.Zero();
            x[3][3] = unit;

            return this;
        }

        public Matrix4d4<T> Shear(Vector3d<T> h)
        {
            for (int i = 0; i < 4; i++)
            {
                x[2][i] += h[1] * x[0][i] + h[2] * x[1][i];
                x[1][i] += h[0] * x[0][i];
            }

            return this;
        }

        public Matrix4d4<T> Shear(Shear6<T> h)
        {
            Matrix4d4<T> P = new Matrix4d4<T>(this);

            for (int i = 0; i < 4; i++)
            {
                x[0][i] = P[0][i] + h.YX * P[1][i] + h.ZX * P[2][i];
                x[1][i] = h.XY * P[0][i] + P[1][i] + h.ZY * P[2][i];
                x[2][i] = h.XZ * P[0][i] + h.YZ * P[1][i] + P[2][i];
            }

            return this;
        }

        public static Vector3d<T> operator *(Vector3d<T> v, Matrix4d4<T> m)
        {
            Numeric<T> x = (v.X * m[0][0] + v.Y * m[1][0] + v.Z * m[2][0] + m[3][0]);
            Numeric<T> y = (v.X * m[0][1] + v.Y * m[1][1] + v.Z * m[2][1] + m[3][1]);
            Numeric<T> z = (v.X * m[0][2] + v.Y * m[1][2] + v.Z * m[2][2] + m[3][2]);
            Numeric<T> w = (v.X * m[0][3] + v.Y * m[1][3] + v.Z * m[2][3] + m[3][3]);

            return new Vector3d<T>(x / w, y / w, z / w);
        }

        public Matrix4d4<V> Select<V>(Func<T, V> transf) where V : IEquatable<V>
        {
            Matrix4d4<V> transfMatrix = new Matrix4d4<V>(Numeric<V>.Zero());

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    transfMatrix[i][j] = transf(x[i][j]);
                }
            }

            return transfMatrix;
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append("{");

            for (int k = 0; k < 4; k++)
            {
                strBuilder.Append(" {");

                for (int l = 0; l < 4; l++)
                {
                    if (l != 0)
                        strBuilder.Append(", ");
                    strBuilder.Append(x[k][l]);
                }

                strBuilder.Append("} ");
            }

            strBuilder.Append("}");

            return strBuilder.ToString();
        }
    }
}
