using System;
using System.Linq;
using Wasteless.Web.Helpers;
using Wasteless.Web.Infrastructure;
using Xunit;

namespace Wasteless.Tests
{
    public class UnitTest1
    {
        public UnitTest1()
        {
            RepoDb.SqlServerBootstrap.Initialize();
            MapperDefinitions.Setup();
        }

        [Fact]
        public void DateTest()
        {
            var firstDate = new DateTime(2020, 1, 1);
            var secondDate = new DateTime(2020, 1, 5);

            var dates = firstDate.DatesTo(secondDate).ToArray();
           
            Assert.Contains(new DateTime(2020, 1, 1), dates);
            Assert.Contains(new DateTime(2020, 1, 2), dates);
            Assert.Contains(new DateTime(2020, 1, 3), dates);
            Assert.Contains(new DateTime(2020, 1, 4), dates);
            Assert.Contains(new DateTime(2020, 1, 5), dates);
        }
    }
}