using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using SMProject.Utils;

namespace SMProject
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CircularBuffer<Point> dataSampleBuffer;
        private readonly HardwareService hardwareService;
        private readonly Timer signalGeneratingTimer;
        private readonly int TickIntervalMs = 1;
        private int currentIterator;
        private Signal currentSignal;
        private IEnumerable<double> preCalculatedSamples;

        public MainWindow()
        {
            InitializeComponent();
            Title = "ADUC831 Signal generator v1.00";
            ;
            hardwareService = new HardwareService(DataReceived);
            dataSampleBuffer = new CircularBuffer<Point>(700);
            signalGeneratingTimer = new Timer(Callback, null, 0, TickIntervalMs);

            currentSignal = new Signal
            {
                Amplitude = 4,
                FallingTime = 2.5,
                Offset = 0,
                Period = 3,
                RisingTime = 0.5,
                StopTime = 0,
                TimePassed = 0,
                SignalType = SignalType.Triangle
            };

            preCalculatedSamples = DataDictionary.PreCalculatesSamplesDictionary[currentSignal.SignalType]
                .Invoke(currentSignal);

            currentIterator = -1;
            InitViews();
        }


        private async void DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                var sp = sender as SerialPort;
                var line = sp?.ReadLine();
                var displayResult = "";
                if (line.Length < 7)
                {
                    var statusCode = long.Parse(line.First().ToString());
                    switch (statusCode)
                    {
                        case 0:
                            displayResult = $"Status code: {statusCode} - everything fine, signal set.";
                            break;
                        case 4:
                            displayResult = $"Status code: {statusCode} - parameters count is not valid.";
                            break;
                        case 2:
                            displayResult =
                                $"Status code: {statusCode} - maximum amplitude exceeded, setting value to 3.0.";
                            break;
                        case 3:
                            displayResult =
                                $"Status code: {statusCode} - maximum amplitude with offset exceeded , setting value to 3.0.";
                            break;
                        case 1:
                            displayResult =
                                $"Status code: {statusCode} - parameters' time windows mismatch - all should sum up to the period time, check the integrity of preview.";
                            break;
                    }
                }
                else
                {
                    displayResult = $"Port returned message: {line.Replace("\r\n", "")}";
                }

                WHATHAPPENEDTEXTBLOCK.Text += displayResult + Environment.NewLine;
            });
        }

        private void InitViews()
        {
            COMComboBox.ItemsSource = hardwareService.AllowedPortNames;
            SignalTypeComboBox.SelectedItem = currentSignal.SignalType;
            COMComboBox.SelectedItem = hardwareService.CurrentPortName;
            COMComboBox.SelectionChanged += OnComComboBoxChanged;
            AmplitudeTextBox.Text = currentSignal.Amplitude.ToString(CultureInfo.InvariantCulture);
            PeriodTextBox.Text = currentSignal.Period.ToString(CultureInfo.InvariantCulture);
            OffsetTextBox.Text = currentSignal.Offset.ToString(CultureInfo.InvariantCulture);
            RisingPointTextBox.Text = currentSignal.RisingTime.ToString(CultureInfo.InvariantCulture);
            FallingPointTextBox.Text = currentSignal.FallingTime.ToString(CultureInfo.InvariantCulture);
            StopPointTextBox.Text = currentSignal.StopTime.ToString(CultureInfo.InvariantCulture);

            SignalParametersTextBlock.Text = $"Data frame to send : {new DataFrame(currentSignal)}";
        }

        private void Callback(object state)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
                () =>
                {
                    DrawingSurface.Children.Clear();
                    currentIterator++;
                    currentIterator =
                        currentIterator % dataSampleBuffer.Length;
                    var value = preCalculatedSamples.ElementAt(currentIterator);
                    dataSampleBuffer.Add(new Point(currentIterator, value));

                    var line = new Polyline
                    {
                        Points = new PointCollection(dataSampleBuffer.DataBuffer),

                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Black)
                    };


                    DrawingSurface.Children.Add(line);
                }
            ));
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dataFrame = new DataFrame(currentSignal);

            WHATHAPPENEDTEXTBLOCK.Text += hardwareService.Send(dataFrame.ToString());
        }


        private void OnComComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            hardwareService.CurrentPortName = (sender as ComboBox)?.SelectedItem as string;
            if (hardwareService.SetCurrentPortName(hardwareService.CurrentPortName))
                WHATHAPPENEDTEXTBLOCK.Text +=
                    "Connected to port: " + hardwareService.CurrentPortName + Environment.NewLine;
            else
                WHATHAPPENEDTEXTBLOCK.Text +=
                    "Port did not switch. Cannot connect to port: " + hardwareService.CurrentPortName +
                    Environment.NewLine;
        }

        private void PreviewSetButton_OnClick(object sender, RoutedEventArgs e)
        {
            dataSampleBuffer.Clear();
            currentIterator = -1;
            Signal signal;
            try
            {
                signal = new Signal
                {
                    Amplitude = double.Parse(AmplitudeTextBox.Text, CultureInfo.InvariantCulture),
                    FallingTime = double.Parse(FallingPointTextBox.Text, CultureInfo.InvariantCulture),
                    Offset = double.Parse(OffsetTextBox.Text, CultureInfo.InvariantCulture),
                    Period = double.Parse(PeriodTextBox.Text, CultureInfo.InvariantCulture),
                    RisingTime = double.Parse(RisingPointTextBox.Text, CultureInfo.InvariantCulture),
                    StopTime = double.Parse(StopPointTextBox.Text, CultureInfo.InvariantCulture),
                    TimePassed = 0,
                    SignalType = (SignalType)SignalTypeComboBox.SelectedItem
                };
            }
            catch (Exception exception)
            {

                WHATHAPPENEDTEXTBLOCK.Text +=
                    $"Signal parameters parsing error.{Environment.NewLine}";
                return;
            }
          
            if (Math.Abs(signal.FallingTime + signal.RisingTime + signal.StopTime -
                         signal.Period) > 0.01)
            {
                WHATHAPPENEDTEXTBLOCK.Text +=
                    $"Signal parameters time windows mismatch.{Environment.NewLine}Validate signal again.{Environment.NewLine}";
                return;
            }

            if (signal.Amplitude + signal.Offset > 5)
            {
                WHATHAPPENEDTEXTBLOCK.Text +=
                    $"Amplitude set to high.{Environment.NewLine}";
                return;
            }

            currentSignal = signal;
            SignalParametersTextBlock.Text = $"Data frame to send : {new DataFrame(currentSignal)}";
            preCalculatedSamples = DataDictionary.PreCalculatesSamplesDictionary[currentSignal.SignalType]
                .Invoke(currentSignal);
        }
    }
}