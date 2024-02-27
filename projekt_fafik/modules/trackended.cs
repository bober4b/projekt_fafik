using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace projekt_fafik.trackended
{
    public class Trackended
    {
        readonly Stopwatch stopwatch = new();
        public bool playnext=true;

        public void Start()
            =>stopwatch.Start();
        public void Stop()
            =>stopwatch.Stop();
        public void Restart()
            =>stopwatch.Restart();
        public long Mili()
        {
            return (stopwatch.ElapsedMilliseconds);

        }
        public bool Ison()
            => stopwatch.IsRunning;

    }
}
