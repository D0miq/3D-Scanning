using System.Collections.Generic;
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTKLib;

namespace OpenTKLib
{

    public class PointCloudRenderable : RenderableObject
    {

        public PointCloudRenderable(Vector3 mycolor)
        {
            color = mycolor;
        }
        public PointCloudRenderable() : this(new Vector3(1, 1, 1))
        {

            this.Position = Vector3.Zero;
            this.Scale = 1f;
        }
     
        public override void InitializeGL()
        {
            this.primitiveType = PrimitiveType.Points;
            initialized = true;


            if (this.PointCloud == null)
            {
                System.Diagnostics.Debug.Assert(false, "SW Error - please set the point cloud data f5irst ");
                return;

            }


            if (InitShaders("PointCloud.vert", "PointCloud.frag", path + "Shaders\\"))
            {
                this.initBuffers();
                //at this point the data is transferred to GPU - therefore have to reset vector data here, otherwise it is useless.
                if (GLSettings.PointCloudCentered)
                    this.PointCloud.ResetCentroid(true);
                this.RefreshRenderableData();
            }

        }

        
      
        public override void Dispose()
        {

            base.Dispose();
        }


      
      
      
      
     
   
    }
}