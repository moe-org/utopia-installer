using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace UtopiaInstaller
{
    /// <summary>
    /// Download.xaml的交互逻辑
    /// </summary>
    public partial class DownloadRuntime : Window
    {
        private void SetRemoveRuntimeBtnStatus()
        {
            if (DownloadTool.CheckRuntime())
            {
                WarnLabel.Content = 
                    "注意:runtime已经存在!可删除后重新下载。\nRuntime(JAVA_HOME)路径:" 
                    + DownloadTool.GetRuntime();
                RemoveRuntimeBtn.IsEnabled = true;
            }
            else
            {
                WarnLabel.Content = string.Empty;
                RemoveRuntimeBtn.IsEnabled = false;
            }
        }

        public DownloadRuntime()
        {
            InitializeComponent();

            RuntimeDownloadUrl.Text = DownloadTool.RUNTIME_DOWNLOAD_URL;

            ProcessProgress.Maximum = 1;
            ProcessProgress.Minimum = 0;

            SetRemoveRuntimeBtnStatus();
        }

        private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadBtn.IsEnabled = false;
                RemoveRuntimeBtn.IsEnabled = false;
                RuntimeDownloadUrl.IsEnabled = false;
               
                var runtimeDownloadProgress = new Progress<double>(
                    value =>
                    {
                        ProcessProgress.Value = value;

                        if(value >= 1)
                        {
                            Unzip.Content = "解压中...";
                        }
                    });

                var runtime = DownloadTool.DownloadRuntime(
                       RuntimeDownloadUrl.Text == "默认" || RuntimeDownloadUrl.Text == string.Empty
                       ? DownloadTool.RUNTIME_DOWNLOAD_URL
                       : RuntimeDownloadUrl.Text,
                       runtimeDownloadProgress);

                await runtime;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Unzip.Content = string.Empty;
                ProcessProgress.Value = 0;
                DownloadBtn.IsEnabled = true;
                RuntimeDownloadUrl.IsEnabled = true;
                SetRemoveRuntimeBtnStatus();
            }
        }

        private void RemoveRuntimeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DownloadTool.CheckRuntime())
            {
                DownloadTool.RemoveRuntime();
            }
            SetRemoveRuntimeBtnStatus();
        }
    }
}
