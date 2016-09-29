
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
        
     
       
        
        private bool CheckExitOnIterations()
        {
            this.NumberOfIterations++;
            System.Diagnostics.Debug.WriteLine("Iteration: " + this.NumberOfIterations);
            if (this.NumberOfIterations >= ICPSettings.MaximumNumberOfIterations)
            {
                return true;
            }
            return false;
        }



        private void SetStartPoints(ref PointCloudVertices points1, ref PointCloudVertices points2, PointCloudVertices pointsInput1, PointCloudVertices pointsInput2)
        {

            List<int> randomIndices = RandomUtils.UniqueRandomIndices(3, pointsInput1.Count);


            points1 = RandomUtils.ExtractPoints(pointsInput1, randomIndices);
            points2 = RandomUtils.ExtractPoints(pointsInput2, randomIndices);

        }


        private static Matrix4d TryoutNewPoint(int iPoint, PointCloudVertices pointsTarget, PointCloudVertices pointsSource, PointCloudVertices pointsTargetTrial, PointCloudVertices pointsSourceTrial, LandmarkTransform myLandmarkTransform)
        {

            Vertex p1 = pointsTarget[iPoint];
            Vertex p2 = pointsSource[iPoint];
            pointsTargetTrial.Add(p1);
            pointsSourceTrial.Add(p2);



            MathUtilsVTK.FindTransformationMatrix(PointCloudVertices.ToVectors(pointsSourceTrial), PointCloudVertices.ToVectors(pointsTargetTrial), myLandmarkTransform);//, accumulate);
     
            Matrix4d myMatrix = myLandmarkTransform.Matrix;
          

            return myMatrix;
        }
        public static Matrix4d TryoutPoints(PointCloudVertices pointsTarget, PointCloudVertices pointsSource, ICPSolution res, LandmarkTransform myLandmarkTransform)
        {
            res.PointsTarget = RandomUtils.ExtractPoints(pointsTarget, res.RandomIndices);
            res.PointsSource = RandomUtils.ExtractPoints(pointsSource, res.RandomIndices);

            //transform:
            MathUtilsVTK.FindTransformationMatrix(PointCloudVertices.ToVectors(res.PointsSource), PointCloudVertices.ToVectors(res.PointsTarget), myLandmarkTransform);//, accumulate);

            res.Matrix = myLandmarkTransform.Matrix;

            return res.Matrix;

        }
        private static ICPSolution IterateStartPoints(PointCloudVertices pointsSource, PointCloudVertices pointsTarget, int myNumberPoints, LandmarkTransform myLandmarkTransform, int maxNumberOfIterations)
        {
            int maxIterationPoints = pointsSource.Count;
            int currentIteration = 0;
            try
            {
                if (myNumberPoints > pointsSource.Count)
                    myNumberPoints = pointsSource.Count;

                List<ICPSolution> solutionList = new List<ICPSolution>();

                for (currentIteration = 0; currentIteration < maxNumberOfIterations; currentIteration++)
                {

                    ICPSolution res = ICPSolution.SetRandomIndices(myNumberPoints, maxIterationPoints, solutionList);


                    res.Matrix = TryoutPoints(pointsTarget, pointsSource, res, myLandmarkTransform);//, accumulate);
                    res.PointsTransformed = MathUtilsVTK.TransformPoints(res.PointsSource, res.Matrix);

                    res.MeanDistance = PointCloudVertices.MeanDistance(res.PointsTarget, res.PointsTransformed);
                    //res.MeanDistance = totaldist / Convert.ToSingle(res.PointsSource.Count);
                  
                    solutionList.Add(res);

                  
                }


                if (solutionList.Count > 0)
                {
                    solutionList.Sort(new ICPSolutionComparer());
                    RemoveSolutionIfMatrixContainsNaN(solutionList);
                    if(solutionList.Count == 0)
                        System.Windows.Forms.MessageBox.Show("No start solution could be found !");


                    Debug.WriteLine("Solutions found after: " + currentIteration.ToString() + " iterations, number of solution " + solutionList.Count.ToString());

                    if (solutionList.Count > 0)
                    {
                        ICPSolution result = solutionList[0];
                        //write solution to debug ouput
                        //System.Diagnostics.Debug.WriteLine("Solution of start sequence is: ");
                        DebugWriteUtils.WriteTestOutputVertex("Solution of start sequence", result.Matrix, result.PointsSource, result.PointsTransformed, result.PointsTarget);
                        return result;
                   
                    }

                }
                return null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error in IterateStartPoints of ICP at: " + currentIteration.ToString() + " : " + err.Message);
                return null;
            }


        }
        private static bool CheckIfMatrixIsOK(Matrix4d myMatrix)
        {
            //ContainsNaN
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (double.IsNaN( myMatrix[i, 0]  ))
                        return false;

                }
            }
            return true;

        }
        private static void RemoveSolutionIfMatrixContainsNaN(List<ICPSolution> solutionList)
        {
            int iTotal = 0;
            for (int i = solutionList.Count - 1; i >= 0; i--)
            {
                if (!CheckIfMatrixIsOK(solutionList[i].Matrix))
                {
                    iTotal++;
                    
                    solutionList.RemoveAt(i);
                }
            }
           // Debug.WriteLine("-->Removed a total of: " + iTotal.ToString() + " solutions - because invalid matrixes");
        }
        /// <summary>
        /// calculates a start solution set in total of "myNumberPoints" points
        /// </summary>
        /// <param name="pointsTargetSubset"></param>
        /// <param name="pointsSourceSubset"></param>
        /// <returns></returns>
        private static ICPSolution CalculateStartSolution(ref  PointCloudVertices pointsSourceSubset, ref PointCloudVertices pointsTargetSubset, int myNumberPoints,
            LandmarkTransform myLandmarkTranform, PointCloudVertices pointsTarget, PointCloudVertices pointsSource, int maxNumberOfIterations)
        {
            try
            {
                if (CheckSourceTarget(pointsTarget, pointsSource))
                    return null;
                pointsTargetSubset = PointCloudVertices.CopyVertices(pointsTarget);
                pointsSourceSubset = PointCloudVertices.CopyVertices(pointsSource);

                ICPSolution res = IterateStartPoints(pointsSourceSubset, pointsTargetSubset, myNumberPoints, myLandmarkTranform, maxNumberOfIterations);
                if (res == null)
                {
                    System.Windows.Forms.MessageBox.Show("Could not find starting points for ICP Iteration - bad matching");
                    return null;
                }
                PointCloudVertices.RemoveEntriesByIndices(ref pointsSourceSubset, ref pointsTargetSubset, res.RandomIndices);

                return res;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error in CalculateStartSolution of ICP: " + err.Message);
                return null;
            }
        }
  
        private double CheckNewPointDistance(int iPoint, Matrix4d myMatrix, PointCloudVertices pointsTarget, PointCloudVertices pointsSource)
        {
            Vertex p1 = pointsTarget[iPoint];
            Vertex p2 = pointsSource[iPoint];
            PointCloudVertices tempPointReference = new PointCloudVertices();
            PointCloudVertices tempPointToBeMatched = new PointCloudVertices();

            tempPointReference.Add(p1);
            tempPointToBeMatched.Add(p2);

            PointCloudVertices tempPointRotate = MathUtilsVTK.TransformPoints(tempPointToBeMatched, myMatrix);
            double dist = PointCloudVertices.MeanDistance(tempPointReference, tempPointRotate);
            return dist;

        }
        public PointCloudVertices PerformICP_Stitching()
        {
            int iPoint = 0;
            try
            {
               
                PointCloudVertices pointsTarget = null;
                PointCloudVertices pointsSource = null;

                ICPSolution res = CalculateStartSolution(ref  pointsSource, ref pointsTarget, ICPSettings.NumberOfStartTrialPoints, this.LandmarkTransform, this.PTarget, this.PSource, ICPSettings.MaximumNumberOfIterations);
                if (res == null)
                    return null;

                Matrix4d myMatrix = res.Matrix;
                
               

                double oldMeanDistance = 0;
                //now try all points and check if outlier
                for (iPoint = (pointsTarget.Count - 1); iPoint >= 0; iPoint--)
                {
                    double distanceOfNewPoint = CheckNewPointDistance(iPoint, myMatrix, pointsTarget, pointsSource);

                    ////experimental

                    ////--compare this distance to:
                    //pointsTargetTrial.Add[pointsTargetTrial.Count, p1[0], p1[1], p1[2]);
                    //pointsSourceTrial.Add[pointsSourceTrial.Count, p2[0], p2[1], p2[2]);
                    //PointCloud tempPointRotateAll = TransformPoints(pointsSourceTrial, myMatrix, pointsSourceTrial.Count);


                    //dist = CalculateTotalDistance(pointsTargetTrial, tempPointRotateAll);
                    //DebugWriteUtils.WriteTestOutput(myMatrix, pointsSourceTrial, tempPointRotateAll, pointsTargetTrial, pointsTargetTrial.Count);
                    Debug.WriteLine("------>ICP Iteration Trial: " + iPoint.ToString() + " : Mean Distance: " + distanceOfNewPoint.ToString());
                    if (Math.Abs(distanceOfNewPoint - res.MeanDistance) < ICPSettings.ThresholdOutlier)
                    {
                        PointCloudVertices pointsTargetTrial = PointCloudVertices.CopyVertices(res.PointsTarget);
                        PointCloudVertices pointsSourceTrial = PointCloudVertices.CopyVertices(res.PointsSource);


                        myMatrix = TryoutNewPoint(iPoint, pointsTarget, pointsSource, pointsTargetTrial, pointsSourceTrial, this.LandmarkTransform);

                        PointCloudVertices myPointsTransformed = MathUtilsVTK.TransformPoints(pointsSourceTrial, myMatrix);
                        this.MeanDistance = PointCloudVertices.MeanDistance(pointsTargetTrial, myPointsTransformed);
                       // this.MeanDistance = totaldist / Convert.ToSingle(pointsTargetTrial.Count);


                        DebugWriteUtils.WriteTestOutputVertex("Iteration " + iPoint.ToString(),  myMatrix, pointsSourceTrial, myPointsTransformed, pointsTargetTrial);

                        //could also remove this check...
                        if (Math.Abs(oldMeanDistance - this.MeanDistance) < ICPSettings.ThresholdOutlier)
                        {

                            res.PointsTarget = pointsTargetTrial;
                            res.PointsSource = pointsSourceTrial;
                            res.Matrix = myMatrix;
                            res.PointsTransformed = myPointsTransformed;
                            oldMeanDistance = this.MeanDistance;

                            //Debug.WriteLine("************* Point  OK : ");
                            DebugWriteUtils.WriteTestOutputVertex("************* Point  OK :" , myMatrix, res.PointsSource, myPointsTransformed, res.PointsTarget);

                        }
                        //remove point from point list
                        pointsTarget.RemoveAt(iPoint);
                        pointsSource.RemoveAt(iPoint);
                       

                    }


                }
                this.Matrix = res.Matrix;
                //System.Diagnostics.Debug.WriteLine("Solution of ICP is : ");
                DebugWriteUtils.WriteTestOutputVertex("Solution of ICP", Matrix, res.PointsSource, res.PointsTransformed, res.PointsTarget);
                pointsTransformed = res.PointsTransformed;

                return pointsTransformed;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error in Update ICP at point: " + iPoint.ToString() + " : " + err.Message);
                return null;

            }
            //Matrix4d newMatrix = accumulate.GetMatrix();
            //this.Matrix = newMatrix;

        }

        private PointCloudVertices ICPOnPoints_WithSubset(PointCloudVertices myPCLTarget, PointCloudVertices myPCLToBeMatched, PointCloudVertices myPointsTargetSubset, PointCloudVertices mypointsSourceSubset)
        {

            List<Vector3d> myVectorsTransformed = null;
            PointCloudVertices myPCLTransformed = null;

            try
            {
                Matrix4d m;


                PerformICP(mypointsSourceSubset, myPointsTargetSubset);
                myVectorsTransformed = PointCloudVertices.ToVectors(PTransformed);
                m = Matrix;

                //DebugWriteUtils.WriteTestOutput(m, mypointsSourceSubset, myPointsTransformed, myPointsTargetSubset);
                //extend points:
                //myPointsTransformed = icpSharp.TransformPointsToPointsData(mypointsSourceSubset, m);
                //-----------------------------
                //DebugWriteUtils.WriteTestOutput(m, mypointsSourceSubset, myPointsTransformed, myPointsTargetSubset);

                //now with all other points as well...
                myVectorsTransformed = new List<Vector3d>();

                myVectorsTransformed = m.TransformPoints(PointCloudVertices.ToVectors(myPCLToBeMatched));
                myPCLTransformed = PointCloudVertices.FromVectors(myVectorsTransformed);
                //write all results in debug output
                DebugWriteUtils.WriteTestOutputVertex("Soluation of Points With Subset", m, myPCLToBeMatched, myPCLTransformed, myPCLTarget);

            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Error in ICP : " + err.Message);
                return null;
            }
            //for output:
           

            return myPCLTransformed;

        }


        private PointCloudVertices ICPOnPoints_WithSubset_PointsData(List<PointCloudVertices> PointsDataList, List<System.Drawing.Point> pointsLeft, List<System.Drawing.Point> pointsRight)
        {

            PointCloudVertices myPointsTarget = PointsDataList[0];
            PointCloudVertices mypointsSource = PointsDataList[1];
            if (PointsDataList.Count > 1)
            {
                if (pointsLeft != null)
                {
                    PointCloudVertices mySubsetLeft = PointCloudVertices.FromPoints2d(pointsLeft, PointsDataList[0], pointsRight);
                    PointCloudVertices mySubsetRight = PointCloudVertices.FromPoints2d(pointsRight, PointsDataList[1], pointsLeft);

                    if (mySubsetLeft.Count == mySubsetRight.Count)
                    {

                        PointCloudVertices myPointsTransformed = ICPOnPoints_WithSubset(myPointsTarget, mypointsSource, mySubsetLeft, mySubsetRight);
                        return myPointsTransformed;


                    }
                    else
                    {
                        MessageBox.Show("Error in identifying stitched points ");

                    }
                }
            }

            return null;
        }
        public PointCloudVertices ICPOnPointss_WithSubset_Vector3d(PointCloudVertices myVector3dReference, PointCloudVertices myVector3dToBeMatched, List<System.Drawing.Point> pointsLeft2D, List<System.Drawing.Point> pointsRight2D)
        {
            List<PointCloudVertices> PointsDataList = new List<PointCloudVertices>();
            PointCloudVertices myPointsTarget = myVector3dReference;
            PointsDataList.Add(myPointsTarget);

            PointCloudVertices mypointsSource = myVector3dToBeMatched;
            PointsDataList.Add(mypointsSource);


            PointCloudVertices myPointsTransformed = ICPOnPoints_WithSubset_PointsData(PointsDataList, pointsLeft2D, pointsRight2D);
            if (myPointsTransformed != null)
            {

                //PointsTarget = myPointsTarget;
                //pointsSource = mypointsSource;
                //PointsTransformed = myPointsTransformed;

              
                return myPointsTransformed;
            }
            return null;

        }
        
    }
   
}