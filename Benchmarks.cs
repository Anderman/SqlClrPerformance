using System;

namespace testApp
{
    public  class Benchmarks
    {
        private readonly Action<string> _reporter;

        public Benchmarks(Action<string> reporter)
        {
            _reporter = reporter;
            
        }
        public void Run()
        {
            for (var i = 0; i < 5; i++) HandCodedOrm();
            for (var i = 0; i < 5; i++) TimeStampP();
            for (var i = 0; i < 5; i++) ParseDec();

        }
        private void HandCodedOrm()
        {
            using (var orm = new HandCodedOrm())
            using (var sw = new AutoStopWatch($"{nameof(HandCodedOrm),-20} {{0}}", 100, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < 100; j++)
                        orm.Run();
            }
        }

        private  void TimeStampP()
        {
            using (var sw = new AutoStopWatch($"{nameof(TimeStampP),-20} {{0}}", 100, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < 100; j++)
                        Pinvoke.Empty();
            }
        }

        private  void ParseDec()
        {
            var decStr = "-49823174320.9293800";
            using (var sw = new AutoStopWatch($"{nameof(ParseDec),-20} {{0}}", 100, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < 100; j++)
                        decimal.Parse(decStr);
            }
        }

    }
}