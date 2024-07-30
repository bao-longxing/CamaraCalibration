using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using CamaraCalibration.View;
using MaterialDesignThemes.Wpf;

namespace CamaraCalibration
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public static class SnackbarManager
    {
        private static Snackbar _snackbar;
        public static void Initialize(Snackbar snackbar)
        {
            _snackbar = snackbar;
        }
        public static void ShowMessage(string message)
        {
            if (_snackbar != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _snackbar.MessageQueue.Clear();
                    _snackbar.MessageQueue.Enqueue(message);
                });
            }
        }
    }
    public partial class MainWindow : Window
    {
        private CalibrationData calibrationData;
        private DataControler dataControler;
        private Camara CamaraPage;
        private Calibrating CalibratingPage;
        private HalconFuntion HalconFuntion;
        private ViewControler ViewControler;
        private CreateDescrFile CreateDescrFile;
        private CamaraParamaList camaraParamaList;
        private AberrationCorrection correction;
        public MainWindow()
        {
            InitializeComponent();
            SnackbarManager.Initialize(this.MessageQueue);
            calibrationData = new CalibrationData();
            dataControler = new DataControler(calibrationData);
            CamaraPage = new Camara(dataControler);
            CalibratingPage = new Calibrating();
            CreateDescrFile = new CreateDescrFile(dataControler);
            camaraParamaList = new CamaraParamaList();
            correction = new AberrationCorrection();
            ViewControler = new ViewControler(this, CamaraPage, CalibratingPage, CreateDescrFile,camaraParamaList,dataControler, correction);
            HalconFuntion = new HalconFuntion(dataControler, ViewControler);
            ViewControler.SetHalconFunction(HalconFuntion);
            AppFrame.Navigate(CamaraPage);
        }
        
    }
}
