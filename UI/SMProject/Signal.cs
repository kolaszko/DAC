namespace SMProject
{
    public class Signal
    {
        public double Period { get; set; }
        public double Amplitude { get; set; }
        public double Offset { get; set; }
        public double StopTime { get; set; }
        public double RisingTime { get; set; }
        public double FallingTime { get; set; }
        public double TimePassed { get; set; }

        public SignalType SignalType { get; set; }
    }
}