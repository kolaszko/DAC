using System.Globalization;
using SMProject.Utils;

namespace SMProject
{
    public class DataFrame
    {
        public readonly double Amplitude;
        public readonly double FallingPoint;
        public readonly double Offset;

        public readonly double Period;
        public readonly double RisingPoint;
        public readonly SignalType SignalType;
        public readonly double StopPoint;

        public DataFrame(Signal currentSignal)
        {
            Amplitude = currentSignal.Amplitude;
            FallingPoint = currentSignal.FallingPoint;
            Offset = currentSignal.Offset;
            Period = currentSignal.Period;
            RisingPoint = currentSignal.RisingPoint;
            SignalType = currentSignal.SignalType;
            StopPoint = currentSignal.StopPoint;
        }

        public DataFrame(double period, double amplitude, double offset, double stopPoint, double risingPoint,
            double fallingPoint, SignalType signalType)
        {
            Period = period;
            Amplitude = amplitude;
            Offset = offset;
            StopPoint = stopPoint;
            RisingPoint = risingPoint;
            FallingPoint = fallingPoint;
            SignalType = signalType;
            SignalType = signalType;
        }

        //tO4.0A3.0F0.0R2.0P0.0S0.0\r
        public override string ToString()
        {
            var b =
                $"{SignalType.GetDescription()} {Period.ToString(CultureInfo.InvariantCulture)} {Amplitude.ToString(CultureInfo.InvariantCulture)}" +
                $" {Offset.ToString(CultureInfo.InvariantCulture)} {RisingPoint.ToString(CultureInfo.InvariantCulture)} {FallingPoint.ToString(CultureInfo.InvariantCulture)}" +
                $" {StopPoint.ToString(CultureInfo.InvariantCulture)}!";

            return b;
        }
    }
}