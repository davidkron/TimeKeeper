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
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            started = DateTime.Now;
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Tick;
            _timer.Start();



            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TimeKeeper\\";
            Directory.CreateDirectory(folder);
            var today = DateTime.Today.Date.ToShortDateString().Replace("/","-");
            var dayFile = folder + today;
            if (File.Exists(dayFile))
            {
                var content = File.ReadAllLines(dayFile);
                var startedLine = content[0];
                var stoppedLine = content[1];
                var ranLine = content[2];

                var startedString = startedLine.Substring("Started: ".Length);

                started = DateTime.Parse(startedString);

                File.WriteAllText(dayFile,
                    $@"Started: {started.TimeOfDay}
stopped: -
ran: -");
            }
            else
            {
                File.WriteAllText(dayFile,
                    $@"Started: {started.TimeOfDay}
stopped: -
ran: -");
            }
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
                _timer.Stop();
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
            _timer.Stop();

            base.OnClosing(e);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            Show();
            _timer.Start();
        }

        private void DoubleClickTray(object sender, RoutedEventArgs e)
        {
            Show();
            _timer.Start();
            Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TimeKeeper\\";
            System.IO.Directory.CreateDirectory(folder);
            var today = DateTime.Today.Date.ToShortDateString();
            File.WriteAllText(folder+ today,
$@"Started: {started}
stopped: {DateTime.Now}
ran: {DateTime.Now - started}");
            File.AppendAllText(folder + "All.txt", $@"{today}: { DateTime.Now - started}");

        }
    }
}
