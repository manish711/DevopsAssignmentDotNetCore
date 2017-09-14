using Microsoft.VisualStudio.TestTools.UnitTesting;
using HelloWorld;

namespace UnitTest
{
    [TestClass]
    public class HelloWorldTests
    {
        [TestMethod]
        public void HelloWorldMainTest()
        {
            Program.Add(1, 2);
        }
    }
}
