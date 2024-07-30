using CamaraCalibration.View;
using HalconDotNet;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CamaraCalibration
{

    
    //标定板厚度
    internal class HalconFuntion
    {
        List<HObject> hImages;
        ViewControler viewControler;
        CalibrationData calibrationData;
        DataControler dataControler;
         bool isRuning = false;

        //畸变矫正开关
        public bool CorrectionSwtich;

        public bool captureTrigger = false;//采集触发
        public HalconFuntion(DataControler data,ViewControler view)
        {
            dataControler = data;
            viewControler = view;
            CorrectionSwtich = false;
            hImages = new List<HObject>();
        }

        public async void OpenCamara(Camara window)
        {
            InitCamaraHandle();
            await GetCamaraImageAync_CamaraPage();
        }
        public async void OpenCamara(Calibrating window)
        {
            InitCamaraHandle();
            await GetCamaraImageAync_calibratingPage();
        }
        public async void OpenCamara(AberrationCorrection window)
        {
            InitCamaraHandle();
            await GetCamaraImageAync_AberrationCorrectionPage();
        }

        public async Task GetCamaraImageAync_AberrationCorrectionPage()
        {
            try
            {
                HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                HOperatorSet.GrabImageStart(calibrationData.CamaraHandle, -1);
                HOperatorSet.ChangeRadialDistortionCamPar("adaptive", calibrationData.CamaraParamra_Finish, 0, out HTuple camParamOut);
                HOperatorSet.GenRadialDistortionMap(out HObject map, calibrationData.CamaraParamra_Finish, camParamOut, "bilinear");
                isRuning = true;
                while (isRuning)
                {

                    calibrationData.hImage.Dispose();
                    // 采集图像
                    HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CamaraHandle, -1);
                    if (CorrectionSwtich)
                    {
                        //畸变矫正
                        HOperatorSet.MapImage(calibrationData.hImage, map, out HObject imageMapped);
                        //// 获取图像尺寸
                        HOperatorSet.GetImageSize(imageMapped, out HTuple width, out HTuple height);
                        // 设置 Halcon 图像显示尺寸，一般来说，图像会铺满 Halcon 控件，因此会有一定程度拉伸
                        HOperatorSet.SetPart(viewControler.AberrationCorrection.hSmartWindowControl.HalconWindow, 0, 0, height - 1, width - 1);
                        // 显示图像
                        HOperatorSet.DispObj(imageMapped, viewControler.AberrationCorrection.hSmartWindowControl.HalconWindow);
                        // 设置原图像比例缩放，这个效果和双击左键效果一样
                        viewControler.AberrationCorrection.hSmartWindowControl.SetFullImagePart();
                    }
                    else
                    {
                        //// 获取图像尺寸
                        HOperatorSet.GetImageSize(calibrationData.hImage, out HTuple width, out HTuple height);
                        // 设置 Halcon 图像显示尺寸，一般来说，图像会铺满 Halcon 控件，因此会有一定程度拉伸
                        HOperatorSet.SetPart(viewControler.AberrationCorrection.hSmartWindowControl.HalconWindow, 0, 0, height - 1, width - 1);
                        // 显示图像
                        HOperatorSet.DispObj(calibrationData.hImage, viewControler.AberrationCorrection.hSmartWindowControl.HalconWindow);
                        // 设置原图像比例缩放，这个效果和双击左键效果一样
                        viewControler.AberrationCorrection.hSmartWindowControl.SetFullImagePart();
                    }
                    await Task.Delay(10);
                }
            }
            catch (Exception e)
            {
                CloseCamara();
                MessageBox.Show(e.Message);
            }
        }
        public void InitCamaraHandle() 
        {
            calibrationData = dataControler.GetData();
            HOperatorSet.GenEmptyObj(out calibrationData.hImage);
            HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "rgb", -1, "false", "default", "[0] ", 0, -1, out calibrationData.CamaraHandle);
            dataControler.SetData(calibrationData);
        }
        public void CloseCamara()
        {
            calibrationData = dataControler.GetData();
            try
            {
                isRuning = false;
                HOperatorSet.CloseFramegrabber(calibrationData.CamaraHandle);
                dataControler.SetData(calibrationData);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
           
        }

        struct PlateMargin
        {
            public HObject contours;
            public HObject cross;
        }
        public async Task GetCamaraImageAync_calibratingPage()
            {
            try
            {
                
                PlateMargin plateMargin = new PlateMargin();
                calibrationData = dataControler.GetData();
                HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                HOperatorSet.GrabImageStart(calibrationData.CamaraHandle, -1);
                isRuning = true;
                bool isCalibrating = false;
                while (isRuning)
                {
                    await Task.Run(async () =>
                    { 
                        //calibrationData.hImage.Dispose();
                        // 采集图像
                        //HOperatorSet.GrabImage(out calibrationData.hImage, calibrationData.CamaraHandle);
                        //viewControler.CalibratingPage.hSmartWindowControl.HalconWindow.DispObj(calibrationData.hImage);
                        HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CamaraHandle, -1);
                        //// 获取图像尺寸
                        HOperatorSet.GetImageSize(calibrationData.hImage, out HTuple width, out HTuple height);
                        // 设置 Halcon 图像显示尺寸，一般来说，图像会铺满 Halcon 控件，因此会有一定程度拉伸
                        HOperatorSet.SetPart(viewControler.CalibratingPage.hSmartWindowControl.HalconWindow, 0, 0, height - 1, width - 1);
                        // 显示图像
                        
                        // 设置原图像比例缩放，这个效果和双击左键效果一样
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            HOperatorSet.DispObj(calibrationData.hImage, viewControler.CalibratingPage.hSmartWindowControl.HalconWindow);
                            viewControler.CalibratingPage.hSmartWindowControl.SetFullImagePart();
                            try
                            {
                                if (plateMargin.cross != null && plateMargin.contours != null)
                                {
                                    HOperatorSet.SetColor(viewControler.CalibratingPage.hSmartWindowControl.HalconWindow, "green");
                                    HOperatorSet.DispObj(plateMargin.contours, viewControler.CalibratingPage.hSmartWindowControl.HalconWindow);
                                    HOperatorSet.DispObj(plateMargin.cross, viewControler.CalibratingPage.hSmartWindowControl.HalconWindow);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                            
                        });


                        await Task.Delay(30);
                        if (isCalibrating == false)
                        {
                            isCalibrating = true;
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await Task.Delay(100);
                                    Calibaration_Halcon.FindCalib(calibrationData.hImage, out HObject contours_out, out HObject cross_out, calibrationData.DataGroup, 0);
                                    plateMargin.contours = contours_out;
                                    plateMargin.cross = cross_out;
                                    if (captureTrigger==true)
                                    {
                                        hImages.Add(calibrationData.hImage);
                                        captureTrigger=false;
                                        SnackbarManager.ShowMessage("采集成功");
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            viewControler.CalibratingPage.txtCapturedImagesCount.Text = hImages.Count.ToString();   
                                        });
                                    }
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        viewControler.CalibratingPage.txtImageState.Text = hImages.Count.ToString();
                                        viewControler.CalibratingPage.txtImageState.Text = "OK";
                                        isCalibrating = false;
                                    });
                                }
                                catch (HDevEngineException e) 
                                {
                                    if (e.HalconError == 8404)
                                    {
                                        plateMargin.cross = null;
                                        plateMargin.contours = null;
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            viewControler.CalibratingPage.txtImageState.Text = "NG";
                                        });
                                    }
                                    else if (e.HalconError == 8431)
                                    {
                                        plateMargin.cross = null;
                                        plateMargin.contours = null;
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            viewControler.CalibratingPage.txtImageState.Text = "无法校准标定板方向";
                                        });
                                    }
                                    else if (e.HalconError == 3264)
                                    {
                                        plateMargin.cross = null;
                                        plateMargin.contours = null;
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            viewControler.CalibratingPage.txtImageState.Text = "点分类为离群值太小或使用的点与原始值不相似";
                                        });
                                    }
                                    else if (e.HalconError == 8404)
                                    {
                                        plateMargin.cross = null;
                                        plateMargin.contours = null;
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            viewControler.CalibratingPage.txtImageState.Text = "未能找到满足条件的目标";
                                        });
                                    }
                                    else
                                    {
                                        MessageBox.Show(e.Message);
                                    }
                                    isCalibrating = false;
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message);
                                    isCalibrating = false;
                                }
                            });
                        }
                    });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public async Task GetCamaraImageAync_CamaraPage()
        {
            try
            {
                HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                HOperatorSet.GrabImageStart(calibrationData.CamaraHandle, -1);
                isRuning = true;
                while (isRuning)
                {
                    calibrationData.hImage.Dispose();
                    // 采集图像
                   // HOperatorSet.GrabImage(out calibrationData.hImage, calibrationData.CamaraHandle);
                    HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CamaraHandle, -1);
                    //// 获取图像尺寸
                    HOperatorSet.GetImageSize(calibrationData.hImage, out HTuple width, out HTuple height);
                    // 设置 Halcon 图像显示尺寸，一般来说，图像会铺满 Halcon 控件，因此会有一定程度拉伸
                    HOperatorSet.SetPart(viewControler.CamaraPage.hSmartWindowControl.HalconWindow, 0, 0, height - 1, width - 1);
                    // 显示图像
                    HOperatorSet.DispObj(calibrationData.hImage,viewControler.CamaraPage.hSmartWindowControl.HalconWindow);
                    // 设置原图像比例缩放，这个效果和双击左键效果一样
                    viewControler.CamaraPage.hSmartWindowControl.SetFullImagePart();
                    await Task.Delay(10);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //初始化标定参数
        public bool InitCalibrationParama()
        {
            calibrationData = dataControler.GetData();

            if (calibrationData.CamaraParama[2] < 0.00000000)
            {
                SnackbarManager.ShowMessage("参数为空");
                return false;
            }
            if (calibrationData.CamaraParama[3] == 0.00000000)
            {
                SnackbarManager.ShowMessage("参数为空");
                return false;
            }
            if (calibrationData.CamaraParama[4] == 0.00000000)
            {
                SnackbarManager.ShowMessage("参数为空");
                return false;
            }

            try
            {
                calibrationData = dataControler.GetData();
                calibrationData.DataGroup.Dispose();
                Calibaration_Halcon.BulidDataGroup(calibrationData.CamaraParama, calibrationData.DescrFileAddress,out calibrationData.DataGroup);
                dataControler.SetData(calibrationData);
                return true;
            }
            catch (Exception e)
            {
               MessageBox.Show($"Error: {e.Message}");
                return false;
            }
        }
        //创建标定文件
        public void CreateDescrFile() 
        {
            calibrationData = dataControler.GetData();
            if (calibrationData.PlateFileParama.Length<1)
            {
                string msg = "数据为空";
                SnackbarManager.ShowMessage(msg);
                return;
            }
            if (calibrationData.PlateFileParama[0] == 0)
            {
                string msg = "列数据为空";
                SnackbarManager.ShowMessage(msg);
                return;
            }
            if (calibrationData.PlateFileParama[1] == 0)
            {
                string msg = "行数据为空";
                SnackbarManager.ShowMessage(msg);
                return;
            }
            if (calibrationData.PlateFileParama[2] < 0.00000001)
            {
                string msg = "中心间距数据为空";
                SnackbarManager.ShowMessage(msg);
                return;
            }
            if (calibrationData.PlateFileParama[3] <0.00000001)
            {
                string msg = "中心间距比例数据为空";
                SnackbarManager.ShowMessage(msg);
                return;
            }
            if (calibrationData.PlateFileParama[4] == "")
            {
                string msg = "descr 文件地址数据为空";
                SnackbarManager.ShowMessage(msg);
                return;
            }
            if (calibrationData.PlateFileParama[5] == "")
            {
                string msg = "ps 文件地址列数据为空";
                SnackbarManager.ShowMessage(msg);
                return;
            }

            HOperatorSet.GenCaltab(calibrationData.PlateFileParama[0],
                calibrationData.PlateFileParama[2],
                calibrationData.PlateFileParama[1],
                calibrationData.PlateFileParama[3],
                calibrationData.PlateFileParama[4],
                calibrationData.PlateFileParama[5]);
            dataControler.SetData(calibrationData);
            SnackbarManager.ShowMessage(calibrationData.PlateFileParama[4]+"已保存");
        }
        public void SetcaptureTrigger(bool Value)
        {
            captureTrigger = Value;
        }
        public void ClearhImageList() 
        {
            hImages.Clear();
            viewControler.CalibratingPage.txtCapturedImagesCount.Text = "0";    
        }
        //创建位姿数据
        public void CreateCamaraParama() 
        {
            if (hImages.Count<10)
            {
                SnackbarManager.ShowMessage("图片数量过少");
                return;
            }
            calibrationData = dataControler.GetData();
            calibrationData.DataGroup.Dispose();
            Calibaration_Halcon.BulidDataGroup(calibrationData.CamaraParama, calibrationData.DescrFileAddress, out calibrationData.DataGroup);
            dataControler.SetData(calibrationData);
            for (int i = 0; i < hImages.Count; i++)
            {
                Calibaration_Halcon.FindCalib(hImages[i],out HObject contours,out HObject cross, calibrationData.DataGroup, i);
            }
            Calibaration_Halcon.GetCamaraCalibrationParama(calibrationData.DataGroup, calibrationData.PlateThick, out HTuple tmpCtrl_Errors, out HTuple cameraParameters, out HTuple cameraPose);
            calibrationData.CamaraParamra_Finish = cameraParameters;
            calibrationData.CamaraPose_Finish = cameraPose;
            dataControler.SetData(calibrationData);
        }
        //保存相机参数文件
        public void SavaCamaraParamaToFile() 
        {
            calibrationData = dataControler.GetData();
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = AppContext.BaseDirectory;
                saveFileDialog.Filter = "相机参数文件 (*.cal)|*.cal";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = saveFileDialog.FileName;
                    calibrationData.AddressForCamaraParamFile = path;
                }
                else
                {
                    SnackbarManager.ShowMessage("文件未选择");
                }
                dataControler.SetData(calibrationData);
                HOperatorSet.WriteCamPar(calibrationData.CamaraParamra_Finish, calibrationData.AddressForCamaraParamFile);
                SnackbarManager.ShowMessage("保存成功");
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }
        //保存相机位姿文件
        public void SavaCamaraPoseToFile() 
        {
            calibrationData = dataControler.GetData();
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = AppContext.BaseDirectory;
                saveFileDialog.Filter = "相机位姿文件 (*.dat)|*.dat";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = saveFileDialog.FileName;
                    calibrationData.AddressForCamaraPoseFile = path;
                }
                else
                {
                    SnackbarManager.ShowMessage("文件未选择");
                }
                dataControler.SetData(calibrationData);
                HOperatorSet.WritePose(calibrationData.CamaraPose_Finish, calibrationData.AddressForCamaraPoseFile);
                SnackbarManager.ShowMessage("保存成功");
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }
        //加载相机参数文件
        public bool LoadCamaraParamFile() 
        {
            calibrationData = dataControler.GetData();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = AppContext.BaseDirectory;
                openFileDialog.Filter = "相机参数文件|*.dat;*.cal";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    HOperatorSet.ReadCamPar(openFileDialog.FileName,out calibrationData.CamaraParamra_Finish);
                    dataControler.SetData(calibrationData);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
