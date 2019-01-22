namespace SMProject
{
    public class Signal
    {
        public double Period { get; set; }
        public double Amplitude { get; set; }
        public double Offset { get; set; }
        public double StopPoint { get; set; }
        public double RisingPoint { get; set; }
        public double FallingPoint { get; set; }
        public double TimePassed { get; set; }

        public SignalType SignalType { get; set; }
    }
}