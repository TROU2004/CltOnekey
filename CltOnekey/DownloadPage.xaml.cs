using Downloader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace CltOnekey
{
    /// <summary>
    /// Interaction logic for DownloadPage.xaml
    /// </summary>
    public partial class DownloadPage : UserControl
    {
        public ObservableCollection<CltOnekeyBeatmap> Queue = new ObservableCollection<CltOnekeyBeatmap>();
        private int num = 0;
        public DownloadPage(List<CltOnekeyBeatmap> misMatchedMaps)
        {
            InitializeComponent();
            snackBar.MessageQueue.Enqueue(string.Format("正在下载缺失的{0}个谱面", misMatchedMaps.Count));
            treeView.ItemsSource = Queue;
            progressBar.Maximum = Queue.Count;
            foreach (var item in misMatchedMaps)
            {
                Queue.Add(item);
                DownloadService downloadService = new DownloadService(new DownloadConfiguration { ChunkCount = 1 });
                downloadService.DownloadProgressChanged += DownloadService_DownloadProgressChanged;
                downloadService.DownloadFileCompleted += DownloadService_DownloadFileCompleted;
                downloadService.DownloadFileTaskAsync(string.Format("https://dl.sayobot.cn/beatmaps/download/novideo/{0}", item.SID), Path.Combine(MainWindow.Database.GamePath, "Songs", item.SID.ToString(), ".osz"));
            }
        }

        private void DownloadService_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            num++;
            if (num >= Queue.Count)
            {
                MainWindow.Ins.Dispatcher.Invoke(() =>
                {
                    MainWindow.Ins.dialogHost.IsOpen = false;
                    MainWindow.Database.CollectionDatabase = null;
                    MainWindow.Ins.snackbar.MessageQueue.Enqueue("谱面补全已完成, 进入游戏自动导入");
                });
            }
        }

        private void DownloadService_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => { progressBar.Value = num / Queue.Count; }));
        }
    }
}
