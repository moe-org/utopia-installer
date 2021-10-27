using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UtopiaInstaller
{
    /// <summary>
    /// DownloadNewGame.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadNewGame : Window
    {
        public const string GameArchiveDir = "./games";

        public DownloadNewGame()
        {
            InitializeComponent();
            DownloadBtn.IsEnabled = false;

            DownloadProgress.Maximum = 1;
            DownloadProgress.Minimum = 0;
            DownloadProgress.Value = 0;
        }


        /// <summary>
        /// 刷新版本号列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FlushVersionBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FlushVersionBtn.IsEnabled = false;

                var list = await GithubAccesser.GetVersionList();

                list = list.Distinct().ToList();

                GameVersionList.Items.Clear();
                foreach (var item in list)
                    GameVersionList.Items.Add(item);
            }
            finally
            {
                FlushVersionBtn.IsEnabled = true;
            }
        }

        private void GameVersionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = GameVersionList.SelectedItem;

            if (selected != null)
            {
                var tag = (GitTag)selected;
                GameVersionInfo.Content = "版本:" + tag.Name + "\n提交sha:" + tag.GitCommit?.Sha;
                DownloadBtn.IsEnabled = true;
            }
            else
            {
                DownloadBtn.IsEnabled = false;
            }
        }

        private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadBtn.IsEnabled = false;
                if (GameVersionList.SelectedItem == null)
                {
                    return;
                }
                else
                {
                    FlushVersionBtn.IsEnabled = false;

                    // 下载游戏
                    var tag = (GitTag)GameVersionList.SelectedItem;

                    IProgress<double> progress = new Progress<double>(
                            value =>
                            {
                                DownloadProgress.Value = value;
                            }
                        );

                    await DownloadTool.DownloadAndExtractAndDeleteTemp(
                        tag.ZipballUrl!,
                        $"{DownloadTool.TEMP_DIR}/{tag.Name}.zip",
                        $"{GameArchiveDir}/{tag.Name}",
                        progress
                    );

                    // 记录
                    GameArchive.archive.GameVersions!.Add(
                        new GameArchive.GameVersion()
                        {
                            Path = Tool.GetOnlyDir($"{GameArchiveDir}/{tag.Name}"),
                            Tag = tag,
                        });

                    // 保存
                    GameArchive.SaveSettings();
                }
            }
            finally
            {
                FlushVersionBtn.IsEnabled = true;
                DownloadProgress.Value = 0;
                GameVersionList.SelectedItem = null;
            }
        }
    }
}
