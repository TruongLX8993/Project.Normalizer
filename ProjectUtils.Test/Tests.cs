using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace ProjectUtils.Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var slnReader = new SlnReader("E:\\Newfolder\\onemes3.codes\\ONEMES3.YC\\ONEMES3.YC.sln");
            var prefix = slnReader.SlnPrefix();
            var csProjPaths = slnReader.GetCsProjPaths();
            Assert.True(csProjPaths.Count > 0);
            foreach (var s in csProjPaths)
            {
                Console.WriteLine(s);
            }

            Assert.True(prefix == "ONEMES3.YC");
        }

        [Test]
        public void Test2()
        {
            var path = "E:\\Newfolder\\onemes3.codes\\Libs";
            var fileName = "Spire.Pdf.dll";
            var dirProxy = new DirectoryProxy(path);
            var filePath = dirProxy.GetPath(fileName);
            Assert.IsTrue(!string.IsNullOrEmpty(filePath));
            Console.WriteLine(filePath);
        }

        [Test]
        public void TestCreateRelativePath()
        {
            var file = "E:\\Newfolder\\onemes3.codes\\Libs\\Spire.Pdf.dll";
            var currentPath = "E:\\Newfolder\\onemes3.codes\\ONEMES3.HD\\";
            var res = PathUtil.GetRelative(currentPath, file);
            Console.WriteLine(res);
        }

        [Test]
        public void TestUpdateCsproj()
        {
            // const string csProjPath =
            //     "E:\\Newfolder\\onemes3.codes\\ONEMES3.HD\\ONEMES3.HD.Gui.WebParts\\ONEMES3.HD.Gui.WebParts.csproj";
            // const string coreDllPath = @"E:\Newfolder\onemes3.codes\ONEMES3.KCB\CoreDlls\";
            // const string libDllPath = @"E:\Newfolder\onemes3.codes\Libs";
            // var projectRefUpdater = new ProjectRefUpdater(csProjPath, coreDllPath, libDllPath, "ONEMES3.HD");
            // var res = projectRefUpdater.Update();
            // Console.WriteLine(res.OuterXml);
        }

        [Test]
        public void TestUpdateBuildEvent()
        {
            const string csProjPath =
                @"E:\Newfolder\onemes3.codes\ONEMES3.XN\ONEMES3.XN.Model\ONEMES3.XN.Model.csproj";
            var csDoc = new XmlDocument();
            csDoc.Load(csProjPath);
            const string csProjDir = @"E:\Newfolder\onemes3.codes\ONEMES3.XN\ONEMES3.XN.Model\";
            var keyMap = new Dictionary<string, string>()
            {
                { "ONEMES3.CL", @"E:\Newfolder\onemes3.codes\ONEMES3.CL\CoreDlls" },
                { "ONEMES3.KCB", @"E:\Newfolder\onemes3.codes\ONEMES3.KCB\CoreDlls" }
            };

            var buildEvent =
                @"copy $(TargetPath) E:\ONEMES3.HD\Runtime\web\bin copy $(TargetPath) E:\ONEMES3.HD\CoreDlls";
            var buildEventUpdater = new ProjectBuildEventNormalizer(csProjDir, csDoc, keyMap);
            var xmlDocument = buildEventUpdater.Normalize();
            Console.WriteLine(xmlDocument.OuterXml);
        }

        [Test]
        public void ReadTestProject()
        {
            var slnReader = new SlnReader("E:\\Newfolder\\onemes3.codes\\ONEMES3.YC\\ONEMES3.YC.sln");
            var pInfo = slnReader.GetProjectInfo("ONEMES3.YC.Model.csproj");
            Console.WriteLine($"{pInfo.Guid}");
            Console.WriteLine($"{pInfo.Name}");
            Console.WriteLine($"{pInfo.AbsPath}");
        }
        
    }
}