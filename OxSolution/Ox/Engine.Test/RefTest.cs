using NUnit.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.Test
{
    [TestFixture]
    public class RefTest
    {
        [Test]
        public void Create_Test()
        {
            Ref<int> reference = new Ref<int>();
            int value;
            Assert.AreEqual(0, reference.Value);
            reference.Get(out value);
            Assert.AreEqual(0, value);
        }

        [Test]
        public void CreateWithValue_Test()
        {
            Ref<int> reference = new Ref<int>(10);
            int value;
            Assert.AreEqual(10, reference.Value);
            reference.Get(out value);
            Assert.AreEqual(10, value);
        }

        [Test]
        public void Create_Set_Test()
        {
            Ref<int> reference = new Ref<int>();
            int value;
            reference.Value = 10;
            Assert.AreEqual(10, reference.Value);
            reference.Get(out value);
            Assert.AreEqual(10, value);
        }

        [Test]
        public void Create_SetByRef_Test()
        {
            Ref<int> reference = new Ref<int>();
            int value = 10;
            reference.Set(ref value);
            Assert.AreEqual(10, reference.Value);
            reference.Get(out value);
            Assert.AreEqual(10, value);
        }
    }
}
