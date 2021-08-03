using Microsoft.Xna.Framework;
using NUnit.Framework;
using Ox.Engine.MathNamespace;

namespace Ox.Engine.Test
{
    [TestFixture]
    public class BoundingBoxHelperTest
    {
        [TestCase(10, 10, false)]
        [TestCase(float.MinValue, 10, false)]
        [TestCase(10, float.MaxValue, false)]
        [TestCase(float.MaxValue, float.MinValue, false)]
        [TestCase(float.MinValue, float.MaxValue, true)]
        public void AllEncompassingTest(float min, float max, bool expectedResult)
        {
            BoundingBox box = new BoundingBox(new Vector3(min), new Vector3(max));
            Assert.AreEqual(expectedResult, BoundingBoxHelper.AllEncompassing(box));
        }

        [TestCase(-5, 5, 2, -10, 10)]
        [TestCase(-10, 10, 0.5f, -5, 5)]
        public void TransformTest(float min, float max, float scale, float expectedMin, float expectedMax)
        {
            BoundingBox box = new BoundingBox(new Vector3(min), new Vector3(max));
            BoundingBox expectedBox = new BoundingBox(new Vector3(expectedMin), new Vector3(expectedMax));
            BoundingBox resultBox;
            Matrix transform = Matrix.CreateScale(scale);
            BoundingBoxHelper.Transform(ref box, ref transform, out resultBox);
            Assert.AreEqual(expectedBox, resultBox);
        }

        [TestCase(new float[] { 10, 20, 5, -10, -20 }, -20, 20)]
        [TestCase(new float[] { 10, 20, 5, -10, 20 }, -10, 20)]
        [TestCase(new float[] { 10, -20, 5, -10, -20 }, -20, 10)]
        public void CreateFromPointsTest(float[] points, float expectedMin, float expectedMax)
        {
            BoundingBox expectedBox = new BoundingBox(new Vector3(expectedMin), new Vector3(expectedMax));
            BoundingBox resultBox;
            Vector3[] points3D =
            {
                new Vector3(points[0]),
                new Vector3(points[1]),
                new Vector3(points[2]),
                new Vector3(points[3]),
                new Vector3(points[4])
            };
            resultBox = BoundingBoxHelper.GenerateBoundingBox(points3D);
            Assert.AreEqual(expectedBox, resultBox);
        }
    }
}
