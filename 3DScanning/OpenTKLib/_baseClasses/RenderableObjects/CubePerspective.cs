using System.Collections.Generic;
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
namespace OpenTKLib.FastGLControl
{

    public class CubePerspective : RenderableObject
    {
        

        public CubePerspective() : this(new Vector3(1, 1, 1))
        {
            this.primitiveType = PrimitiveType.Triangles;
        }
     

            
        public CubePerspective(Vector3 col)
        {
            this.primitiveType = PrimitiveType.Triangles;
            color = col;


            if (InitShaders("cubePerspective.vert", "cubePerspective.frag", path + "Shaders\\"))
            {
                this.initBuffers();
                this.FillPointCloud();
                this.FillIndexBuffer();
                this.RefreshRenderableData();
            }

        }
      
        public override void Dispose()
        {

            base.Dispose();
        }

      
      
        public void FillPointCloudVertices()
        {
           

            this.PointCloud.Vectors = new Vector3[8];
            this.PointCloud.Vectors[0] = new Vector3(-0.5f, -0.5f, -0.5f);
            this.PointCloud.Vectors[1] = new Vector3(0.5f, -0.5f, -0.5f);
            this.PointCloud.Vectors[2] = new Vector3(0.5f, 0.5f, -0.5f);
            this.PointCloud.Vectors[3] = new Vector3(-0.5f, 0.5f, -0.5f);
            this.PointCloud.Vectors[4] = new Vector3(-0.5f, -0.5f, 0.5f);
            this.PointCloud.Vectors[5] = new Vector3(0.5f, -0.5f, 0.5f);
            this.PointCloud.Vectors[6] = new Vector3(0.5f, 0.5f, 0.5f);
            this.PointCloud.Vectors[7] = new Vector3(-0.5f, 0.5f, 0.5f);


            this.PointCloud.Colors = new Vector3[8];
            this.PointCloud.Colors[0] = new Vector3(1f, 0f, 0f);
            this.PointCloud.Colors[1] = new Vector3(0f, 0f, 1f);
            this.PointCloud.Colors[2] = new Vector3(0f, 1f, 0f);
            this.PointCloud.Colors[3] = new Vector3(1f, 0f, 0f);
            this.PointCloud.Colors[4] = new Vector3(0f, 0f, 1f);
            this.PointCloud.Colors[5] = new Vector3(0f, 1f, 0f);
            this.PointCloud.Colors[6] = new Vector3(1f, 0f, 0f);
            this.PointCloud.Colors[7] = new Vector3(0f, 0f, 1f);


            //for (int i = 0; i < 8; i++)
            //{
            //    this.PointCloud.Colors[i] = new Vector3(0, 1, 0);
            //}

        }
        public override void FillPointCloud()
        {


            this.PointCloud.Vectors = new Vector3[]{
            new Vector3(-1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f,  1.0f,  1.0f),
            new Vector3(-1.0f,  1.0f,  1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3( 1.0f, -1.0f, -1.0f), 
            new Vector3( 1.0f,  1.0f, -1.0f),
            new Vector3(-1.0f,  1.0f, -1.0f) };



            this.PointCloud.Colors = new Vector3[8];
            this.PointCloud.Colors[0] = new Vector3(1f, 0f, 0f);
            this.PointCloud.Colors[1] = new Vector3(0f, 0f, 1f);
            this.PointCloud.Colors[2] = new Vector3(0f, 1f, 0f);
            this.PointCloud.Colors[3] = new Vector3(1f, 0f, 0f);
            this.PointCloud.Colors[4] = new Vector3(0f, 0f, 1f);
            this.PointCloud.Colors[5] = new Vector3(0f, 1f, 0f);
            this.PointCloud.Colors[6] = new Vector3(1f, 0f, 0f);
            this.PointCloud.Colors[7] = new Vector3(0f, 0f, 1f);


            //for (int i = 0; i < 8; i++)
            //{
            //    this.PointCloud.Colors[i] = new Vector3(0, 1, 0);
            //}

        }
        public override void FillIndexBuffer()
        {
            this.PointCloud.Indices = new uint[]{
             // front face
                0, 1, 2, 2, 3, 0,
                // top face
                3, 2, 6, 6, 7, 3,
                // back face
                7, 6, 5, 5, 4, 7,
                // left face
                4, 0, 3, 3, 7, 4,
                // bottom face
                0, 1, 5, 5, 4, 0,
                // right face
                1, 5, 6, 6, 2, 1 };


            //this.PointCloud.Indices = new ushort[] {
            //     //bottom
            //   0, 1, 5,
            //    0, 5, 4,
            //    //top
            //    2, 3, 6,
            //    6, 3, 7,
            //     //front
            //    0, 7, 3,
            //    0, 4, 7,
            //      //back
            //    1, 2, 6,
            //    6, 5, 1,
            //   //left
            //    0, 2, 1,
            //    0, 3, 2,
            //    //right
            //    4, 5, 6,
            //    6, 7, 4
          
            //};

            //this.PointCloud.Indices = new ushort[36];

            ////bottom face
            //this.PointCloud.Indices[0] = 0;             
            //this.PointCloud.Indices[1] = 5;
            //this.PointCloud.Indices[2] = 4;
            //this.PointCloud.Indices[3] = 5;
            //this.PointCloud.Indices[4] = 0;
            //this.PointCloud.Indices[5] = 1;


            ////top face
            //this.PointCloud.Indices[6] = 3;
            //this.PointCloud.Indices[7] = 7;
            //this.PointCloud.Indices[8] = 6;
            //this.PointCloud.Indices[9] = 3;
            //this.PointCloud.Indices[10] = 6;
            //this.PointCloud.Indices[11] = 2;

           

            ////front face
            //this.PointCloud.Indices[12] = 7;
            //this.PointCloud.Indices[13] = 4;
            //this.PointCloud.Indices[14] = 6;
            //this.PointCloud.Indices[15] = 6;
            //this.PointCloud.Indices[16] = 4;
            //this.PointCloud.Indices[17] = 5;
           

            ////back face
            //this.PointCloud.Indices[18] = 2;
            //this.PointCloud.Indices[19] = 1;
            //this.PointCloud.Indices[20] = 3;
            //this.PointCloud.Indices[21] = 3;
            //this.PointCloud.Indices[22] = 1;
            //this.PointCloud.Indices[23] = 0;
          

            ////left face
            //this.PointCloud.Indices[24] = 3;
            //this.PointCloud.Indices[25] = 0;
            //this.PointCloud.Indices[26] = 7;
            //this.PointCloud.Indices[27] = 7;
            //this.PointCloud.Indices[28] = 0;
            //this.PointCloud.Indices[29] = 4;
           

            ////right face
            //this.PointCloud.Indices[30] = 6;
            //this.PointCloud.Indices[31] = 5;
            //this.PointCloud.Indices[32] = 2;
            //this.PointCloud.Indices[33] = 2;
            //this.PointCloud.Indices[34] = 5;
            //this.PointCloud.Indices[35] = 1;
          
        }

        
    }
}