﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass()]
    public class NuGetComponentTest
    {
        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest1()
        {
            var target = new NuGetComponent();
            target.parse("[NuGet NotRealSubtype.check]");
        }

        /// <summary>
        ///A test for parse - gnt
        ///</summary>
        [TestMethod()]
        public void gntTest1()
        {
            var target = new NuGetComponent();

            try {
                target.parse("[NuGet gnt]");
                Assert.Fail("1");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[NuGet gnt()]");
                Assert.Fail("2");
            }
            catch(IncorrectNodeException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[NuGet gnt.NotRealNode]");
                Assert.Fail("3");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for parse - gnt.raw
        ///</summary>
        [TestMethod()]
        public void gntRawTest1()
        {
            var target = new NuGetComponent();

            try {
                target.parse("[NuGet gnt.raw()]");
                Assert.Fail("1");
            }
            catch(InvalidArgumentException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[NuGet gnt.raw(\"the is not a correct command\")]");
                Assert.Fail("2");
            }
            catch(System.Exception) {
                Assert.IsTrue(true); // should be any exception from gnt.core as normal behavior
            }
        }
    }
}