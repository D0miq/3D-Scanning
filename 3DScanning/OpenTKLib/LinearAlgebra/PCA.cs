using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenTKLib
{

    public class PCA
    {
       
        public Matrix3d U;
        

        public Matrix3d VT;
        public Matrix3d V;
        public Vector3d EV;

        public Matrix3d VT_NotNormalized;
        public Matrix3d U_NotNormalized;
        public Vector3d EV_NotNormalized;

     
        List<Vector3d> pointsTransformed;
        public Vector3d Centroid;

       
        public double MeanDistance;
        public Matrix4d Matrix;

        PointCloudVertices pointCloudSourceCentered;
        PointCloudVertices pointCloudTargetCentered;
        PointCloudVertices pointCloudResultCentered;

        KDTreeVertex kdtree;
        PointCloudVertices pointCloudResult = null;
        PointCloudVertices pointCloudResultBest = null;

        PointCloudVertices pointCloudTargetKDTree = null;
        public int MaxmimumIterations = 5;
        double thresholdConvergence = 1e-5f;

        bool axesRotateEffect = true;
       

        public static PointCloudVertices RotateToOriginAxes(PointCloudVertices mypointCloudSource)
        {
           

            PCA pca = new PCA();
            pca.PCA_OfPointCloud(mypointCloudSource);


            Matrix3d R = new Matrix3d();
            PointCloudVertices mypointCloudResult = PointCloudVertices.CopyVertices(mypointCloudSource);
            R = R.Rotation_ToOriginAxes(mypointCloudResult.PCAAxes);
            PointCloudVertices.Rotate(mypointCloudResult, R);
            pca.PCA_OfPointCloud(mypointCloudResult);

            mypointCloudResult.Path = mypointCloudSource.Path;
            mypointCloudResult.FilaName = mypointCloudSource.FilaName;


            return mypointCloudResult;
        }
        /// <summary>
        /// calculates Matrix for alignment of sourceAxes and targetAxes; sets pointCloudResult
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="bestResultMeanDistance"></param>
        /// <param name="meanDistance"></param>
        /// <param name="myMatrixBestResult"></param>
        /// <param name="sourceAxes"></param>
        private void SVD_ForTwoPointCloudAlignment(int i, int j, ref  double bestResultMeanDistance, ref double meanDistance, ref Matrix4d myMatrixBestResult , PointCloudVertices sourceAxes)
        {
            PointCloudVertices targetAxes = InvertAxes(pointCloudTargetCentered, pointCloudTargetCentered.PCAAxes, j);

            Matrix4d myMatrix = SVD.FindTransformationMatrix(PointCloudVertices.ToVectors(sourceAxes), PointCloudVertices.ToVectors(targetAxes), ICP_VersionUsed.Scaling_Umeyama);
            //Matrix4d myMatrix = SVD.FindTransformationMatrix_WithoutCentroids(PointCloudVertices.ToVectors(sourceAxes), PointCloudVertices.ToVectors(targetAxes), ICP_VersionUsed.Scaling_Umeyama);

            //-----------------------
            //for check - should give TargetPCVectors
            List<Vector3d> resultAxes = Matrix4dExtension.TransformPoints(myMatrix, PointCloudVertices.ToVectors(sourceAxes));
            resultAxes = resultAxes.Subtract(PointCloudVertices.ToVectors(targetAxes));



            List<Vector3d> myPointsResult = myMatrix.TransformPoints(PointCloudVertices.ToVectors(pointCloudSourceCentered));
            PointCloudVertices myPointsResultTemp = PointCloudVertices.FromVectors(myPointsResult);

           PointCloudVertices myPointCloudTargetTemp = kdtree.FindNearest_Rednaxela_Parallel(ref myPointsResultTemp, pointCloudTargetCentered, -1);
            //PointCloudVertices myPointCloudTargetTemp = kdtree.FindNearest_Rednaxela(ref myPointsResultTemp, pointCloudTargetCentered, -1);

            double trace = myMatrix.Trace;
            meanDistance = kdtree.MeanDistance;

            //double trace = kdtree.MeanDistance;
            //double meanDistance = myMatrix.Trace;
            //Check:

            System.Diagnostics.Debug.WriteLine("   in iteration: MeanDistance between orientations: " + i.ToString() + " : " + j.ToString() + " : " + meanDistance.ToString("G") + " : Trace: " + trace.ToString("G"));

            if (meanDistance < bestResultMeanDistance)
            {
                myMatrixBestResult = myMatrix;
                bestResultMeanDistance = meanDistance;
                pointCloudResultBest = PointCloudVertices.FromVectors(myPointsResult);
            }



            pointCloudResult = PointCloudVertices.FromVectors(myPointsResult);
            pointCloudTargetKDTree = myPointCloudTargetTemp;

        }
        /// <summary>
        /// sets the result point cloud - resp. pointCloudResultBest 
        /// </summary>
        /// <param name="mypointCloudSource"></param>
        /// <returns></returns>
        private double SVD_Iteration(PointCloudVertices mypointCloudSource)
        {
            double bestResultMeanDistance = double.MaxValue;
            double meanDistance = double.MaxValue; 
            

            pointCloudSourceCentered = CalculatePCA_Internal(mypointCloudSource);

            Matrix4d myMatrixBestResult = Matrix4d.Identity;
            // int i = -1;
            //SVD_ForTwoPointCloudAlignment(-1, -1, ref  bestResultMeanDistance, ref meanDistance, ref myMatrixBestResult, pointCloudSourceCentered.PCAAxes);
            SVD_ForTwoPointCloudAlignment(-1, -1, ref  bestResultMeanDistance, ref meanDistance, ref myMatrixBestResult, pointCloudSourceCentered.PCAAxes);

            //leads to a lot of iterations
            if (axesRotateEffect)
            {
                //additionally try other solutions: Invert all axes 
                if (meanDistance > thresholdConvergence)
                {

                    for (int i = -1; i < 3; i++)
                    {

                        PointCloudVertices sourceAxes = InvertAxes(pointCloudSourceCentered, pointCloudSourceCentered.PCAAxes, i);

                        for (int j = -1; j < i; j++)
                        {
                            SVD_ForTwoPointCloudAlignment(i, j, ref  bestResultMeanDistance, ref meanDistance, ref myMatrixBestResult, sourceAxes);
                            if (meanDistance < thresholdConvergence)
                                break;
                        }

                        if (meanDistance < thresholdConvergence)
                            break;

                    }
                }
                
            }
            Matrix4d.Mult(ref myMatrixBestResult, ref this.Matrix, out this.Matrix);

            //pointCloudResultBest

            return bestResultMeanDistance;


        }
   
        public PointCloudVertices AlignPointClouds_SVD(PointCloudVertices pointCloudSource, PointCloudVertices pointCloudTarget)
        {
            
            try
            {
                if (pointCloudSource == null || pointCloudTarget == null || pointCloudSource.Count == 0 || pointCloudTarget.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("PCA - please check point clouds ");
                    return null;

                }
                this.Matrix = Matrix4d.Identity;
               // pointCloudSourceCentered = ShiftByCenterOfMass(pointCloudSource);
                pointCloudSourceCentered = CalculatePCA_Internal(pointCloudSource);
                PrepareTargetTree(pointCloudTarget);

                PointCloudVertices myPointCloudIteration = PointCloudVertices.CloneVertices(pointCloudSource);

                for (int i = 0; i < MaxmimumIterations; i++)
                {
                    double meanDistance = SVD_Iteration(myPointCloudIteration);
                    System.Diagnostics.Debug.WriteLine("-->>  Iteration " + i.ToString() + " : Mean Distance : " + meanDistance.ToString("G") + ": duration: " + GlobalVariables.TimeSpanString());

                    if (meanDistance < thresholdConvergence)
                        break;
                    myPointCloudIteration = pointCloudResultBest;
                }

                //final check:

                pointCloudResultCentered = CalculatePCA_Internal(pointCloudResult);


                //"Shuffle" effect - the target points are in other order after kdtree search:
                //The mean distance calculated again, as check (was calculated before in the kdTree routine)

                MeanDistance = PointCloudVertices.MeanDistance(pointCloudResult, pointCloudTargetKDTree);
                System.Diagnostics.Debug.WriteLine("-->>  TO CHECK: PCA (SVD) - Final Mean Distance : " + MeanDistance.ToString("G"));

                //MeanDistance = PointCloudVertices.MeanDistance(pointCloudResult, pointCloudTarget);
                //System.Diagnostics.Debug.WriteLine("-->>  PCA (SVD) - Final Mean Distance : " + MeanDistance.ToString("G"));

                this.Matrix = AdjustSourceTargetByTranslation(Matrix, pointCloudSource, pointCloudTarget);
                pointCloudResult = Matrix.TransformPoints(pointCloudSource);
                pointCloudResultCentered = CalculatePCA_Internal(pointCloudResult);

              
                
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error aligning point cloud");
            }
            return pointCloudResult;
        }
        public PointCloudVertices AlignPointClouds_SVD_WithShuflleEffect(bool axesRotateEffect, PointCloudVertices pointCloudSource, PointCloudVertices pointCloudTarget)
        {
            try
            {
                if (pointCloudSource == null || pointCloudTarget == null || pointCloudSource.Count == 0 || pointCloudTarget.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("PCA - please check point clouds ");
                    return null;

                }
                this.Matrix = Matrix4d.Identity;
                pointCloudSourceCentered = CalculatePCA_Internal(pointCloudSource);
                PrepareTargetTree(pointCloudTarget);


                PointCloudVertices myPointCloudIteration = PointCloudVertices.CloneVertices(pointCloudSource);

                for (int i = 0; i < MaxmimumIterations; i++)
                {
                    double meanDistance = SVD_Iteration(myPointCloudIteration);
                    System.Diagnostics.Debug.WriteLine("-->>  Iteration " + i.ToString() + " : Mean Distance : " + meanDistance.ToString("G") + ": duration: " + GlobalVariables.TimeSpanString());

                    if (meanDistance < thresholdConvergence)
                        break;
                    myPointCloudIteration = pointCloudResultBest;
                }

                //final check:

                pointCloudResultCentered = CalculatePCA_Internal(pointCloudResult);


                //"Shuffle" effect - the target points are in other order after kdtree search:
                //The mean distance calculated again, as check (was calculated before in the kdTree routine)

                MeanDistance = PointCloudVertices.MeanDistance(pointCloudResult, pointCloudTargetKDTree);
                System.Diagnostics.Debug.WriteLine("-->>  TO CHECK: PCA (SVD) - Final Mean Distance : " + MeanDistance.ToString("G"));

                //MeanDistance = PointCloudVertices.MeanDistance(pointCloudResult, pointCloudTarget);
                //System.Diagnostics.Debug.WriteLine("-->>  PCA (SVD) - Final Mean Distance : " + MeanDistance.ToString("G"));

                this.Matrix = AdjustSourceTargetByTranslation(Matrix, pointCloudSource, pointCloudTarget);
                pointCloudResult = Matrix.TransformPoints(pointCloudSource);
                pointCloudResultCentered = CalculatePCA_Internal(pointCloudResult);

                //MeanDistance = PointCloudVertices.MeanDistance(pointCloudResult, pointCloudTarget);
                //System.Diagnostics.Debug.WriteLine("-->>  PCA (SVD) - Final Mean Distance : " + MeanDistance.ToString("G"));

                //for display later:




            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error aligning point cloud");
            }
            return pointCloudResult;
        }
        /// <summary>
        /// projected result is in PointResult0, etc.
        /// </summary>
        /// <param name="pointCloud"></param>
        private PointCloudVertices CalculatePCA_Internal(PointCloudVertices pointCloud)
        {
            PointCloudVertices mypointCloudCentered = new PointCloudVertices();
            mypointCloudCentered = ShiftByCenterOfMass(pointCloud);
            SVD_OfPointCloud(mypointCloudCentered, false);

            AssignPCAxes(pointCloud, mypointCloudCentered);
           
            return mypointCloudCentered;

        }
       
        /// <summary>
        /// projected result is in PointResult0, etc.
        /// </summary>
        /// <param name="pointsSource"></param>
        public void PCA_OfPointCloud(PointCloudVertices pointsSource)
        {
            pointCloudSourceCentered = CalculatePCA_Internal(pointsSource);

        }

        private void PrepareTargetTree(PointCloudVertices pointCloudTarget)
        {

            //second object:
            //-----------
            pointCloudTargetCentered = CalculatePCA_Internal(pointCloudTarget);
            //pointCloudTargetCentered = ShiftByCenterOfMass(pointCloudTarget);

            kdtree = new KDTreeVertex();
           kdtree.BuildKDTree_Rednaxela(pointCloudTargetCentered);

            pointCloudResult = null;
            pointCloudTargetKDTree = null;


        }
        
      
        public PointCloudVertices AlignPointClouds_OneVector(PointCloudVertices pointCloudSource, PointCloudVertices pointCloudTarget, int vectorNumberSource, int vectorNumberTarget)
        {

           
            //-------------------
            pointCloudSourceCentered = CalculatePCA_Internal(pointCloudSource);
            
            
           

            //second object:
            //-----------
            pointCloudTargetCentered = CalculatePCA_Internal(pointCloudTarget);
          
         
            //Vector3d v = TargetPCVectors[vectorNumberTarget];
            //v.X = -v.X;
            //v.Y = -v.Y;
            //v.Z = -v.Z;
            //TargetPCVectors[vectorNumberTarget] = v;


            Matrix3d R = new Matrix3d();
            //R = R.RotationOneVectorToAnother(TargetPCVectors[vectorNumber], SourcePCVectors[vectorNumber]);
            R = R.RotationOneVertexToAnother(pointCloudSource.PCAAxes[vectorNumberSource], pointCloudTarget.PCAAxes[vectorNumberTarget]);

           
            //R.CheckRotationMatrix();

            //

            //test:
            //Vector3d testV = R.MultiplyVector(sourceV);


            PointCloudVertices pointCloudResult = PointCloudVertices.CopyVertices(pointCloudSource);
            PointCloudVertices.SubtractVectorRef(pointCloudResult, pointCloudSource.CentroidVector);
            PointCloudVertices.Rotate(pointCloudResult, R);
            PointCloudVertices.AddVector(pointCloudResult, pointCloudTarget.CentroidVector);

            pointCloudResultCentered = CalculatePCA_Internal(pointCloudResult);
            
         

           

            MeanDistance = PointCloudVertices.MeanDistance(pointCloudResult, pointCloudTarget);
            System.Diagnostics.Debug.WriteLine("-->>  PCA (V) - Mean Distance : " + MeanDistance.ToString("0.000000"));

        

            return pointCloudResult;


        }

        private List<Vector3d> ShiftByCenterOfMassVectorList(PointCloudVertices pointsSource)
        {
            Centroid = pointsSource.CentroidVectorGet;
            List<Vector3d> listSource = PointCloudVertices.ToVectors(pointsSource);
            listSource.SubtractVector(this.Centroid);
            return listSource;
        }
        private PointCloudVertices ShiftByCenterOfMass(PointCloudVertices pointsSource)
        {
            Centroid = pointsSource.CentroidVectorGet;
            PointCloudVertices listSource = PointCloudVertices.SubtractVector(pointsSource, this.Centroid);
            return listSource;
        }
       
     
        /// <summary>
        /// assume - vectors are mass - centered!
        /// </summary>
        /// <param name="pointCloud"></param>
        /// <param name="axesVectors"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private PointCloudVertices InvertAxes(PointCloudVertices pointCloud, PointCloudVertices axesVectors, int i)
        {

            PointCloudVertices resultList = PointCloudVertices.CopyVertices(axesVectors);
            
            if (i == -1)
                return resultList;

            Vector3d v = resultList[i].Vector.Negate();
            resultList[i] = new Vertex(resultList[i].IndexInModel, v);
            return resultList;

        }
        public PointCloudVertices AlignToCenter(PointCloudVertices pointCloudSource)
        {

            pointCloudSourceCentered = CalculatePCA_Internal(pointCloudSource);
           
            
            Matrix3d R = new Matrix3d();
            R = R.RotationChangeBasis(PointCloudVertices.ToVectors(pointCloudSource.PCAAxes));


            
            PointCloudVertices pointCloudResult = PointCloudVertices.CopyVertices(pointCloudSource);
            PointCloudVertices.SubtractVectorRef(pointCloudResult, pointCloudSource.CentroidVector);
            PointCloudVertices.Rotate(pointCloudResult, R);

            pointCloudResultCentered = CalculatePCA_Internal(pointCloudResult);
            
           
            
           
            return pointCloudResult;


        }

        private void AssignPCVectorsOld(PointCloudVertices pointsSource, PointCloudVertices mypointCloudSourceCentered)
        {
            //pointsSource.PCAAxesNew = new PointCloud();
            pointsSource.CentroidVector = Centroid;
            pointsSource.PCAAxes = new PointCloudVertices();
            List<Vector3d> vectorList = PointCloudVertices.ToVectors(mypointCloudSourceCentered);
            for (int i = 0; i < 3; i++)
            {
                List<Vector3d> vList = GetResultOfEigenvector(i, vectorList);
                vList.SubtractVector(Centroid);

                Vector3d v = vList.GetMax();
                if (EV[i] == 0)
                    v = new Vector3d();
                pointsSource.PCAAxes.Add(new Vertex(v));
                

            }
            
            for (int i = 0; i < 3; i++)
            {

                pointsSource.PCAAxes[i].Vector += Centroid;
                              

            }
            mypointCloudSourceCentered.PCAAxes = pointsSource.PCAAxes;



        }
        /// <summary>
        /// assigns the PC Axes to both pointsSource and mypointCloudSourceCentered
        /// </summary>
        /// <param name="pointsSource"></param>
        /// <param name="mypointCloudSourceCentered"></param>
        private void AssignPCAxes(PointCloudVertices pointsSource, PointCloudVertices mypointCloudSourceCentered)
        {
            
            pointsSource.CentroidVector = Centroid;
            pointsSource.PCAAxes = new PointCloudVertices();
            List<Vector3d> vectorList = PointCloudVertices.ToVectors(mypointCloudSourceCentered);
            for (int i = 0; i < 3; i++)
            {
                Vector3d v = VT.ExtractColumn(i);
                v = v * Convert.ToSingle(Math.Sqrt(EV[i]));
                Vertex ve = new Vertex(i, v);
                pointsSource.PCAAxes.Add(ve);

            }

            mypointCloudSourceCentered.PCAAxes = pointsSource.PCAAxes;


        }
        ////}
        ///// <summary>
        ///// PCA are center of mass - centered
        ///// </summary>
        ///// <param name="pointCloud"></param>
        ///// <param name="myCentroid"></param>
        //private void AssignPCVectors(PointCloud pointCloud, PointCloud mypointCloudSourceCentered)
        //{
        //    pointCloud.CentroidPCA = Centroid;
        //    pointCloud.PCAAxes = new PointCloud();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        Vector3d v = VT.ExtractColumn(i);
        //        //v = v * Math.Sqrt(EV[i]);
        //        v = v * EV[i];
        //        double d = v.Length;
        //        Vertex ve = new Vertex(i, v);
        //        pointCloud.PCAAxes.Add(ve);
        //    }

        //    mypointCloudSourceCentered.PCAAxes = pointCloud.PCAAxes;


        ////}
        ////}
        ////}
        ///// <summary>
        ///// PCA are center of mass - centered
        ///// </summary>
        ///// <param name="pointCloud"></param>
        ///// <param name="myCentroid"></param>
        //private void AssignPCVectors(PointCloud pointCloud, PointCloud mypointCloudSourceCentered)
        //{
        //    pointCloud.CentroidPCA = Centroid;
        //    pointCloud.PCAAxes = new PointCloud();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        Vector3d v = VT_NotNormalized.ExtractColumn(i);
        //        //v = v * Math.Sqrt(EV[i]);
        //        v = v * EV_NotNormalized[i];
        //        double d = v.Length;
        //        Vertex ve = new Vertex(i, v);
        //        pointCloud.PCAAxes.Add(ve);
        //    }

        //    mypointCloudSourceCentered.PCAAxes = pointCloud.PCAAxes;


        //}
       

   
         
   
        private static PointCloudVertices CalculateResults(Matrix3d Ub, Matrix3d Ua, PointCloudVertices pointCloudSource, Vector3d centroidB, Vector3d centroidA)
        {
            Matrix3d R;
            Matrix3d.Mult(ref Ub, ref Ua, out R);
            

            PointCloudVertices pointCloudResult = PointCloudVertices.CopyVertices(pointCloudSource);
            PointCloudVertices.Rotate(pointCloudResult, R);

            Vector3d t = centroidB - R.MultiplyVector(centroidA);
            //Vertices.AddVector(pointCloudResult, t);
            return pointCloudResult;

        }

        
       
   

        /// <summary>
        /// compute normals for a all vectors of pointSource
        /// </summary>
        /// <param name="pointsSource"></param>
        /// <returns></returns>
        public List<Vector3> Normals(PointCloudVertices pointsSource, bool centerOfMassMethod, bool flipNormalWithOriginVector)
        {
            Vector3 normalPrevious = new Vector3();
            List<Vector3> normals = new List<Vector3>();
            for(int i = 0; i < pointsSource.Count; i++)
            {
                Vertex v = pointsSource[i];
                List<Vector3d> sublist = new List<Vector3d>();
                for (int j = 0; j < v.KDTreeSearch.Count; j++)
                {
                    Vertex vNearest = pointsSource[v.KDTreeSearch[j].Key];
                    sublist.Add(pointsSource[v.KDTreeSearch[j].Key].Vector);
                  
                }
                if (centerOfMassMethod)
                {
                    Vector3d centroid = sublist.CalculateCentroid();
                    sublist.SubtractVector(centroid);
                }
                else
                {
                    sublist.SubtractVector(v.Vector);
                }
                SVD_ForListVectorsMassCentered(sublist, true);
          
                Vector3 normal = V.ExtractRow(2).ToVector();
                if (flipNormalWithOriginVector)
                    AdjustOrientationWithVector(ref normal, v.Vector.ToVector());

                //if (i > 0)
                //    AdjustOrientation(ref normal, normalPrevious);
                normalPrevious = normal;


                normal.Normalize();
                normals.Add(normal);
                v.IndexNormals.Add(normals.Count - 1);

               // to show ALL vectors (including the 2 vectors on the plane:
               //AddAllPlaneVectors(v, normals);

            }

            return normals;

        }
   
      
        private void AddAllPlaneVectors(Vertex v, List<Vector3d> normals)
        {
            //to show ALL vectors (including the 2 vectors on the plane:
            for (int j = 0; j < 3; j++)
            {
                Vector3d normal = V.ExtractRow(j);

                normal.Normalize();
                normals.Add(normal);
                v.IndexNormals.Add(normals.Count - 1);

            }

        }
        private void AdjustOrientation(ref Vector3d normal, Vector3d previousNormal)
        {
            //return;

            double d1 = Vector3d.Dot(previousNormal, normal);
            
         
            if(d1 < 0 && d1 < -1e-3f)
            {
                //normal = normalFlipped;
                normal = Vector3d.Multiply(normal, -1);
            }
            //else
            //{
            //    if (d2 < d1)
            //        normal = normalFlipped;
            //}

        }
        private void AdjustOrientationWithVector(ref Vector3 normal, Vector3 v)
        {
           
            float d1 = Vector3.Dot(v, normal);

            if (d1 < 0 && d1 < -1e-3f)
            {
               
                normal = Vector3.Multiply(normal, -1);
            }
          

        }
      
        private Matrix3d ExtractMatrixColumnN(int N, Matrix3d W)
        {
            Matrix3d Vnew = new Matrix3d();
            Vnew = W.Copy();


            for (int i = 0; i < 3; i++)
            {
               
                if(i != N)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vnew[i, j] = 0;
                    }
                }
            }
            return Vnew;


        }
        private Matrix3d ExtractMatrixRowN(int N, Matrix3d W)
        {
            Matrix3d Vnew = new Matrix3d();
            Vnew = W.Copy();


            for (int i = 0; i < 3; i++)
            {

                if (i != N)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vnew[j, i] = 0;
                    }
                }
            }
            return Vnew;


        }
       
        private List<Vector3d> TransformPoints(List<Vector3d> pointsToTransform, Matrix3d mat)
        {
            
            pointsTransformed = new List<Vector3d>();
            for (int i = 0; i < pointsToTransform.Count; i++)
            {
                Vector3d v = pointsToTransform[i];
                Vector3d result = mat.MultiplyVector(v);
                pointsTransformed.Add(result);
            }
            return pointsTransformed;

        }
        private void SVD_ForListVectorsMassCentered(List<Vector3d> pointsSource, bool normalsCovariance)
        {
           
            //calculate correlation matrix
            Matrix3d C = PointCloudVertices.CovarianceMatrix(pointsSource, normalsCovariance);
          
            SVD.Eigenvalues_Helper(C);
            EV = SVD.EV;
            VT = SVD.VT;
            U = SVD.U;

            V = Matrix3d.Transpose(VT);

            
        }
        private void SVD_OfPointCloud(PointCloudVertices pointsSource, bool normalsCovariance)
        {
            //calculate correlation matrix
            
            Matrix3d C = PointCloudVertices.CovarianceMatrix(pointsSource, normalsCovariance);
                      
            SVD.Eigenvalues_Helper(C);

            EV = SVD.EV;
            VT = SVD.VT;
            U = SVD.U;

        }

        private void CheckSVD(Matrix3d C)
        {
            //check transformation
            Matrix3d R = Matrix3d.Mult(U, VT);
            Matrix3d testRight = VT.MultiplyDiagonalElements(EV);
            //R should be now C
            Matrix3d testleft = Matrix3d.Mult(U, C);
            testleft = Matrix3d.Mult(testleft, VT);
            testRight.CompareMatrices(testleft, 1e-2f);
            //PointCloud.Rotate(pointsSource, VT);

        }
        private void CheckSVD_NotNormalized(Matrix3d C)
        {
            //check transformation
            Matrix3d R = Matrix3d.Mult(U_NotNormalized, VT_NotNormalized);
            R = R.MultiplyDiagonalElements(EV_NotNormalized);
            //R should be now C
            R.CompareMatrices(C, 1e-2f);
            //PointCloud.Rotate(pointsSource, VT);

        }
        private List<Vector3d> GetResultOfEigenvector(int eigenvectorUsed, List<Vector3d> listTranslated)
        {
            Matrix3d VTNew = ExtractMatrixColumnN(eigenvectorUsed, VT);
            Matrix3d UNew = ExtractMatrixRowN(eigenvectorUsed, U);
            
            Matrix3d R = Matrix3d.Mult(UNew, VTNew);
            List<Vector3d> resultList = TransformPoints(listTranslated, R);


            resultList.AddVector(Centroid);

            return resultList;

        }
       
  
        public List<Vector3d> ProjectPointsOnPCAAxes()
        {
            List<Vector3d> listProjected = new List<Vector3d>();
            listProjected.AddRange(PointsResult0);
            listProjected.AddRange(PointsResult1);
            listProjected.AddRange(PointsResult2);

            
            return listProjected;


        }
        private List<Vector3d> TransformPointsAfterPCA(List<Vector3d> listVector3d)
        {
            
            List<Vector3d> listResult = PointCloudVertices.CopyVectors(listVector3d);
           
          
            Matrix3d R = Matrix3d.Mult(U, VT);
            listResult = TransformPoints(listResult, R);

            return listResult;

        }
      
        public List<Vector3d> CalculatePCA(List<Vector3d> pointsSource, int eigenvectorUsed)
        {

            Centroid = pointsSource.CalculateCentroid();
            
            List<Vector3d> listTranslated = pointsSource.Clone();

            pointsSource.SubtractVector(Centroid);
            SVD_ForListVectorsMassCentered(listTranslated, false);
            return GetResultOfEigenvector(eigenvectorUsed, listTranslated);

            
        }
      
        
       
        public PointCloudVertices CalculatePCA(PointCloudVertices pointsSource, int eigenvectorUsed)
        {
            List<Vector3d> vector3dSource = PointCloudVertices.ToVectors(pointsSource);
            vector3dSource = CalculatePCA(vector3dSource, eigenvectorUsed);

            return PointCloudVertices.FromVectors(vector3dSource);


        }
   
        private List<Vector3d> CheckTransformation(List<Vector3d> pointsSource)
        {
            List<Vector3d> listResult = TransformPointsAfterPCA(pointsSource);
            listResult.AddVector(this.Centroid);

            return listResult; // should be pointsSource
        }
      

        public List<Vector3d> PointsResult0
        {
            get
            {
                List<Vector3d> list = GetResultOfEigenvector(0, PointCloudVertices.ToVectors(pointCloudSourceCentered));
                return list;

            }
        }
        public List<Vector3d> PointsResult1
        {
            get
            {
                List<Vector3d> list = GetResultOfEigenvector(1, PointCloudVertices.ToVectors(pointCloudSourceCentered));
                return list;

            }
        }
        public List<Vector3d> PointsResult2
        {
            get
            {
                List<Vector3d> list = GetResultOfEigenvector(2, PointCloudVertices.ToVectors(pointCloudSourceCentered));
                return list;

            }
        }
        private Matrix4d AdjustSourceTargetByTranslation(Matrix4d myMatrixFound, PointCloudVertices pointCloudSource, PointCloudVertices pointCloudTarget)
        {
            Matrix3d R = myMatrixFound.ExtractMatrix3d();
            Vector3d T = SVD.CalculateTranslation(pointCloudSource.CentroidVector, pointCloudTarget.CentroidVector, R);
            myMatrixFound = myMatrixFound.AddTranslation(T);
            return myMatrixFound;

        }

        
    }
}
