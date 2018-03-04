namespace _3DScanning
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using Microsoft.Kinect;

    /// <summary>
    /// An instance of the <see cref="ObjMeshWriter"/> represents a writer to obj format.
    /// </summary>
    /// <seealso cref="IMeshWriter"/>
    public class ObjMeshWriter : IMeshWriter
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Format of numbers in generated file.
        /// </summary>
        private NumberFormatInfo numberFormatInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjMeshWriter"/> class.
        /// </summary>
        public ObjMeshWriter()
        {
            this.numberFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = string.Empty,
                NumberDecimalDigits = 10
            };
        }

        /// <summary>
        /// Writes the given <paramref name="mesh"/> into a obj file.
        /// </summary>
        /// <param name="mesh">The given mesh that is written into output stream.</param>
        /// <param name="path">Path of an output file.</param>
        /// <param name="append">True if data should be appended, false otherwise.</param>
        /// <param name="triangles">True if triangles should be written too, false otherwise.</param>
        /// <param name="description">Description of the mesh.</param>
        public void WriteMesh(Mesh mesh, string path, bool append, bool triangles, string description)
        {
            Log.Debug("Path of a file: " + path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (StreamWriter streamWriter = new StreamWriter(path, append))
            {
                streamWriter.WriteLine(description);
                for (int i = 0; i < mesh.Vertexes.Length; i++)
                {
                    CameraSpacePoint point = mesh.Vertexes[i];
                    Color color = mesh.Colors[i];
                    streamWriter.WriteLine(
                        "v {0} {1} {2} {3} {4} {5} {6}",
                        point.X.ToString("N", this.numberFormatInfo),
                        point.Y.ToString("N", this.numberFormatInfo),
                        point.Z.ToString("N", this.numberFormatInfo),
                        (color.R / 255.0).ToString("N", this.numberFormatInfo),
                        (color.G / 255.0).ToString("N", this.numberFormatInfo),
                        (color.B / 255.0).ToString("N", this.numberFormatInfo),
                        mesh.Dispersions[i].ToString("N", this.numberFormatInfo));
                }

                if (triangles)
                {
                    for (int i = 0; i < mesh.Triangles.Count; i += 3)
                    {
                        streamWriter.WriteLine("f {0} {1} {2}", mesh.Triangles[i], mesh.Triangles[i + 1], mesh.Triangles[i + 2]);
                    }
                }
            }
        }
    }
}
