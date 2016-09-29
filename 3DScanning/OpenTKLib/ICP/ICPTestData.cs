using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Diagnostics;
using OpenTKLib;

namespace ICPLib
{
    public class ICPTestData
    {
        //private static int cubeSize ;

        //private static PointCloud pointCloudTarget;
        //private static PointCloud pointCloudSource;
       // private static PointCloud vectorsResult;

        public static double Test1_Translation(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {
            myPCLTarget = PointCloudVertices.CreateSomePoints();
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);
            PointCloudVertices.Translate(myPCLSource, 10, 3, 8);

            
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;

        }
        public static double Test2_RotationX30Degrees(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {
            
            
            //myPCLTarget = Vertices.CreateSomePoints();
            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            Matrix3d R = Matrix3d.CreateRotationX(30);
            PointCloudVertices.Rotate(myPCLSource, R);


            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;

        }
        public static double Test2_Identity(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {


            //myPCLTarget = Vertices.CreateSomePoints();
            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);


            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;

        }
  
        public static double Test2_RotationXYZ(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {
            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);


            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(90, 124, -274);

            PointCloudVertices.Rotate(myPCLSource, R);


            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test3_Scale(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {
          
            //myPCLTarget = CreateSomePoints();
            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.ScaleByFactor(myPCLSource, 0.2f);
            
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
       
        public static double Test5_CubeTranslation(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(cubeSize);
            
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.Translate(myPCLSource, 0, -300, 0);
            
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_CubeTranslation2(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(cubeSize);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.Translate(myPCLSource, 10, 10,-10);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_CubeRotate(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(90, 124, -274);
            PointCloudVertices.Rotate(myPCLSource, R);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        private static Matrix3d CreateAndPrintMatrix(double x, double y, double z)
        {
             Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(x,y,z);
            R.WriteMatrix();

            return R;
        }
        public static double Test5_CubeRotate45(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {


            myPCLTarget = PointCloud.CreateCube_RandomPointsOnPlanes(cubeSize, 10).ToPointCloudVertices();
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            Matrix3d R = CreateAndPrintMatrix(45, 45, 45);
            PointCloudVertices.Rotate(myPCLSource, R);
         
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_Cube8RotateShuffle(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(cubeSize);
            
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            Matrix3d R = CreateAndPrintMatrix(45, 45, 45);
            PointCloudVertices.Rotate(myPCLSource, R);

            PointCloudVertices.ShuffleTest(myPCLSource);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_Cube8Shuffle(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(cubeSize);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.ShuffleTest(myPCLSource);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_Cube8Shuffle_60000(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 100);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.ShuffleRandom(myPCLSource);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_Cube8Shuffle_1Milion(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 409);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.ShuffleRandom(myPCLSource);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_Cube8TranslateRotateShuffleNew(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(cubeSize);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);
            //Model3D myModel = Example3DModels.Cuboid("Cuboid", 20f, 40f, 100, System.Drawing.Color.White, null);

            Matrix3d R = CreateAndPrintMatrix(65, -123, 35);
            PointCloudVertices.Rotate(myPCLSource, R);
            PointCloudVertices.Translate(myPCLSource, cubeSize * 1.2f, -cubeSize * 2.5f, cubeSize *2);

            PointCloudVertices.ShuffleTest(myPCLSource);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_CubeInhomogenous(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.ScaleByVertex(myPCLSource, new Vertex(1,2,3));
            
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_CubeScale_Uniform(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);
            PointCloudVertices.ScaleByFactor(myPCLSource, 0.2f);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_CubeScale_Inhomogenous(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);
            
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);
            PointCloudVertices.ScaleByVertex(myPCLSource, new Vertex(1, 2, 3));
            
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_CuboidIdentity(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            double cubeSize = 1;
            double cubeSizeY = 2;
            int numberOfPoints = 3;
            Model myModel = Example3DModels.Cuboid("Cuboid", cubeSize, cubeSizeY, numberOfPoints, System.Drawing.Color.White, null);
            myPCLTarget = myModel.PointCloudVertices;



            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            //Vertices.Translate(myPCLSource, 220, -300, 127);
            //Vertices.ScaleByFactor(myPCLSource, 0.2);

            //Matrix3d R = new Matrix3d();
            //R = R.RotationXYZDegrees(90, 124, -274);
            //Vertices.Rotate(myPCLSource, R);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_CuboidRotate(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            double cubeSize = 1;
            double cubeSizeY = 2;
            int numberOfPoints = 3;
            Model myModel = Example3DModels.Cuboid("Cuboid", cubeSize, cubeSizeY, numberOfPoints, System.Drawing.Color.White, null);
            myPCLTarget = myModel.PointCloudVertices;



            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            //Vertices.Translate(myPCLSource, 220, -300, 127);
            //Vertices.ScaleByFactor(myPCLSource, 0.2);
            
            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(90, 124, -274);
            PointCloudVertices.Rotate(myPCLSource, R);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_CubeRotateTranslate_ScaleUniform(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

           

            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            //Vertices.Translate(myPCLSource, 220, -300, 127);
            //Vertices.ScaleByFactor(myPCLSource, 0.2);

            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(90, 124, -274);
            PointCloudVertices.Rotate(myPCLSource, R);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test5_CubeRotateTranslate_ScaleInhomogenous(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.Translate(myPCLSource, 0, 0, 149);
            PointCloudVertices.ScaleByVertex(myPCLSource, new Vertex(1, 2, 3));

            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(90, 124, -274);
            PointCloudVertices.Rotate(myPCLSource, R);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
     
        public static double Test6_Bunny(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {


            string path = AppDomain.CurrentDomain.BaseDirectory + "TestData";
            
            Model model3DTarget = new Model(path + "\\bunny.obj");
            myPCLTarget = model3DTarget.PointCloudVertices;
            

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.Translate(myPCLSource, -0.15f, 0.05f, 0.02f);
            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(30, 30, 30);
            PointCloudVertices.Rotate(myPCLSource, R);
            PointCloudVertices.ScaleByFactor(myPCLSource, 0.8f);


            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);

            return IterativeClosestPointTransform.Instance.MeanDistance;


        }
        public static double Test6_Bunny_PCA(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {


            string path = AppDomain.CurrentDomain.BaseDirectory + "TestData";

            Model model3DTarget = new Model(path + "\\bunny.obj");
            myPCLTarget = model3DTarget.PointCloudVertices;
            
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);


            
            Matrix3d R = new Matrix3d();
            //ICP converges with a rotation of
            //R = R.RotationXYZ(60, 60, 60);
            R = R.RotationXYZDegrees(124, 124, 124);
            

            PointCloudVertices.Rotate(myPCLSource, R);
            PCA pca = new PCA();
            myPCLResult = pca.AlignPointClouds_SVD( myPCLSource, myPCLTarget);
            if (pca.MeanDistance > 1e-5)
                myPCLResult = pca.AlignPointClouds_SVD( myPCLResult, myPCLTarget);

            //myPCLResult = pca.AlignPointClouds_SVD(myPCLResult, myPCLTarget);

            //myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLResult, myPCLTarget);

            return IterativeClosestPointTransform.Instance.MeanDistance;


        }
        public static double Test6_Bunny_ExpectedError(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {


            string path = AppDomain.CurrentDomain.BaseDirectory + "TestData";

            Model model3DTarget = new Model(path + "\\bunny.obj");
            myPCLTarget = model3DTarget.PointCloudVertices;

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);



            Matrix3d R = new Matrix3d();
            //ICP converges with a rotation of
            //R = R.RotationXYZ(60, 60, 60);
            R = R.RotationXYZRadiants(65, 65, 65);


            PointCloudVertices.Rotate(myPCLSource, R);
           

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);

            return IterativeClosestPointTransform.Instance.MeanDistance;


        }
        public static double Test7_Face_KnownTransformation_15000(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + "Models\\UnitTests";
            Model model3DTarget = new Model(path + "\\KinectFace_1_15000.obj");
            myPCLTarget = model3DTarget.PointCloudVertices;
            

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);


           

            PointCloudVertices.ScaleByFactor(myPCLSource, 0.9f);
            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(60, 60, 60);
            PointCloudVertices.Rotate(myPCLSource, R);
            PointCloudVertices.Translate(myPCLSource, 0.3f, 0.5f, -0.4f);

            IterativeClosestPointTransform.Instance.ICPSettings.MaximumNumberOfIterations = 44;
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test7_Face_KnownTransformation_PCA_55000(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + "Models\\UnitTests";
            Model model3DTarget = new Model(path + "\\KinectFace_1_55000.obj");
            myPCLTarget = model3DTarget.PointCloudVertices;


            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);


            PointCloudVertices.ScaleByFactor(myPCLSource, 0.9f);
            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(60, 60, 60);
            PointCloudVertices.Rotate(myPCLSource, R);
            PointCloudVertices.Translate(myPCLSource, 0.3f, 0.5f, -0.4f);

            PCA pca = new PCA();
            myPCLResult = pca.AlignPointClouds_SVD( myPCLSource, myPCLTarget);
            myPCLSource = myPCLResult;

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test7_Face_KnownTransformation_PCA_15000(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + "Models\\UnitTests";
            Model model3DTarget = new Model(path + "\\KinectFace_1_15000.obj");
            myPCLTarget = model3DTarget.PointCloudVertices;
            
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);


            PointCloudVertices.ScaleByFactor(myPCLSource, 0.9f);
            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(60, 60, 60);
            PointCloudVertices.Rotate(myPCLSource, R);
            PointCloudVertices.Translate(myPCLSource, 0.3f, 0.5f, -0.4f);

            PCA pca = new PCA();
            myPCLResult = pca.AlignPointClouds_SVD(myPCLSource, myPCLTarget);
            myPCLSource = myPCLResult;

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
      
        public static double Test8_CubeOutliers_Translate(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            myPCLTarget = PointCloudVertices.CreateCube_Corners(20);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);
            PointCloudVertices.Translate(myPCLSource, 0, -300, 0);
            PointCloudVertices.CreateOutliers(myPCLSource, 5);
            
            
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test8_CubeOutliers_Rotate(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {

            //myPCLTarget = Vertices.CreateCube_Corners(50);
            Model myModel = Example3DModels.Cuboid("Cuboid", 20f, 40f, 100, System.Drawing.Color.White, null);
            myPCLTarget = myModel.PointCloudVertices;

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);
            Matrix3d R = new Matrix3d();
            R = R.RotationXYZDegrees(30, 30, 30);
            PointCloudVertices.Rotate(myPCLSource, R);

            PointCloudVertices.CreateOutliers(myPCLSource, 5);
            

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test9_Inhomogenous(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {
            myPCLTarget = PointCloudVertices.CreateCube_Corners(50);
            //myPCLTarget = Vertices.CreateSomePoints();
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.InhomogenousTransform(myPCLSource, 2);

            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test9_Face_Stitch(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {


            string path = AppDomain.CurrentDomain.BaseDirectory + "Models\\UnitTests";
            Model model3DTarget = new Model(path + "\\KinectFace_1_15000.obj");
            myPCLTarget = model3DTarget.PointCloudVertices;
            
            Model model3DSource = new Model(path + "\\KinectFace_2_15000.obj");
            myPCLSource = model3DSource.PointCloudVertices;
            
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
 
        public static double Test10_Cube8pRotateShuffle(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 1);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);
            
            Matrix3d R = CreateAndPrintMatrix(65, -123, 35);
            PointCloudVertices.Rotate(myPCLSource, R);
            
            PointCloudVertices.ShuffleTest(myPCLSource);

         
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test10_Cube8pRotateTranslateShuffle(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 1);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.Translate(myPCLSource , cubeSize* 1.2f, -cubeSize * 2.5f , cubeSize * 2);

            Matrix3d R = CreateAndPrintMatrix(65, -123, 35);
            PointCloudVertices.Rotate(myPCLSource, R);

            PointCloudVertices.ShuffleTest(myPCLSource);


            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test10_Cube8pRotateTranslateScaleShuffle(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 1);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.Translate(myPCLSource, cubeSize * 1.2f, -cubeSize * 2.5f, cubeSize * 2);
            PointCloudVertices.ScaleByFactor(myPCLSource, 0.2f);
            Matrix3d R = CreateAndPrintMatrix(65, -123, 35);
            PointCloudVertices.Rotate(myPCLSource, R);

            PointCloudVertices.ShuffleTest(myPCLSource);


            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test10_Cube26pRotateTranslateScaleShuffle(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 2);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            PointCloudVertices.Translate(myPCLSource, cubeSize * 1.2f, -cubeSize * 2.5f, cubeSize * 2);

            PointCloudVertices.ScaleByFactor(myPCLSource, 0.2f);
            Matrix3d R = CreateAndPrintMatrix(65, -123, 35);
            PointCloudVertices.Rotate(myPCLSource, R);

            PointCloudVertices.ShuffleTest(myPCLSource);


            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test10_Cube26p_RotateShuffle(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {

            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 2);
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);

            Matrix3d R = CreateAndPrintMatrix(65, -123, 35);
            PointCloudVertices.Rotate(myPCLSource, R);

            PointCloudVertices.ShuffleRandom(myPCLSource);


            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test10_Cube98p_Rotate(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {
            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 4);
            //myPCLTarget = Vertices.CreateCube_Corners(10);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);


            Matrix3d R = CreateAndPrintMatrix(65, -123, 35);
            PointCloudVertices.Rotate(myPCLSource, R);
            


            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test10_Cube26p_Rotate(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {
            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 2);
            //myPCLTarget = Vertices.CreateCube_Corners(10);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);


            Matrix3d R = CreateAndPrintMatrix(65, -123, 35);
            PointCloudVertices.Rotate(myPCLSource, R);



            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test10_CubeRTranslate(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult, double cubeSize)
        {
            myPCLTarget = PointCloudVertices.CreateCube_RegularGrid_Empty(cubeSize, 5);
            //myPCLTarget = Vertices.CreateCube_Corners(10);

            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);


            PointCloudVertices.Translate(myPCLSource, cubeSize * 1.2f, -cubeSize * 2.5f, cubeSize * 2);



            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }


        public static double Test11_Person(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {


            string path = AppDomain.CurrentDomain.BaseDirectory + "TestData";
            Model model3DTarget = new Model(path + "\\1.obj");
            myPCLTarget = model3DTarget.PointCloudVertices;
            
            myPCLSource = PointCloudVertices.CloneVertices(myPCLTarget);
            PointCloudVertices.RotateDegrees(myPCLSource, 25, 10, 25);

            
            
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }
        public static double Test11_Person_TwoScans(ref PointCloudVertices myPCLTarget, ref PointCloudVertices myPCLSource, ref PointCloudVertices myPCLResult)
        {


            string path = AppDomain.CurrentDomain.BaseDirectory + "TestData";
            Model model3DTarget = new Model(path + "\\1.obj");
            myPCLTarget = model3DTarget.PointCloudVertices;
            
            Model model3DSource = new Model(path + "\\2.obj");
            myPCLSource = model3DSource.PointCloudVertices;
            
            myPCLResult = IterativeClosestPointTransform.Instance.PerformICP(myPCLSource, myPCLTarget);
            return IterativeClosestPointTransform.Instance.MeanDistance;
        }

    
    }
}
