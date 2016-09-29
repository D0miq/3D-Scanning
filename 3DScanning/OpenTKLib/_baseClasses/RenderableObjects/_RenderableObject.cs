using System.Collections.Generic;

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKLib
{

    public abstract class RenderableObject
    {
        
        public PointCloud PointCloud = new PointCloud();
        public Shader shader = new Shader();

        protected bool initialized;

        public Vector3 Position = Vector3.Zero;
       
        //public Vector3d Scale = Vector3d.One;
        public double Scale = 1f;
        
        private Matrix4 p;
       // private Matrix4d mvp;
        Matrix4 m;
        Matrix4 v;
        Matrix4 mvp;


        protected string path = AppDomain.CurrentDomain.BaseDirectory;
        public Vector3 color = new Vector3(1, 1, 1);

        protected int vaoID;
        protected int vboVerticesID;
        protected int vboColorsID;
        protected int vboIndicesID;
        protected int vboNormalsID;
       // protected int vboUniform;

            

        protected PrimitiveType primitiveType;

        public virtual void FillPointCloud()
        { }
        public virtual void FillIndexBuffer()
        { }
       
      
       
        public RenderableObject()
        {

        }
        public RenderableObject Clone()
        {
            RenderableObject o = new PointCloudRenderable();
            o.PointCloud = this.PointCloud;

            return o;

        }
        public virtual void InitializeGL()
        {
        }
        private void deleteBuffers()
        {
            //Destroy vao and vbo
            GL.DeleteBuffers(1, ref vboVerticesID);
            GL.DeleteBuffers(1, ref vboIndicesID);
            GL.DeleteBuffers(1, ref vboColorsID);
            GL.DeleteBuffers(1, ref vboNormalsID);
            GL.DeleteVertexArrays(1, ref vaoID);
        }
        public virtual void Dispose()
        {
            //Destroy shader
            shader.Dispose();
            deleteBuffers();
           
        }
       
        protected void initBuffers()
        {
               
            // Generate Array Buffer Id-s
            GL.GenVertexArrays(1, out vaoID);
            GL.GenBuffers(1, out vboVerticesID);
            GL.GenBuffers(1, out vboColorsID);
            GL.GenBuffers(1, out vboIndicesID);
            GL.GenBuffers(1, out vboNormalsID);
            

        }
        protected bool InitShaders(string vertShaderFilename, string fragShaderFilename, string mypath)
        {
            try
            {
                shader = new Shader();
                if (!shader.InitializeShaders(vertShaderFilename, fragShaderFilename, mypath))
                    return false;
                GL.UseProgram(shader.ProgramID);

            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Shader initialization failed - check if shader files are available : " + vertShaderFilename + " : " + err.Message);
                System.Windows.Forms.MessageBox.Show("Shader initialization failed - check if shader files are available: " + mypath + "\\" + vertShaderFilename + " : " + err.Message);
                throw new Exception("Shader initialization failed - check if shader files are available : " + vertShaderFilename + " : " + err.Message);
                //return false;

            }
            return true;

        }
        protected void CheckGLError()
        {
            ErrorCode code = GL.GetError();
            if(code != ErrorCode.NoError)
            {
                throw new Exception("GL Error : " + code.ToString());
            }

        }
        protected void RefreshRenderableData()
        {
            try
            {
                //seems to be essential for multiple GL contexts
                deleteBuffers();
                initBuffers();

                GL.BindVertexArray(vaoID);

                //bind vertices: this.PointCloud.Vectors (pointer: vboVerticesID) to -> "vVertex" in shader 
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboVerticesID);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(PointCloud.Vectors.Length * Vector3.SizeInBytes), this.PointCloud.Vectors, BufferUsageHint.StaticDraw);
                //CheckGLError();

                shader.EnableAttribute("vVertex", false);

                //CheckGLError();

                if (this.PointCloud.Colors != null)
                {

                    //bind vertices: this.PointCloud.Colors (pointer: vboColorsID) to -> "vColor" in shader 
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vboColorsID);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(this.PointCloud.Colors.Length * Vector3.SizeInBytes), this.PointCloud.Colors, BufferUsageHint.StaticDraw);

                    shader.EnableAttribute("vColor", false);

                    //CheckGLError();

                }

                //normals - bound to Position vectors
                //only if normals are there in attributes
                if (shader.GetAttributeAddress("vNormal") != -1)
                {

                    GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormalsID);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(PointCloud.Vectors.Length * Vector3.SizeInBytes), this.PointCloud.Vectors, BufferUsageHint.StaticDraw);

                    shader.EnableAttribute("vNormal", false);

                }


                // Bind current context to Array Buffer ID
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboIndicesID);
                // Send data to buffer
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(this.PointCloud.Indices.Length * sizeof(uint)), this.PointCloud.Indices, BufferUsageHint.StaticDraw);


                // Validate that the buffer is the correct size
                //ValidateBufferSize();

            }
            catch(Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Error in Activate Shaders " + err.Message);
                System.Windows.Forms.MessageBox.Show("OpenGL Error in Shaders: " + err.Message);
                //throw new Exception("Error in Activate Shaders " + err.Message);

            }
        }
       
  
        private void ValidateBufferSize()
        {
            int bufferSize = 0;
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (this.PointCloud.Indices.Length * sizeof(uint) != bufferSize)
                throw new ApplicationException("Indices array not uploaded correctly");

            //CheckGLError();
        }

        public PrimitiveType PrimitiveType
        {
            get
            {
                return primitiveType;
            }
        }

        #region public properties
        public virtual Matrix4 M
        {
            get
            {
                return m;
            }
            set
            {
                m = value;
            }
        }
     
        public virtual Matrix4 V
        {
            get
            {

                return v;
            }
            set
            {
                v = value;

            }
        }
       
        
        public virtual Matrix4 P
        {
            get
            {
                return p;

            }
            set
            {
                p = value;
            }

        }
        public virtual Matrix4 MVP
        {
            get
            {
                return mvp;

            }
            set
            {
                mvp = value;
            }

        }

        #endregion

        public void Render(PrimitiveType myRenderMode, PolygonMode myPolygonMode)
        {
            RefreshRenderableData();//essential if data has changed after initial call of ActivateShaders

            this.shader.Use();
            GL.UniformMatrix4(shader.GetUniformAddress("MVP"), false, ref this.mvp);
            switch (myRenderMode)
            {

                case PrimitiveType.Triangles:
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                        GL.DrawElements(PrimitiveType.Triangles, this.PointCloud.Indices.Length, DrawElementsType.UnsignedInt, 0);
                       
                        break;
                    }
                case PrimitiveType.TriangleFan:
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                        GL.DrawElements(PrimitiveType.TriangleFan, this.PointCloud.Indices.Length, DrawElementsType.UnsignedInt, 0);
                        
                        break;
                    }
                case PrimitiveType.TriangleStrip:
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                        GL.DrawElements(PrimitiveType.TriangleStrip, this.PointCloud.Indices.Length, DrawElementsType.UnsignedInt, 0);
                        //GL.DrawArrays(PrimitiveType.Triangles, 0, this.PointCloudGL.Indices.Length);
                        break;
                    }
                //case PrimitiveType.Polygon:
                //    {
                //        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                //        GL.DrawElements(PrimitiveType.Polygon, this.PointCloudOpenGL.Indices.Length, DrawElementsType.UnsignedInt, 0);
                //        break;
                //    }
            
                case PrimitiveType.Points:
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
                    //    GL.PolygonMode(MaterialFace.FrontAndBack, myPolygonMode);
                        GL.DrawArrays(myRenderMode, 0, this.PointCloud.Indices.Length);
                        break;
                    }
                default:
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, myPolygonMode);
                        GL.DrawElements(myRenderMode, this.PointCloud.Indices.Length, DrawElementsType.UnsignedInt, 0);
                        break;
                       
                    }
              
            }

        }
     
        public void RenderArrays(PrimitiveType myRenderMode)
        {
            GL.UniformMatrix4(shader.GetUniformAddress("MVP"), false, ref this.mvp);
            //to draw vertices:
            GL.DrawArrays(myRenderMode, 0, this.PointCloud.Indices.Length);

        }
        public void RenderIndices()
        {
           
            GL.UniformMatrix4(shader.GetUniformAddress("MVP"), false, ref this.mvp);

         
            GL.DrawElements(this.primitiveType, this.PointCloud.Indices.Length, DrawElementsType.UnsignedInt, 0);

        }
        public void RenderIndices(PrimitiveType myRenderMode)
        {
            GL.UniformMatrix4(shader.GetUniformAddress("MVP"), false, ref this.mvp);

            GL.DrawElements(myRenderMode, this.PointCloud.Indices.Length, DrawElementsType.UnsignedInt, 0);


        }
      
      
    }


}