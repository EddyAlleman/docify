﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using NUnit.Framework;
using System;
using System.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Core;

namespace Core.Tests
{
    public class LocationTest
    {
        [Test]
        public void FromPathTest() 
        {
            var r1 = Location.FromPath(@"index.md", @"C:\MySite");
            var r2 = Location.FromPath(@"C:\MySite\page1\index.md", @"C:\MySite");
            var r3 = Location.FromPath(@"page2\index.md", @"C:\MySite");
            var r4 = Location.FromPath(@"\page3\subpage3\index.md", @"C:\MySite");

            Assert.AreEqual("index.md", r1.FileName);
            Assert.AreEqual("index.md", r2.FileName);
            Assert.AreEqual("index.md", r3.FileName);
            Assert.AreEqual("index.md", r4.FileName);
            Assert.AreEqual("index.md", r1.ToId());
            Assert.AreEqual("page1::index.md", r2.ToId());
            Assert.AreEqual("page2::index.md", r3.ToId());
            Assert.AreEqual("::page3::subpage3::index.md", r4.ToId());
        }

        [Test]
        public void FromPathDirTest() 
        {
            var r1 = Location.FromPath(@"C:\dir1\dir2", @"C:\");
            var r2 = Location.FromPath(@"dir1\dir2");

            Assert.IsEmpty(r1.FileName);
            Assert.IsEmpty(r2.FileName);

            Assert.AreEqual("dir1::dir2", r1.ToId());
            Assert.AreEqual("dir1::dir2", r2.ToId());
        }

        [Test]
        public void ToUrlTest() 
        {
            var u1 = new Location("page.html", "dir1", "dir2");
            var u2 = new Location("page.html");
            var u3 = new Location("index.html", "dir1", "dir2");
            var u4 = new Location("index.html");
            var u5 = new Location("file1.txt", "http:/", "www.example.com");
            var u6 = new Location("file1.txt", "https:/", "www.example.com");

            var r1 = u1.ToUrl();
            var r2 = u2.ToUrl();
            var r3 = u3.ToUrl();
            var r4 = u4.ToUrl();
            var r5 = u1.ToUrl("www.site.com");
            var r6 = u2.ToUrl("www.site.com");
            var r7 = u3.ToUrl("www.site.com");
            var r8 = u4.ToUrl("www.site.com");
            var r9 = u5.ToUrl();
            var r10 = u6.ToUrl();

            Assert.AreEqual("/dir1/dir2/page.html", r1);
            Assert.AreEqual("/page.html", r2);
            Assert.AreEqual("/dir1/dir2/", r3);
            Assert.AreEqual("/", r4);
            Assert.AreEqual("www.site.com/dir1/dir2/page.html", r5);
            Assert.AreEqual("www.site.com/page.html", r6);
            Assert.AreEqual("www.site.com/dir1/dir2/", r7);
            Assert.AreEqual("www.site.com", r8);
            Assert.AreEqual("http://www.example.com/file1.txt", r9);
            Assert.AreEqual("https://www.example.com/file1.txt", r10);
        }

        [Test]
        public void IsFileTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("", "dir1", "dir2");

            var r1 = l1.IsFile();
            var r2 = l2.IsFile();

            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
        }

        [Test]
        public void GetParentTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("", "dir1", "dir2");

            var r1 = l1.GetParent();
            var r2 = l2.GetParent();
            var r3 = l1.GetParent(2);
            var r4 = l2.GetParent(2);

            Assert.AreEqual("dir1::dir2", r1.ToId());
            Assert.AreEqual("dir1", r2.ToId());
            Assert.AreEqual("dir1", r3.ToId());
            Assert.AreEqual("", r4.ToId());
        }

        [Test]
        public void IsInLocationTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("", "dir1", "dir2");
            var l3 = new Location("", "dir0", "dir1", "dir2");
            var l4 = new Location("", "dir0", "dir1", "dir2", "dir3");

            var r1 = l1.IsInLocation(l2);
            var r2 = l1.IsInLocation(l3);
            var r3 = l4.IsInLocation(l3);
            var r4 = l3.IsInLocation(l4);

            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
            Assert.IsTrue(r3);
            Assert.IsFalse(r4);

            Assert.Throws<Exception>(() => l2.IsInLocation(l1));
        }

        [Test]
        public void GetRelativeTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("", "dir1", "dir2");
            var l3 = new Location("", "dir0", "dir1", "dir2");
            var l4 = new Location("", "dir0", "dir1", "dir2", "dir3", "dir4");

            var r1 = l1.GetRelative(l2);
            var r2 = l4.GetRelative(l3);

            Assert.AreEqual("page.html", r1.ToId());
            Assert.AreEqual("dir3::dir4", r2.ToId());
            Assert.Throws<Exception>(() => l1.GetRelative(l3));
            Assert.Throws<Exception>(() => l3.GetRelative(l4));
        }

        [Test]
        public void IsSameTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("page.html", "Dir1", "Dir2");
            var l3 = new Location("", "Dir1", "Dir2");
            var l4 = new Location("", "dir1", "dir2");

            var r1 = l1.IsSame(l2);
            var r2 = l1.IsSame(l2, StringComparison.CurrentCulture);
            var r3 = l3.IsSame(l2);
            var r4 = l3.IsSame(l4);

            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
            Assert.IsFalse(r3);
            Assert.IsTrue(r4);
        }

        [Test]
        public void TestMatchPositive()
        {
            var r1 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "D:\\*" });
            var r2 = Location.FromPath("D:\\path1.txt").Matches(new string[] { ".dll", "*.txt" });
            var r3 = Location.FromPath("D:\\path1.txt1").Matches(new string[] { "*.txt" });
            var r4 = Location.FromPath("D:\\dir1\\dir2\\path1.txt").Matches(new string[] { "D:\\*\\dir2\\*" });
            var r5 = Location.FromPath("D:\\dir2\\dir3\\path1.txt").Matches(new string[] { "D:\\*\\dir2\\*" });
            var r6 = Location.FromPath("dir3\\path1.txt").Matches(new string[] { "*.*" });

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.IsFalse(r3);
            Assert.IsTrue(r4);
            Assert.IsFalse(r5);
            Assert.IsTrue(r6);
        }

        [Test]
        public void TestMatchNegative() 
        {
            var r1 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "|D:\\*" });
            var r2 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "|.dll", "|*.txt" });
            var r3 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "|.dll" });

            Assert.IsFalse(r1);
            Assert.IsFalse(r2);
            Assert.IsTrue(r3);
        }

        [Test]
        public void TestMatchMixed()
        {
            var r1 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "D:\\*", "|*.txt" });
            var r2 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "D:\\*", "|*.dll" });
            var r3 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "C:\\*", "|*.dll" });

            Assert.IsFalse(r1);
            Assert.IsTrue(r2);
            Assert.IsFalse(r3);
        }

        [Test]
        public void FromStringTest()
        {
            var r1 = Location.FromString("abc\\xyz");
            var r2 = Location.FromString("abc/xyz");
            var r3 = Location.FromString("abc::xyz");
            var r4 = Location.FromString("abc\\xyz\\file1.txt");
            var r5 = Location.FromString("abc/xyz/file1.txt");
            var r6 = Location.FromString("abc::xyz::file1.txt");
            var r7 = Location.FromString("D:\\dir\\file1.txt");
            var r8 = Location.FromString("https://www.example.com/file1.txt");

            Assert.AreEqual("", r1.FileName);
            Assert.AreEqual("", r2.FileName);
            Assert.AreEqual("", r3.FileName);
            Assert.AreEqual("file1.txt", r4.FileName);
            Assert.AreEqual("file1.txt", r5.FileName);
            Assert.AreEqual("file1.txt", r6.FileName);
            Assert.AreEqual("file1.txt", r7.FileName);
            Assert.AreEqual("file1.txt", r8.FileName);

            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r1.Path));
            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r2.Path));
            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r3.Path));
            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r4.Path));
            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r5.Path));
            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r6.Path));
        }

        [Test]
        public void FromUrlTest() 
        {
            var r1 = Location.FromUrl("http://example.com");
            var r2 = Location.FromUrl("https://example.com");
            var r3 = Location.FromUrl("https://example.com/url1/");
            var r4 = Location.FromUrl("https://example.com/url1/file1.txt");
            var r5 = Location.FromUrl("/url1/file1.txt");

            Assert.AreEqual("example.com", r1.FileName);
            Assert.AreEqual("example.com", r2.FileName);
            Assert.AreEqual("", r3.FileName);
            Assert.AreEqual("file1.txt", r4.FileName);
            Assert.AreEqual("file1.txt", r5.FileName);

            Assert.IsTrue(new string[] { "http:/" }.SequenceEqual(r1.Path));
            Assert.IsTrue(new string[] { "https:/" }.SequenceEqual(r2.Path));
            Assert.IsTrue(new string[] { "https:/", "example.com", "url1" }.SequenceEqual(r3.Path));
            Assert.IsTrue(new string[] { "https:/", "example.com", "url1" }.SequenceEqual(r4.Path));
            Assert.IsTrue(new string[] { "url1" }.SequenceEqual(r5.Path));
        }
    }
}
