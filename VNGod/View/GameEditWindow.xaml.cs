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
using System.Windows.Shapes;
using VNGod.Data;
using VNGod.Services;

namespace VNGod.View
{
    /// <summary>
    /// GameEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GameEditWindow : HandyControl.Controls.Window
    {
        public GameEditWindow(Game game)
        {
            InitializeComponent();
            DataContext = game;
        }

        private async void getInfoButton_Click(object sender, RoutedEventArgs e)
        {
            getInfoButton.IsEnabled = false;
            Game game = DataContext as Game ?? throw new Exception("Error getting game.");
            game = await NetworkService.GetBangumiInfoAsync(game);
            getInfoButton.IsEnabled = true;
        }
    }
}
