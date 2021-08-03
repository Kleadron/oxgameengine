using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.Test
{
    [TestFixture]
    public class IQueriableCollectionTest
    {
        public class Factory
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new FastQueriableCollection<object>();
                    yield return new QueriableCollection<object>();
                }
            }
        }

        [SetUp]
        public void SetUp()
        {
            objects = new List<object>();
            strings = new List<string>();
        }

        [Test, TestCaseSource(typeof(Factory), "TestCases")]
        public void Add_Remove_TestContent(IQueriableCollection<object> collection)
        {
            for (int i = 0; i < 5; ++i) collection.Add(new object());
            collection.Collect<object>(objects);
            Assert.AreEqual(5, objects.Count, "Error adding objects.");
            foreach (object obj in objects) collection.Remove(obj);
            objects.Clear();
            collection.Collect<object>(objects);
            Assert.AreEqual(0, objects.Count, "Error removing objects.");
        }

        [Test, TestCaseSource(typeof(Factory), "TestCases")]
        public void Add_Clear_TestContent(IQueriableCollection<object> collection)
        {
            for (int i = 0; i < 5; ++i) collection.Add(new object());
            collection.Clear();
            collection.Collect<object>(objects);
            Assert.AreEqual(0, objects.Count, "Error on adding then clearing objects.");
        }

        [Test, TestCaseSource(typeof(Factory), "TestCases")]
        public void Add_TestContent(IQueriableCollection<object> collection)
        {
            for (int i = 0; i < 5; ++i) collection.Add(new object());
            for (int i = 0; i < 5; ++i) collection.Add(i.ToString());
            collection.Collect<object>(objects);
            collection.Collect<string>(strings);
            Assert.AreEqual(10, objects.Count, "Not 10 objects collected!");
            Assert.AreEqual(5, strings.Count, "Not 5 strings collected!");
        }

        [Test, TestCaseSource(typeof(Factory), "TestCases")]
        public void Add_TestFilter(IQueriableCollection<object> collection)
        {
            for (int i = 0; i < 5; ++i) collection.Add(i.ToString());
            collection.Collect<string>(item => int.Parse(item) % 2 == 0, strings);
            Console.WriteLine(strings.Count.ToString());
            Assert.AreEqual(3, strings.Count);
        }

        private IList<object> objects;
        private IList<string> strings;
    }
}
