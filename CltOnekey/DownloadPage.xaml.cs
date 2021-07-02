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
        public DownloadPage(List<CltOnekeyBeatmap> misMatchedMaps)
        {
            InitializeComponent();
            DownloadService downloadService = new DownloadService(new DownloadConfiguration { ChunkCount = 1 });
            downloadService.DownloadProgressChanged += DownloadService_DownloadProgressChanged;
            downloadService.DownloadFileCompleted += DownloadService_DownloadFileCompleted;
            foreach (var item in misMatchedMaps)
            {
                Queue.Add(item);
                downloadService.DownloadFileTaskAsync(string.Format("https://dl.sayobot.cn/beatmaps/download/novideo/{0}", item.SID), Path.Combine(MainWindow.Database.GamePath, "Songs"));
            }
        }

        private void DownloadService_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("OK");
        }

        private void DownloadService_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
    }
}
