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

            string filepath = "C:\\Users\\bober\\Desktop\\fafik pliki\\fafik log.txt";
            using StreamWriter writer = File.AppendText(filepath);
            writer.WriteLine(arg);
        }
        public void log_write(string log)
        {
            string filepath = "C:\\Users\\bober\\Desktop\\fafik pliki\\fafik log.txt";
            DateTime dt = DateTime.Now;

            log =string.Format("{0:HH:mm:ss}",dt)+" system      " + log;
            Console.WriteLine(log);
            using StreamWriter writer = File.AppendText(filepath);
            writer.WriteLine(log);
        }
        public void log_write(string log, string autor)
        {
            string filepath = "C:\\Users\\bober\\Desktop\\fafik pliki\\fafik log.txt";
            DateTime dt = DateTime.Now;
            log = string.Format("{0:HH:mm:ss}", dt) + $" {autor}      " + log;
            Console.WriteLine(log);
            using StreamWriter writer = File.AppendText(filepath);
            writer.WriteLine(log);
        }
        public int stat_R(string path)
        {
            string x;
            try
            {
                StreamReader reader = new (path);
                x = reader.ReadLine();
                reader.Close();
            }
            catch
            {
                log_write("Błąd odczytu pliku!!");
                return -1;
            }
            int wyn;
            try
            {
                wyn = Int32.Parse(x);
            }
            catch
            {
                log_write("Błąd wartości!!!");
                return -1;
            }
            return wyn;
        }

        public void stat_W(string path, int wartosc)
        {
            try 
            { 
                
                StreamWriter writer = new (path);
                writer.WriteLine(wartosc);
                writer.Close();
            }
            catch
            {
                log_write("Błąd zapisu pliku!!");
            }


        }

    }
}
