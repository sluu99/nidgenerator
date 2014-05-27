using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NidGenerator.Tests
{
    [TestClass]
    public class NidTest
    {
        [TestMethod]
        public void NextIdTest_uniqueness()
        {
            Dictionary<long, bool> ids = new Dictionary<long, bool>(100000);
            

            for (uint instanceId = 0; instanceId < 1024; instanceId++)
            {
                Nid nid = new Nid(instanceId, DateTime.UtcNow);
                ids.Clear();               

                for (int i = 0; i < 100000; i++)
                {
                    long id = nid.NextId();
                    if (ids.ContainsKey(id))
                    {
                        Assert.Fail(string.Format("ID collision happened after {0} IDs (value = {1})", ids.Count, id));
                    }

                    ids[id] = true;
                }
            }
        }

        [TestMethod]
        public void NextIdTest_uniqueness_multithread()
        {
            Nid nid = new Nid(0, DateTime.UtcNow);
            
            Dictionary<long, bool> ids1 = new Dictionary<long, bool>(10000);
            Dictionary<long, bool> ids2 = new Dictionary<long, bool>(10000);

            var thread1 = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    long id = nid.NextId();
                    if (ids1.ContainsKey(id))
                    {
                        Assert.Fail(string.Format("ID collision happened (value = {0})", id));
                    }

                    ids1[id] = true;
                }
            }));

            var thread2 = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    long id = nid.NextId();
                    if (ids2.ContainsKey(id))
                    {
                        Assert.Fail(string.Format("ID collision happened (value = {0})", id));
                    }

                    ids2[id] = true;
                }
            }));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            var intersection = ids1.Keys.Intersect(ids2.Keys);
            Assert.AreEqual(0, intersection.Count());
        }

        [TestMethod]
        public void NextIdTest_uniqueness_multithread_different_instances()
        {
            Nid nid0 = new Nid(0, DateTime.UtcNow);
            Nid nid1 = new Nid(1, DateTime.UtcNow);

            Dictionary<long, bool> ids1 = new Dictionary<long, bool>(10000);
            Dictionary<long, bool> ids2 = new Dictionary<long, bool>(10000);

            var thread1 = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    long id = nid0.NextId();
                    if (ids1.ContainsKey(id))
                    {
                        Assert.Fail(string.Format("ID collision happened (value = {0})", id));
                    }

                    ids1[id] = true;
                }
            }));

            var thread2 = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    long id = nid1.NextId();
                    if (ids2.ContainsKey(id))
                    {
                        Assert.Fail(string.Format("ID collision happened (value = {0})", id));
                    }

                    ids2[id] = true;
                }
            }));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            var intersection = ids1.Keys.Intersect(ids2.Keys);
            Assert.AreEqual(0, intersection.Count());
        }
    }
}
