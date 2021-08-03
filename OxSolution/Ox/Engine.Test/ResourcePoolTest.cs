using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.Test
{
    [TestFixture]
    public class ResourcePoolTest
    {
        [SetUp]
        public void SetUp()
        {
            pool = new ResourcePool<string>(4, delegate { return Guid.NewGuid().ToString(); }, delegate { });
        }

        [TearDown]
        public void TearDown()
        {
            pool.Dispose();
        }

        [Test]
        public void TestActiveCount()
        {
            Assert.AreEqual(0, pool.ActiveCount);
        }

        [Test]
        public void TestSize()
        {
            Assert.AreEqual(4, pool.Size);
        }

        [Test]
        public void TestCollectActiveItems()
        {
            IList<string> items = new List<string>();
            pool.CollectActiveItems(items);
            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public void Allocate_TestActiveCount()
        {
            pool.Allocate();
            pool.Allocate();
            Assert.AreEqual(2, pool.ActiveCount);
        }

        [Test]
        public void Allocate_TestActiveItems()
        {
            pool.Allocate();
            pool.Allocate();
            IList<string> items = new List<string>();
            pool.CollectActiveItems(items);
            Assert.AreEqual(2, items.Count);
        }

        [Test]
        public void Allocate_TestAllDifferent()
        {
            pool.Allocate();
            pool.Allocate();
            pool.Allocate();
            pool.Allocate();
            IList<string> items = new List<string>();
            pool.CollectActiveItems(items);
            for (int i = 0; i < items.Count; ++i)
                for (int j = i + 1; j < items.Count; ++j)
                    Assert.AreNotEqual(items[j], items[i]);
        }

        [Test]
        public void Allocate_Free_TestActiveCount()
        {
            string str = pool.Allocate();
            string str2 = pool.Allocate();
            pool.Free(str);
            Assert.AreEqual(1, pool.ActiveCount);
        }

        [Test]
        public void Allocate_Free_TestActiveItems()
        {
            string str = pool.Allocate();
            string str2 = pool.Allocate();
            pool.Free(str);
            IList<string> items = new List<string>();
            pool.CollectActiveItems(items);
            Assert.AreEqual(1, items.Count);
            Assert.AreNotEqual(str, items[0]);
        }

        [Test]
        public void Allocate_Clear_TestActiveCount()
        {
            pool.Allocate();
            pool.Allocate();
            pool.Clear();
            Assert.AreEqual(0, pool.ActiveCount);
        }

        [Test]
        public void Allocate_Clear_TestActiveItems()
        {
            pool.Allocate();
            pool.Allocate();
            pool.Clear();
            IList<string> items = new List<string>();
            pool.CollectActiveItems(items);
            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public void FreeSameObjectTwice_TestActiveItems()
        {
            string item = pool.Allocate();
            pool.Free(item);
            pool.Free(item);
            IList<string> items = new List<string>();
            pool.CollectActiveItems(items);
            Assert.AreEqual(0, items.Count);
        }

        private ResourcePool<string> pool;
    }
}
