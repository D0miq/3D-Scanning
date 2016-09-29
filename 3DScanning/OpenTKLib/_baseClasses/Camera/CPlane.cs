using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKLib.FastGLControl
{
    public class CPlane
    {

        public Vector3d N = new Vector3d();
        public double d;

        public enum Where
        {
            COPLANAR,
            FRONT,
            BACK
        }

        public static class GlobalMembersPlane
        {
            public const double EPSILON = 0.0001f;
        }


        public CPlane()
        {
            N = new Vector3d(0, 1, 0);
            d = 0F;
        }
        public CPlane(Vector3d normal, Vector3d p)
        {
            N = normal;
            d = - Vector3d.Dot(N, p);
        }
        public void Dispose()
        {
        }

        public static CPlane FromPoints(Vector3d v1, Vector3d v2, Vector3d v3)
        {
            CPlane temp = new CPlane();
            Vector3d e1 = v2 - v1;
            Vector3d e2 = v3 - v1;
            Vector3d v = 
            temp.N = Vector3d.Normalize(Vector3d.Cross(e1, e2));
            temp.d = - Vector3d.Dot(temp.N , v1);
            return temp;
        }
        public CPlane.Where Classify(Vector3d p)
        {
            double res = GetDistance(p);
            if (res > GlobalMembersPlane.EPSILON)
            {
                return Where.FRONT;
            }
            else if (res < GlobalMembersPlane.EPSILON)
            {
                return Where.BACK;
            }
            else
            {
                return Where.COPLANAR;
            }
        }
        public double GetDistance(Vector3d p)
        {
            return Vector3d.Dot(N, p) + d;
        }


    }

}
