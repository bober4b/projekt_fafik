using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;

namespace fafikspace.helping
{
    public class Helping
    {
        public void log_write(LogMessage arg)
        {

            string filepath = "C:\\Users\\bober\\Desktop\\fafik log.txt";
            using (StreamWriter writer = File.AppendText(filepath))
            {
                writer.WriteLine(arg);
            }
        }


    }
}
