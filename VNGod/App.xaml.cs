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
            SentrySdk.Init(o =>
            {
                // Tells which project in Sentry to send events to:
                o.Dsn = "https://df662d1fe5a888d639cbe919b4ab24fd@o4510685531734016.ingest.us.sentry.io/4510685538942976";
            });
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            ILog logger = LogManager.GetLogger("-Unhandled Task Exception-");
            logger.Error(e.Exception.Message, e.Exception);
            HandleRestart(e.Exception);
        }

        private static void HandleRestart(Exception? e)
        {
            SentrySdk.CaptureException(e!);
            if (e != null)
            {
                if (
                MessageBox.Show("遇到了未经处理的异常，已自动发送错误报告，要重启吗？", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    Process.Start("VNGod.exe");
                }
            }
            else
            {
                if (
                MessageBox.Show("遇到了未经处理的异常，已自动发送错误报告，要重启吗？", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    Process.Start("VNGod.exe");
                }

            }
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
