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


namespace OpenTKLib
{
  
   
    public partial class KDTreeVertex 
    {
        public KDTreeMode KDTreeMode = KDTreeMode.Rednaxela;
        public bool DistanceOptimization = false;
        public KDTreeRednaxela.KDTree_Rednaxela<EllipseWrapper> KdTree_Rednaxela;
        public KDTree_Stark KdTree_Stark;

        public List<Vector3> NormalsSource;
        public List<Vector3> NormalsTarget;

        public float NormalAngleThreshold = 30;
        public bool Normals_RemovePoints;
        public bool Normals_SortPoints;

        public float MeanDistance ;

        public int NumberOfNeighboursToSearch = 1;
        //public int NumberOfNeighboursToSearch = 5;
        //private static KDTreeVertex instance;
        private delegate void FindNeighbourDelegate();
        PointCloudVertices pointsSource;
        PointCloudVertices pointsTarget;
        List<List<Neighbours>> listNeighbours;

        private bool NeighboursFound = false;
        public KDTreeVertex()
        {
            //NumberOfNeighboursToSearch = 5;
            NumberOfNeighboursToSearch = 1;

            KDTreeMode = KDTreeMode.Rednaxela;
            //instance = this;

        }
       
        public bool BuildKDTree_Rednaxela(PointCloudVertices target)
        {
            GlobalVariables.ResetTime();
            ResetVerticesLists(target);

            try
            {
                KdTree_Rednaxela = new KDTreeRednaxela.KDTree_Rednaxela<EllipseWrapper>(3);

                for (int i = 0; i < target.Count; ++i)
                {

                    Vertex p = target[i];
                    KdTree_Rednaxela.AddPoint(new float[] { Convert.ToSingle(p.Vector.X), Convert.ToSingle(p.Vector.Y), Convert.ToSingle(p.Vector.Z) }, new EllipseWrapper(p));

                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error building kd-tree " + err.Message);
                return false;

            }

            GlobalVariables.ShowLastTimeSpan("Build Tree Rednaxela - Number of points: " + target.Count.ToString() + " : " );

            return true;

        }
        public bool BuildKDTree_Stark(PointCloudVertices target)
        {
            GlobalVariables.ResetTime();
            KdTree_Stark = KDTree_Stark.Build(target);
            GlobalVariables.ShowLastTimeSpan("Build Tree Stark");

            return true;

        }

        public PointCloudVertices FindNearest_Rednaxela(ref PointCloudVertices mypointsSource, PointCloudVertices mypointsTarget, float angleThreshold)
        {
            this.pointsSource = mypointsSource;
            this.pointsTarget = mypointsTarget;

            int indexI = -1;
            try
            {
                PointCloudVertices pointsSourceNew = new PointCloudVertices(pointsSource);
                PointCloudVertices pointsResult = new PointCloudVertices();


                FindNearest_Rednaxela_Helper();



                for (int i = 0; i < listNeighbours.Count; i++)
                {

                    indexI = i;
                    List<Neighbours> mySublist = listNeighbours[i];
                    mySublist.Sort(new NeighboursComparer());
                    Vertex v = pointsTarget[mySublist[0].IndexTarget];
                    pointsResult.Add(v);


                }
                if (Normals_RemovePoints)
                {
                    listNeighbours.Sort(new NeighboursListComparer());
                    int minPoints = Math.Min(10, listNeighbours.Count - 1);


                    for (int i = 0; i < listNeighbours.Count; i++)
                    {

                        List<Neighbours> mySublist = listNeighbours[i];
                        int indexInsert = mySublist[0].IndexSource;
                        Vertex vSource = pointsSource[mySublist[0].IndexSource];
                        Vertex vTarget = pointsTarget[mySublist[0].IndexTarget];

                        pointsSourceNew[indexInsert] = vSource;
                        pointsResult[indexInsert] = vTarget;


                        if (i > minPoints)
                        {
                            if (mySublist[0].Angle > angleThreshold)
                            {
                                pointsSourceNew[indexInsert] = null;
                                pointsResult[indexInsert] = null;
                            }

                        }


                    }
                    for (int i = pointsSource.Count - 1; i >= 0; i--)
                    {
                        if (pointsSourceNew[i] == null)
                        {
                            pointsSourceNew.RemoveAt(i);
                            pointsResult.RemoveAt(i);
                        }

                    }

                    Debug.WriteLine("Remaining points : " + (pointsResult.Count * 1.0 / pointsTarget.Count * 100).ToString("0.00") + " %");

                }

                this.MeanDistance = PointCloudVertices.MeanDistance(pointsResult, pointsSource);
                pointsSource = pointsSourceNew;

                if (pointsSource.Count != pointsResult.Count)
                {
                    MessageBox.Show("Error finding neighbours, found " + pointsResult.Count.ToString() + " out of " + pointsSource.Count.ToString());
                    //return false;
                }

                mypointsSource = this.pointsSource;
                return pointsResult;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error in Finding neighbors at: " + indexI.ToString() + " : " + err.Message);
                return null;
            }

        }
        public PointCloudVertices FindNearest_Rednaxela_Parallel(ref PointCloudVertices mypointsSource, PointCloudVertices mypointsTarget, float angleThreshold)
        {
            this.pointsSource = mypointsSource;
            this.pointsTarget = mypointsTarget;

            int indexI = -1;
            try
            {
                PointCloudVertices pointsSourceNew = new PointCloudVertices(pointsSource);
                PointCloudVertices pointsResult = new PointCloudVertices();


                FindNearest_Rednaxela_HelperParallel();
               

                for (int i = 0; i < pointsSource.Count; i++)
                {

                    indexI = i;

                    Vertex v = pointsTarget[pointsSource[i].IndexKDTreeTarget];
                    pointsResult.Add(v);


                }
                
                this.MeanDistance = PointCloudVertices.MeanDistance(pointsResult, pointsSource);
                pointsSource = pointsSourceNew;

                if (pointsSource.Count != pointsResult.Count)
                {
                    MessageBox.Show("Error finding neighbours, found " + pointsResult.Count.ToString() + " out of " + pointsSource.Count.ToString());
                    //return false;
                }

                mypointsSource = this.pointsSource;
                return pointsResult;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error in Finding neighbors at: " + indexI.ToString() + " : " + err.Message);
                return null;
            }

        }
        public PointCloudVertices FindNearest_Rednaxela_ExcludePoints(PointCloudVertices source, PointCloudVertices target, bool findNormals)
        {


            NumberOfNeighboursToSearch = source.Count;
            FindNearest_NormalsCheck_Rednaxela(source, findNormals);

            PointCloudVertices result = new PointCloudVertices();

            float totalDistance = 0;


            int i = -1;
            int loopMax = source.Count;
            
            if (target.Count < loopMax)
            {
                loopMax = target.Count;
                
                for (i = source.Count - 1; i >= loopMax; i--)
                {
                    source.RemoveAt(i);
                }

            }

            try
            {
                for (i = 0; i < loopMax; i++)
                {
                    totalDistance += source[i].KDTreeSearch[0].Value;
                    result.Add(target[source[i].KDTreeSearch[0].Key]);


                }

            }
            catch 
            {
                System.Windows.Forms.MessageBox.Show("Error setting search key at: " + i.ToString());

            }
            MeanDistance = totalDistance / loopMax;



            return result;


        }
       
        private void RemovePointsByAngle(ref PointCloudVertices pointsSource, ref PointCloudVertices pointsTarget, List<List<Neighbours>> listNeighbours)
        {
            
            if (Normals_RemovePoints)
            {
                //adjust normals - because of Search, the number of PointTarget my be different

                int pointsRemoved = 0;
                for (int i = pointsTarget.Count - 1; i >= 0; i--)
                {
                    Vector3d vT = pointsTarget[i].Vector;
                    Vector3d vS = pointsSource[i].Vector;
                    //float angle = Vector3d.CalculateAngle(vT, vS);
                    int indexVec = pointsTarget[i].IndexInModel;
                    Vector3 vTNormal = NormalsTarget[pointsTarget[i].IndexInModel];
                    //Vector3d vTNormal = NormalsTarget[i];
                    Vector3 vSNormal = NormalsSource[i];

                    float angle = vTNormal.AngleInDegrees(vSNormal);
                    //float angle = vT.AngleInDegrees(vS);
                    float angleToCheck = 30;// (this.startAngleForNormalsCheck - 5) * (1 - this.NumberOfIterations / this.ICPSettings.MaximumNumberOfIterations) + 5;
                    Debug.WriteLine("Angle: " + i.ToString() + " : " + angle.ToString("0.00"));

                    if (Math.Abs(angle) > angleToCheck)
                    {
                        pointsTarget.RemoveAt(i);
                        pointsSource.RemoveAt(i);
                        pointsRemoved++;

                    }
                }
                Debug.Assert(pointsTarget.Count > 2, "Error after kdtree - checking normals : Should have at least 3 points found");

                Debug.WriteLine("--NormalCheck: Removed a total of: " + pointsRemoved.ToString());

            }

        }
        public void FindNearest_Rednaxela_Helper()
        {
            NeighboursFound = false;

            listNeighbours = new List<List<Neighbours>>();
            for (int i = 0; i < pointsSource.Count; i++)
            {
              


               Vertex vSource = pointsSource[i];
               // Perform a nearest neighbour search around that point.
               KDTreeRednaxela.NearestNeighbour<EllipseWrapper> nearestNeighbor = null;

               nearestNeighbor = KdTree_Rednaxela.FindNearest_EuclidDistance(new float[] { Convert.ToSingle(vSource.Vector.X), Convert.ToSingle(vSource.Vector.Y), Convert.ToSingle(vSource.Vector.Z) }, 1, -1);
               List<Neighbours> neighboursForVertex = new List<Neighbours>();
               
               lock (listNeighbours)
               {
                   listNeighbours.Add(neighboursForVertex);
               }

               while (nearestNeighbor.MoveNext())
               {
                   Vertex vTarget = nearestNeighbor.CurrentPoint.Vertex;

                   Neighbours kv = new Neighbours();
                   kv.IndexSource = vSource.IndexInModel;
                   kv.Distance = nearestNeighbor.CurrentDistance;
                   kv.IndexTarget = vTarget.IndexInModel;

                   if (Normals_SortPoints && NormalsSource != null)
                   {
                       Vector3 nSource = NormalsSource[vSource.IndexInModel];
                       Vector3 nTarget = NormalsTarget[vTarget.IndexInModel];

                       float angle = nSource.AngleInDegrees(nTarget);
                       kv.Angle = angle;


                   }
                   neighboursForVertex.Add(kv);

               }

           };
            



            if (DistanceOptimization)
                RemovePointsWithDistanceGreaterThanAverage(pointsSource, pointsTarget, listNeighbours);


            NeighboursFound = true;
        }

        public void FindNearest_Rednaxela_HelperParallel()
        {
            NeighboursFound = false;

            listNeighbours = new List<List<Neighbours>>();
            //for (int i = 0; i < pointsSource.Count; i++)
            System.Threading.Tasks.Parallel.For(0, pointsSource.Count, i =>
            {

                Vertex vSource = pointsSource[i];
                // Perform a nearest neighbour search around that point.
                KDTreeRednaxela.NearestNeighbour<EllipseWrapper> nearestNeighbor = null;

                nearestNeighbor = KdTree_Rednaxela.FindNearest_EuclidDistance(new float[] { Convert.ToSingle(vSource.Vector.X), Convert.ToSingle(vSource.Vector.Y), Convert.ToSingle(vSource.Vector.Z) }, 1, -1);
                List<Neighbours> neighboursForVertex = new List<Neighbours>();

                lock (listNeighbours)
                {
                    listNeighbours.Add(neighboursForVertex);
                }

                while (nearestNeighbor.MoveNext())
                {
                    Vertex vTarget = nearestNeighbor.CurrentPoint.Vertex;
                    vSource.IndexKDTreeTarget = vTarget.IndexInModel;
                    

                }

            });

            if (DistanceOptimization)
                RemovePointsWithDistanceGreaterThanAverage(pointsSource, pointsTarget, listNeighbours);


            NeighboursFound = true;
        }
        private void RemovePointsWithDistanceGreaterThanAverage(PointCloudVertices pointsSource, PointCloudVertices pointsTarget, List<List<Neighbours>> listNeighbours)
        {
            float median = GetAverage(listNeighbours);

            for (int i = listNeighbours.Count - 1; i >= 0; i--)
            {
                if (listNeighbours[i][0].Distance > median)
                {
                    pointsSource.RemoveAt(i);
                    pointsTarget.RemoveAt(i);

                }

            }
        }
        private void RemovePointsWithDistanceGreaterThanMedian(List<float> listDistances, PointCloudVertices pointsSource, PointCloudVertices pointsTarget)
        {
            float median = GetMedian(listDistances);

            for (int i = listDistances.Count -1 ; i >=0 ; i--)
            {
                if (listDistances[i] > median)
                {
                    pointsSource.RemoveAt(i);
                    pointsTarget.RemoveAt(i);

                }

            }
        }
        public float GetAverage(List<List<Neighbours>> source)
        {
            float[] dArr = new float[source.Count];
            for(int i = 0; i < source.Count; i++)
            {
                dArr[i] = source[i][0].Distance;
            }

            Array.Sort(dArr);

            return dArr[dArr.Length / 2];

           

        }
        public float GetMedian(List<float> source)
        {
            // Create a copy of the input, and sort the copy
            float[] temp1 = source.ToArray();
            float[] temp  = new float[temp1.Length];
            temp1.CopyTo(temp, 0);

            Array.Sort(temp);


            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                float a = temp[count / 2 - 1];
                float b = temp[count / 2];
                return (a + b) / 2;
            }
            else
            {
                // count is odd, return the middle element
                return temp[Convert.ToInt32(count * 0.8)] ;
            }
        }
        public PointCloudVertices FindNearest_Stark(PointCloudVertices source, PointCloudVertices target)
        {
            KdTree_Stark.ResetSearch();
            PointCloudVertices result = new PointCloudVertices();
            
            List<int> pointsFound = new List<int>();
            List<List<int>> searchdoubleList = new List<List<int>>(3);

            for (int i = source.Count - 1; i >= 0; i--)
            //for (int i = 0; i < source.Count ; i ++)
            {
                
                //int indexNearest = KdTree_Stark.FindNearest(source[i]);
                int indexNearest = KdTree_Stark.FindNearestAdapted(source[i], pointsFound);

                //result.Add(target[indexNearest]);

                if (!pointsFound.Contains(indexNearest))
                {
                    pointsFound.Add(indexNearest);
                    result.Add(target[indexNearest]);
                }
                else
                {
                    bool bfound = false;
                    for (int j = 0; j < KDTree_Stark.LatestSearchResults.Count; j++)
                    {
                        int newIndex = KDTree_Stark.LatestSearchResults[j].Key;
                        if (!pointsFound.Contains(newIndex))
                        {
                            bfound = true;
                            pointsFound.Add(newIndex);
                            result.Add(target[newIndex]);
                            break;

                        }

                    }
                    if(!bfound)
                        source.RemoveAt(i);
                }


            }

            return result;
        }
        public PointCloudVertices NearestPointIndices_Stark(PointCloudVertices source)
        {
            KdTree_Stark.ResetSearch();
            PointCloudVertices result = new PointCloudVertices();
            PointCloudVertices sourceTemp = source;
            List<int> indicesTargetFound = new List<int>();

            for (int i = sourceTemp.Count - 1; i >= 0; i--)
            {
                Vertex v = sourceTemp[i];
                //tempList.RemoveAt(i);

                int neighboursFound = 0;
                v.KDTreeSearch.Clear();
                
                for (int j = 0; j < NumberOfNeighboursToSearch; j++)
                {
                    int indexNearest = KdTree_Stark.FindNearest(sourceTemp[i]);
                    if (!v.KDTreeSearch.Contains(indexNearest))
                    {
                        KDTree_Stark.LatestSearchResults.Sort(new KeyValueComparer());
                        neighboursFound++;
                        KeyValuePair<int, float> res = KDTree_Stark.LatestSearchResults[0];
                        //check if the index is right
                        v.KDTreeSearch.Add(res);
                       
                    }
                    if ((neighboursFound + 1) > NumberOfNeighboursToSearch)
                        break;
                   
                   
                }


            }

            return result;
        }
     
        public PointCloudVertices FindNearest_BruteForce(PointCloudVertices source, PointCloudVertices target)
        {



            PointCloudVertices result = new PointCloudVertices();
            List<int> indicesTargetFound = new List<int>();

            PointCloudVertices tempTarget = PointCloudVertices.CopyVertices(target);

            for (int i = source.Count - 1; i >= 0; i--)
            {
                BuildKDTree_Stark(tempTarget);

                int indexNearest = KdTree_Stark.FindNearest(source[i]);
                result.Add(target[indexNearest]);
                tempTarget.RemoveAt(indexNearest);

            }

            return result;


           
        }
        public PointCloudVertices FindNearest_BruteForceOld(PointCloudVertices vSource, PointCloudVertices vTarget)
        {
            PointCloudVertices nearestNeighbours = new PointCloudVertices();
            int iMax = 10;
            PointCloudVertices tempTarget = PointCloudVertices.CopyVertices(vTarget);

            for (int i = 0; i < vSource.Count; i++)
            {
                //BuildKDTree_Standard(tempTarget);

                Vertex p = vSource[i];
                // Perform a nearest neighbour search around that point.
                KDTreeRednaxela.NearestNeighbour<EllipseWrapper> pIter = null;
                pIter = KdTree_Rednaxela.FindNearest_EuclidDistance(new float[] { Convert.ToSingle(p.Vector.X), Convert.ToSingle(p.Vector.Y), Convert.ToSingle(p.Vector.Z)}, iMax, -1);
                while (pIter.MoveNext())
                {
                    // Get the ellipse.
                    //var pEllipse = pIter.Current;
                    EllipseWrapper wr = pIter.CurrentPoint;
                    nearestNeighbours.Add(wr.Vertex);
                    tempTarget.RemoveAt(pIter.CurrentIndex);
                    break;
                }

            }
            return nearestNeighbours;
        }
       
      
        public void ResetVerticesLists(PointCloudVertices pointCloud)
        {
            
            for (int i = pointCloud.Count -1; i >=0 ; i--)
            {
                Vertex v = pointCloud[i];
                v.KDTreeSearch.Clear();
                pointCloud[i] = v;

            }
        }

        public void FindNearest_NormalsCheck_Rednaxela(PointCloudVertices pointCloud, bool normalsCheck)
        {

            PointCloudVertices nearestNeighbours = new PointCloudVertices();

            //for (int i = pointCloud.Count - 1; i >= 0; i--)
            for (int i = 0; i < pointCloud.Count; i++)
            {
                Vertex vSource = pointCloud[i];

                // Perform a nearest neighbour search around that point.
                KDTreeRednaxela.NearestNeighbour<EllipseWrapper> nearestNeighbor = null;

                if (normalsCheck)
                {
                    nearestNeighbor = KdTree_Rednaxela.FindNearest_EuclidDistance(new float[] { Convert.ToSingle(vSource.Vector.X), Convert.ToSingle(vSource.Vector.Y), Convert.ToSingle(vSource.Vector.Z) }, NumberOfNeighboursToSearch + 1, -1);
                }
                else
                {
                    nearestNeighbor = KdTree_Rednaxela.FindNearest_EuclidDistance(new float[] { Convert.ToSingle(vSource.Vector.X), Convert.ToSingle(vSource.Vector.Y), Convert.ToSingle(vSource.Vector.Z) }, NumberOfNeighboursToSearch, -1);
                }
            
                while (nearestNeighbor.MoveNext())
                {
                    EllipseWrapper wr = nearestNeighbor.CurrentPoint;
                    Vertex vTarget = wr.Vertex;

                    if (vSource != vTarget)
                    {

                        if (!vSource.KDTreeSearch.Contains(vTarget.IndexInModel))
                        {
                            if (!vTarget.TakenInTree)
                            {
                                vTarget.TakenInTree = true;
                                KeyValuePair<int, float> el = new KeyValuePair<int, float>(vTarget.IndexInModel, nearestNeighbor.CurrentDistance);
                                vSource.KDTreeSearch.Add(el);
                                break;

                            }


                        }

                        if ((vSource.KDTreeSearch.Count) >= 1)
                            break;
                    }
                }
                //if (vSource.KDTreeSearch.Count == 0)
                //{
                //    System.Windows.Forms.MessageBox.Show("Error in finding neighbour for index " + i.ToString());
                //}


            }

            if (KdTree_Rednaxela.pLeft != null)
                SetRecursive(KdTree_Rednaxela.pLeft);
            if (KdTree_Rednaxela.pRight != null)
                SetRecursive(KdTree_Rednaxela.pRight);

            GlobalVariables.ShowLastTimeSpan("Find neighbours");

            //RemoveAllVerticesBasedOnRadius(pointCloud);


        }
        private void SetRecursive(KDTreeRednaxela.KDNode_Rednaxela<EllipseWrapper> node)
        {
            
            if(node.data != null)
            {
                for (int i = 0; i < node.data.Length; i++)
                {
                    if (node.data[i] != null)
                        node.data[i].Vertex.TakenInTree = false;
                }
            }
            //if(node.IsLeaf || node.IsSinglePoint)
            //{
               
            //}
            if (node.pLeft != null)
                SetRecursive(node.pLeft);
            if (node.pRight != null)
                SetRecursive(node.pRight);


        }
        private void RemoveAllVerticesBasedOnRadius(PointCloudVertices pointCloud)
        {
            //remove all pointCloud beyound minimal radius
            //(distance list is automatically sorted due to KDTree)
            for (int i = pointCloud.Count - 1; i >= 0; i--)
            {
                Vertex v = pointCloud[i];
                //if(NeighboursDistance.Count < NumberOfNeighboursToSearch)
                float distanceMax = v.KDTreeSearch[NumberOfNeighboursToSearch - 1].Value;
                for (int j = v.KDTreeSearch.Count - 1; j >= 2; j--)
                {

                    if (v.KDTreeSearch[j].Value > distanceMax)
                    {
                        v.KDTreeSearch.RemoveAt(j);
                       
                    }

                }


            }
        }
    }
  
}