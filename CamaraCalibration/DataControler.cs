using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace CameraCalibration.View
{
    public struct PointGroup
    {
        public HTuple row;
        public HTuple col;
    }
    public class CalibrationData
    {
        //标定板描述文件地址
        public HTuple DescrFileAddress = string.Empty;
        
        //标定板厚度
        public double PlateThick = double.NaN;

        //相机参数
        public HTuple CameraParama = null;
             
        //CameraParama[0] = "area_scan_division";
        //CameraParama[1] = Convert.ToDouble(CameraPage.txtFocalLength.Text) / 1000;//焦距
        //CameraParama[2] = 0;//主点位置
        //CameraParama[3] = Convert.ToDouble((Convert.ToDouble(CameraPage.txtSinglePixelWidth.Text)* 0.000001).ToString("E3"));//单个像素宽度
        //CameraParama[4] = Convert.ToDouble((Convert.ToDouble(CameraPage.txtSinglePixelHeight.Text)* 0.000001).ToString("E3"));//单个像素高度
        //CameraParama[5] = 960;//主点x坐标
        //CameraParama[6] = 536;//主点y坐标
        //CameraParama[7] = 1920;//图像分辨率宽度
        //CameraParama[8] = 1072;//图像分辨率高度
             
        //数据集
        public HTuple DataGroup = null;

        //相机句柄
        public HTuple CameraHandle = null;

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
        public HTuple CameraParamra_Finish = null;
        public HTuple CameraPose_Finish = null;
        //相机标定文件和位姿文件保存路径
        public HTuple AddressForCameraParamFile;
        public HTuple AddressForCameraPoseFile;


        //九点/十二点标定，标定点坐标
        public HTuple CalibrationPoint_Row;
        public HTuple CalibrationPoint_Clomn;

        //图像映射文件（手眼标定）
        //地址
        public HTuple HomMat2DAddress;
        //数据
        public HTuple HomMat2D;

        //InputPointData ()外部标定数据
        public PointGroup PointGroup;
        public CalibrationData()
        {
            DescrFileAddress = new HTuple();
            CameraParama = new HTuple();
            DataGroup = new HTuple();
            CameraHandle = new HTuple();
            PlateFileParama = new HTuple();
            CameraParamra_Finish = new HTuple();
            CameraPose_Finish = new HTuple();
            HomMat2DAddress = new HTuple();
            HomMat2D = new HTuple();
            CalibrationPoint_Row = new HTuple();
            CalibrationPoint_Clomn = new HTuple();
            PointGroup = new PointGroup();
            PointGroup.row = new HTuple();
            PointGroup.col = new HTuple();
            CameraParama[0] = "area_scan_division";//相机类型
            CameraParama[2] = 0;//主点位置
            CameraParama[5] = 960;//主点x坐标
            CameraParama[6] = 536;//主点y坐标
            CameraParama[7] = 1920;//图像分辨率宽度
            CameraParama[8] = 1072;//图像分辨率高度
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
