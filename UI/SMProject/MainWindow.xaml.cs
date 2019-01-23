using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMProject
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int bufferLength;
        private readonly CircularBuffer<Point> dataSampleBuffer;
        private readonly HardwareService hardwareService = new HardwareService();
        private readonly IEnumerable<double> preCalculatedSamples;
        private readonly Timer signalGeneratingTimer;
        private readonly int TickIntervalMs = 1;
        private int currentIterator;
        private Signal currentSignal;

        public MainWindow()
        {
            InitializeComponent();
            Title = "ADUC831 Signal generator v1.00";
            bufferLength = 700;

            dataSampleBuffer = new CircularBuffer<Point>(bufferLength);
            signalGeneratingTimer = new Timer(Callback, null, 0, TickIntervalMs);

            currentSignal = new Signal
            {
                Amplitude = 100,
                FallingPoint = 0,
                Offset = 0,
                Period = 10,
                RisingPoint = 1,
                StopPoint = 4,
                TimePassed = 0,
                SignalType = SignalType.SawSignal
            };
            preCalculatedSamples = PreCalculatesSamplesDictionary[currentSignal.SignalType].Invoke(currentSignal);
            currentIterator = -1;

            InitViews();
            
        }

        private void InitViews()
        {
            COMComboBox.ItemsSource = hardwareService.AllowedPortNames;
            SignalTypeComboBox.SelectedItem = currentSignal.SignalType;
            COMComboBox.SelectedItem = hardwareService.CurrentPortName;
            COMComboBox.SelectionChanged += OnComComboBoxChanged;
            AmplitudeTextBox.Text = currentSignal.Amplitude.ToString(ContentStringFormat);
            PeriodTextBox.Text = currentSignal.Period.ToString(ContentStringFormat);
            OffsetTextBox.Text = currentSignal.Offset.ToString(ContentStringFormat);
            RisingPointTextBox.Text = currentSignal.RisingPoint.ToString(ContentStringFormat);
            FallingPointTextBox.Text = currentSignal.FallingPoint.ToString(ContentStringFormat);
            StopPointTextBox.Text = currentSignal.StopPoint.ToString(ContentStringFormat);

        }

        public Dictionary<SignalType, Func<Signal, IEnumerable<double>>> PreCalculatesSamplesDictionary =>
            new Dictionary<SignalType, Func<Signal, IEnumerable<double>>>
            {
                {SignalType.Triangle, signal => Enumerable.Repeat(signal.Amplitude, bufferLength)},
                {
                    SignalType.Sinusoid, signal =>
                    {
                        const int periodShown = 2;
                        var samplesResolution = bufferLength;
                        var samplesPerPeriod = samplesResolution / periodShown;

                        var sinMax = Math.PI * 2;

                        var firstPeriod = Enumerable.Range(0, samplesPerPeriod)
                            .Select(sample => Math.Sin(sinMax * sample / samplesPerPeriod));
                        var secondPeriod = Enumerable.Range(0, samplesPerPeriod)
                            .Select(sample => Math.Sin(sinMax * sample / samplesPerPeriod));
                        var result = firstPeriod.Concat(secondPeriod).Select(s => signal.Amplitude * s);

                        return result;
                    }
                },
                {
                    SignalType.SawSignal, signal =>
                    {
                        const int periodShown = 2;
                        var samplesResolution = bufferLength;
                        var samplesPerPeriod = samplesResolution / periodShown;

                        var risingStartSample = (int) (samplesPerPeriod * signal.RisingPoint / signal.Period);
                        var fallingStartSample = (int) (samplesPerPeriod * signal.FallingPoint / signal.Period);
                        var stopStartSample = (int) (samplesPerPeriod * signal.StopPoint / signal.Period);
                        var result = new List<double>();
                        var risingSamples = stopStartSample - risingStartSample;
                        var increment = signal.Amplitude / risingSamples;
                        var helperIterator = 1;
                        foreach (var sample in Enumerable.Range(0, samplesPerPeriod))
                            if (sample <= risingStartSample)
                            {
                                result.Add(0);
                            }

                            else if (sample >= stopStartSample)
                            {
                                result.Add(0);
                            }
                            else
                            {
                                result.Add(helperIterator * increment);
                                helperIterator++;
                            }

                        return result.Concat(result);
                    }
                }
            };

        private void Callback(object state)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
                () =>
                {
                    DrawingSurface.Children.Clear();
                    currentIterator++;
                    currentIterator =
                        currentIterator % bufferLength;
                    var value = preCalculatedSamples.ElementAt(currentIterator);
                    dataSampleBuffer.Add(new Point(currentIterator, value));

                    var xLine = new Polyline
                    {
                        Points = new PointCollection(Enumerable.Range(0, bufferLength).Select(s => new Point(s, 0))),
                        StrokeThickness = 4,
                        Stroke = new SolidColorBrush(Colors.AntiqueWhite)
                    };
                    var yLine = new Polyline
                    {
                        Points = new PointCollection {new Point(0, 0), new Point(0, 10 * currentSignal.Amplitude)},
                        StrokeThickness = 4,
                        Stroke = new SolidColorBrush(Colors.AntiqueWhite)
                    };
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

        private void ChangeSignal(Signal signal)
        {
            currentSignal = signal;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dataFrame = new DataFrame(currentSignal);
            hardwareService.Send(dataFrame.ToString());
        }
        private void OnComComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            hardwareService.CurrentPortName = (sender as ComboBox).SelectedItem as string;
            if(hardwareService.SetCurrentPortName(hardwareService.CurrentPortName))
            {
                WHATHAPPENEDTEXTBLOCK.Text += "Connected to port: " + hardwareService.CurrentPortName.ToString() + "\r\n";
            }
            else
            {
                WHATHAPPENEDTEXTBLOCK.Text += "Cannot connect to port: " + hardwareService.CurrentPortName.ToString() + "\r\n";
            }
        }
    }

    public class HalvingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double) value * -0.495;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}