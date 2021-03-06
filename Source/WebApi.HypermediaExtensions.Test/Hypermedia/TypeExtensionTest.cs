﻿using System;
using WebApi.HypermediaExtensions.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApi.HypermediaExtensions.Test.Hypermedia
{
    [TestClass]
    public class TypeExtensionTest
    {
        [TestMethod]
        public void GetGenericTypeName()
        {
            var typeName = typeof(Nullable<int>).BeautifulName();
            Assert.AreEqual("Nullable<Int32>", typeName);
        }

        [TestMethod]
        public void GetNestedTypeName()
        {
            var typeName = typeof(Outer.Inner).BeautifulName();
            Assert.AreEqual($"{nameof(TypeExtensionTest)}.{nameof(Outer)}.{nameof(Outer.Inner)}", typeName);
        }

        class Outer
        {
            public class Inner
            {
            }
        }
    }
}