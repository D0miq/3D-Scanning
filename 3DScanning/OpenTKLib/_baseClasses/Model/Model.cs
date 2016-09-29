using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.IO;
using System.Drawing;
using System.Globalization;

namespace OpenTKLib
{
    public class Model
    {

        public PointCloud PointCloud;
        public delegate double[] CoordFuncXYZ(double u, double v);

        public string Name;

        public Model()
        {
        }
        public Model(string path, string fileName)
            : this(path + "\\" + fileName)
        {
            this.Name = fileName;

        }
        public Model(string fileName)
        {
            string str = Path.GetExtension(fileName).ToLower();

            this.Name = IOUtils.ExtractFileNameShort(fileName);
            this.Name = IOUtils.ExtractFileNameWithoutExtension(this.Name);

            if (str == ".obj")
                this.PointCloud = PointCloud.FromObjFile(fileName);
            if (str == ".xyz")
                this.PointCloud = PointCloud.FromXYZFile(fileName);
            if (GLSettings.PointCloudCentered)
            {
                if (PointCloud != null)
                    this.PointCloud.ResetCentroid(true);
            }


        }
       

        public PointCloudVertices PointCloudVertices
        {
            get
            {
                return PointCloud.ToPointCloudVertices();
            }

        }
        public void CalculateNormals_PCA()
        {
            List<Vector3> normals = Model.CalculateNormals_PCA(this.PointCloud.ToPointCloudVertices(), 4, false, true);
            this.PointCloud.Normals = normals.ToArray();


        }
        public static List<Vector3> CalculateNormals_PCA(PointCloudVertices pointCloud, int numberOfNeighbours, bool centerOfMassMethod, bool flipNormalWithOriginVector)
        {

            KDTreeVertex kv = new KDTreeVertex();
            kv.NumberOfNeighboursToSearch = numberOfNeighbours;
            kv.BuildKDTree_Rednaxela(pointCloud);
            kv.ResetVerticesLists(pointCloud);
            kv.FindNearest_NormalsCheck_Rednaxela(pointCloud, false);



            PCA pca = new PCA();
            List<Vector3> normals = pca.Normals(pointCloud, centerOfMassMethod, flipNormalWithOriginVector);

            return normals;


        }
        public List<Vector3> Normals
        {
            get
            {
                return this.PointCloud.Normals.ToList<Vector3>();
            }
            set
            {
                this.PointCloud.Normals = value.ToArray();
            }
        }
     

    }
}
