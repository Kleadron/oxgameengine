using Microsoft.Xna.Framework;
using NUnit.Framework;
using Ox.Engine.MathNamespace;

namespace Ox.Engine.Test
{
    [TestFixture]
    public class HeightMapTest
    {
        [SetUp]
        public void SetUp()
        {
            float[,] points =
            {
                { -2, +1, +3 },
                { +4, +2, -2 },
                { +3, +0, +1 }
            };
            heightMap = new HeightMap(points, new Vector3(2), Vector2.Zero);
        }

        [Test]
        public void GrabTriangleTest()
        {
            Triangle triangle;
            heightMap.GrabTriangle(new Vector2(3, 3.5f), out triangle);
            Assert.AreEqual(new Vector3(2, 4, 2), triangle.A);
            Assert.AreEqual(new Vector3(4, 2, 4), triangle.B);
            Assert.AreEqual(new Vector3(2, -4, 4), triangle.C);
        }

        [Test]
        public void GrabTriangleTest2()
        {
            Triangle triangle;
            heightMap.GrabTriangle(new Vector2(3.5f, 3), out triangle);
            Assert.AreEqual(new Vector3(2, 4, 2), triangle.A);
            Assert.AreEqual(new Vector3(4, 0, 2), triangle.B);
            Assert.AreEqual(new Vector3(4, 2, 4), triangle.C);
        }

        private HeightMap heightMap;
    }
}
