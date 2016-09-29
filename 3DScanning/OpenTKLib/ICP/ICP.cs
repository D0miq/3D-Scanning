
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Windows.Media;
using System.Diagnostics;
using OpenTK;
using OpenTKLib;

namespace ICPLib
{


    public partial class IterativeClosestPointTransform
    {
        #region public members

        public SettingsICP ICPSettings = new SettingsICP();

        public int NumberOfIterations;
        public double MeanDistance;

        public PointCloudVertices PSource;
        public PointCloudVertices PTarget;
        public Matrix4d Matrix;
        
        public PointCloudVertices PMerged;


#endregion

        #region private members
        double startAngleForNormalsCheck = 45;

        private List<Vector3> normalsSource;

        
        private List<Vector3> normalsTarget;         
        
        private PointCloudVertices pointsTransformed;
        private List<ICPSolution> solutionList;
        private LandmarkTransform LandmarkTransform;
        private static IterativeClosestPointTransform instance;

        #endregion

        public void Settings_Reset_RealData()
        {

            ICPSettings.MaximumNumberOfIterations = 10;
            ICPSettings.FixedTestPoints = false;
            ICPSettings.Normal_RemovePoints = false;
            ICPSettings.Normal_SortPoints = false;

            ICPSettings.ResetVertexToOrigin = true;
            ICPSettings.KDTreeMode = KDTreeMode.Rednaxela;
            ICPSettings.ICPVersion = ICP_VersionUsed.Scaling_Umeyama;

        }
      
        public void Settings_Reset_GeometricObject()
        {
            ICPSettings.MaximumNumberOfIterations = 5;
            ICPSettings.FixedTestPoints = true;
            ICPSettings.ResetVertexToOrigin = false;
            ICPSettings.KDTreeMode = KDTreeMode.Rednaxela;
            ICPSettings.Normal_RemovePoints = false;

        }
        public IterativeClosestPointTransform()//:base(PointerUtils.GetIntPtr(new double[3]), true, true)
        {
            Reset();

            this.PSource = null;
            this.PTarget = null;

            this.LandmarkTransform = new LandmarkTransform();

            this.NumberOfIterations = 0;
            this.MeanDistance = 0.0f;

        }

        public void Reset()
        {
            instance = this;
            this.ICPSettings.ICPVersion = ICP_VersionUsed.Scaling_Umeyama;
            this.ICPSettings.SimulatedAnnealing = false;
            this.ICPSettings.Normal_RemovePoints = false;
            this.ICPSettings.Normal_SortPoints = false;

            this.ICPSettings.FixedTestPoints = false;
            this.ICPSettings.MaximumNumberOfIterations = 100;
            this.ICPSettings.ResetVertexToOrigin = true;
            this.ICPSettings.DistanceOptimization = false;
            this.ICPSettings.MaximumMeanDistance = 1.0e-3f;

        }
        public PointCloudVertices PerformICP(PointCloudVertices mypointsSource, PointCloudVertices myPointsTarget)
        {
            this.PTarget = myPointsTarget;
            this.PSource = mypointsSource;


            if (ICPSettings.ICPVersion == ICP_VersionUsed.UsingStitchData)
                return PerformICP_Stitching();
            else if (ICPSettings.ICPVersion == ICP_VersionUsed.RandomPoints)
                return PerformICP();
            else
                return PerformICP();
        }
        public static IterativeClosestPointTransform Instance
        {
            get
            {
                if (instance == null)
                    instance = new IterativeClosestPointTransform();
                return instance;

            }
        }
        public PointCloudVertices PTransformed
        {
            get
            {
                return pointsTransformed;
            }
            set
            {
                pointsTransformed = value;
            }
        }



        public void Inverse()
        {
            PointCloudVertices tmp1 = this.PSource;
            this.PSource = this.PTarget;
            this.PTarget = tmp1;
            //this.Modified();
        }

        private KDTreeVertex Helper_CreateTree(PointCloudVertices pointsTarget)
        {
            KDTreeVertex kdTree = new KDTreeVertex();
            kdTree.KDTreeMode = ICPSettings.KDTreeMode;

            if (!ICPSettings.FixedTestPoints)
            {
                if (kdTree.KDTreeMode == KDTreeMode.Stark)
                    kdTree.BuildKDTree_Stark(pointsTarget);
                else if (kdTree.KDTreeMode == KDTreeMode.Rednaxela || kdTree.KDTreeMode == KDTreeMode.Rednaxela_ExcludePoints)
                    kdTree.BuildKDTree_Rednaxela(pointsTarget);
                //else if(KDTreeMode == KDTreeMode.BruteForce)

            }
            return kdTree;
        }

        private bool Helper_FindNeighbours(ref PointCloudVertices pointsSource, ref PointCloudVertices pointsTarget, KDTreeVertex kdTree, float angleThreshold)
        {
            
            if (!ICPSettings.FixedTestPoints)
            {
                if (ICPSettings.KDTreeMode == KDTreeMode.Stark)
                    pointsTarget = kdTree.FindNearest_Stark(pointsSource, pointsTarget);
                else if (ICPSettings.KDTreeMode == KDTreeMode.Rednaxela)
                {

                    pointsTarget = kdTree.FindNearest_Rednaxela_Parallel(ref pointsSource, pointsTarget, angleThreshold);
                    //pointsTarget = kdTree.FindNearest_Rednaxela(ref pointsSource, pointsTarget, angleThreshold);
                }
                else if (ICPSettings.KDTreeMode == KDTreeMode.Rednaxela_ExcludePoints)
                    pointsTarget = kdTree.FindNearest_Rednaxela_ExcludePoints(pointsSource, pointsTarget, false);
                else if (ICPSettings.KDTreeMode == KDTreeMode.BruteForce)
                    pointsTarget = kdTree.FindNearest_BruteForce(pointsSource, pointsTarget);


               
            }
            else
            {
                //adjust number of points - for the case if there are outliers
                int min = pointsSource.Count;
                if (pointsTarget.Count < min)
                {
                    min = pointsTarget.Count;
                    pointsSource.RemoveRange(pointsTarget.Count, pointsSource.Count - min);

                }
                else
                {
                    pointsTarget.RemoveRange(pointsSource.Count, pointsTarget.Count - min);
                }

            }
            return true;

        }
        private static double TransformPoints(ref PointCloudVertices myPointsTransformed, PointCloudVertices pointsTarget, PointCloudVertices pointsSource, Matrix4d myMatrix)
        {
            myPointsTransformed = MathUtilsVTK.TransformPoints(pointsSource, myMatrix);
            double meanDistance = PointCloudVertices.MeanDistance(pointsTarget, myPointsTransformed);
            //double meanDistance = totaldist / Convert.ToSingle(pointsTarget.Count);
            return meanDistance;

        }
        private Matrix4d Helper_FindTransformationMatrix(PointCloudVertices pointsSource, PointCloudVertices pointsTarget)
        {
            Matrix4d myMatrix;

            if (ICPSettings.ICPVersion == ICP_VersionUsed.Horn)
            {
                MathUtilsVTK.FindTransformationMatrix(PointCloudVertices.ToVectors(pointsSource), PointCloudVertices.ToVectors(pointsTarget), this.LandmarkTransform);
                myMatrix = LandmarkTransform.Matrix;
             
            }
            else
            {

                myMatrix = SVD.FindTransformationMatrix(PointCloudVertices.ToVectors(pointsSource), PointCloudVertices.ToVectors(pointsTarget), ICPSettings.ICPVersion);
                
            }
            return myMatrix;

           

        }
        private PointCloudVertices Helper_SetNewInterationSets(ref PointCloudVertices pointsSource, ref PointCloudVertices pointsTarget, PointCloudVertices PS, PointCloudVertices PT )
        {
            PointCloudVertices myPointsTransformed = MathUtilsVTK.TransformPoints(PS, Matrix);
            this.Matrix.TransformVectorList(normalsSource);

            pointsSource = myPointsTransformed;
            pointsTarget = PointCloudVertices.CopyVertices(PT);
            
            return myPointsTransformed;
        }
     
        /// <summary>
        /// A single ICP Iteration
        /// </summary>
        /// <param name="pointsTarget"></param>
        /// <param name="pointsSource"></param>
        /// <param name="PT"></param>
        /// <param name="PS"></param>
        /// <param name="kdTree"></param>
        /// <returns></returns>
        private PointCloudVertices Helper_ICP_Iteration(ref PointCloudVertices pointsSource, ref PointCloudVertices pointsTarget, PointCloudVertices PT, PointCloudVertices PS, KDTreeVertex kdTree, float angleThreshold)
        {
            //Take care - might return less points than originally, since NormalsCheck or TreeStark remove points
            if (!Helper_FindNeighbours(ref pointsSource, ref pointsTarget, kdTree, angleThreshold))
                return null;


            Matrix4d myMatrix = Helper_FindTransformationMatrix(pointsSource, pointsTarget);
            if (myMatrix.CheckNAN())
                return null;

            PointCloudVertices myPointsTransformed = MathUtilsVTK.TransformPoints(pointsSource, myMatrix);
            
            //DebugWriteUtils.WriteTestOutputVertex("Iteration Result", myMatrix, pointsSource, myPointsTransformed, pointsTarget);

            if (ICPSettings.SimulatedAnnealing)
            {
                this.Matrix = myMatrix;

                this.MeanDistance = PointCloudVertices.MeanDistance(pointsTarget, myPointsTransformed);
               

                //new set:
                pointsSource = myPointsTransformed;
                pointsTarget = PointCloudVertices.CopyVertices(PT);

               
                
            }
            else
            {
                Matrix4d.Mult(ref myMatrix, ref this.Matrix, out this.Matrix);
                //DebugWriteUtils.WriteMatrix("Cumulated Matrix", Matrix);
                
                //for the "shuffle" effect (point order of source and target is different)
                if (! this.ICPSettings.ShuffleEffect)
                {
                    myPointsTransformed = Helper_SetNewInterationSets(ref pointsSource, ref pointsTarget, PS, PT);
                }
                else
                {
                    CheckDuplicates(ref myPointsTransformed, pointsSource, pointsTarget, PS, PT);

                }
                this.MeanDistance = PointCloudVertices.MeanDistance(pointsTarget, myPointsTransformed);
                
              
                //Debug.WriteLine("--------------Iteration: " + iter.ToString() + " : Mean Distance: " + MeanDistance.ToString("0.00000000000"));

                if (MeanDistance < ICPSettings.MaximumMeanDistance) //< Math.Abs(MeanDistance - oldMeanDistance) < this.MaximumMeanDistance)
                    return myPointsTransformed;

                //for the "shuffle" effect (point order of source and target is different)
                if (this.ICPSettings.ShuffleEffect)
                {
                    myPointsTransformed = Helper_SetNewInterationSets(ref pointsSource, ref pointsTarget, PS, PT);
                }
                this.pointsTransformed = myPointsTransformed;
                

                
            }
            return null;

        }
        private void CheckDuplicates(ref PointCloudVertices myPointsTransformed, PointCloudVertices pointsSource, PointCloudVertices pointsTarget, PointCloudVertices PS, PointCloudVertices PT)
        {
            
            //in some cases, the pointsTarget contain point duplicates
            //check if at least 3 points are different
            List<int> indexToCheck = new List<int>() { 0, pointsTarget.Count - 1, (pointsTarget.Count - 1) / 2 };
            bool duplicates = false;
            for (int i = 0; i < indexToCheck.Count - 1; i++)
            {
                if (pointsTarget[indexToCheck[i]] == pointsTarget[indexToCheck[i + 1]])
                {
                    duplicates = true;
                    break;
                }

            }
            if (duplicates)
                myPointsTransformed = Helper_SetNewInterationSets(ref pointsSource, ref pointsTarget, PS, PT);
           
            

        }
        private bool Helper_ICP_Iteration_SA(PointCloudVertices PS, PointCloudVertices PT, KDTreeVertex kdTree, float angleThreshold)
        {
            try
            {

                //first iteration
                if (solutionList == null)
                {
                    solutionList = new List<ICPSolution>();


                    if (ICPSettings.NumberOfStartTrialPoints > PS.Count)
                        ICPSettings.NumberOfStartTrialPoints = PS.Count;
                    if (ICPSettings.NumberOfStartTrialPoints == PS.Count)
                        ICPSettings.NumberOfStartTrialPoints = PS.Count * 80 / 100;
                    if (ICPSettings.NumberOfStartTrialPoints < 3)
                        ICPSettings.NumberOfStartTrialPoints = 3;



                    for (int i = 0; i < ICPSettings.MaxNumberSolutions; i++)
                    {
                        ICPSolution myTrial = ICPSolution.SetRandomIndices(ICPSettings.NumberOfStartTrialPoints, PS.Count, solutionList);
                        
                        if (myTrial != null)
                        {
                            myTrial.PointsSource = RandomUtils.ExtractPoints(PS, myTrial.RandomIndices);
                            solutionList.Add(myTrial);
                        }
                    }
                    ////test....
                    ////maxNumberSolutions = 1;
                    //ICPSolution myTrial1 = new ICPSolution();
                    //for (int i = 0; i < NumberPointsSolution; i++)
                    //{
                    //    myTrial1.RandomIndices.Add(i);
                    //}
                    //myTrial1.PointsSource = RandomUtils.ExtractPoints(PS, myTrial1.RandomIndices);
                    //solutionList[0] = myTrial1;


                }


                for (int i = 0; i < solutionList.Count; i++)
                {
                    PointCloudVertices transformedPoints = null;

                    ICPSolution myTrial = solutionList[i];


                    Helper_ICP_Iteration(ref myTrial.PointsSource, ref myTrial.PointsTarget, PT, PS, kdTree, angleThreshold);
                    myTrial.Matrix = Matrix4d.Mult(myTrial.Matrix, this.Matrix);
                    myTrial.MeanDistanceSubset = this.MeanDistance;
                                      
                    myTrial.MeanDistance = TransformPoints(ref transformedPoints, PT, PS, myTrial.Matrix);

                   // solutionList[i] = myTrial;

                }
                if (solutionList.Count > 0)
                {
                    solutionList.Sort(new ICPSolutionComparer());
                    RemoveSolutionIfMatrixContainsNaN(solutionList);
                    if (solutionList.Count == 0)
                        System.Windows.Forms.MessageBox.Show("No solution could be found !");
                    
                    this.Matrix = solutionList[0].Matrix;
                    this.MeanDistance = solutionList[0].MeanDistance;

                    if (solutionList[0].MeanDistance < ICPSettings.MaximumMeanDistance)
                    {
                        return true;
                    }
                   
                    
                }
                
            }
            catch (Exception err)
            {
                MessageBox.Show("Error in Helper_ICP_Iteration_SA: " + err.Message);
                return false;
            }

            return false;


        }
        private void CalculateNormals(PointCloud pointsSource, PointCloud pointsTarget)
        {

            Model myModelTarget = new Model("Target");
            myModelTarget.PointCloud = pointsTarget;
            myModelTarget.CalculateNormals_PCA();
            //myModelTarget.CalculateNormals_Triangulation();
            

            Model myModelSource = new Model("Source");
            myModelSource.PointCloud = pointsSource;
            //myModelSource.CalculateNormals_Triangulation();
            myModelSource.CalculateNormals_PCA();


            normalsTarget = myModelTarget.Normals;
            normalsSource = myModelSource.Normals;
        }
        private void CalculateNormals(PointCloud pointsSource, PointCloud pointsTarget, KDTreeVertex kdTreee)
        {
            if (ICPSettings.Normal_RemovePoints || ICPSettings.Normal_SortPoints)
            {

                CalculateNormals(pointsSource, pointsTarget);
                kdTreee.NormalsSource = this.normalsSource;
                kdTreee.NormalsTarget = this.normalsTarget;
                if (ICPSettings.Normal_RemovePoints)
                    kdTreee.Normals_RemovePoints = true;
                if (ICPSettings.Normal_SortPoints)
                    kdTreee.Normals_SortPoints = true;


            }
        }
        private PointCloudVertices CalculateMergedPoints(PointCloudVertices pResult, PointCloudVertices pTarget)
        {
            PointCloudVertices pMerged = PointCloudVertices.CloneVertices(pResult);

            for (int i = 0; i < pTarget.Count; i++)
            {
                pMerged.Add(pTarget[i]);
            }
            //for(int i = 0; i < pMerged.Count; i++)
            //{
            //    Vertex vMerged = pMerged[i];
            //    Vertex vSource = pTarget[i];
            //    if(vMerged.Vector.X == vSource.Vector.X && vMerged.Vector.Y == vSource.Vector.Y)
            //    {
            //        if(vMerged.Vector.Z == 0)
            //        {
            //            vMerged.Vector.Z = vSource.Vector.Z;
            //        }
            //        if (vMerged.Color == System.Drawing.Color.Black)
            //        {
            //            vMerged.Color = vSource.Color;
            //        }
            //    }


            //}
            return pMerged;


        }
    
        public PointCloudVertices PerformICP()
        {
            double convergenceThreshold = PTarget.BoundingBoxMaxFloat * ICPSettings.ConvergenceThreshold;

            PointCloudVertices PT = PointCloudVertices.CopyVertices(PTarget);
            PointCloudVertices PS = PointCloudVertices.CopyVertices(PSource);
            Vertex pSOrigin = new Vertex(0, new Vector3d(0,0,0));
            Vertex pTOrigin = new Vertex(0, new Vector3d(0, 0, 0));
            PointCloudVertices myPointsTransformed = null;
            ICPSettings.ResetVertexToOrigin = false;
            if (ICPSettings.ResetVertexToOrigin)
            {
                pTOrigin = PointCloudVertices.ResetCentroid(PT, true);
                pSOrigin = PointCloudVertices.ResetCentroid(PS, true);
            }
            
            KDTreeVertex kdTreee = Helper_CreateTree(PT);
            kdTreee.DistanceOptimization = this.ICPSettings.DistanceOptimization;
            CalculateNormals(PS.ToPointCloud(), PT.ToPointCloud(), kdTreee);
            
            
            try
            {
                if (!CheckSourceTarget(PT, PS))
                    return null;
                
                PointCloudVertices pointsTarget = PointCloudVertices.CopyVertices(PT);
                PointCloudVertices pointsSource = PointCloudVertices.CopyVertices(PS);
                
                this.Matrix = Matrix4d.Identity;
                double oldMeanDistance = 0;
                
               


                for (NumberOfIterations = 0; NumberOfIterations < ICPSettings.MaximumNumberOfIterations; NumberOfIterations++)
                {
                    kdTreee.NormalsSource = this.normalsSource;
                    float angleThreshold = Convert.ToSingle (this.startAngleForNormalsCheck - 5) * (1.0f- this.NumberOfIterations * 1.0f/ this.ICPSettings.MaximumNumberOfIterations) + 5;

                    if (ICPSettings.SimulatedAnnealing)
                    {
                        if (Helper_ICP_Iteration_SA(PS, PT, kdTreee, angleThreshold))
                            break;
                    }
                    else
                    {
                        myPointsTransformed = Helper_ICP_Iteration(ref pointsSource, ref pointsTarget, PT, PS, kdTreee, angleThreshold);
                        if (myPointsTransformed != null)
                            break;                        
                    }

                    Debug.WriteLine("--------------Iteration: " + NumberOfIterations.ToString() + " : Mean Distance: " + MeanDistance.ToString("0.00000000000") + ": duration: " + GlobalVariables.TimeSpanString());
                    if (Math.Abs(oldMeanDistance - MeanDistance) < convergenceThreshold)
                    {
                        Debug.WriteLine("Convergence reached - changes under: " + convergenceThreshold.ToString());
                        break;
                    }
                    oldMeanDistance = MeanDistance;
                    

                }

                Debug.WriteLine("--------****** Solution of ICP after : " + NumberOfIterations.ToString() + " iterations, and Mean Distance: " + MeanDistance.ToString("0.00000000000"));
                //if number of Iteration
                if (myPointsTransformed == null)
                    myPointsTransformed = pointsTransformed;

                if(this.ICPSettings.ShuffleEffect )
                {
                    PT = pointsTarget;
                    PTransformed = myPointsTransformed;
                }
                else
                {
                    PTransformed = MathUtilsVTK.TransformPoints(PS, Matrix);
                }
                
                //re-reset vector 
                if (ICPSettings.ResetVertexToOrigin)
                {
                    PointCloudVertices.AddVertex(PTransformed, pTOrigin);
                    

                }
               
                //DebugWriteUtils.WriteTestOutputVertex("Solution of ICP", Matrix, this.PSource, PTransformed, PTarget);
                if (myPointsTransformed != null)
                    DebugWriteUtils.WriteTestOutputVertex("Solution of ICP", Matrix, pointsSource, myPointsTransformed, pointsTarget);
                else
                {
                    //no convergence - write matrix
                    this.Matrix.Print("Cumulated Matrix ");
                }


                PMerged = CalculateMergedPoints(PTransformed, PT);

                return PTransformed;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error in Update ICP at iteration: " + NumberOfIterations.ToString() + " : " + err.Message);
                return null;

            }

        }
        private static bool CheckSourceTarget(PointCloudVertices myPointsTarget, PointCloudVertices mypointsSource)
        {
            // Check source, target
            if (mypointsSource == null || mypointsSource.Count == 0)
            {
                MessageBox.Show("Source point set is empty");
                System.Diagnostics.Debug.WriteLine("Can't execute with null or empty input");
                return false;
            }

            if (myPointsTarget == null || myPointsTarget.Count == 0)
            {
                MessageBox.Show("Target point set is empty");
                System.Diagnostics.Debug.WriteLine("Can't execute with null or empty target");
                return false;
            }
            return true;
        }


    }
  
}