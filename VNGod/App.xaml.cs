using log4net;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace VNGod
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Catch unhandled exceptions
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            ILog logger = LogManager.GetLogger("-Unhandled Task Exception-");
            logger.Error(e.Exception.Message, e.Exception);
            HandleRestart(e.Exception);
        }

        private static void HandleRestart(Exception? e)
        {
            if (e != null)
                MessageBox.Show("An unexpected error occurred:\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show("An unexpected error occurred.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Process.Start("VNGod.exe");
            Environment.Exit(1);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ILog logger = LogManager.GetLogger("-Unhandled Exception-");
            logger.Error((e.ExceptionObject as Exception)?.Message, e.ExceptionObject as Exception);
            HandleRestart(e.ExceptionObject as Exception);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ILog logger = LogManager.GetLogger("-Dispatcher Unhandled Exception-");
            logger.Error(e.Exception.Message, e.Exception);
            HandleRestart(e.Exception);
        }
    }

}
