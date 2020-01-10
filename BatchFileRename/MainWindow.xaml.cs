using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FullFolderRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _folder;
        public string Folder
        {
            get => _folder;
            set => TxtBlkSelectedFolder.Text = _folder = value;
        }
        public string Find { get; set; }
        public string Replace { get; set; }

        public bool ShouldRenameFiles { get; set; }
        public bool ShouldRenameFolders { get; set; }
        public bool ShouldRenameSubfolders { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Rename(string path)
        {
            if (string.IsNullOrEmpty(Folder))
            {
                MessageBox.Show("Please choose a folder");
                return;
            }
            if (string.IsNullOrEmpty(Find))
            {
                MessageBox.Show("Please write what text to find");
                return;
            }
            if (string.IsNullOrEmpty(Replace))
            {
                MessageBox.Show("Please write what text to replace");
                return;
            }

            if (ShouldRenameFiles)
            {
                RenameFiles(path);
            }

            if (ShouldRenameFolders)
            {
                RenameFolders(path);
            }

            MessageBox.Show("Done!");
        }

        private void RenameFiles(string path)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                var newPath = string.Empty;

                var fileParts = file.Split('\\');

                if (fileParts.Last().Contains(Find))
                {
                    for (int i = 0; i < fileParts.Length; i++)
                    {
                        var part = fileParts[i];
                        if (i == fileParts.Length - 1)
                        {
                            part = part.Replace(Find, Replace);
                        }

                        newPath = Path.Combine(newPath, part);
                    }

                    if (file != newPath)
                    {
                        Directory.Move(file, newPath);
                    }
                }
            }
        }

        private void RenameFolders(string path)
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                var dirFinal = dir;
                var newPath = string.Empty;

                var dirParts = dir.Split('\\');

                if (dirParts.Last().Contains(Find))
                {
                    for (int i = 0; i < dirParts.Length; i++)
                    {
                        var part = dirParts[i];
                        if (i == dirParts.Length - 1)
                        {
                            part = part.Replace(Find, Replace);
                        }

                        newPath = Path.Combine(newPath, part);
                    }

                    if (dir != newPath)
                    {
                        Directory.Move(dir, newPath);
                    }

                    dirFinal = newPath;
                }

                if (ShouldRenameSubfolders)
                {
                    Rename(dirFinal);
                }
            }
        }

        #region Folder Selection

        private void ButtonSelectDialog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            dialog.ShowDialog(this);
            Folder = dialog.FileName;
        }

        private void ButtonTakeAction_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to rename?",
                    "You may change back switching Find and Replace fields, but it will not ensure that files would get back their old names.",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            Find = TxtBoxFind.Text;
            Replace = TxtBoxReplace.Text;

            ShouldRenameFiles = ChkBoxFiles.IsChecked == true;
            ShouldRenameFolders = ChkBoxFolders.IsChecked == true;
            ShouldRenameSubfolders = ChkBoxSubfolders.IsChecked == true;

            Rename(Folder);
        }

        #endregion
    }
}
