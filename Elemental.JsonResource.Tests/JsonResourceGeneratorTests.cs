﻿using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Elemental.JsonResource
{
    public static class MSBuildTest
    {
        public static void Initialize()
        {
            var msbAsm = typeof(ProjectCollection).Assembly;
            var behType = msbAsm.GetTypes().FirstOrDefault(t => t.FullName == "Microsoft.Build.Shared.BuildEnvironmentHelper");
            var behsType = msbAsm.GetTypes().FirstOrDefault(t => t.Name == "BuildEnvironmentHelperSingleton");
            var beType = msbAsm.GetTypes().FirstOrDefault(t => t.FullName == "Microsoft.Build.Shared.BuildEnvironment");
            var singletonField = behsType.GetFields().Single(f => f.Name == "s_instance");
            var beCtor = beType.GetConstructors().Single();

            var msbPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin";
            //var msbPath = Environment.GetEnvironmentVariable("MSBUILD_EXE_PATH");
            var be = beCtor.Invoke(new object[] { 1, msbPath, false, false, null });

            var instProp = behType.GetProperties().FirstOrDefault(p => p.Name == "Instance");
            singletonField.SetValue(null, be);
        }

    }

    public class JsonResourceGeneratorTests
    {
        ILogger logger;
        ITestOutputHelper o;

        static Dictionary<string, string> gp =
            new Dictionary<string, string> {
                ["Configuration"] = "Debug",
                ["Platform"] = "AnyCPU",
            };

        public JsonResourceGeneratorTests(ITestOutputHelper o)
        {
            this.o = o;
            this.logger = new XUnitTestLogger(o);

          
        }

        [Fact]
        public void BuildTest()
        {
            MSBuildTest.Initialize();
            var pc = new ProjectCollection(gp);
            var proj = pc.LoadProject("Data/Proj1/Proj.csproj");
            var result = proj.Build(logger);
            Assert.True(result);
        }
    }
}
