using CamaraCalibration.View;
using System;
using System.Collections.Generic;
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

namespace CamaraCalibration
{
    /// <summary>
    /// CreateDescrFile.xaml 的交互逻辑
    /// </summary>
    
    public partial class CreateDescrFile : Page
    {
        CalibrationData calibrationData;
        DataControler dataControler;
        public CreateDescrFile(DataControler data)
        {
            InitializeComponent();
            dataControler = data;
        }

        private void txtColumnNum_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtColumnNum.Text == "") return;
            try
            {
                int value = Convert.ToInt32(txtColumnNum.Text);
                if (value >= 1)
                {
                    calibrationData = dataControler.GetData();
                    calibrationData.PlateFileParama[0] = Convert.ToInt32(txtColumnNum.Text);
                    dataControler.SetData(calibrationData);
                }
                else
                {
                    txtColumnNum.Text = null;
                    SnackbarManager.ShowMessage("最小列数为1");
                }
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }

        private void txtRowNum_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtRowNum.Text == "") return;
            try
            {
                int value = Convert.ToInt32(txtRowNum.Text);
                if (value >= 1)
                {
                    calibrationData = dataControler.GetData();
                    calibrationData.PlateFileParama[1] = Convert.ToInt32(txtRowNum.Text);
                    dataControler.SetData(calibrationData);
                }
                else
                {
                    txtRowNum.Text = null;
                    SnackbarManager.ShowMessage("最小列数为1");
                }
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }

        private void txtCentres_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCentres.Text == "") return;
            try
            {
                double value = Convert.ToDouble(txtCentres.Text);
                if (value > 0)
                {
                    calibrationData = dataControler.GetData();
                    calibrationData.PlateFileParama[2] = Convert.ToDouble(txtCentres.Text);
                    dataControler.SetData(calibrationData);
                }
                else
                {
                    SnackbarManager.ShowMessage("距离须>0");
                }
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }

        private void txtCentresRate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCentresRate.Text == "") return;
            try
            {
                calibrationData = dataControler.GetData();

                double value = Convert.ToDouble(txtCentresRate.Text);
                if (value >= 0.00000001 && value < 1.0000000)
                {
                    calibrationData.PlateFileParama[3] = Convert.ToDouble(txtCentresRate.Text);
                    dataControler.SetData(calibrationData);
                }
                else
                {
                    txtCentresRate.Text = null;
                    SnackbarManager.ShowMessage("中心间距比例为0<x<1");
                }
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
            
            
        }

        private void btnSaveDescrFileAddress_Click(object sender, RoutedEventArgs e)
        {
            calibrationData = dataControler.GetData();
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = AppContext.BaseDirectory;
                saveFileDialog.Filter = "All Files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = saveFileDialog.FileName;
                    calibrationData.PlateFileParama[4] = path + ".descr";
                    calibrationData.PlateFileParama[5] = path + ".ps";
                    txtSaveDescrFileAddress.Text = saveFileDialog.FileName;
                }
                else
                {
                    SnackbarManager.ShowMessage("文件未选择");
                }
                dataControler.SetData(calibrationData);
            }
            catch (Exception ex)
            {
                SnackbarManager.ShowMessage(ex.Message);
            }
        }
    }
}
