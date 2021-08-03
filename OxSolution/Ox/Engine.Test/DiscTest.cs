using Microsoft.Xna.Framework;
using NUnit.Framework;
using Ox.Engine.MathNamespace;

namespace Ox.Engine.Test
{
    [TestFixture]
    public class DiscTest
    {
        [Test]
        public void RadiusTest()
        {
            Vector3 center = new Vector3(10);
            Disc disc = new Disc(center, center + Vector3.Up, center + Vector3.Right);
            Assert.AreEqual(1, disc.Radius);
        }

        [Test]
        public void DiameterTest()
        {
            Vector3 center = new Vector3(10);
            Disc disc = new Disc(center, center + Vector3.Up, center + Vector3.Right);
            Assert.AreEqual(2, disc.Diameter);
        }

        [Test]
        public void NormalTest()
        {
            Vector3 center = new Vector3(10);
            Disc disc = new Disc(center, center + Vector3.Up, center + Vector3.Right);
            Assert.AreEqual(Vector3.Forward, disc.Normal);
        }
    }
}
