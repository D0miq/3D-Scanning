namespace _3DScanningTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using _3DScanning;
    using Microsoft.Kinect;
    using TypeMock.ArrangeActAssert;
    using System.Linq;

    [TestClass]
    public class DepthDataTest
    {
        /// <summary>
        /// 
        /// </summary>
        private DepthData depthData;

        /// <summary>
        /// 
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            depthData = new DepthData();
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestSetLastFrame()
        {
            MultiSourceFrame multiSourceFrame = Isolate.Fake.Instance<MultiSourceFrame>();
            depthData.SetLastFrame(multiSourceFrame);
            Assert.IsNotNull(depthData.LastFrame);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestSetLastFrameNullFrame()
        {
            MultiSourceFrame multiSourceFrame = Isolate.Fake.Instance<MultiSourceFrame>();
            Isolate.WhenCalled(() => multiSourceFrame.DepthFrameReference.AcquireFrame()).WillReturn(null);
            depthData.SetLastFrame(multiSourceFrame);
            Assert.IsNull(depthData.LastFrame);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestClearData()
        {
            ushort[] depthDataArray = { 3, 2, 1 };
            float[] dispersionData = { 3.3f, 2.2f, 1.1f };
            Isolate.NonPublic.InstanceField(depthData, "depthData").Value = depthDataArray;
            Isolate.NonPublic.InstanceField(depthData, "depthDataDispersion").Value = dispersionData;
            Isolate.NonPublic.InstanceField(depthData, "framesCounter").Value = 1;
            depthData.ClearData();
            Assert.AreEqual(0, depthData.FramesCount);
            Assert.IsFalse(depthDataArray.SequenceEqual(depthData.Data));
            Assert.IsFalse(dispersionData.SequenceEqual(depthData.Dispersion));
        }

        [TestMethod]
        public void TestReduceRange()
        {
            ushort[] depthDataArray = { 4, 3, 2, 1 };
            KinectAttributes kinectAttributes = Isolate.Fake.Instance<KinectAttributes>();
            Isolate.WhenCalled(() => kinectAttributes.MaxDepth).WillReturn(3);
            Isolate.WhenCalled(() => kinectAttributes.MinDepth).WillReturn(2);
            Isolate.NonPublic.InstanceField(depthData, "kinectAttributes").Value = kinectAttributes;
            Isolate.NonPublic.InstanceField(depthData, "depthData").Value = depthDataArray;
            depthData.ReduceRange();
            Assert.IsTrue(depthData.Data.SequenceEqual(new ushort[] { 0, 3, 2, 0 }));
        }

        [TestMethod]
        public void TestFinalizeDispersion()
        {
            depthData.FinalizeDispersion();
        }

        [TestMethod]
        public void TestAddLastFrameNullLastFrame()
        {
            depthData.AddLastFrame();
            Assert.AreEqual(0, depthData.FramesCount);
        }

        [TestMethod]
        public void TestAddLastFrameZeroDepth()
        {
            ushort[] lastdepthArray = { 1, 2, 3 };

            Isolate.NonPublic.InstanceField(depthData, "lastDepthData").Value = lastdepthArray;
            depthData.AddLastFrame();
            Assert.AreEqual(lastdepthArray[0],depthData.Data[0]);
            Assert.AreEqual(lastdepthArray[1], depthData.Data[1]);
            Assert.AreEqual(lastdepthArray[2], depthData.Data[2]);

            Assert.AreEqual(1.0f, depthData.Dispersion[0]);
            Assert.AreEqual(4.0f, depthData.Dispersion[1]);
            Assert.AreEqual(9.0f, depthData.Dispersion[2]);
        }

        [TestMethod]
        public void TestAddLastFrameNonZeroDepth()
        {
            ushort[] lastdepthArray = { 1, 2, 3 };
            ushort[] depthDataArray = { 3, 4, 5 };
            
            Isolate.NonPublic.InstanceField(depthData, "lastDepthData").Value = lastdepthArray;
            Isolate.NonPublic.InstanceField(depthData, "depthData").Value = depthDataArray;
            depthData.AddLastFrame();

            Assert.IsTrue(depthData.Data.SequenceEqual(new ushort[] { 2, 3, 4 }));
        }

    }
}