using Downloader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace CltOnekey
{
    /// <summary>
    /// Interaction logic for DownloadPage.xaml
    /// </summary>
    public partial class DownloadPage : Window
    {
        public ObservableCollection<CltOnekeyBeatmap> Queue = new ObservableCollection<CltOnekeyBeatmap>();
        private int position = 0;
        public DownloadPage(List<CltOnekeyBeatmap> misMatchedMaps)
        {
            InitializeComponent();
            snackBar.MessageQueue.Enqueue(string.Format("正在下载缺失的{0}个谱面", misMatchedMaps.Count));
            treeView.ItemsSource = Queue;
            foreach (var item in misMatchedMaps)
            {
                if (item.BID == 0 || item.SID == 0) continue;
                Queue.Add(item);
            }
            BeginDownloadFiles();
        }

        public async void BeginDownloadFiles()
        {
            DownloadService downloadService = new DownloadService(new DownloadConfiguration { ChunkCount = 1 });
            downloadService.DownloadProgressChanged += DownloadService_DownloadProgressChanged;
            for (; position < Queue.Count; position++)
            {
                await Dispatcher.BeginInvoke(new Action(() => { textPosition.Text = "正在下载: " + Queue[position].UnicodeTitle; }));
                await downloadService.DownloadFileTaskAsync(string.Format("https://dl.sayobot.cn/beatmaps/download/novideo/{0}", Queue[position].SID), Path.Combine(MainWindow.Database.GamePath, "Songs", Queue[position].SID.ToString() + ".osz"));
            }
            MainWindow.Ins.Dispatcher.Invoke(() =>
            {
                MainWindow.Ins.dialogHost.IsOpen = false;
                MainWindow.Database.CollectionDatabase = null;
                MainWindow.Ins.snackbar.MessageQueue.Enqueue("谱面补全已完成, 进入游戏自动导入");
            });
        }

        private void DownloadService_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            { 
                progressBar.Value = e.ProgressPercentage;
                Title = string.Format("({0}/{1}) {2} KB/s", position + 1, Queue.Count, Math.Round(e.BytesPerSecondSpeed / 1024, 2));
            }));
        }
    }
}
