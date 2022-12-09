using JsonWrapper;
using LSlicer.Data.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlicingParametersTests
{
    [TestClass]
    public class JsonSetParametersProviderTest
    {
        [TestMethod]
        public void TestSetParameters()
        {
            JSONSetSlicingParametersProvider setSlicingParametersProvider = new JSONSetSlicingParametersProvider();
            SlicingParameters slicingParameters = new SlicingParameters() { Thickness = 0.0001};
            setSlicingParametersProvider.SetParameters(slicingParameters, new System.IO.FileInfo("..\\..\\..\\TestResults\\test.json")); 
        }
    }
}
