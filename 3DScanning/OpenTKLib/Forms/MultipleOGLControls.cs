// Pogramming by
//     Douglas Andrade ( http://www.cmsoft.com.br, email: cmsoft@cmsoft.com.br)
//               Implementation of most of the functionality
//     Edgar Maass: (email: maass@logisel.de)
//               Code adaption, changed to user control
//
//Software used: 
//    OpenGL : http://www.opengl.org
//    OpenTK : http://www.opentk.com
//
// DISCLAIMER: Users rely upon this software at their own risk, and assume the responsibility for the results. Should this software or program prove defective, 
// users assume the cost of all losses, including, but not limited to, any necessary servicing, repair or correction. In no event shall the developers or any person 
// be liable for any loss, expense or damage, of any type or nature arising out of the use of, or inability to use this software or program, including, but not
// limited to, claims, suits or causes of action involving alleged infringement of copyrights, patents, trademarks, trade secrets, or unfair competition. 
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using OpenTKLib;
using System.Windows.Media.Media3D;
using OpenTK;

namespace OpenTKLib
{
    public partial class MultipleOGLControls : Form
    {

        
        
        public OpenGLUC OpenGLControl;

        public MultipleOGLControls()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(CultureInfo.CurrentCulture.LCID);
            InitializeComponent();
           
         
            AddOpenGLControl();

            if (!GLSettings.IsInitializedFromSettings)
                GLSettings.InitFromSettings();

            this.Height = GLSettings.Height;
            this.Width = GLSettings.Width;


        }
        private void AddOpenGLControl()
        {
            this.OpenGLControl = new OpenGLUC();
            this.SuspendLayout();
            // 
            // openGLControl1
            // 
            this.OpenGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenGLControl.Location = new System.Drawing.Point(0, 0);
            this.OpenGLControl.Name = "openGLControl1";
            this.OpenGLControl.Size = new System.Drawing.Size(854, 453);
            this.OpenGLControl.TabIndex = 0;

           
          
            this.ResumeLayout(false);
        }
    
   

        protected override void OnClosed(EventArgs e)
        {
            GlobalVariables.FormFast = null;
            GLSettings.Height = this.Height;
            GLSettings.Width = this.Width;

            GLSettings.SaveSettings();
            base.OnClosed(e);
        }
        public void AddVerticesAsModel(string name, PointCloudVertices myPCLList)
        {
            this.OpenGLControl.AddVertexListAsModel(name, myPCLList);

           
        }
       

        //public void ShowListOfVertices(PointCloudVertices myPCLList)
        //{
        //    this.OpenGLControl.ShowPointCloud("Point Cloud", myPCLList);

        //}
        public void ShowPointCloudOpenGL(PointCloud myP, bool removeOthers)
        {
            if (removeOthers)
                this.OpenGLControl.RemoveAllModels();
            
            Model myModel = new Model();
            myModel.PointCloud = myP;

            this.OpenGLControl.OGLControl.GLrender.AddModel(myModel);

        }
        /// <summary>
        /// at least source points should be non zero
        /// </summary>
        /// <param name="myPCLTarget"></param>
        /// <param name="myPCLSource"></param>
        /// <param name="myPCLResult"></param>
        /// <param name="changeColor"></param>
        public void Show3PointCloudOpenGL(PointCloud myPCLSource, PointCloud myPCLTarget, PointCloud myPCLResult, bool changeColor)
        {

            this.OpenGLControl.RemoveAllModels();

            //target in green
            
            if (myPCLTarget != null)
            {

                if (changeColor)
                {
                    myPCLTarget.Colors = ColorExtensions.ToVector3Array(myPCLTarget.Vectors.GetLength(0), 0, 255, 0);
                    
                }
                ShowPointCloudOpenGL(myPCLTarget, false);

            }

            if (myPCLSource != null)
            {
                //source in white
               
                if (changeColor)
                    myPCLSource.Colors = ColorExtensions.ToVector3Array(myPCLSource.Vectors.GetLength(0), 255, 255, 255);

                ShowPointCloudOpenGL(myPCLSource, false);

            }

            if (myPCLResult != null)
            {

                //transformed in red
                if (changeColor)
                    myPCLResult.Colors = ColorExtensions.ToVector3Array(myPCLResult.Vectors.GetLength(0), 255, 0, 0);

                ShowPointCloudOpenGL(myPCLResult, false);

               

            }

        }
   
        /// <summary>
        /// at least source points should be non zero
        /// </summary>
        /// <param name="myPCLTarget"></param>
        /// <param name="myPCLSource"></param>
        /// <param name="myPCLResult"></param>
        /// <param name="changeColor"></param>
        public void Show3PointClouds(PointCloudVertices myPCLSource, PointCloudVertices myPCLTarget, PointCloudVertices myPCLResult, bool changeColor)
        {

            this.OpenGLControl.RemoveAllModels();

            //target in green
            List<System.Drawing.Color> myColors;
            if (myPCLTarget != null)
            {

                if (changeColor)
                {
                    myColors = ColorExtensions.ToColorList(myPCLTarget.Count, 0, 255, 0, 255);
                    PointCloudVertices.SetColorToList(myPCLTarget, myColors);
                }
                this.OpenGLControl.ShowPointCloud("ICP Target", myPCLTarget);

            }

            if (myPCLSource != null)
            {
                //source in white
                myColors = ColorExtensions.ToColorList(myPCLSource.Count, 255, 255, 255, 255);
                if (changeColor)
                    PointCloudVertices.SetColorToList(myPCLSource, myColors);
                this.OpenGLControl.ShowPointCloud("ICP To be matched", myPCLSource);

            }

            if (myPCLResult != null)
            {

                //transformed in red
                myColors = ColorExtensions.ToColorList(myPCLResult.Count, 255, 0, 0, 255);
                if (changeColor)
                    PointCloudVertices.SetColorToList(myPCLResult, myColors);
                this.OpenGLControl.ShowPointCloud("ICP Solution", myPCLResult);

            }

        }
       
        public void ClearModels()
        {
            OpenGLControl.RemoveAllModels();
        }
       
        public bool UpdateFirstModel(PointCloudVertices pc)
        {
            //ClearModels();


            ShowPointCloud(pc);
            return true;
        }

        public void ShowPointCloud(PointCloudVertices pc)
        {

            this.OpenGLControl.ShowPointCloud("Color Point Cloud", pc);
            
        }

      
        private void OpenTKForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (GlobalVariables.FormFast != null)
                GlobalVariables.FormFast.Dispose();

            GlobalVariables.FormFast = null;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.OpenGLControl.OGLControl.MouseWheelActions(e);
            base.OnMouseWheel(e);
        }

    
        public void IPCOnTwoPointClouds()
        {

            this.OpenGLControl.RemoveAllModels();
            //this.OpenGLControl.OpenTwoTrialPointClouds();
            ICP_OnCurrentModels();

        }

        public bool UpdatePointCloud(PointCloudVertices pc)
        {
            if (pc != null && pc.Count > 0)
                this.OpenGLControl.ShowPointCloud("Color Point Cloud", pc);

            //if (this.OpenGLControl.GLrender.Models3D.Count == 0)
            //{
            //    ShowPointCloud(pc);
            //}
            //else
            //{
            //    this.OpenGLControl.RemoveFirstModel(true);
            //    Model3D myNewModel = new Model3D();
            //    myNewModel.Pointcloud = pc;
            //    //this.OpenGLControl.GLrender.Models3D[0].Pointcloud = pc;
            //    this.OpenGLControl.GLrender.Models3D.Add(myNewModel);
            //    this.OpenGLControl.RedrawAllModels(false);
            //    //this.OpenGLControl.Refresh();
            //}

            ////ClearModels();


            return true;
        }
        public void ICP_OnCurrentModels()
        {

            ////convert Points
            //if (this.OpenGLControl.GLrender.Models3D.Count > 1)
            //{
            //    PointCloud myPCLTarget = this.OpenGLControl.GLrender.Models3D[0].Pointcloud;
            //    PointCloud myPCLSource = this.OpenGLControl.GLrender.Models3D[1].Pointcloud;


            //    ResetModelsToOrigin();

            //    IterativeClosestPointTransform icpSharp = new IterativeClosestPointTransform();
            //    PointCloud myVertexTransformed = icpSharp.PerformICP(myPCLSource, myPCLTarget);

            //    if (myVertexTransformed != null)
            //    {
            //        //show result
            //        PointCloud.SetColorOfListTo(myVertexTransformed, Color.Red);
            //        this.OpenGLControl.ShowPointCloud("IPC Solution", myVertexTransformed);
            //    }

            //}


        }
        //public void ShowModel(Model myModel, bool removeAllOthers)
        //{
        //    if(removeAllOthers)
        //        this.OpenGLControl.RemoveAllModels();
        //    this.OpenGLControl.ShowModel(myModel);


        //    Model myModel = new Model();
        //    myModel.pointCloudGL = myModel.Pointcloud.ToPointCloudOpenGL();

        //    //this.glControl1.GLrender.AddModel(myModel);
        //    this.OpenGLControl.OGLControl.GLrender.AddModel(myModel);

        //}

    }
}
