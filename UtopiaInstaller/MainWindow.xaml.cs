using System;
using System.Collections.Generic;
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
using static UtopiaInstaller.GameArchive;

namespace UtopiaInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void FlushGameList()
        {
            GameList.Items.Clear();

            foreach(var game in GameArchive.archive.GameVersions)
            {
                GameList.Items.Add(game);
            }
        }

        public MainWindow()
        {
            LogManager.EnableConfig();
            GameArchive.ReadSettings();

            InitializeComponent();

            FlushGameList();

            Show();

            // 检查是否下载了运行时
            if (!DownloadTool.CheckRuntime())
            {
                MessageBox.Show("未检测到运行时", "note", MessageBoxButton.OK, MessageBoxImage.Question);
                DownloadRuntime downloadWindow = new();
                downloadWindow.Owner = this;
                downloadWindow.ShowInTaskbar = false;
                downloadWindow.ShowDialog();
            }
        }

        /// <summary>
        /// 下载运行时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadRuntimeBtn_Click(object sender, RoutedEventArgs e)
        {
            DownloadRuntime downloadWindow = new();
            downloadWindow.Owner = this;
            downloadWindow.DataContext = DataContext;
            downloadWindow.ShowInTaskbar = false;
            _ = downloadWindow.ShowDialog();
        }

        private void DownloadNewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            DownloadNewGame downloadWindow = new();
            downloadWindow.Owner = this;
            downloadWindow.ShowInTaskbar = false;
            _ = downloadWindow.ShowDialog();

            FlushGameList();
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveSettings();
            base.OnClosed(e);
        }

        private void GameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(GameList.SelectedItem != null)
            {
                var archive = (GameVersion)GameList.SelectedItem;
                GameInfoLabel.Content = 
                    "路径" + archive.Path + "\n版本号:" + archive?.Tag?.Name + "\nsha:" + archive?.Tag?.GitCommit?.Sha;
            }
            else
            {
                GameInfoLabel.Content = string.Empty;
            }
        }

        /// <summary>
        /// 设置jvm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JvmSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
