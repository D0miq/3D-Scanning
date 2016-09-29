using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKLib.FastGLControl
{
  


    public abstract class CAbstractCamera
    {
        public Matrix4d V = Matrix4d.Identity; //view matrix
        //public Matrix4d P = new Matrix4d(); //projection matrix
        
        //vectors
        public Vector3d Position = new Vector3d();
        protected Vector3d CenterOfInterest = new Vector3d();
        protected Vector3d Up = new Vector3d();
        //------------
        public Matrix4d R = Matrix4d.Identity;


         //frustum points
        public Vector3d[] farPts = new Vector3d[4];
        public Vector3d[] nearPts = new Vector3d[4];
        



        protected double yaw;
        protected double pitch;
        protected double roll;
        protected double fov;
        protected double aspect_ratio;
        protected double Znear;
        protected double Zfar;
        protected static Vector3d UP = new Vector3d(0, 1, 0);
       
        protected Vector3d right = new Vector3d();
        
        //Frsutum planes
        protected CPlane[] planes = new CPlane[6]; 

        public CAbstractCamera()
        {
            Znear = 0.1f;
            Zfar = 1000F;
        }
        public void Dispose()
        {
        }
        //public void SetupProjection(double fovy, double aspRatio)
        //{
        //    SetupProjection(fovy, aspRatio, 0.1f, 1000.0f);
        //}
        //public void SetupProjection(double fovy, double aspRatio, double nr)
        //{
        //    SetupProjection(fovy, aspRatio, nr, 1000.0f);
        //}
        //public void SetupProjection(double fovy, double aspRatio, double ZNear, double ZFar)
        //{

        //    this.Znear = ZNear;
        //    this.Zfar = ZFar;
        //    this.fov = fovy;
        //    this.aspect_ratio = aspRatio;

        //   // P = P.PerspectiveNew(fovy, aspRatio, ZNear, ZFar);
        //    P = Matrix4d.CreatePerspectiveFieldOfView(fovy, aspRatio, ZNear, ZFar);
            

           
        //}

        public abstract void Update();

        public virtual void Rotate(double y, double p, double r)
        {
            yaw = Matrix4dExtension.DegreesToRadians(y);
            pitch = Matrix4dExtension.DegreesToRadians(p);
            roll = Matrix4dExtension.DegreesToRadians(r);
            Matrix4d rNew = yawPitchRoll(yaw, pitch, 0.0f);
            R = Matrix4d.Mult(R, rNew);

            Update();
        }

      

        public void CalcFrustumPlanes()
        {


            Vector3d cN = Position + CenterOfInterest * Znear;
            Vector3d cF = Position + CenterOfInterest * Zfar;

            double Hnear = 2.0f * Convert.ToSingle(Math.Tan(Matrix4dExtension.DegreesToRadians(fov / 2.0f)) * Znear);
            double Wnear = Hnear * aspect_ratio;
            double Hfar = 2.0f * Convert.ToSingle(Math.Tan(Matrix4dExtension.DegreesToRadians(fov / 2.0f)) * Zfar);
            double Wfar = Hfar * aspect_ratio;
            double hHnear = Hnear / 2.0f;
            double hWnear = Wnear / 2.0f;
            double hHfar = Hfar / 2.0f;
            double hWfar = Wfar / 2.0f;


            farPts[0] = cF + Up * hHfar - right * hWfar;
            farPts[1] = cF - Up * hHfar - right * hWfar;
            farPts[2] = cF - Up * hHfar + right * hWfar;
            farPts[3] = cF + Up * hHfar + right * hWfar;

            nearPts[0] = cN + Up * hHnear - right * hWnear;
            nearPts[1] = cN - Up * hHnear - right * hWnear;
            nearPts[2] = cN - Up * hHnear + right * hWnear;
            nearPts[3] = cN + Up * hHnear + right * hWnear;

            planes[0] = CPlane.FromPoints(nearPts[3], nearPts[0], farPts[0]);
            planes[1] = CPlane.FromPoints(nearPts[1], nearPts[2], farPts[2]);
            planes[2] = CPlane.FromPoints(nearPts[0], nearPts[1], farPts[1]);
            planes[3] = CPlane.FromPoints(nearPts[2], nearPts[3], farPts[2]);
            planes[4] = CPlane.FromPoints(nearPts[0], nearPts[3], nearPts[2]);
            planes[5] = CPlane.FromPoints(farPts[3], farPts[0], farPts[1]);
        }
        public bool IsPointInFrustum(Vector3d point)
        {
            for (int i = 0; i < 6; i++)
            {
                if (planes[i].GetDistance(point) < 0)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsSphereInFrustum(Vector3d center, double radius)
        {
            for (int i = 0; i < 6; i++)
            {
                double d = planes[i].GetDistance(center);
                if (d < -radius)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsBoxInFrustum(Vector3d min, Vector3d max)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3d p = min;
                Vector3d n = max;
                Vector3d N = planes[i].N;
                if (N.X >= 0)
                {
                    p.X = max.X;
                    n.X = min.X;
                }
                if (N.Y >= 0)
                {
                    p.Y = max.Y;
                    n.Y = min.Y;
                }
                if (N.Z >= 0)
                {
                    p.Z = max.Z;
                    n.Z = min.Z;
                }

                if (planes[i].GetDistance(p) < 0)
                {
                    return false;
                }
            }
            return true;
        }
        public void GetFrustumPlanes(ref Vector4d[] fp)
        {
            for (int i = 0; i < 6; i++)
            {
                fp[i] = new Vector4d(planes[i].N, planes[i].d);
            }
        }

      

     
        protected Matrix4d yawPitchRoll(double yaw, double pitch, double roll)
        {
            Matrix4d Result = Matrix4d.Identity;

            double tmp_ch = Convert.ToSingle(Math.Cos(yaw));
            double tmp_sh = Convert.ToSingle(Math.Sin(pitch));
            double tmp_cp = Convert.ToSingle(Math.Cos(pitch));
            double tmp_sp = Convert.ToSingle(Math.Sin(pitch));

            double tmp_cb = Convert.ToSingle(Math.Cos(roll));
            double tmp_sb = Convert.ToSingle(Math.Sin(roll));



            Result[0, 0] = tmp_ch * tmp_cb + tmp_sh * tmp_sp * tmp_sb;
            Result[0, 1] = tmp_sb * tmp_cp;
            Result[0, 2] = -tmp_sh * tmp_cb + tmp_ch * tmp_sp * tmp_sb;
            Result[0, 3] = 0f;
            Result[1, 0] = -tmp_ch * tmp_sb + tmp_sh * tmp_sp * tmp_cb;
            Result[1, 1] = tmp_cb * tmp_cp;
            Result[1, 2] = tmp_sb * tmp_sh + tmp_ch * tmp_sp * tmp_cb;
            Result[1, 3] = 0f;
            Result[2, 0] = tmp_sh * tmp_cp;
            Result[2, 1] = -tmp_sp;
            Result[2, 2] = tmp_ch * tmp_cp;
            Result[2, 3] = 0f;
            Result[3, 0] = 0f;
            Result[3, 1] = 0f;
            Result[3, 2] = 0f;
            Result[3, 3] = 0f;
            return Result;
        }
        //protected Matrix4d lookAt(Vector3d eye, Vector3d center, Vector3d up)
        //{
        //    Matrix4d Result = Matrix4d.Identity;
        //    Vector3d f = (center - eye);
        //    f.Normalize();
        //    Vector3d u = new Vector3d(up);
        //    u.Normalize();
        //    Vector3d s = Vector3d.Cross(f, u);
        //    s.Normalize();
        //    u = Vector3d.Cross(s, f);

        //    Result[0, 0] = s.X;
        //    Result[1, 0] = s.Y;
        //    Result[2, 0] = s.Z;
        //    Result[0, 1] = u.X;
        //    Result[1, 1] = u.Y;
        //    Result[2, 1] = u.Z;
        //    Result[0, 2] = -f.X;
        //    Result[1, 2] = -f.Y;
        //    Result[2, 2] = -f.Z;
        //    Result[3, 0] = - Vector3d.Dot(s, eye);
        //    Result[3, 1] = - Vector3d.Dot(u, eye);
        //    Result[3, 2] = Vector3d.Dot(f, eye);
        //    return Result;
        //}
    }
 
}

