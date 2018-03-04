using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _3DScanning;
using TypeMock.ArrangeActAssert;
using Microsoft.Kinect;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

namespace _3DScanningTest
{
    /// <summary>
    /// Tests the <see cref="ObjMeshWriter"/> class.
    /// </summary>
    [TestClass]
    public class ObjMeshWriterTest
    {
        /// <summary>
        /// The name of a file.
        /// </summary>
        private const string FILE_PATH = "3DScans\\testFile.obj";

        private IMeshWriter meshWriter;

        private Mesh mesh;

        /// <summary>
        /// 
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.meshWriter = new ObjMeshWriter();
            this.mesh = Isolate.Fake.Instance<Mesh>();
            CameraSpacePoint[] vertexes = { new CameraSpacePoint { X = 1, Y = 1, Z = 1 }, new CameraSpacePoint { X = 2, Y = 2, Z = 1 }, new CameraSpacePoint { X = 1, Y = 2, Z = 1 } };
            Color[] colors = { Color.Red, Color.Lime, Color.Blue };
            List<int> triangles = new List<int>
            {
                1,
                2,
                3
            };
            float[] dispersions = { 1.1f, 2.2f, 3.3f };
            Isolate.WhenCalled(() => mesh.Vertexes).WillReturn(vertexes);
            Isolate.WhenCalled(() => mesh.Colors).WillReturn(colors);
            Isolate.WhenCalled(() => mesh.Triangles).WillReturn(triangles);
            Isolate.WhenCalled(() => mesh.Dispersions).WillReturn(dispersions);
        }

        /// <summary>
        /// Tests the <see cref="ObjMeshWriter.WriteMesh(Mesh, string)"/>. It tests file existance and its content.
        /// </summary>
        [TestMethod]
        public void TestWriteMeshWithTriangles()
        {
            this.meshWriter.WriteMesh(this.mesh, FILE_PATH, false, true, "Testovaci mesh");
            Assert.IsTrue(File.Exists(FILE_PATH));

            StreamReader streamReader = new StreamReader(FILE_PATH);
            Assert.AreEqual("Testovaci mesh", streamReader.ReadLine());
            Assert.AreEqual("v 1.0000000000 1.0000000000 1.0000000000 1.0000000000 0.0000000000 0.0000000000 1.1000000000", streamReader.ReadLine());
            Assert.AreEqual("v 2.0000000000 2.0000000000 1.0000000000 0.0000000000 1.0000000000 0.0000000000 2.2000000000", streamReader.ReadLine());
            Assert.AreEqual("v 1.0000000000 2.0000000000 1.0000000000 0.0000000000 0.0000000000 1.0000000000 3.3000000000", streamReader.ReadLine());
            Assert.AreEqual("f 1 2 3", streamReader.ReadLine());
            streamReader.Close();
        }

        /// <summary>
        /// Tests the <see cref="ObjMeshWriter.WriteMesh(Mesh, string)"/>. It tests file existance and its content.
        /// </summary>
        [TestMethod]
        public void TestWriteMeshWithoutTriangles()
        {
            float[] dispersions = { 1.1f, 2.2f, 3.3f };

            Isolate.WhenCalled(() => mesh.Dispersions).WillReturn(dispersions);
            this.meshWriter.WriteMesh(this.mesh, FILE_PATH, false, false, "Testovaci mesh");
            Assert.IsTrue(File.Exists(FILE_PATH));

            StreamReader streamReader = new StreamReader(FILE_PATH);
            Assert.AreEqual("Testovaci mesh", streamReader.ReadLine());
            Assert.AreEqual("v 1.0000000000 1.0000000000 1.0000000000 1.0000000000 0.0000000000 0.0000000000 1.1000000000", streamReader.ReadLine());
            Assert.AreEqual("v 2.0000000000 2.0000000000 1.0000000000 0.0000000000 1.0000000000 0.0000000000 2.2000000000", streamReader.ReadLine());
            Assert.AreEqual("v 1.0000000000 2.0000000000 1.0000000000 0.0000000000 0.0000000000 1.0000000000 3.3000000000", streamReader.ReadLine());
            streamReader.Close();
        }
    }
}