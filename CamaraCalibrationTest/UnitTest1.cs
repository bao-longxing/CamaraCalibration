using CamaraCalibration.View;
using Moq;
namespace CamaraCalibrationTest
{
    public class Tests
    {
        private DataControler _controler;
        private CalibrationData _calibrationData;
        [SetUp]
        public void Setup()
        {
            _calibrationData = new CalibrationData();
            _controler = new DataControler( _calibrationData );
        }

        [Test]
        public void Test1()
        {
            var moc = new Mock<CalibrationData>();
            CalibrationData data = null;
            data = _controler.GetData();
            if (data!=null)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}