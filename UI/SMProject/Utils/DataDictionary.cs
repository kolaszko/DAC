using System;
using System.Collections.Generic;
using System.Linq;

namespace SMProject.Utils
{
    public static class DataDictionary
    {
        public static readonly int GeneratedSamplesBufferLength = 700;

        public static Dictionary<SignalType, Func<Signal, IEnumerable<double>>> PreCalculatesSamplesDictionary =>
            new Dictionary<SignalType, Func<Signal, IEnumerable<double>>>
            {
                {
                    SignalType.Trapezoidal, signal =>
                    {
                        const int periodShown = 2;
                        var samplesResolution = GeneratedSamplesBufferLength;
                        var samplesPerPeriod = samplesResolution / periodShown;

                        var risingSamplesCount = (int) (samplesPerPeriod * signal.RisingTime / signal.Period);
                        var fallingSamplesCount = (int) (samplesPerPeriod * signal.FallingTime / signal.Period);
                        var stopSamplesCount = (int) (samplesPerPeriod * signal.StopTime / signal.Period);
                        var result = new List<double>();
                        var helperIterator = 1;
                        var secondHelperIterator = 1;
                        var increment = 20 * signal.Amplitude / risingSamplesCount;
                        var decrement = 20 * signal.Amplitude / fallingSamplesCount;

                        foreach (var sample in Enumerable.Range(0, samplesPerPeriod))
                            if (sample < risingSamplesCount)
                            {
                                result.Add(helperIterator * increment +  20 * signal.Offset);
                                helperIterator++;
                            }
                            else if (sample > risingSamplesCount && sample < risingSamplesCount + stopSamplesCount)
                            {
                                result.Add(20 * signal.Amplitude +  20 * signal.Offset);
                            }
                            else
                            {
                                result.Add(20 * signal.Amplitude - secondHelperIterator * decrement +  20 * signal.Offset);
                                secondHelperIterator++;
                            }

                        return result.Concat(result);
                    }
                },
                {
                    SignalType.Triangle, signal =>
                    {
                        const int periodShown = 2;
                        var samplesResolution = GeneratedSamplesBufferLength;
                        var samplesPerPeriod = samplesResolution / periodShown;

                        var risingSamplesCount = (int) (samplesPerPeriod * signal.RisingTime / signal.Period);
                        var fallingSamplesCount = (int) (samplesPerPeriod * signal.FallingTime / signal.Period);

                        var result = new List<double>();
                        var helperIterator = 1;
                        var secondHelperIterator = 1;
                        var increment = 20 * signal.Amplitude / risingSamplesCount;
                        var decrement = 20 * signal.Amplitude / fallingSamplesCount;
                        foreach (var sample in Enumerable.Range(0, samplesPerPeriod))
                            if (sample < risingSamplesCount)
                            {
                                result.Add(helperIterator * increment +  20 * signal.Offset);
                                helperIterator++;
                            }
                            else
                            {
                                result.Add(20 * signal.Amplitude - secondHelperIterator * decrement + 20 * signal.Offset);
                                secondHelperIterator++;
                            }

                        return result.Concat(result);
                    }
                },
                {
                    SignalType.SawSignal, signal =>
                    {
                        const int periodShown = 2;
                        var samplesResolution = GeneratedSamplesBufferLength;
                        var samplesPerPeriod = samplesResolution / periodShown;

                        var result = new List<double>();
                        var risingSamplesCount = samplesPerPeriod;
                        var increment = 20 * signal.Amplitude / risingSamplesCount;
                        var helperIterator = 1;
                        foreach (var sample in Enumerable.Range(0, samplesPerPeriod))
                        {
                            result.Add(helperIterator * increment + 20 * signal.Offset);
                            helperIterator++;
                        }


                        return result.Concat(result);
                    }
                }
            };
    }
}