using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TimeKeeper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow  
    {
        private DispatcherTimer _secondTickTimer;
        private DispatcherTimer _writeEvery10MinTimer;

        public MainWindow()
        {
            InitializeComponent();
            started = DateTime.Now;
            SetupTImers();

            var dayFile = GetDayFilePath();
            if (File.Exists(dayFile))
            {
                var content = File.ReadAllLines(dayFile);
                var startedLine = content[0];
                var stoppedLine = content[1];
                var ranLine = content[2];

                var startedString = startedLine.Substring("Started: ".Length);

                started = DateTime.Parse(startedString);

                Write(null,null);
            }
            else
            {
                File.WriteAllText(dayFile,
                    $@"Started: {started.TimeOfDay}
stopped: -
ran: -");
            }
        }

        private void SetupTImers()
        {
            _secondTickTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _secondTickTimer.Tick += Tick;
            _secondTickTimer.Start();

            _writeEvery10MinTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMinutes(10)
            };
            _writeEvery10MinTimer.Tick += WriteTick;
            _writeEvery10MinTimer.Start();
        }

        private void WriteTick(object sender, EventArgs e)
        {
            Write(null,null);
        }

        private void Tick(object sender, EventArgs eventArgs)
        {
                var timeSpan = DateTime.Now - started;
                Label.Content = timeSpan.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        public DateTime started { get; set; }


        // minimize to system tray when applicaiton is minimized
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                _secondTickTimer.Stop();
            }

            base.OnStateChanged(e);
        }

        // minimize to system tray when applicaiton is closed
        protected override void OnClosing(CancelEventArgs e)
        {
            // setting cancel to true will cancel the close request
            // so the application is not closed
            e.Cancel = true;

            Hide();
            _secondTickTimer.Stop();

            base.OnClosing(e);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            Show();
            _secondTickTimer.Start();
        }

        private void DoubleClickTray(object sender, RoutedEventArgs e)
        {
            Show();
            _secondTickTimer.Start();
            Focus();
        }

        private void Write(object sender, RoutedEventArgs e)
        {
            var dayFile = GetDayFilePath();
            File.WriteAllText(dayFile,
$@"Started: {started.ToShortTimeString()}
stopped: {DateTime.Now.ToShortTimeString()}
ran: {(DateTime.Now - started)}");
            //File.AppendAllText(folder + "All.txt", $@"{today}: { DateTime.Now - started}");
        }

        private static string GetDayFilePath()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TimeKeeper\\";
            Directory.CreateDirectory(folder);
            var today = DateTime.Today.Date.ToShortDateString().Replace("/", "-");
            var dayFile = folder + today;
            return dayFile;
        }
    }
}
