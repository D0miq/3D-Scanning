/******************************************************************************
 *
 *    MIConvexHull, Copyright (C) 2013 David Sehnal, Matthew Campbell
 *
 *  This library is free software; you can redistribute it and/or modify it 
 *  under the terms of  the GNU Lesser General Public License as published by 
 *  the Free Software Foundation; either version 2.1 of the License, or 
 *  (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful, 
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of 
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser 
 *  General Public License for more details.
 *  
 *****************************************************************************/
using OpenTKLib;
namespace MIConvexHull
{
    using System.Collections.Generic;
    using System.Linq;
    using System;

    /// <summary>
    /// Calculation and representation of Delaunay triangulation.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TCell"></typeparam>
    public class DelaunayTriangulation<TVertex, TCell> : ITriangulation<TVertex, TCell>
        where TCell : TriangulationCell<TVertex, TCell>, new()
        where TVertex : IVector
    {
        /// <summary>
        /// Cells of the triangulation.
        /// </summary>
        public IEnumerable<TCell> Cells { get; private set; }

        /// <summary>
        /// Creates the Delaunay triangulation of the input data.
        /// Be careful with concurrency, because during the computation, the vertex position arrays get resized.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DelaunayTriangulation<TVertex, TCell> Create(IEnumerable<TVertex> data)
        {
            if (data == null) throw new ArgumentException("data can't be null.");
            if (!(data is IList<TVertex>)) data = data.ToArray();
            if (data.Count() == 0) return new DelaunayTriangulation<TVertex, TCell> { Cells = Enumerable.Empty<TCell>() };

            int dimension = data.First().PositionArray.Length;

            // Resize the arrays and lift the data.
            foreach (var p in data)
            {
                double lenSq = MathHelper.LengthSquared(p.PositionArray);
                var v = p.PositionArray;
                Array.Resize(ref v, dimension + 1);
                p.PositionArray = v;
                p[dimension] = lenSq;
            }

            // Find the convex hull
            var delaunayFaces = ConvexHullInternal.GetConvexFacesInternal<TVertex, TCell>(data);

            // Resize the data back
            foreach (var p in data)
            {
                var v = p.PositionArray;
                Array.Resize(ref v, dimension);
                p.PositionArray = v;
            }
            // Remove the "upper" faces
            for (var i = delaunayFaces.Count - 1; i >= 0; i--)
            {
                var candidate = delaunayFaces[i];
                if (candidate.Normal[dimension] >= 0)
                {
                    for (int fi = 0; fi < candidate.AdjacentFaces.Length; fi++)
                    {
                        var f = candidate.AdjacentFaces[fi];
                        if (f != null)
                        {
                            for (int j = 0; j < f.AdjacentFaces.Length; j++)
                            {
                                if (object.ReferenceEquals(f.AdjacentFaces[j], candidate))
                                {
                                    f.AdjacentFaces[j] = null;
                                }
                            }
                        }
                    }
                    var li = delaunayFaces.Count - 1;
                    delaunayFaces[i] = delaunayFaces[li];
                    delaunayFaces.RemoveAt(li);
                }
            }

            // Create the "TCell" representation.
            int cellCount = delaunayFaces.Count;
            var cells = new TCell[cellCount];

            for (int i = 0; i < cellCount; i++)
            {
                var face = delaunayFaces[i];
                var pointCloud = new TVertex[dimension + 1];
                for (int j = 0; j <= dimension; j++) pointCloud[j] = (TVertex)face.Vertices[j].Vertex;
                cells[i] = new TCell
                {
                    Vertices = pointCloud,
                    Adjacency = new TCell[dimension + 1]
                };
                face.Tag = i;
            }

            for (int i = 0; i < cellCount; i++)
            {
                var face = delaunayFaces[i];
                var cell = cells[i];
                for (int j = 0; j <= dimension; j++)
                {
                    if (face.AdjacentFaces[j] == null) continue;
                    cell.Adjacency[j] = cells[face.AdjacentFaces[j].Tag];
                }
            }

            return new DelaunayTriangulation<TVertex, TCell> { Cells = cells };
        }

        /// <summary>
        /// Can only be created using a factory method.
        /// </summary>
        private DelaunayTriangulation()
        {

        }
    }
}
