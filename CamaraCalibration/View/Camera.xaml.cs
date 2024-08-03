using CameraCalibration.View;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CameraCalibration
{
    /// <summary>
    /// Camera.xaml 的交互逻辑
    /// </summary>
    public partial class Camera : Page
    {
        CalibrationData calibrationData = null;
        DataControler dataControler = null;
        public Camera(DataControler data)
        {
            InitializeComponent();
            dataControler = data;
        }

      

        private void btnSeclectDescrFile_Click(object sender, RoutedEventArgs e)
        {
            calibrationData = dataControler.GetData();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = AppContext.BaseDirectory;
                openFileDialog.Filter = "标定描述文件|*.descr";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    calibrationData.DescrFileAddress = openFileDialog.FileName;
                    txtDesrFileAddress.Text = openFileDialog.FileName;
                }
                dataControler.SetData(calibrationData);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void txtSinglePixelWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSinglePixelWidth.Text == "")
            {
                return;
            }
            calibrationData = dataControler.GetData();
            calibrationData.CameraParama[3] = double.Parse(txtSinglePixelWidth.Text) * 1e-6;
            dataControler.SetData(calibrationData); 
        }

        private void txtSinglePixelHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSinglePixelHeight.Text=="")
            {
                return;
            }
            calibrationData = dataControler.GetData();
            calibrationData.CameraParama[4] = double.Parse(txtSinglePixelHeight.Text) * 1e-6;
            dataControler.SetData(calibrationData);
        }

        private void txtFocalLength_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtFocalLength.Text== "")
                {
                    return;
                }
                calibrationData = dataControler.GetData();
                calibrationData.CameraParama[1] = Convert.ToDouble(txtFocalLength.Text) / 1000;
                dataControler.SetData(calibrationData);
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }

        private void txtPlateThick_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtPlateThick.Text == "")
            {
                return;
            }
            try
            {
                calibrationData = dataControler.GetData();
                calibrationData.PlateThick = Math.Round(Convert.ToDouble(txtPlateThick.Text) / 1000, 6);
                dataControler.SetData(calibrationData);
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
            
        }
    }
}
