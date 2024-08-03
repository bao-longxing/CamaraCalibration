using CameraCalibration.View;
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

namespace CameraCalibration
{


    //标定板厚度
    internal class HalconFuntion
    {
        List<HObject> hImages;
        ViewControler viewControler;
        CalibrationData calibrationData;
        DataControler dataControler;
        public ModbusFunction modbusFunction;
        bool isRuning = false;

        //畸变矫正开关
        public bool CorrectionSwtich;

        public bool captureTrigger = false;//采集触发
        public HalconFuntion(DataControler data, ViewControler view)
        {
            dataControler = data;
            viewControler = view;
            CorrectionSwtich = false;
            modbusFunction = new ModbusFunction();
            hImages = new List<HObject>();
        }

        public async void OpenCamera(Camera window)
        {
            if (InitCameraHandle())
            {
                await GetCameraImageAync_CameraPage();
            }
        }
        public async void OpenCamera(Calibrating window)
        {
            if (InitCameraHandle())
            {
                await GetCameraImageAync_calibratingPage();
            }
        }
        public async void OpenCamera(AberrationCorrection window)
        {
            if (InitCameraHandle())
            {
                await GetCameraImageAync_AberrationCorrectionPage();
            }
        }
        public async void OpenCamera(PositioningGuidance window)
        {
            if (InitCameraHandle())
            {
                await GetCameraImageAync_PositioningGuidancePage();
            }
        }
        public async void OpenCamera(MultipointCalibration window)
        {
            if (InitCameraHandle())
            {
                await GetCameraImageAync_MultipointCalibration();
            }
        }
        public async Task GetCameraImageAync_MultipointCalibration()
        {
            try
            {
                HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                HOperatorSet.GrabImageStart(calibrationData.CameraHandle, -1);
                isRuning = true;
                while (isRuning)
                {
                    calibrationData.hImage.Dispose();
                    // 采集图像
                    // HOperatorSet.GrabImage(out calibrationData.hImage, calibrationData.CameraHandle);
                    HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CameraHandle, -1);
                    //// 获取图像尺寸
                    HOperatorSet.GetImageSize(calibrationData.hImage, out HTuple width, out HTuple height);
                    // 设置 Halcon 图像显示尺寸，一般来说，图像会铺满 Halcon 控件，因此会有一定程度拉伸
                    HOperatorSet.SetPart(viewControler.MultipointCalibration.hSmartWindowControl.HalconWindow, 0, 0, height - 1, width - 1);
                    // 显示图像
                    HOperatorSet.DispObj(calibrationData.hImage, viewControler.MultipointCalibration.hSmartWindowControl.HalconWindow);
                    // 设置原图像比例缩放，这个效果和双击左键效果一样
                    viewControler.MultipointCalibration.hSmartWindowControl.SetFullImagePart();
                    await Task.Delay(20);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public async Task GetCameraImageAync_PositioningGuidancePage()
        {
            try
            {
                HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                HOperatorSet.GrabImageStart(calibrationData.CameraHandle, -1);
                isRuning = true;
                while (isRuning)
                {
                    calibrationData.hImage.Dispose();
                    // 采集图像
                    // HOperatorSet.GrabImage(out calibrationData.hImage, calibrationData.CameraHandle);
                    HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CameraHandle, -1);
                    //// 获取图像尺寸
                    HOperatorSet.GetImageSize(calibrationData.hImage, out HTuple width, out HTuple height);
                    // 设置 Halcon 图像显示尺寸，一般来说，图像会铺满 Halcon 控件，因此会有一定程度拉伸
                    HOperatorSet.SetPart(viewControler.PositioningGuidance.hSmartWindowControl.HalconWindow, 0, 0, height - 1, width - 1);
                    // 显示图像
                    HOperatorSet.DispObj(calibrationData.hImage, viewControler.PositioningGuidance.hSmartWindowControl.HalconWindow);
                    // 设置原图像比例缩放，这个效果和双击左键效果一样
                    viewControler.PositioningGuidance.hSmartWindowControl.SetFullImagePart();
                    await Task.Delay(20);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public async Task GetCameraImageAync_AberrationCorrectionPage()
        {
            try
            {
                HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                HOperatorSet.GrabImageStart(calibrationData.CameraHandle, -1);
                isRuning = true;
                HObject map = null;
                while (isRuning)
                {
                    calibrationData.hImage.Dispose();
                    // 采集图像
                    HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CameraHandle, -1);
                    if (CorrectionSwtich)
                    {
                        if (map == null)
                        {
                            HOperatorSet.ChangeRadialDistortionCamPar("adaptive", calibrationData.CameraParamra_Finish, 0, out HTuple camParamOut);
                            HOperatorSet.GenRadialDistortionMap(out map, calibrationData.CameraParamra_Finish, camParamOut, "bilinear");
                        }
                        //畸变矫正
                        HOperatorSet.MapImage(calibrationData.hImage, map, out HObject imageMapped);
                        // 获取图像尺寸
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
                CloseCamera();
                MessageBox.Show(e.Message);
            }
        }
        public bool InitCameraHandle()
        {
            if (isRuning == true)
            {
                SnackbarManager.ShowMessage("相机已启动，若要重新启动请先关闭相机");
                return false;
            }
            calibrationData = dataControler.GetData();
            HOperatorSet.GenEmptyObj(out calibrationData.hImage);
            HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "rgb", -1, "false", "default", "[0] ", 0, -1, out calibrationData.CameraHandle);
            dataControler.SetData(calibrationData);
            return true;
        }
        public void CloseCamera()
        {
            calibrationData = dataControler.GetData();
            try
            {
                isRuning = false;
                HOperatorSet.CloseFramegrabber(calibrationData.CameraHandle);
                calibrationData.CameraHandle.Dispose();
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
        public async Task GetCameraImageAync_calibratingPage()
        {
            try
            {

                PlateMargin plateMargin = new PlateMargin();
                calibrationData = dataControler.GetData();
                HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                HOperatorSet.GrabImageStart(calibrationData.CameraHandle, -1);
                isRuning = true;
                bool isCalibrating = false;
                while (isRuning)
                {
                    await Task.Run(async () =>
                    {
                        //calibrationData.hImage.Dispose();
                        // 采集图像
                        //HOperatorSet.GrabImage(out calibrationData.hImage, calibrationData.CameraHandle);
                        //viewControler.CalibratingPage.hSmartWindowControl.HalconWindow.DispObj(calibrationData.hImage);
                        HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CameraHandle, -1);
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
                                    Calibaration.FindCalib(calibrationData.hImage, out HObject contours_out, out HObject cross_out, calibrationData.DataGroup, 0);
                                    plateMargin.contours = contours_out;
                                    plateMargin.cross = cross_out;
                                    if (captureTrigger == true)
                                    {
                                        hImages.Add(calibrationData.hImage);
                                        captureTrigger = false;
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
        public async Task GetCameraImageAync_CameraPage()
        {
            calibrationData = dataControler.GetData();
            try
            {
                HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                HOperatorSet.GrabImageStart(calibrationData.CameraHandle, -1);
                isRuning = true;
                while (isRuning)
                {
                    calibrationData.hImage.Dispose();
                    // 采集图像
                    // HOperatorSet.GrabImage(out calibrationData.hImage, calibrationData.CameraHandle);
                    HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CameraHandle, -1);
                    //// 获取图像尺寸
                    HOperatorSet.GetImageSize(calibrationData.hImage, out HTuple width, out HTuple height);
                    // 设置 Halcon 图像显示尺寸，一般来说，图像会铺满 Halcon 控件，因此会有一定程度拉伸
                    HOperatorSet.SetPart(viewControler.CameraPage.hSmartWindowControl.HalconWindow, 0, 0, height - 1, width - 1);
                    // 显示图像
                    HOperatorSet.DispObj(calibrationData.hImage, viewControler.CameraPage.hSmartWindowControl.HalconWindow);
                    // 设置原图像比例缩放，这个效果和双击左键效果一样
                    viewControler.CameraPage.hSmartWindowControl.SetFullImagePart();
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

            if (calibrationData.CameraParama[2] < 0.00000000)
            {
                SnackbarManager.ShowMessage("参数为空");
                return false;
            }
            if (calibrationData.CameraParama[3] == 0.00000000)
            {
                SnackbarManager.ShowMessage("参数为空");
                return false;
            }
            if (calibrationData.CameraParama[4] == 0.00000000)
            {
                SnackbarManager.ShowMessage("参数为空");
                return false;
            }

            try
            {
                calibrationData = dataControler.GetData();
                calibrationData.DataGroup.Dispose();
                Calibaration.BulidDataGroup(calibrationData.CameraParama, calibrationData.DescrFileAddress, out calibrationData.DataGroup);
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
            if (calibrationData.PlateFileParama.Length < 1)
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
            if (calibrationData.PlateFileParama[3] < 0.00000001)
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
            SnackbarManager.ShowMessage(calibrationData.PlateFileParama[4] + "已保存");
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
        public void CreateCameraParama()
        {
            if (hImages.Count < 10)
            {
                SnackbarManager.ShowMessage("图片数量过少");
                return;
            }
            calibrationData = dataControler.GetData();
            calibrationData.DataGroup.Dispose();
            Calibaration.BulidDataGroup(calibrationData.CameraParama, calibrationData.DescrFileAddress, out calibrationData.DataGroup);
            dataControler.SetData(calibrationData);
            for (int i = 0; i < hImages.Count; i++)
            {
                Calibaration.FindCalib(hImages[i], out HObject contours, out HObject cross, calibrationData.DataGroup, i);
            }
            Calibaration.GetCameraCalibrationParama(calibrationData.DataGroup, calibrationData.PlateThick, out HTuple tmpCtrl_Errors, out HTuple cameraParameters, out HTuple cameraPose);
            calibrationData.CameraParamra_Finish = cameraParameters;
            calibrationData.CameraPose_Finish = cameraPose;
            dataControler.SetData(calibrationData);
        }
        //保存相机参数文件
        public void SavaCameraParamaToFile()
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
                    calibrationData.AddressForCameraParamFile = path;
                }
                else
                {
                    SnackbarManager.ShowMessage("文件未选择");
                }
                dataControler.SetData(calibrationData);
                HOperatorSet.WriteCamPar(calibrationData.CameraParamra_Finish, calibrationData.AddressForCameraParamFile);
                SnackbarManager.ShowMessage("保存成功");
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }
        //保存相机位姿文件
        public void SavaCameraPoseToFile()
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
                    calibrationData.AddressForCameraPoseFile = path;
                }
                else
                {
                    SnackbarManager.ShowMessage("文件未选择");
                }
                dataControler.SetData(calibrationData);
                HOperatorSet.WritePose(calibrationData.CameraPose_Finish, calibrationData.AddressForCameraPoseFile);
                SnackbarManager.ShowMessage("保存成功");
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }
        //加载相机参数文件
        public bool LoadCameraParamFile()
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
                    HOperatorSet.ReadCamPar(openFileDialog.FileName, out calibrationData.CameraParamra_Finish);
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
        //读取图像映射文件
        public void ReadHomMat2DForFile()
        {
            calibrationData = dataControler.GetData();
            HOperatorSet.ReadTuple(calibrationData.HomMat2DAddress, out calibrationData.HomMat2D);
            dataControler.SetData(calibrationData);
        }
        //MultiPoint
        //截取
        public async Task MultiPointCaptuerAsync()
        {
            CloseCamera();
            if (InitCameraHandle())
            {
                calibrationData = dataControler.GetData();
                try
                {
                    HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                    HOperatorSet.GrabImageStart(calibrationData.CameraHandle, -1);
                    isRuning = true;

                    calibrationData.hImage.Dispose();
                    // 采集图像
                    // HOperatorSet.GrabImage(out calibrationData.hImage, calibrationData.CameraHandle);
                    HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CameraHandle, -1);
                    //// 获取图像尺寸
                    HOperatorSet.GetImageSize(calibrationData.hImage, out HTuple width, out HTuple height);
                    // 设置 Halcon 图像显示尺寸，一般来说，图像会铺满 Halcon 控件，因此会有一定程度拉伸
                    HOperatorSet.SetPart(viewControler.MultipointCalibration.hSmartWindowControl.HalconWindow, 0, 0, height - 1, width - 1);

                    PointCalibration.FindCalibrationPoint(calibrationData.hImage, out HTuple arrRow, out HTuple arrCol);

                    calibrationData.CalibrationPoint_Row = arrRow;
                    calibrationData.CalibrationPoint_Clomn = arrCol;

                    if (arrRow.Length > 0 && arrCol > 0 && arrCol.Length == arrRow.Length)
                    {
                        HOperatorSet.DispObj(calibrationData.hImage, viewControler.MultipointCalibration.hSmartWindowControl.HalconWindow);
                        for (int i = 0; i < arrRow.Length; i++)
                        {
                            HOperatorSet.DispText(viewControler.MultipointCalibration.hSmartWindowControl.HalconWindow, (i + 1).ToString(), "image", arrRow[i], arrCol[i], "coral", new HTuple(), new HTuple());
                        }
                    }
                    else
                    {
                        // 显示图像
                        HOperatorSet.DispObj(calibrationData.hImage, viewControler.MultipointCalibration.hSmartWindowControl.HalconWindow);
                    }
                    // 设置原图像比例缩放，这个效果和双击左键效果一样
                    viewControler.MultipointCalibration.hSmartWindowControl.SetFullImagePart();
                    await Task.Delay(100);
                    CloseCamera();
                    dataControler.SetData(calibrationData);
                }
                catch (Exception e)
                {
                    SnackbarManager.ShowMessage(e.Message);
                }
            }

        }
        public void MultiPointGenHomMat2DFile()
        {
            calibrationData = dataControler.GetData();
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = AppContext.BaseDirectory;
                saveFileDialog.Filter = "HomMat2D数据|*.*";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    calibrationData.HomMat2DAddress = saveFileDialog.FileName;
                    GetPoint();
                    PointCalibration.GetAndSaveHomMat2D(calibrationData.CalibrationPoint_Clomn, calibrationData.CalibrationPoint_Row, calibrationData.PointGroup.col, calibrationData.PointGroup.row, calibrationData.HomMat2DAddress, out calibrationData.HomMat2D);
                    dataControler.SetData(calibrationData);
                    SnackbarManager.ShowMessage("保存成功");
                }
                else
                {
                    SnackbarManager.ShowMessage("文件未选择");
                }
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }
        public void GetPoint()
        {
            calibrationData = dataControler.GetData();
            //九点
            if (viewControler.MultipointCalibration.tab9or12Point.SelectedIndex == 0)
            {
                // 设置X坐标值
                calibrationData.PointGroup.row[0] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint1For9Point.Text);
                calibrationData.PointGroup.row[1] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint2For9Point.Text);
                calibrationData.PointGroup.row[2] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint3For9Point.Text);
                calibrationData.PointGroup.row[3] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint4For9Point.Text);
                calibrationData.PointGroup.row[4] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint5For9Point.Text);
                calibrationData.PointGroup.row[5] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint6For9Point.Text);
                calibrationData.PointGroup.row[6] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint7For9Point.Text);
                calibrationData.PointGroup.row[7] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint8For9Point.Text);
                calibrationData.PointGroup.row[8] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint9For9Point.Text);

                // 设置Y坐标值
                calibrationData.PointGroup.col[0] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint1For9Point.Text);
                calibrationData.PointGroup.col[1] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint2For9Point.Text);
                calibrationData.PointGroup.col[2] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint3For9Point.Text);
                calibrationData.PointGroup.col[3] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint4For9Point.Text);
                calibrationData.PointGroup.col[4] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint5For9Point.Text);
                calibrationData.PointGroup.col[5] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint6For9Point.Text);
                calibrationData.PointGroup.col[6] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint7For9Point.Text);
                calibrationData.PointGroup.col[7] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint8For9Point.Text);
                calibrationData.PointGroup.col[8] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint9For9Point.Text);


            }
            //十二点
            if (viewControler.MultipointCalibration.tab9or12Point.SelectedIndex == 2)
            {
                // 设置X坐标值
                calibrationData.PointGroup.row[0] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint1For12Point.Text);
                calibrationData.PointGroup.row[1] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint2For12Point.Text);
                calibrationData.PointGroup.row[2] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint3For12Point.Text);
                calibrationData.PointGroup.row[3] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint4For12Point.Text);
                calibrationData.PointGroup.row[4] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint5For12Point.Text);
                calibrationData.PointGroup.row[5] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint6For12Point.Text);
                calibrationData.PointGroup.row[6] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint7For12Point.Text);
                calibrationData.PointGroup.row[7] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint8For12Point.Text);
                calibrationData.PointGroup.row[8] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint9For12Point.Text);
                calibrationData.PointGroup.row[9] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint10For12Point.Text);
                calibrationData.PointGroup.row[10] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint11For12Point.Text);
                calibrationData.PointGroup.row[11] = Convert.ToDouble(viewControler.MultipointCalibration.txtXpoint12For12Point.Text);

                // 设置Y坐标值
                calibrationData.PointGroup.col[0] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint1For12Point.Text);
                calibrationData.PointGroup.col[1] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint2For12Point.Text);
                calibrationData.PointGroup.col[2] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint3For12Point.Text);
                calibrationData.PointGroup.col[3] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint4For12Point.Text);
                calibrationData.PointGroup.col[4] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint5For12Point.Text);
                calibrationData.PointGroup.col[5] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint6For12Point.Text);
                calibrationData.PointGroup.col[6] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint7For12Point.Text);
                calibrationData.PointGroup.col[7] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint8For12Point.Text);
                calibrationData.PointGroup.col[8] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint9For12Point.Text);
                calibrationData.PointGroup.col[9] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint10For12Point.Text);
                calibrationData.PointGroup.col[10] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint11For12Point.Text);
                calibrationData.PointGroup.col[11] = Convert.ToDouble(viewControler.MultipointCalibration.txtYpoint12For12Point.Text);

            }
            dataControler.SetData(calibrationData);
        }
        public async void OpenPosGuidance()
        {
            try
            {
                calibrationData = dataControler.GetData();
                if (calibrationData.HomMat2D.Type == HTupleType.EMPTY)
                {
                    throw new Exception("HomMat2D数据为空");
                }
                if (!modbusFunction.isConnect)
                {
                    throw new Exception("Modbus未连接");
                }
                CloseCamera();
                if (InitCameraHandle())
                {
                    calibrationData = dataControler.GetData();
                    HOperatorSet.GenEmptyObj(out calibrationData.hImage);
                    HOperatorSet.GrabImageStart(calibrationData.CameraHandle, -1);
                    isRuning = true;
                    calibrationData.hImage.Dispose();

                    while (isRuning)
                    {
                        try
                        {
                            await Task.Delay(2000);
                            HOperatorSet.GrabImageAsync(out calibrationData.hImage, calibrationData.CameraHandle, -1);
                            HOperatorSet.GetImageSize(calibrationData.hImage, out HTuple width, out HTuple height);
                            HOperatorSet.SetPart(viewControler.PositioningGuidance.hSmartWindowControl.HalconWindow, 0, 0, height - 1, width - 1);
                            // 显示图像
                            HOperatorSet.DispObj(calibrationData.hImage, viewControler.PositioningGuidance.hSmartWindowControl.HalconWindow);
                            // 设置原图像比例缩放
                            viewControler.PositioningGuidance.hSmartWindowControl.SetFullImagePart();
                            GetPhysicalCoordinates.GetPhysicalCoordinate(calibrationData.hImage, out HObject circle, calibrationData.HomMat2D, out HTuple Qx, out HTuple Qy);
                            HOperatorSet.DispObj(circle, viewControler.PositioningGuidance.hSmartWindowControl.HalconWindow);
                            int[] ints = new int[2];
                            double tempQx = Qx;
                            double tempQy = Qy;
                            ints[0] = ((int)tempQx); ints[1] = ((int)tempQy);
                            viewControler.PositioningGuidance.txtImageProcessinglog.Text += "\r\n X:" + ints[0].ToString() + "    Y:" + ints[1].ToString();
                            modbusFunction.WriteRegisters(ints);
                            dataControler.SetData(calibrationData);
                        }
                        catch (HDevEngineException he)
                        {
                            if (he.HalconError == 1401)
                            {
                                SnackbarManager.ShowMessage("未找到目标");
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        
                    }
                }
            }
            
            catch (Exception e)
            {
                SnackbarManager.ShowMessage(e.Message);
            }
        }
        public void ClosePostGuide()
        {
            CloseCamera();
            modbusFunction.Disconnect();
        }
    }
}

