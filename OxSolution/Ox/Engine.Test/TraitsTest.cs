using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.Test
{
    [TestFixture]
    public class TraitsTest
    {
        [SetUp]
        public void SetUp()
        {
            traits = new Traits();
        }

        [Test]
        public void TestContains()
        {
            Assert.IsFalse(traits.Contains<int>("ten"));
        }

        [Test]
        public void TestTryGet()
        {
            int value;
            Assert.IsFalse(traits.TryGet<int>("ten", out value));
        }

        [Test, ExpectedException(typeof(KeyNotFoundException))]
        public void TestGet()
        {
            traits.Get<int>("ten");
        }

        [Test]
        public void TestRemove()
        {
            Assert.IsFalse(traits.Remove("ten"));
        }

        [Test]
        public void Add_TestContains()
        {
            traits.Add<int>("ten");
            Assert.IsTrue(traits.Contains<int>("ten"));
        }

        [Test]
        public void Set_TestGet()
        {
            traits.Set<int>("ten", 10);
            Assert.AreEqual(10, traits.Get<int>("ten"));
        }

        [Test]
        public void Set_TestTryGet()
        {
            traits.Set<int>("ten", 10);

            int value;
            Assert.IsTrue(traits.TryGet<int>("ten", out value));
        }

        [Test]
        public void Set_TestTryGet2()
        {
            traits.Set<int>("ten", 10);
            Assert.AreEqual(10, traits.TryGet<int>("ten", 5));
        }

        [Test]
        public void Add_Remove_TestContains()
        {
            traits.Add<int>("ten");
            traits.Remove("ten");
            Assert.IsFalse(traits.Contains<int>("ten"));
        }

        [Test, ExpectedException(typeof(KeyNotFoundException))]
        public void Add_Remove_TestGet()
        {
            traits.Add<int>("ten");
            traits.Remove("ten");
            traits.Get<int>("ten");
        }

        [Test]
        public void Set_TestWrongContains()
        {
            traits.Add<int>("ten");
            Assert.IsFalse(traits.Contains<string>("ten"));
        }

        [Test]
        public void Set_TestWrongTryGet()
        {
            traits.Set<int>("ten", 10);
            string value;
            Assert.IsFalse(traits.TryGet<string>("ten", out value));
        }

        [Test]
        public void Set_TestWrongTryGet2()
        {
            traits.Set<int>("ten", 10);
            Assert.AreEqual(5, traits.TryGet<int>("five", 5));
        }

        [Test, ExpectedException(typeof(InvalidCastException))]
        public void Set_TestWrongGet()
        {
            traits.Set<int>("ten", 10);
            traits.Get<string>("ten");
        }

        [Test]
        public void AddThree_TestCollect()
        {
            traits.Add<int>("ten");
            traits.Add<int>("twenty");
            traits.Add<int>("thirty");
            IList<Trait> traitList = new List<Trait>();
            traits.Collect(traitList);
            Assert.AreEqual(3, traitList.Count);
        }

        [Test]
        public void AddThree_Clear_TestCollect()
        {
            traits.Add<int>("ten");
            traits.Add<int>("twenty");
            traits.Add<int>("thirty");
            traits.Clear();
            IList<Trait> traitList = new List<Trait>();
            traits.Collect(traitList);
            Assert.AreEqual(0, traitList.Count);
        }

        private Traits traits;
    }
}
