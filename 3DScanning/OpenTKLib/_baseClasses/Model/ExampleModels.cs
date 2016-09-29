
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using OpenTK;

namespace OpenTKLib
{
    public static class Example3DModels
    {
        private static double rCyl = 1f;
        private static double rCon = 1f;
        private static double rSph = 1f;

    

        private static double[] CylFunction(double u, double v)
        {
            double[] listOfPoints = new double[6] { 0.0f, 0.0f, 0.0f, (double)Math.Cos((double)u), (double)Math.Sin((double)u), 0.0f };
            listOfPoints[0] = Example3DModels.rCyl * listOfPoints[3];
            listOfPoints[1] = Example3DModels.rCyl * listOfPoints[4];
            listOfPoints[2] = v;
            return listOfPoints;
        }
        /// <summary>Generates a 3D Model for a cone.</summary>
     

        /// <summary>
        /// Generates a 3D Model for a cuboid
        /// </summary>
        /// <param name="Name">Model name</param>
        /// <param name="u">Length of the lower part</param>
        /// <param name="v">Length of the high part</param>
        /// <param name="numberOfPoints">Number of points to use in circumference</param>
        /// <param name="Color">Color vector</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        /// <returns></returns>
        public static Model Cuboid(string Name, double u, double v, int numberOfPoints, System.Drawing.Color color, System.Drawing.Bitmap Texture)
        {

            PointCloudVertices points = PointCloudVertices.CreateCuboid(u, v, numberOfPoints);
            PointCloudVertices.SetColorOfListTo(points, color);

            Model myModel = new Model("Cuboid");
            myModel.PointCloud = points.ToPointCloud();
           
            return myModel;

        }
        public static Vector3[] Cuboid(int numberOfPoints, double u, double v)
        {

            Vector3[] arr = new Vector3[numberOfPoints * 4];

            double v0 = 0f;
            int indexInModel = 0;
            for (int i = 0; i < numberOfPoints; i++)
            {

                arr[indexInModel] = new Vector3(0, v0, 0);
                indexInModel++;
                arr[indexInModel] = new Vector3(0, v0, u);
                indexInModel++;
                arr[indexInModel] = new Vector3(u, v0, u);
                indexInModel++;
                arr[indexInModel] = new Vector3(u, v0, 0);

                v0 += v / numberOfPoints;

            }

            return arr;

        }
        /// <summary>
        /// Generates a 3D Model for a cuboid
        /// </summary>
        /// <param name="Name">Model name</param>
        /// <param name="u">Length of the lower part</param>
        /// <param name="v">Length of the high part</param>
        /// <param name="numberOfPoints">Number of points to use in circumference</param>
        /// <param name="Color">Color vector</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        /// <returns></returns>
        public static PointCloud Cuboid(double u, double v, int numberOfPoints, System.Drawing.Color color)
        {
            PointCloud pcl = new PointCloud();
            pcl.Vectors = Example3DModels.Cuboid(numberOfPoints, u, v);
            pcl.SetColor(new Vector3(color.R, color.G, color.B));


            return pcl;


        }


        public static List<Vector3> Cube_Corners(double cubeSizeX, double cubeSizeY, double cubeSizeZ)
        {


            List<Vector3> listVectors = new List<Vector3>();

            listVectors.Add(new Vector3(-cubeSizeX / 2, -cubeSizeY / 2, cubeSizeZ / 2));
            listVectors.Add(new Vector3(cubeSizeX / 2, -cubeSizeY / 2, cubeSizeZ / 2));
            listVectors.Add(new Vector3(cubeSizeX / 2, cubeSizeY / 2, cubeSizeZ / 2));
            listVectors.Add(new Vector3(-cubeSizeX / 2, cubeSizeY / 2, cubeSizeZ / 2));

            listVectors.Add(new Vector3(-cubeSizeX / 2, -cubeSizeY / 2, -cubeSizeZ / 2));
            listVectors.Add(new Vector3(cubeSizeX / 2, -cubeSizeY / 2, -cubeSizeZ / 2));
            listVectors.Add(new Vector3(cubeSizeX / 2, cubeSizeY / 2, -cubeSizeZ / 2));
            listVectors.Add(new Vector3(-cubeSizeX / 2, cubeSizeY / 2, -cubeSizeZ / 2));


            return listVectors;

        }
        public static List<Vector3> CreateCube_RegularGrid_Filled(double cubeSize, int numberOfPointsPerPlane)
        {
            List<Vector3> listVectors = new List<Vector3>();

            double startP = cubeSize;


            for (int i = 0; i <= numberOfPointsPerPlane; i++)
            {
                for (int j = 0; j <= numberOfPointsPerPlane; j++)
                {

                    for (int k = 0; k <= numberOfPointsPerPlane; k++)
                    {
                        Vector3 v = new Vector3(startP * (-.5f + i / Convert.ToSingle(numberOfPointsPerPlane)), startP * (-0.5f + j / Convert.ToSingle(numberOfPointsPerPlane)), startP * (-0.5f + k / Convert.ToSingle(numberOfPointsPerPlane)));
                        listVectors.Add(v);

                    }

                }
            }
            return listVectors;
        }
        public static List<Vector3> CreateCube_RegularGrid_Empty(double cubeSize, int numberOfPointsPerPlane)
        {
            List<Vector3> points = new List<Vector3>();

            double startP = cubeSize;

            List<Vector3> pointsList = new List<Vector3>();

            
            for (int i = 0; i <= numberOfPointsPerPlane; i++)
            {
                for (int j = 0; j <= numberOfPointsPerPlane; j++)
                {
                    if ((i == 0 || i == numberOfPointsPerPlane) || (j == 0 || j == numberOfPointsPerPlane))
                    {
                        for (int k = 0; k <= numberOfPointsPerPlane; k++)
                        {
                            
                            Vector3 v = new Vector3(startP * (-.5f + i / Convert.ToSingle(numberOfPointsPerPlane)), startP * (-0.5f + j / Convert.ToSingle(numberOfPointsPerPlane)), startP * (-0.5f + k / Convert.ToSingle(numberOfPointsPerPlane)));
                            pointsList.Add(v);

                        }
                    }

                    else
                    {
                        
                        Vector3 v = new Vector3(startP * (-.5f + i / Convert.ToSingle(numberOfPointsPerPlane)), startP * (-0.5f + j / Convert.ToSingle(numberOfPointsPerPlane)), startP * (-0.5f));
                        pointsList.Add(v);

                        
                        v = new Vector3(startP * (-.5f + i / Convert.ToSingle(numberOfPointsPerPlane)), startP * (-0.5f + j / Convert.ToSingle(numberOfPointsPerPlane)), startP * (0.5f));
                        pointsList.Add(v);
                    }
                }
            }
            points = pointsList;
            return points;
        }
        /// <summary>
        /// Generates a 3D Model for a cuboid, by setting all lines with points
        /// </summary>
        /// <param name="Name">Model name</param>
        /// <param name="u">Length of the lower part</param>
        /// <param name="v">Length of the high part</param>
        /// <param name="numberOfPoints">Number of points to use in circumference</param>
        /// <param name="Color">Color vector</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        /// <returns></returns>
        public static Model Cuboid_AllLines(string Name, double u, double v, int numberOfPoints, System.Drawing.Color color, System.Drawing.Bitmap Texture)
        {

            PointCloud points = new PointCloud();

            double u0 = 0f;
            double v0 = 0f;
            List<Vector3> pointsList = new List<Vector3>();
            int indeInModel = -1;
            for (int i = 0; i < numberOfPoints; i++)
            {
                pointsList.Add(new Vector3(u0, 0, 0));
                pointsList.Add(new Vector3(0, 0, u0));
                pointsList.Add(new Vector3(u0, 0, u));
                pointsList.Add(new Vector3(u, 0, u0));
                pointsList.Add(new Vector3(0, v0, 0));
                pointsList.Add(new Vector3(0, v0, u));
                pointsList.Add(new Vector3(u, v0, u));
                pointsList.Add(new Vector3(u, v0, 0));
                pointsList.Add(new Vector3(u0, v, 0));
                pointsList.Add(new Vector3(0, v, u0));
                pointsList.Add(new Vector3(u0, v, u));
                pointsList.Add(new Vector3(u, v, u0));

                u0 += u / 100;
                v0 += v / 100;


            }

            
            Model myModel = new Model();
            myModel.PointCloud.Vectors = pointsList.ToArray();

            return myModel;

        }
        /// <summary>Generates a 3D Model for a cylinder.</summary>
        /// <param name="Name">Model name.</param>
        /// <param name="Radius">Cylinder radius.</param>
        /// <param name="Height">Cylinder height.</param>
        /// <param name="numPoints">Number of points for circular section.</param>
        /// <param name="Color">Color vector.</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        public static PointCloud Tile(double xMax, double yMax, double zMax, int pointsMaxX, int pointsMaxY, int pointsMaxZ)
        {
            double stepX = xMax / pointsMaxX;
            double stepY = yMax / pointsMaxY;
            double stepZ = zMax / pointsMaxZ;

            PointCloud pCloud = new PointCloud();
            int indexInModel = -1;
            List<Vector3> pointsList = new List<Vector3>();
            for (int i = 0; i <= pointsMaxX; i++)
            {
                for (int j = 0; j <= pointsMaxY; j++)
                {
                    for (int k = 0; k <= pointsMaxZ; k++)
                    {
                        indexInModel++;
                        Vector3 v = new Vector3(i * stepX, j * stepY, k * stepZ);
                        pointsList.Add(v);
                    }
                }

            }
            pCloud.Vectors = pointsList.ToArray();
            return pCloud;
        }
        /// <summary>Generates a 3D Model for a cylinder.</summary>
        /// <param name="Name">Model name.</param>
        /// <param name="Radius">Cylinder radius.</param>
        /// <param name="Height">Cylinder height.</param>
        /// <param name="numPoints">Number of points for circular section.</param>
        /// <param name="Color">Color vector.</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        public static PointCloud TileEmpty(double xMax, double yMax, double zMax, int pointsMaxX, int pointsMaxY, int pointsMaxZ)
        {
            double stepX = xMax / pointsMaxX;
            double stepY = yMax / pointsMaxY;
            double stepZ = zMax / pointsMaxZ;

            PointCloud pCloud = new PointCloud();
            List<Vector3> pointsList = new List<Vector3>();
            int indexInModel = -1;
            for (int i = 0; i <= pointsMaxX; i++)
            {
                for (int j = 0; j <= pointsMaxY; j++)
                {

                    if ((i == 0 || i == pointsMaxX) || (j == 0 || j == pointsMaxY))
                    {
                        for (int k = 0; k <= pointsMaxZ; k++)
                        {
                            indexInModel++;
                            Vector3 v = new Vector3(i * stepX, j * stepY, k * stepZ);
                            pointsList.Add(v);

                            //indexInModel++;
                            //Vertex v = new Vector3(indexInModel, startP * (-.5 + i / Convert.ToSingle(numberOfPointsPerPlane)), startP * (-0.5f + j / Convert.ToSingle(numberOfPointsPerPlane)), startP * (-0.5f + k / Convert.ToSingle(numberOfPointsPerPlane)));
                            //pointsList.Add(v);

                        }
                    }

                    else
                    {
                        
                        Vector3 v = new Vector3( i * stepX, j * stepY, 0);
                        pointsList.Add(v);

                        
                        v = new Vector3( i * stepX, j * stepY, zMax);
                        pointsList.Add(v);

                      
                    }



                }

            }
            pCloud.Vectors = pointsList.ToArray();
            return pCloud;
        }

        /// <summary>Generates a 3D Model for a cylinder.</summary>
        /// <param name="Name">Model name.</param>
        /// <param name="Radius">Cylinder radius.</param>
        /// <param name="Height">Cylinder height.</param>
        /// <param name="numPoints">Number of points for circular section.</param>
        /// <param name="Color">Color vector.</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        public static PointCloud Rectangle(double xMax, double yMax, int pointsMaxX, int pointsMaxY)
        {
            double stepX = xMax / pointsMaxX;
            double stepY = yMax / pointsMaxY;

            PointCloud pCloud = new PointCloud();
            List<Vector3> pointsList = new List<Vector3>();
            int indexInModel = -1;
            for (int i = 0; i < pointsMaxX; i++)
            {
                for (int j = 0; j < pointsMaxY; j++)
                {
                    indexInModel++;
                    Vector3 v = new Vector3(i * stepX, j * stepY, 0);
                    pointsList.Add(v);

                }

            }

            pCloud.Vectors = pointsList.ToArray();
            return pCloud;
        }
    }
}
