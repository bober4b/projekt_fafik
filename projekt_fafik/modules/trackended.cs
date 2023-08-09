using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace projekt_fafik.trackended
{
    public class trackended
    {
        Stopwatch stopwatch = new Stopwatch();
        public bool playnext=true;

        public void start()
            =>stopwatch.Start();
        public void stop()
            =>stopwatch.Stop();
        public void restart()
            =>stopwatch.Restart();
        public long mili()
        {
            return (stopwatch.ElapsedMilliseconds);

        }
        public bool ison()
            => stopwatch.IsRunning;

    }
}
