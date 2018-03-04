namespace _3DScanning
{
    using System;

    /// <summary>
    /// An instance of the <see cref="IMeshWriter"/> interface represents a writer.
    /// It should be able to write <paramref name="mesh"/> into a file.
    /// </summary>
    /// <seealso cref="ObjMeshWriter"/>
    public interface IMeshWriter
    {
        /// <summary>
        /// Writes the given <paramref name="mesh"/> into a file.
        /// </summary>
        /// <param name="mesh">The given mesh that is written into output stream.</param>
        /// <param name="path">Path of an output file.</param>
        /// <param name="append">True if data should be appended, false otherwise.</param>
        /// <param name="triangles">True if triangles should be written too, false otherwise.</param>
        /// <param name="description">Description of the mesh.</param>
        void WriteMesh(Mesh mesh, string path, bool append, bool triangles, string description);
    }
}
