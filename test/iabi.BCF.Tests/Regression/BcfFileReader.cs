﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace iabi.BCF.Tests.Regression
{
    public class BcfFileReader
    {
        [Fact]
        public void CanReadSophieV20()
        {
            using (var bcfStream = TestCaseResourceFactory.GetResourceStreamFromResourcePath("iabi.BCF.Tests.Resources.Regression.SophieV20.bcfzip"))
            {
                var container = BCF.BCFv2.BCFv2Container.ReadStream(bcfStream);
                Assert.NotNull(container);
            }
        }

        [Fact]
        public void CanReadSophieV21()
        {
            using (var bcfStream = TestCaseResourceFactory.GetResourceStreamFromResourcePath("iabi.BCF.Tests.Resources.Regression.SophieV21.bcf"))
            {
                var container = BCF.BCFv21.BCFv21Container.ReadStream(bcfStream);
                Assert.NotNull(container);
            }
        }
    }
}
