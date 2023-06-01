using NUnit.Framework;

namespace ServerTests
{
    public class Tests
    {
        private int A = 0;
        
        [SetUp]
        public void Setup()
        {
            A = 10;
        }

        [Test]
        public void Test1()
        {
            Assert.True(A == 10);
        }
    }
}