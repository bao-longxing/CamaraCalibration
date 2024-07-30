using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CamaraCalibration.View
{
    internal class ViewControler
    {
        HalconFuntion HalconFuntion;

        //主窗口
        MainWindow MainWindowPage = null;
        
        //相机参数页面
        public Camara CamaraPage = null;

        //标定文件生成页面
        CreateDescrFile CreateDescrFilePage = null;

        //标定页面
        public Calibrating CalibratingPage = null;

        //相机参数页面
        public CamaraParamaList CamaraParamaListPage = null;

        //数据控制器
        private DataControler DataControler;

        //畸变矫正
        public AberrationCorrection AberrationCorrection;

        public ViewControler(MainWindow main, Camara camara, Calibrating calibrating, CreateDescrFile create,CamaraParamaList camaraParamaList,DataControler dataControler,AberrationCorrection correction)
        {
            MainWindowPage = main;
            CamaraPage = camara;
            CalibratingPage = calibrating;
            CreateDescrFilePage = create;
            CamaraParamaListPage = camaraParamaList;
            DataControler = dataControler; 
            AberrationCorrection = correction;
            CamaraPage.btnCreateDescrFile.Click += BtnCreateDescrFile_Click;//创建Descr文件按钮
            CreateDescrFilePage.btnCreateDescrFile.Click += BtnGenDescrFile_Click;//生成Descr文件按钮

            //Main
            //Menu
            MainWindowPage.btn_CamaraCalibration.Click += Btn_CamaraCalibration_Click;
            //畸变矫正
            MainWindowPage.btnAberrationCorrection.Click += BtnAberrationCorrection_Click;
            //打开和关闭相机
            MainWindowPage.btnOpenCamara.Click += BtnOpenCamara_Click;
            MainWindowPage.btnCloseCamara.Click += BtnCloseCamara_Click;


            //开始标定
            CamaraPage.btnStartCalibration.Click += BtnStartCalibration_Click;

            //采集按钮
            CalibratingPage.btnCaptureButton.Click += BtnCaptureButton_Click;
            //清除按钮
            CalibratingPage.btnClearImagesButton.Click += BtnClearImagesButton_Click;
            //生成按钮
            CalibratingPage.btnFinishButton.Click += BtnFinishButton_Click;
            DataControler = dataControler;
       


            //ParamaList
            //选择CamaraParam文件路径
            CamaraParamaListPage.btnSaveCamaraParama.Click += BtnSaveCamaraParama_Click;
            //选择CamaraPose文件路径
            CamaraParamaListPage.btnSaveCamaraPoseParama.Click += BtnSaveCamaraPoseParama_Click;

            //AberrationCorrection
            //读取相机参数文件
            AberrationCorrection.btnLoadCamaraParama.Click += BtnLoadCamaraParama_Click;
            //畸变矫正开关
            AberrationCorrection.tobCorrectionSwitch.Checked += TobCorrectionSwitch_Checked;
            AberrationCorrection.tobCorrectionSwitch.Unchecked += TobCorrectionSwitch_Unchecked;

            //定位引导
            MainWindowPage.btnPositioningGuidance.Click += BtnPositioningGuidance_Click;
        }

        private void BtnPositioningGuidance_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CloseCamara();
            //MainWindowPage.AppFrame.Navigate();
        }

        private void TobCorrectionSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CorrectionSwtich = false;
        }

        private void TobCorrectionSwitch_Checked(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CorrectionSwtich = true;
        }

        private void BtnLoadCamaraParama_Click(object sender, RoutedEventArgs e)
        {
            bool result = HalconFuntion.LoadCamaraParamFile();
            if (result)
            {
                AberrationCorrection.txtCameraParametersStatus.Text = "参数已读取";
            }
        }
            
        private void BtnSaveCamaraPoseParama_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.SavaCamaraPoseToFile();
        }

        private void BtnSaveCamaraParama_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.SavaCamaraParamaToFile();
        }

        private void BtnAberrationCorrection_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CloseCamara();
            CalibrationData data = DataControler.GetData();
            if (!(data.CamaraParamra_Finish.Length==0))
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
            HalconFuntion.CreateCamaraParama();
            HalconFuntion.CloseCamara();
            CalibrationData data = new CalibrationData();
            MainWindowPage.AppFrame.Navigate(CamaraParamaListPage);
            data = DataControler.GetData();
            CamaraParamaListPage.txtFocalLength.Text = Convert.ToInt32(data.CamaraParamra_Finish[1].O).ToString();
            CamaraParamaListPage.txtKappa.Text = data.CamaraParamra_Finish[2].O.ToString();
            CamaraParamaListPage.txtSiglePixelWidth.Text = data.CamaraParamra_Finish[3].O.ToString();
            CamaraParamaListPage.txtSiglePixelHight.Text = data.CamaraParamra_Finish[4].O.ToString();
            CamaraParamaListPage.txtCenterXPoint.Text = data.CamaraParamra_Finish[5].O.ToString();
            CamaraParamaListPage.txtCenterYPoint.Text = data.CamaraParamra_Finish[6].O.ToString();
            CamaraParamaListPage.txtImangeWidth.Text = data.CamaraParamra_Finish[7].O.ToString();
            CamaraParamaListPage.txtImageHight.Text = data.CamaraParamra_Finish[8].O.ToString();
            CamaraParamaListPage.txtXPoint.Text = data.CamaraPose_Finish[0].O.ToString();
            CamaraParamaListPage.txtYPoint.Text = data.CamaraPose_Finish[1].O.ToString();
            CamaraParamaListPage.txtZPoint.Text = data.CamaraPose_Finish[2].O.ToString();
            CamaraParamaListPage.txtXSpinPoint.Text = data.CamaraPose_Finish[3].O.ToString();
            CamaraParamaListPage.txtYSpinPoint.Text = data.CamaraPose_Finish[4].O.ToString();
            CamaraParamaListPage.txtZSpinPoint.Text = data.CamaraPose_Finish[5].O.ToString();
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
        private void Btn_CamaraCalibration_Click(object sender, RoutedEventArgs e)
        {
            MainWindowPage.AppFrame.Navigate(CamaraPage);
            HalconFuntion.CloseCamara();
        }
        private async void BtnStartCalibration_Click(object sender, RoutedEventArgs e)
        {
            if (HalconFuntion.InitCalibrationParama())
            {
                MainWindowPage.AppFrame.Navigate(CalibratingPage);
                HalconFuntion.CloseCamara();
                Debug.WriteLine("camaraPageTocalibrating");
                await Task.Delay(500);
                HalconFuntion.OpenCamara(CalibratingPage);
            }
        }

        private void BtnOpenCamara_Click(object sender, RoutedEventArgs e)
        {
            if (CalibratingPage.hSmartWindowControl.IsVisible)
            {
                HalconFuntion.OpenCamara(CalibratingPage);
            }
            if (CamaraPage.hSmartWindowControl.IsVisible)
            {
                HalconFuntion.OpenCamara(CamaraPage);
            }
            if (AberrationCorrection.hSmartWindowControl.IsVisible)
            {
                HalconFuntion.OpenCamara(AberrationCorrection);
            }
        }

        private void BtnCloseCamara_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CloseCamara();
        }

        //生成Descr文件
        private void BtnGenDescrFile_Click(object sender, RoutedEventArgs e)
        {
            HalconFuntion.CreateDescrFile();
        }

        //跳转至文件创建页面
        private void BtnCreateDescrFile_Click(object sender, RoutedEventArgs e)
        {
            CamaraPage.splCamaraCalibrationList.Visibility = Visibility.Collapsed;
            CamaraPage.frameCreateDescrFile.Navigate(CreateDescrFilePage);
            CamaraPage.frameCreateDescrFile.Visibility = Visibility.Visible;

            //防止重复订阅
            CreateDescrFilePage.btnBack.Click -= BtnBack_Click;
            CreateDescrFilePage.btnBack.Click += BtnBack_Click;
        }

        //返回到标定页面
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            CamaraPage.frameCreateDescrFile.Visibility = Visibility.Collapsed;
            CamaraPage.splCamaraCalibrationList.Visibility = Visibility.Visible;
        }

    }
}
