using System;
using System.Diagnostics;

namespace testApp
{
    public class Benchmarks
    {
        private readonly Action<string> _reporter;

        public Benchmarks(Action<string> reporter)
        {
            _reporter = reporter;

        }
        public void Run()
        {
            //run static initializers
            Pinvoke.Empty();
            Process.GetCurrentProcess();
            HandCodedOrm();
            decimal.Parse("-49823174320.9293800");
            //test
            for (var i = 0; i < 5; i++) HandCodedOrm();
            for (var i = 0; i < 10; i++) PInvokeEmpty();
            for (var i = 0; i < 5; i++) GetCurrentProcess();
            for (var i = 0; i < 5; i++) ParseDec();
            SwGetCurrentProcess();
        }
        private void SwGetCurrentProcess()
        {
            var i = 0;
            var sw = Stopwatch.StartNew();
            do
            {
                i++;
                Process.GetCurrentProcess();
            } while (sw.ElapsedMilliseconds < 5000);
            var nanoSecodsPerIteration = 5_000_000_000 / i;

            _reporter($"GetCurrentProcess with StopWatch {nanoSecodsPerIteration,5:0.0} ns ");
        }

        private void HandCodedOrm()
        {
            using (var orm = new HandCodedOrm())
            using (var sw = new AutoStopWatch($"{nameof(HandCodedOrm),-20} {{0}}", 1, _reporter))
            {
                while (sw.IsRunnning)
                        orm.Run();
            }
        }

        private void PInvokeEmpty()
        {

            using (var sw = new AutoStopWatch($"{nameof(PInvokeEmpty),-20} {{0}}", 1000, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < 1000; j++)
                    {
                        Pinvoke.Empty();
                    }
            }
        }


        private void GetCurrentProcess()
        {
            using (var sw = new AutoStopWatch($"{nameof(GetCurrentProcess),-20} {{0}}", 350, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < 350; j++)
                        Process.GetCurrentProcess();
            }
        }

        private void ParseDec()
        {
            var decStr = "-49823174320.9293800";
            using (var sw = new AutoStopWatch($"{nameof(ParseDec),-20} {{0}}", 40, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < 40; j++)
                        decimal.Parse(decStr);
            }
        }
    }
}