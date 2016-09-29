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
    /// Represents a half space in the 3-dimensions.
    /// </summary>
    /// <typeparam name="T">A numeric type</typeparam>
    public struct Plane3<T> : IEquatable<Plane3<T>>
        where T : IEquatable<T>
    {
        Vector3d<T> normal;

        Numeric<T> distance;

        void Set(Vector3d<T> point1,
                 Vector3d<T> point2,
                 Vector3d<T> point3,
                 T unit)
        {
            normal = (point2 - point1) % (point3 - point1);
            normal.Normalize(unit);
            distance = normal ^ point1;
        }

        public Plane3(Vector3d<T> point1, Vector3d<T> point2, Vector3d<T> point3, T unit)
        {
            normal = (point2 - point1) % (point3 - point1);
            normal.Normalize(unit);
            distance = normal ^ point1;
        }
        
        public Plane3(Vector3d<T> n, T d, T unit)
        {
            normal = n;
            normal.Normalize(unit);
            distance = d;
        }

        public Plane3(Vector3d<T> p, Vector3d<T> n, T unit)
        {
            normal = n;
            normal.Normalize(unit);
            distance = normal ^ p;
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Line3<T>)
            {
                Plane3<T> other = (Plane3<T>)obj;
                return Equals(ref this, ref other);
            }
            return base.Equals(obj);
        }

        public bool Equals(Plane3<T> other)
        {
            return Equals(this, other);
        }

        private static bool Equals(ref Plane3<T> v1, ref Plane3<T> v2)
        {
            return EqualityComparer<Vector3d<T>>.Default.Equals(v1.normal, v2.normal)
                && EqualityComparer<Numeric<T>>.Default.Equals(v1.distance, v2.distance);
        }

        public static bool operator ==(Plane3<T> v1, Plane3<T> v2)
        {
            return EqualityComparer<Vector3d<T>>.Default.Equals(v1.normal, v2.normal)
                && EqualityComparer<Numeric<T>>.Default.Equals(v1.distance, v2.distance);
        }

        public static bool operator !=(Plane3<T> v1, Plane3<T> v2)
        {
            return !(EqualityComparer<Vector3d<T>>.Default.Equals(v1.normal, v2.normal)
                && EqualityComparer<Numeric<T>>.Default.Equals(v1.distance, v2.distance));
        }

        public override int GetHashCode()
        {
            int hashCode = 67;

            hashCode = hashCode * 71 + normal.GetHashCode();
            hashCode = hashCode * 71 + distance.GetHashCode();

            return hashCode;
        }

        #endregion

        public T DistanceTo(Vector3d<T> point)
        {
            return (point ^ normal) - distance;
        }

        public Vector3d<T> ReflectPoint(Vector3d<T> point, Func<double, T> fromdouble = null)
        {
            if (fromdouble == null)
                fromdouble = Numeric<T>.Fromdouble;

            return normal * DistanceTo(point) * fromdouble(-2) + point;
        }
        public Vector3d<T> ClosestPoint(Vector3d<T> point, Func<double, T> fromdouble = null)
        {

            return ReflectPoint(point, fromdouble);

        }

        public Vector3d<T> ReflectVector(Vector3d<T> v, Func<double, T> fromdouble = null)
        {
            if (fromdouble == null)
                fromdouble = Numeric<T>.Fromdouble;

            return normal * (normal ^ v) * fromdouble(2) - v;
        }

        public Vector3d<T> Intersect(Line3<T> line)
        {
            Numeric<T> d = normal ^ line.Dir;

            if (d.Equals(Numeric<T>.Zero()))
                return Vector3d<T>.Zero();

            T t = -((normal ^ line.From) - distance) / d;
            Vector3d<T> point = line.GetPoint(t);

            return point;
        }

        public Vector3d<T> IntersectT(Line3<T> line)
        {
            Numeric<T> d = normal ^ line.Dir;

            if (d.Equals(Numeric<T>.Zero()))
                return Vector3d<T>.Zero();

            T t = -((normal ^ line.From) - distance) / d;
            Vector3d<T> point = line.GetPoint(t);

            return point;
        }
        //public IntersectionResult<Vector3d<T>> Intersect(Line3<T> line)
        //{
        //    Numeric<T> d = normal ^ line.Dir;

        //    if (d.Equals(Numeric<T>.Zero()))
        //        return new IntersectionResult<Vector3d<T>>(false);

        //    T t = -((normal ^ line.From) - distance) / d;
        //    Vector3d<T> point = line.GetPoint(t);

        //    return new IntersectionResult<Vector3d<T>>(point, true);
        //}

        //public IntersectionResult<T> IntersectT(Line3<T> line)
        //{
        //    Numeric<T> d = normal ^ line.Dir;

        //    if (d.Equals(Numeric<T>.Zero()))
        //        return new IntersectionResult<T>(false);

        //    T t = -((normal ^ line.From) - distance) / d;

        //    return new IntersectionResult<T>(t, true);
        //}

        public void MultiplyByMatrix(Matrix4d4<T> m, T unit)
        {
            Vector3d<T> dir1 = new Vector3d<T>(unit, Numeric<T>.Zero(), Numeric<T>.Zero()) % normal;
            Numeric<T> dir1Len = dir1 ^ dir1;

            Vector3d<T> tmp = new Vector3d<T>(Numeric<T>.Zero(), unit, Numeric<T>.Zero()) % normal;
            Numeric<T> tmpLen = tmp ^ tmp;

            if (tmpLen > dir1Len)
            {
                dir1 = tmp;
                dir1Len = tmpLen;
            }

            tmp = new Vector3d<T>(Numeric<T>.Zero(), Numeric<T>.Zero(), unit) % normal;
            tmpLen = tmp ^ tmp;

            if (tmpLen > dir1Len)
            {
                dir1 = tmp;
            }

            Vector3d<T> dir2 = dir1 % normal;
            Vector3d<T> point = distance * normal;

            this = new Plane3<T>(point * m, (point + dir2) * m, (point + dir1) * m, unit);
        }

        static public Plane3<T> MultiplyByMatrix(Plane3<T> plane, Matrix4d4<T> m, T unit)
        {
            Vector3d<T> dir1 = new Vector3d<T>(unit, Numeric<T>.Zero(), Numeric<T>.Zero()) % plane.normal;
            Numeric<T> dir1Len = dir1 ^ dir1;

            Vector3d<T> tmp = new Vector3d<T>(Numeric<T>.Zero(), unit, Numeric<T>.Zero()) % plane.normal;
            Numeric<T> tmpLen = tmp ^ tmp;

            if (tmpLen > dir1Len)
            {
                dir1 = tmp;
                dir1Len = tmpLen;
            }

            tmp = new Vector3d<T>(Numeric<T>.Zero(), Numeric<T>.Zero(), unit) % plane.normal;
            tmpLen = tmp ^ tmp;

            if (tmpLen > dir1Len)
            {
                dir1 = tmp;
            }

            Vector3d<T> dir2 = dir1 % plane.normal;
            Vector3d<T> point = plane.distance * plane.normal;

            return new Plane3<T>(point * m, (point + dir2) * m, (point + dir1) * m, unit);
        }

        public static Plane3<T> operator -(Plane3<T> plane, T unit)
        {
            return new Plane3<T>(-plane.normal, -plane.distance, unit);
        }
    }
}
