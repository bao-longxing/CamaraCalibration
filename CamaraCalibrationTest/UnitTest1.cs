using CameraCalibration;
using CameraCalibration.View;
using Moq;
namespace CameraCalibrationTest
{
    public class Tests
    {
        private DataControler _controler;
        private CalibrationData _calibrationData;
        private ModbusFunction _modbusFunction;
        [SetUp]
        public void Setup()
        {
            _modbusFunction = new ModbusFunction();
            _calibrationData = new CalibrationData();
            _controler = new DataControler( _calibrationData );
        }

        [Test]
        public void Test1()
        {
            var moc = new Mock<CalibrationData>();
            CalibrationData data = null;
            data = _controler.GetData();
            if (data != null)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
        public void Test2()
        {
            _modbusFunction.Connect("172.30.4.59","502");
            double[] doubles = { 5, 5, 10 };
            _modbusFunction.WriteRegisters(doubles);
            _modbusFunction.Disconnect();
        }
    }
}