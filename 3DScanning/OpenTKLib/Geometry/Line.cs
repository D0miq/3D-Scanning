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
    public class Line
    {
        public Vector3d PStart;
        public Vector3d PEnd;
        public Color Color;

        public Line(Vector3d pStart, Vector3d pEnd)
        {
            PStart = pStart;
            PEnd = pEnd;
        }
        public Line(Vector3d pStart, Vector3d pEnd, Color myColor)
        {
            PStart = pStart;
            PEnd = pEnd;
            Color = myColor;
        }
        public override string ToString()
        {

            return "Start: " + PStart.ToString() + " ; End: " + PEnd.ToString();
        }
        public Vector3d PointSymmetricByOrigin()
        {
            Vector3d p = new Vector3d();
            for (int i = 0; i < 3; i++ )
            {
                double delta = PEnd[i] - PStart[i];
                p[i] = PStart[i] - delta;

            }
           return p;


        }
        /// <summary>
        /// result = - v + 2 * myCenterOfMass
        /// </summary>
        /// <param name="myCenterOfMass"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3d PointSymmetricByCenterOfMass(Vector3d myCenterOfMass, Vector3d v)
        {
            //Line l = new Line(pStart, v);
            Vector3d p = v - myCenterOfMass;
            p = myCenterOfMass - p;

          
            return p;


        }
        
    }
}
