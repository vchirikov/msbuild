﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Microsoft.Build.UnitTests;
using System.Threading;

namespace Microsoft.Build.Tasks.UnitTests
{
    public class Semaphore_Tests
    {
        [Fact]
        public void TestRequestingInvalidNumCores()
        {
            // assume multiproc build of 40
            new Semaphore(40, 40, "cpuCount");
            MockEngine mockEngine = new MockEngine();
            
            SemaphoreCPUTask test = new SemaphoreCPUTask();
            test.BuildEngine = mockEngine;

            // 40 - 80 = 0 cores left (claimed 40)
            test.BuildEngine7.RequestCores(12312).ShouldBe(40);
            test.BuildEngine7.RequestCores(10).ShouldBe(0);

            // 0 + 39 = 39 cores left
            test.BuildEngine7.ReleaseCores(39);

            // 39 - 100 = 0 cores left (claimed 39)
            test.BuildEngine7.RequestCores(100).ShouldBe(39);

            // 0 + 0 = 0 cores left
            test.BuildEngine7.ReleaseCores(0);
            test.BuildEngine7.RequestCores(2).ShouldBe(0);

            //0 + 1 = 1 cores left
            test.BuildEngine7.ReleaseCores(1);

            // 1 - 2 = 0 cores left (only claimed 1)
            test.BuildEngine7.RequestCores(2).ShouldBe(1);
        }

        [Fact(Skip = "TODO: test harness to tweak number of assignable cores")]
        public void TestReleasingInvalidNumCores()
        {
            // assume multiproc build of 40
            new Semaphore(40, 40, "cpuCount");
            MockEngine mockEngine = new MockEngine();

            SemaphoreCPUTask test = new SemaphoreCPUTask();
            test.BuildEngine = mockEngine;

            // should still be 40 cores
            test.BuildEngine7.ReleaseCores(-100);
            test.BuildEngine7.RequestCores(41).ShouldBe(40);

            // should be 40 cores to take
            test.BuildEngine7.ReleaseCores(50);
            test.BuildEngine7.RequestCores(39).ShouldBe(39);

            test.BuildEngine7.RequestCores(2).ShouldBe(1);
        }
    }
}