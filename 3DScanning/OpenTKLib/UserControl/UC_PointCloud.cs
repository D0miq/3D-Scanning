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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
 
 
using OpenTK;
namespace OpenTKLib
{
    public partial class OpenGLUC
    {
      
   
        public void AddVertexListAsModel(string name, PointCloudVertices myVertexList)
        {
            
            Model myModel = new Model();
            myModel.PointCloud = myVertexList.ToPointCloud();
           
           
            this.glControl1.GLrender.AddModel(myModel);

        }
        public void ShowPointCloud(string name, PointCloudVertices myVertexList)
        {

            AddVertexListAsModel(name, myVertexList);
            //ShowModels();

        }
        public void RemoveFirstModel(bool refresh)
        {
            //this.OGLControl.GLrender.RemoveAllModels();
            //this.Refresh();

            //Model3D myModel = this.GLrender.Models3D[0];
            //for (int index = 0; index < myModel.Parts.Count; ++index)
            //    GL.DeleteLists(myModel.Parts[index].GLListNumber, 1);
            
            //lock (this.GLrender.Models3D)
            //    this.GLrender.RemoveModel(0);

            //if (refresh)
            //{
            //    GC.Collect();
            //    this.glControl1.Refresh();
            //}
        }
        public void RemoveAllModels()
        {
            this.OGLControl.GLrender.ClearAllObjects();
            this.comboModels.Items.Clear();
            this.comboModels.Items.Add("All");
            this.comboModels.SelectedItem = "All";
            this.OGLControl.Refresh();

           
        }
        public void RemoveAllModelsSlow()
        {
            //for (int i = GLrender.Models3D.Count - 1; i >= 0; i--)
            //{
            //    RemoveModel(i);
            //}
        }
     


    }
}
