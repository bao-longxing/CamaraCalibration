using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace CamaraCalibration.View
{
    public class CalibrationData
    {
        //标定板描述文件地址
        public HTuple DescrFileAddress = string.Empty;
        
        //标定板厚度
        public double PlateThick = double.NaN;

        //相机参数
        public HTuple CamaraParama = null;

        //CamaraParama[0] = "area_scan_division";
        //CamaraParama[1] = Convert.ToDouble(CamaraPage.txtFocalLength.Text) / 1000;//焦距
        //CamaraParama[2] = 0;//主点位置
        //CamaraParama[3] = Convert.ToDouble((Convert.ToDouble(CamaraPage.txtSinglePixelWidth.Text)* 0.000001).ToString("E3"));//单个像素宽度
        //CamaraParama[4] = Convert.ToDouble((Convert.ToDouble(CamaraPage.txtSinglePixelHeight.Text)* 0.000001).ToString("E3"));//单个像素高度
        //CamaraParama[5] = 960;//主点x坐标
        //CamaraParama[6] = 536;//主点y坐标
        //CamaraParama[7] = 1920;//图像分辨率宽度
        //CamaraParama[8] = 1072;//图像分辨率高度

        //数据集
        public HTuple DataGroup = null;

        //相机句柄
        public HTuple CamaraHandle = null;

        //图像句柄
        public HObject hImage = null;

        //采集到的图像句柄
        public HObject hCapturedImage = null;

        //生成标定文件
        public HTuple PlateFileParama = null;
        ////标定列数量PlateFileParama[0]
        ////标定行数PlateFileParama[1]
        ////中心间距PlateFileParama[2]
        ////中心距比值PlateFileParama[3]
        ////descr文件路径PlateFileParama[4]
        ////ps 文件路径PlateFileParama[5]

        //标定完成后参数
        public HTuple CamaraParamra_Finish = null;
        public HTuple CamaraPose_Finish = null;
        //相机标定文件和位姿文件保存路径
        public HTuple AddressForCamaraParamFile;
        public HTuple AddressForCamaraPoseFile;

        public CalibrationData()
        {
            DescrFileAddress = new HTuple();
            CamaraParama = new HTuple();
            DataGroup = new HTuple();
            CamaraHandle = new HTuple();
            PlateFileParama = new HTuple();
            CamaraParamra_Finish = new HTuple();
            CamaraPose_Finish = new HTuple();
            CamaraParama[0] = "area_scan_division";//相机类型
            CamaraParama[2] = 0;//主点位置
            CamaraParama[5] = 960;//主点x坐标
            CamaraParama[6] = 536;//主点y坐标
            CamaraParama[7] = 1920;//图像分辨率宽度
            CamaraParama[8] = 1072;//图像分辨率高度
        }
    }
    public class DataControler
    {
        private CalibrationData Data = null;
        public DataControler(CalibrationData data) 
        {
            Data = data;
        }
        public void SetData(CalibrationData data) { Data = data; }
        public CalibrationData GetData()
        {
            return Data;
        }
    }
}
