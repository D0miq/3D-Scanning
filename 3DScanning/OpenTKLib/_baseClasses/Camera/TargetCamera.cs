#define _USE_MATH_DEFINES
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKLib.FastGLControl
{

    public class CTargetCamera : CAbstractCamera
    {

        public Vector3d Target = new Vector3d();
        //public Vector3d OrientationVector = new Vector3d((double)Math.PI, 0f, 0f);
        //public Vector3d OrientationVector = new Vector3d(0f, 0f, 0f);
        //public double MoveSpeed = 0.002f;
        //public double MouseSensitivity = 0.00001f;


        
        protected double minRy;
        protected double maxRy;
        protected double distance;
        protected double minDistance;
        protected double maxDistance;


        public CTargetCamera()
        {
            right = new Vector3d(1, 0, 0);
            Up = new Vector3d(0, 1, 0);
            CenterOfInterest = new Vector3d(0, 0, -1);
            minRy = -60F;
            maxRy = 60F;
            minDistance = 1F;
            maxDistance = 10F;
        }
        public new void Dispose()
        {
            base.Dispose();
        }
        private void CalcVMatrix()
        {
            //look = (target - position).Normalize();
            CenterOfInterest = (Target - Position);
            CenterOfInterest.Normalize();
            right = Vector3d.Cross(CenterOfInterest, Up);


            V = Matrix4d.LookAt(Position, // Camera is here
                Target, // and looks here : at the same position, plus "direction"
                Up);      // Head is up (set to 0,-1,0 to look upside-down)
        }
        public override void Update()
        {

            Vector3d direction = new Vector3d(0, 0, distance);
            direction = R.MultiplyVector3d(new Vector4d(direction, 0.0f));
            Position = Target + direction;
            Up = R.MultiplyVector3d(new Vector4d(UP, 0.0f));

            CalcVMatrix();


        }
        public void Rotate(double deltaX, double deltaY)
        {
            //double p = ((((((pitch) > (minRy)) ? (pitch) : (minRy))) < (maxRy)) ? ((((pitch) > (minRy)) ? (pitch) : (minRy))) : (maxRy));
            //base.Rotate(yaw, p, roll);

            double mouseSpeed = 0.1f;

            double horizontalAngle = mouseSpeed *  deltaX;
            double verticalAngle = mouseSpeed * deltaY;

            //// Direction : Spherical coordinates to Cartesian coordinates conversion
            Vector3d directionNew = new Vector3d(Convert.ToSingle(Math.Cos(verticalAngle) * Math.Sin(horizontalAngle)),
                Convert.ToSingle(Math.Sin(verticalAngle)),
                Convert.ToSingle(Math.Cos(verticalAngle) * Math.Cos(horizontalAngle)));

            //// Right vector
            Vector3d rightV = new Vector3d(Convert.ToSingle(Math.Sin(horizontalAngle - 3.14f / 2.0f)),
                    0,
                    Convert.ToSingle(Math.Cos(horizontalAngle - 3.14f / 2.0f)));

            //// Up vector
            Vector3d up = Vector3d.Cross(right, directionNew);
            V = Matrix4d.LookAt(Position, // Camera is here
             Position + directionNew, // and looks here : at the same position, plus "direction"
              up);      // Head is up (set to 0,-1,0 to look upside-down)

        }
        public new void Rotate(double yaw, double pitch, double roll)
        {
            double p = ((((((pitch) > (minRy)) ? (pitch) : (minRy))) < (maxRy)) ? ((((pitch) > (minRy)) ? (pitch) : (minRy))) : (maxRy));
            base.Rotate(yaw, p, roll);

            
        }

        public void SetTarget(Vector3d tgt)
        {
            Target = tgt;
            distance = (Position - Target).Length;// glm.distance(position, target);
            distance = (((minDistance) > ((((distance) < (maxDistance)) ? (distance) : (maxDistance)))) ? (minDistance) : ((((distance) < (maxDistance)) ? (distance) : (maxDistance))));

        }
        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: const Vector3d GetTarget() const
        public Vector3d GetTarget()
        {
            return Target;
        }

        public void Pan(double dx, double dy)
        {
            Vector3d X = right * dx;
            Vector3d Y = Up * dy;
            Position += X + Y;
            Target += X + Y;
            Update();
        }
        public void Zoom(double amount)
        {
            Position += CenterOfInterest * amount;
            distance = Vector3d.Subtract(Position, Target).Length;
            distance = (((minDistance) > ((((distance) < (maxDistance)) ? (distance) : (maxDistance)))) ? (minDistance) : ((((distance) < (maxDistance)) ? (distance) : (maxDistance))));
            Update();
        }
        public void Move(double dx, double dy)
        {
            Vector3d X = right * dx;
            Vector3d Y = CenterOfInterest * dy;
            Position += X + Y;
            Target += X + Y;
            Update();
        }
      
     

    }
}