namespace _3DScanningTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using _3DScanning;
    using Microsoft.Kinect;
    using System.Drawing;

    /// <summary>
    /// Tests the <see cref="Utility"/> class.
    /// </summary>
    [TestClass]
    public class UtilityTest
    {
        /// <summary>
        /// Tests the <see cref="Utility.GetColorFromDepth(ushort[])"/>.
        /// </summary>
        [TestMethod]
        public void TestGetColorFromDepth()
        {
            ushort[] depthData = { 0, 2000, 4000, 6000, 8000, 10000 };
            byte[] colors = Utility.GetColorFromDepth(depthData);
            Assert.IsTrue(colors.SequenceEqual(new byte[] { 0, 63, 127, 191, 255, 0 }));
        }

        /// <summary>
        /// Tests the <see cref="Utility.MapColorToDepth(byte[], ColorSpacePoint[], int, int)"/>.
        /// </summary>
        [TestMethod]
        public void TestMapColorToDepth()
        {
            byte[] colorData = { 255, 0, 0, 255, 0, 255, 0, 255, 0, 0, 255, 255, 255, 255, 0, 255 };
            ColorSpacePoint spacePoint1 = new ColorSpacePoint
            {
                X = 0,
                Y = 0
            };

            ColorSpacePoint spacePoint2 = new ColorSpacePoint
            {
                X = 1,
                Y = 1
            };

            ColorSpacePoint[] spacePoints = { spacePoint1, spacePoint2 };
            byte[] mappedColors = Utility.MapColorToDepth(colorData, spacePoints, 2, 2);
            Assert.IsTrue(mappedColors.SequenceEqual(new byte[] { 255, 0, 0, 255, 255, 255, 0, 255 }));
        }

        /// <summary>
        /// Tests the <see cref="Utility.GetColorsFromRGBA(byte[])"/>.
        /// </summary>
        [TestMethod]
        public void TestGetColorsFromRGBA()
        {
            byte[] colorData = { 255, 0, 0, 255, 0, 255, 0, 255 };

            Color[] colors = Utility.GetColorsFromRGBA(colorData);

            Assert.AreEqual(colors[0].R, colorData[0]);
            Assert.AreEqual(colors[0].G, colorData[1]);
            Assert.AreEqual(colors[0].B, colorData[2]);
            Assert.AreEqual(colors[0].A, colorData[3]);
            Assert.AreEqual(colors[1].R, colorData[4]);
            Assert.AreEqual(colors[1].G, colorData[5]);
            Assert.AreEqual(colors[1].B, colorData[6]);
            Assert.AreEqual(colors[1].A, colorData[7]);
        }

        /// <summary>
        /// Tests the <see cref="Utility.GetScaledColor(float, float, float))"/>.
        /// </summary>
        [TestMethod]
        public void TestGetScaledColor()
        {
            Color color = Utility.GetScaledColor(0.5f, 0f, 1f);
            Assert.AreEqual(Color.FromArgb(127,127,0), color);
        }

        /// <summary>
        /// Tests the <see cref="Utility.GetScaledColor(float, float, float))"/>.
        /// </summary>
        [TestMethod]
        public void TestGetScaledColor2()
        {
            Color color = Utility.GetScaledColor(0.2f, 0f, 1f);
            Assert.AreEqual(Color.FromArgb(51, 204, 0), color);
        }
    }
}
