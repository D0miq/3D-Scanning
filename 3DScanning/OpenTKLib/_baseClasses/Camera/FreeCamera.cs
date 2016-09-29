using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKLib.FastGLControl
{
  
    public class CFreeCamera : CAbstractCamera
    {
        public CFreeCamera()
        {
            translation = new Vector3d(0);
            speed = 0.5f; // 0.5f m/s
        }
        public new void Dispose()
        {
            base.Dispose();
        }

        public override void Update()
        {
            Matrix4d R = yawPitchRoll(yaw, pitch, roll);
            Position += translation;

            //set this when no movement decay is needed
            //translation=Vector3d(0);

            CenterOfInterest = R.MultiplyVector3d(new Vector4d(0, 0, 1, 0));
            Up = R.MultiplyVector3d(new Vector4d(0, 1, 0, 0));
            right = Vector3d.Cross(CenterOfInterest, Up);

            Vector3d direction = Position + CenterOfInterest;
            // V = lookAt(Position, tgt, up);
            V = Matrix4d.LookAt(Position, // Camera is here
                direction, // and looks here : at the same position, plus "direction"
                Up);      // Head is up (set to 0,-1,0 to look upside-down)
            //

        }

        public void Walk(double dt)
        {
            translation += (CenterOfInterest * speed * dt);
            Update();
        }
        public void Strafe(double dt)
        {
            translation += (right * speed * dt);
            Update();
        }
        public void Lift(double dt)
        {
            translation += (Up * speed * dt);
            Update();
        }

        public void SetTranslation(Vector3d t)
        {
            translation = t;
            Update();
        }
        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: Vector3d GetTranslation() const
        public Vector3d GetTranslation()
        {
            return translation;
        }

        public void SetSpeed(double s)
        {
            speed = s;
        }
        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: const double GetSpeed() const
        public double GetSpeed()
        {
            return speed;
        }


        protected double speed; //move speed of camera in m/s
        protected Vector3d translation = new Vector3d();
    }
}