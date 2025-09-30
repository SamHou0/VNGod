using System;
using System.Collections.Generic;
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

namespace VNGod.View
{
    /// <summary>
    /// GameSelectionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GameSelectionWindow : HandyControl.Controls.Window
    {
        public static string Result = string.Empty;
        public GameSelectionWindow(string path)
        {
            InitializeComponent();
            string[] filePaths= Directory.GetFiles(path);
            foreach (string filePath in filePaths)
            {
                if(filePath.EndsWith(".exe"))
                    gameListBox.Items.Add(Path.GetFileName(filePath));
            }
        }

        private void gameListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Result=gameListBox.SelectedItem.ToString()??throw new Exception("Empty filename.");
            DialogResult = true;
            Close();
        }
    }
}
