using FolderBrowserEx;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using OsuParsers.Database;
using OsuParsers.Decoders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace CltOnekey
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Database Database { get; set; }
        public static MainWindow Ins { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Ins = this;
            InitDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!", "osu!.db"));
        }

        private void Drag(object sender, MouseButtonEventArgs e) => DragMove();

        private void Close(object sender, RoutedEventArgs e) => Close();

        private void Minimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void text_Hint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "osu! Beatmap Database (osu!.db)|osu!.db" };
            dialog.ShowDialog();
            InitDatabase(dialog.FileName);
        }

        private void InitDatabase(string osudb)
        {
            progressBar.Visibility = Visibility.Visible;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (obj, arg) =>
            {
                try { Database = new Database(DatabaseDecoder.DecodeOsu(osudb), Path.GetDirectoryName(osudb)); }
                catch { }
            };
            worker.RunWorkerCompleted += (obj, arg) =>
            {
                progressBar.Visibility = Visibility.Hidden;
                if (Database != null)
                {
                    Title = string.Format("CltOnekey ({0}个谱面已加载)", Database.OsuDatabase.BeatmapCount);
                    text_Hint.Text = string.Format("加载的谱面数据: {0}", osudb);
                    snackbar.MessageQueue.Enqueue(string.Format("加载了{0}个谱面数据", Database.OsuDatabase.BeatmapCount));
                }
                else
                {
                    Title = "CltOnekey (没有找到谱面)";
                    text_Hint.Text = "点击这里重新指定osu!.db";
                    snackbar.MessageQueue.Enqueue("没有找到osu!目录, 请手动指定");
                }
            };
            worker.RunWorkerAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "osu! Colllection Database (collection.db)|collection.db",
                InitialDirectory = Database.GamePath
            };
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                Database.CollectionDatabase = DatabaseDecoder.DecodeCollection(dialog.FileName);
                FolderBrowserDialog dialog1 = new FolderBrowserDialog
                {
                    Title = "选择CltOnekey格式的Collection目录保存位置",
                    InitialFolder = Environment.CurrentDirectory,
                    AllowMultiSelect = false
                };
                if (((int)dialog1.ShowDialog()) == 1)
                {
                    Directory.CreateDirectory(Path.Combine(dialog1.SelectedFolder, "collection"));
                    progressBar.Visibility = Visibility.Visible;
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += (obj, arg) =>
                    {
                        foreach (var collection in Database.CollectionDatabase.Collections)
                        {
                            Directory.CreateDirectory(Path.Combine(dialog1.SelectedFolder, "collection", collection.Name));
                            List<CltOnekeyBeatmap> CltOnekeyBeatmaps = CltOnekeyBeatmap.ConvertFromDbBeatmaps(Database.FindBeatmapsFromHashes(collection.MD5Hashes));
                            foreach (var item in CltOnekeyBeatmaps)
                            {
                                var name = Util.RemoveInvalidCharacters(string.Format("({0}){1} {2} [{3}].json", item.BID, item.Artist, item.Title, item.Difficulty));
                                File.WriteAllText(Path.Combine(dialog1.SelectedFolder, "collection", collection.Name, name), JsonConvert.SerializeObject(item));
                            }
                        }
                    };
                    worker.RunWorkerCompleted += (obj, arg) =>
                    {
                        progressBar.Visibility = Visibility.Hidden;
                        snackbar.MessageQueue.Enqueue("导出已完成!");
                    };
                    worker.RunWorkerAsync();
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var misMatchedMaps = new List<CltOnekeyBeatmap>();
            FolderBrowserDialog dialog1 = new FolderBrowserDialog
            {
                Title = "选择CltOnekey格式的Collection目录",
                InitialFolder = Environment.CurrentDirectory,
                AllowMultiSelect = false
            };
            if (((int)dialog1.ShowDialog()) == 1)
            {
                BackgroundWorker worker = new BackgroundWorker();
                progressBar.Visibility = Visibility.Visible;
                worker.DoWork += (obj, arg) =>
                {
                    CollectionDatabase collectionDatabase = new CollectionDatabase();
                    collectionDatabase.OsuVersion = 20210520;
                    var collections = Database.BuildCollections(dialog1.SelectedFolder, misMatchedMaps);
                    collectionDatabase.Collections = collections;
                    collectionDatabase.CollectionCount = collections.Count;
                    SaveFileDialog dialog = new SaveFileDialog
                    {
                        Filter = "osu! Colllection Database (collection.db)|collection.db",
                        InitialDirectory = Database.GamePath,
                    };
                    if (dialog.ShowDialog().GetValueOrDefault(false))
                    {
                        collectionDatabase.Save(dialog.FileName);
                    }
                };
                worker.RunWorkerCompleted += (obj, arg) =>
                {
                    progressBar.Visibility = Visibility.Hidden;
                    if (misMatchedMaps.Count > 0)
                    {
                        dialogHost.ShowDialog(new DownloadPage(misMatchedMaps));
                    }
                };
                worker.RunWorkerAsync();
            }
        }
    }
}
