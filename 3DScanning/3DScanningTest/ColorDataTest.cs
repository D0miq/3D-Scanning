namespace _3DScanningTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Kinect;
    using _3DScanning;
    using System.Linq;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class ColorDataTest
    {
        /// <summary>
        /// 
        /// </summary>
        private ColorData colorData;

        /// <summary>
        /// 
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            colorData = new ColorData();    
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestAddFrame()
        {
            MultiSourceFrame multiSourceFrame = Isolate.Fake.Instance<MultiSourceFrame>();
            colorData.AddFrame(multiSourceFrame);
            Assert.AreEqual(1, colorData.FramesCount);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestAddFrameNullFrame()
        {
            byte[] colorDataArray = { };
            MultiSourceFrame multiSourceFrame = Isolate.Fake.Instance<MultiSourceFrame>();
            Isolate.WhenCalled(() => multiSourceFrame.ColorFrameReference.AcquireFrame()).WillReturn(null);
            colorData.AddFrame(multiSourceFrame);
            Assert.AreEqual(0, colorData.FramesCount);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestAddFrameMultipleFrames()
        {
            byte[] oldColorDataArray = { 4, 6, 8 };
            Isolate.NonPublic.InstanceField(colorData, "colorData").Value = oldColorDataArray;
            Isolate.NonPublic.InstanceField(colorData, "framesCounter").Value = 1;
            MultiSourceFrame multiSourceFrame = Isolate.Fake.Instance<MultiSourceFrame>();

            colorData.AddFrame(multiSourceFrame);
            Assert.AreEqual(2, colorData.FramesCount);
            Assert.AreEqual(2, colorData.Data[0]);
            Assert.AreEqual(3, colorData.Data[1]);
            Assert.AreEqual(4, colorData.Data[2]);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestClearData()
        {
            byte[] oldColorDataArray = { 3, 2, 1 };
            Isolate.NonPublic.InstanceField(colorData, "colorData").Value = oldColorDataArray;
            Isolate.NonPublic.InstanceField(colorData, "framesCounter").Value = 1;
            colorData.ClearData();
            Assert.AreEqual(0, colorData.FramesCount);
            Assert.IsFalse(oldColorDataArray.SequenceEqual(colorData.Data));
        }
    }
}
