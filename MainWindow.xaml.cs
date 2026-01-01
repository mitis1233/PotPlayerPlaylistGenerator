using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Forms;

namespace PotPlayerPlaylistGenerator
{
    public partial class MainWindow : Window
    {
        private string _rootVideoPath = string.Empty;
        private string[] _videoExtensions = Array.Empty<string>();
        private readonly string _configFilePath;

        public MainWindow()
        {
            InitializeComponent();
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            LoadFolders();
        }

        private void LoadConfig()
        {
            if (File.Exists(_configFilePath))
            {
                var lines = File.ReadAllLines(_configFilePath);
                if (lines.Length >= 1)
                {
                    _rootVideoPath = lines[0].Trim();
                }
                if (lines.Length >= 2)
                {
                    _videoExtensions = lines[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(ext => ext.Trim().ToLower())
                                             .ToArray();
                }
            }

            if (string.IsNullOrEmpty(_rootVideoPath))
            {
                _rootVideoPath = @"D:\Downloads"; // Default path
            }

            if (_videoExtensions.Length == 0)
            {
                _videoExtensions = new[] { ".mp4", ".mkv", ".avi", ".wmv", ".mov", ".flv", ".webm" }; // Default extensions
            }

            PathTextBox.Text = _rootVideoPath;
            ExtensionsTextBox.Text = string.Join(", ", _videoExtensions);
        }

        private void SaveConfig()
        {
            try
            {
                var lines = new[] { _rootVideoPath, string.Join(",", _videoExtensions) };
                File.WriteAllLines(_configFilePath, lines);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"儲存設定時發生錯誤: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFolders()
        {
            FolderListBox.ItemsSource = null;
            if (string.IsNullOrEmpty(_rootVideoPath) || !Directory.Exists(_rootVideoPath))
            {
                System.Windows.MessageBox.Show("目前的影片根目錄無效。請透過設定按鈕指定一個有效的資料夾。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var directories = Directory.GetDirectories(_rootVideoPath)
                                           .Select(path => new DirectoryInfo(path).Name)
                                           .ToList();
                FolderListBox.ItemsSource = directories;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"讀取資料夾列表時發生錯誤: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (FolderListBox.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("請先從列表中選擇一個資料夾。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selectedFolderName = FolderListBox.SelectedItem.ToString()!;
            string fullPath = Path.Combine(_rootVideoPath, selectedFolderName);

            try
            {
                var videoFiles = Directory.EnumerateFiles(fullPath, "*.*", SearchOption.AllDirectories)
                                          .Where(file => _videoExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
                                          .ToList();

                if (videoFiles.Count == 0)
                {
                    System.Windows.MessageBox.Show("在選定的資料夾及其子資料夾中找不到任何支援的影片檔案。", "找不到檔案", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string tempPlaylistPath = Path.Combine(Path.GetTempPath(), "potplayer_playlist.dpl");
                StringBuilder playlistContent = new StringBuilder();
                playlistContent.AppendLine("DAUMPLAYLIST");
                int index = 1;
                foreach (var file in videoFiles)
                {
                    playlistContent.AppendLine($"{index++}*file*{file}");
                }
                File.WriteAllText(tempPlaylistPath, playlistContent.ToString(), Encoding.UTF8);

                var psi = new ProcessStartInfo(tempPlaylistPath)
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                if (ex is System.ComponentModel.Win32Exception win32Ex && win32Ex.NativeErrorCode == 2)
                {
                    System.Windows.MessageBox.Show("無法啟動 PotPlayer。請確認您已安裝 PotPlayer，並且 .dpl 檔案已與其關聯。", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    System.Windows.MessageBox.Show($"播放時發生錯誤: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPanel.Visibility = SettingsPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "請選擇影片所在的根資料夾";
                if (Directory.Exists(PathTextBox.Text))
                {
                    dialog.SelectedPath = PathTextBox.Text;
                }

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PathTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newPath = PathTextBox.Text.Trim();
            if (!Directory.Exists(newPath))
            {
                System.Windows.MessageBox.Show("您輸入的路徑無效，請重新輸入或瀏覽。", "路徑無效", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _rootVideoPath = newPath;
            _videoExtensions = ExtensionsTextBox.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(ext => ext.Trim().ToLower())
                                                 .ToArray();

            if (_videoExtensions.Length == 0)
            {
                System.Windows.MessageBox.Show("副檔名列表不能為空。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveConfig();
            LoadFolders();
            SettingsPanel.Visibility = Visibility.Collapsed;
            System.Windows.MessageBox.Show("設定已成功儲存並更新。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TitleBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string tempPlaylistPath = Path.Combine(Path.GetTempPath(), "potplayer_playlist.dpl");
                if (File.Exists(tempPlaylistPath))
                {
                    File.Delete(tempPlaylistPath);
                }
            }
            catch
            {
                // Ignore errors on closing
            }
        }
    }
}