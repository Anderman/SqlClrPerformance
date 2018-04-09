using System;
using System.Diagnostics;

namespace testApp
{
    public class AutoStopWatch : IDisposable
    {
        private readonly string _message;
        private readonly double _repeats;
        private readonly Action<string> _reporter;
        private ulong _start;
        private static readonly ulong CyclesPerSecond = GetCyclesPerSeond();
        private const double NanoSecondsPerSecond = 1000 * 1000 * 1000;
        private const double MicroSecondsPerSecond = 1000 * 1000;
        private const double MilliSecondsPerSecond = 1000;
        private readonly double _microSecondsPerCycle;
        private readonly double _milliSecondsPerCycle;
        private readonly double _nanoSecondPerCycle;
        private ulong _end;
        private ulong _mincycles = ulong.MaxValue;
        private readonly ulong _startTest;
        private static bool _showInfo = true;

        public AutoStopWatch(string message, double repeats, Action<string> reporter)
        {
            _message = message;
            _repeats = repeats;
            _reporter = reporter;
            _nanoSecondPerCycle = NanoSecondsPerSecond / CyclesPerSecond;
            _microSecondsPerCycle = MicroSecondsPerSecond / CyclesPerSecond;
            _milliSecondsPerCycle = MilliSecondsPerSecond / CyclesPerSecond;
            Rdtsc.Init();
            _startTest = Rdtsc.TimestampP();
            if (_showInfo)
            {
                _reporter(string.Format($"cycles per second: {CyclesPerSecond,5:0} "));
                _reporter(string.Format($"{NanoSecondsPerSecond / CyclesPerSecond,5:0.000} ns per cycle"));
                _showInfo = false;
            }
        }
        public void Dispose()
        {
            var cyclePerIteration = (_mincycles) / _repeats;
            SendMessage(cyclePerIteration);
        }

        private void SendMessage(double cyclePerIteration)
        {
            var milliSecodsPerIteration = cyclePerIteration * _milliSecondsPerCycle;
            var microSecodsPerIteration = cyclePerIteration * _microSecondsPerCycle;
            var nanoSecodsPerIteration = cyclePerIteration * _nanoSecondPerCycle;
            if (milliSecodsPerIteration > 10)
                _reporter(string.Format(_message, $"{milliSecodsPerIteration,5:0} ms"));
            else if (microSecodsPerIteration > 10)
                _reporter(string.Format(_message, $"{microSecodsPerIteration,5:0} us"));
            else
                _reporter(string.Format(_message, $"{nanoSecodsPerIteration,5:0.0} ns {cyclePerIteration,5:0.0} cycles"));
        }

        private static ulong GetCyclesPerSeond()
        {
            Rdtsc.Init();

            var sw = Stopwatch.StartNew();
            var startms = Rdtsc.TimestampP();
            do { } while (sw.ElapsedMilliseconds < 1000);
            var endms = Rdtsc.TimestampP();
            var cyclesPerSecond = endms - startms;

            return cyclesPerSecond;
        }

        public bool IsRunnning
        {
            get
            {
                _end = Rdtsc.TimestampP();
                var cycles = _end - _start;
                if (cycles <= _mincycles)
                    _mincycles = cycles;
                if (_end - _startTest > 1_000_000_000)
                    return false;
                _start = Rdtsc.TimestampP();
                return true;
            }
        }
    }
}