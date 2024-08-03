using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CameraCalibration.View
{
    internal class ViewControler
    {
        HalconFuntion HalconFuntion;

        //主窗口
        MainWindow MainWindowPage = null;
        
        //相机参数页面
        public Camera CameraPage = null;

        //标定文件生成页面
        CreateDescrFile CreateDescrFilePage = null;

        //标定页面
        public Calibrating CalibratingPage = null;

        //相机参数页面
        public CameraParamaList CameraParamaListPage = null;

        //数据控制器
        private DataControler DataControler;

        //畸变矫正
        public AberrationCorrection AberrationCorrection;

        //定位引导
        public PositioningGuidance PositioningGuidance;

        //生成映射文件
        public MultipointCalibration MultipointCalibration;
        
        public ViewControler(MainWindow main, Camera camara, Calibrating calibrating, CreateDescrFile create,CameraParamaList camaraParamaList,DataControler dataControler,AberrationCorrection correction, PositioningGuidance positioning, MultipointCalibration multipoint)
        {
            MainWindowPage = main;
            CameraPage = camara;
            CalibratingPage = calibrating;
            CreateDescrFilePage = create;
            CameraParamaListPage = camaraParamaList;
            DataControler = dataControler; 
            AberrationCorrection = correction;
            PositioningGuidance = positioning;
            MultipointCalibration = multipoint;
            CameraPage.btnCreateDescrFile.Click += BtnCreateDescrFile_Click;//创建Descr文件按钮
            CreateDescrFilePage.btnCreateDescrFile.Click += BtnGenDescrFile_Click;//生成Descr文件按钮

            //Main
            //Menu
            MainWindowPage.btn_CameraCalibration.Click += Btn_CameraCalibration_Click;
            //畸变矫正
            MainWindowPage.btnAberrationCorrection.Click += BtnAberrationCorrection_Click;
            //打开和关闭相机
            MainWindowPage.btnOpenCamera.Click += BtnOpenCamera_Click;
            MainWindowPage.btnCloseCamera.Click += BtnCloseCamera_Click;
            //生成映射文件
            MainWindowPage.btnMultipointCalibration.Click += BtnMultipointCalibration_Click;
            //定位引导
            MainWindowPage.btnPositioningGuidance.Click += BtnPositioningGuidance_Click;


            //开始标定
            CameraPage.btnStartCalibration.Click += BtnStartCalibration_Click;

            //采集按钮
            CalibratingPage.btnCaptureButton.Click += BtnCaptureButton_Click;
            //清除按钮
            CalibratingPage.btnClearImagesButton.Click += BtnClearImagesButton_Click;
            //生成按钮
            CalibratingPage.btnFinishButton.Click += BtnFinishButton_Click;
            DataControler = dataControler;
       


            //ParamaList
            //选择CameraParam文件路径
            CameraParamaListPage.btnSaveCameraParama.Click += BtnSaveCameraParama_Click;
            //选择CameraPose文件路径
            CameraParamaListPage.btnSaveCameraPoseParama.Click += BtnSaveCameraPoseParama_Click;

            //AberrationCorrection
            //读取相机参数文件
            AberrationCorrection.btnLoadCameraParama.Click += BtnLoadCameraParama_Click;
            //畸变矫正开关
            AberrationCorrection.tobCorrectionSwitch.Checked += TobCorrectionSwitch_Checked;
            AberrationCorrection.tobCorrectionSwitch.Unchecked += TobCorrectionSwitch_Unchecked;

        
            //导入映射文件
            PositioningGuidance.btnInputImageMap.Click += BtnInputImageMap_Click;
            //生成映射文件入口2
            PositioningGuidance.btnGenImageMap.Click += BtnMultipointCalibration_Click;
            //连接Modbus从站
            PositioningGuidance.btnConnectModbus.Click += BtnConnectModbus_Click;
            //开始采集
            PositioningGuidance.btnBeginCapture.Click += BtnBeginCapture_Click;
            //停止采集
            PositioningGuidance.btnStopCapture.Click += BtnStopCapture_Click;

            //截取映射文件
            MultipointCalibration.btnCaptureButton.Click += BtnCaptureButton_Click_MultiPoint;
            //生成映射文件
            MultipointCalibration.btnGenImageHomMat2D.Click += BtnGenImageHomMat2D_Click;
        
        }

        private void BtnStopCapture_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.ClosePostGuide();
        }

        private void BtnBeginCapture_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.OpenPosGuidance();
        }

        private void BtnConnectModbus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PositioningGuidance.btnConnectModbus.Content.ToString() == "连接")
                {
                    HalconFuntion.modbusFunction.Connect(PositioningGuidance.txtModbusIP.Text, PositioningGuidance.txtModbusPort.Text);
                    SnackbarManager.ShowMessage("连接成功");
                    PositioningGuidance.btnConnectModbus.Content = "断开";
                }
                else if (PositioningGuidance.btnConnectModbus.Content.ToString() == "断开")
                {
                    HalconFuntion.modbusFunction.Disconnect();
                    PositioningGuidance.btnConnectModbus.Content = "连接";
                }

            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }

        private void BtnGenImageHomMat2D_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.MultiPointGenHomMat2DFile();
        }

        private void BtnCaptureButton_Click_MultiPoint(object sender, RoutedEventArgs e)
        {
            _= HalconFuntion.MultiPointCaptuerAsync();
        }

        private void BtnMultipointCalibration_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CloseCamera();
            MainWindowPage.AppFrame.Navigate(MultipointCalibration);
        }

        //获取映射文件地址
        private void BtnInputImageMap_Click(object sender, RoutedEventArgs e)
        {
            CalibrationData calibrationData = DataControler.GetData();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = AppContext.BaseDirectory;
                openFileDialog.Filter = "映射文件|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    calibrationData.HomMat2DAddress = openFileDialog.FileName;
                    HalconFuntion.ReadHomMat2DForFile();
                    SnackbarManager.ShowMessage("导入数据成功");
                }
                DataControler.SetData(calibrationData);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void BtnPositioningGuidance_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CloseCamera();
            MainWindowPage.AppFrame.Navigate(PositioningGuidance);
        }

        private void TobCorrectionSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CorrectionSwtich = false;
        }

        private void TobCorrectionSwitch_Checked(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CorrectionSwtich = true;
        }

        private void BtnLoadCameraParama_Click(object sender, RoutedEventArgs e)
        {
            bool result = HalconFuntion.LoadCameraParamFile();
            if (result)
            {
                AberrationCorrection.txtCameraParametersStatus.Text = "参数已读取";
            }
        }
            
        private void BtnSaveCameraPoseParama_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.SavaCameraPoseToFile();
        }

        private void BtnSaveCameraParama_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.SavaCameraParamaToFile();
        }

        private void BtnAberrationCorrection_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CloseCamera();
            CalibrationData data = DataControler.GetData();
            if (!(data.CameraParamra_Finish.Length==0))
            {
                AberrationCorrection.txtCameraParametersStatus.Text = "参数已读取";
            }
            MainWindowPage.AppFrame.Navigate(AberrationCorrection);
        }

        private void BtnFinishButton_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(CalibratingPage.txtCapturedImagesCount.Text) < 10)
            {
                SnackbarManager.ShowMessage("图像数量不足");
                return;
            }
            HalconFuntion.CreateCameraParama();
            HalconFuntion.CloseCamera();
            CalibrationData data = new CalibrationData();
            MainWindowPage.AppFrame.Navigate(CameraParamaListPage);
            data = DataControler.GetData();
            CameraParamaListPage.txtFocalLength.Text = Convert.ToInt32(data.CameraParamra_Finish[1].O).ToString();
            CameraParamaListPage.txtKappa.Text = data.CameraParamra_Finish[2].O.ToString();
            CameraParamaListPage.txtSiglePixelWidth.Text = data.CameraParamra_Finish[3].O.ToString();
            CameraParamaListPage.txtSiglePixelHight.Text = data.CameraParamra_Finish[4].O.ToString();
            CameraParamaListPage.txtCenterXPoint.Text = data.CameraParamra_Finish[5].O.ToString();
            CameraParamaListPage.txtCenterYPoint.Text = data.CameraParamra_Finish[6].O.ToString();
            CameraParamaListPage.txtImangeWidth.Text = data.CameraParamra_Finish[7].O.ToString();
            CameraParamaListPage.txtImageHight.Text = data.CameraParamra_Finish[8].O.ToString();
            CameraParamaListPage.txtXPoint.Text = data.CameraPose_Finish[0].O.ToString();
            CameraParamaListPage.txtYPoint.Text = data.CameraPose_Finish[1].O.ToString();
            CameraParamaListPage.txtZPoint.Text = data.CameraPose_Finish[2].O.ToString();
            CameraParamaListPage.txtXSpinPoint.Text = data.CameraPose_Finish[3].O.ToString();
            CameraParamaListPage.txtYSpinPoint.Text = data.CameraPose_Finish[4].O.ToString();
            CameraParamaListPage.txtZSpinPoint.Text = data.CameraPose_Finish[5].O.ToString();
        }

        private void BtnClearImagesButton_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.ClearhImageList();
        }

        private void BtnCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.SetcaptureTrigger(true);
        }

        public void SetHalconFunction(HalconFuntion halcon) 
        {
            HalconFuntion = halcon;
        }


        //Menu
        private void Btn_CameraCalibration_Click(object sender, RoutedEventArgs e)
        {
            MainWindowPage.AppFrame.Navigate(CameraPage);
            HalconFuntion.CloseCamera();
        }
        private async void BtnStartCalibration_Click(object sender, RoutedEventArgs e)
        {
            if (HalconFuntion.InitCalibrationParama())
            {
                MainWindowPage.AppFrame.Navigate(CalibratingPage);
                HalconFuntion.CloseCamera();
                Debug.WriteLine("camaraPageTocalibrating");
                await Task.Delay(500);
                HalconFuntion.OpenCamera(CalibratingPage);
            }
        }

        private void BtnOpenCamera_Click(object sender, RoutedEventArgs e)
        {
            if (CalibratingPage.hSmartWindowControl.IsVisible)
            {
                HalconFuntion.OpenCamera(CalibratingPage);
            }
            if (CameraPage.hSmartWindowControl.IsVisible)
            {
                HalconFuntion.OpenCamera(CameraPage);
            }
            if (AberrationCorrection.hSmartWindowControl.IsVisible)
            {
                HalconFuntion.OpenCamera(AberrationCorrection);
            }
            if (PositioningGuidance.hSmartWindowControl.IsVisible)
            {
                HalconFuntion.OpenCamera(PositioningGuidance);
            }
            if (MultipointCalibration.hSmartWindowControl.IsVisible)
            {
                HalconFuntion.OpenCamera(MultipointCalibration);
            }
        }

        private void BtnCloseCamera_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CloseCamera();
        }

        //生成Descr文件
        private void BtnGenDescrFile_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CreateDescrFile();
        }

        //跳转至文件创建页面
        private void BtnCreateDescrFile_Click(object sender, RoutedEventArgs e)
        {
            CameraPage.splCameraCalibrationList.Visibility = Visibility.Collapsed;
            CameraPage.frameCreateDescrFile.Navigate(CreateDescrFilePage);
            CameraPage.frameCreateDescrFile.Visibility = Visibility.Visible;

            //防止重复订阅
            CreateDescrFilePage.btnBack.Click -= BtnBack_Click;
            CreateDescrFilePage.btnBack.Click += BtnBack_Click;
        }

        //返回到标定页面
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            CameraPage.frameCreateDescrFile.Visibility = Visibility.Collapsed;
            CameraPage.splCameraCalibrationList.Visibility = Visibility.Visible;
        }

    }
}
