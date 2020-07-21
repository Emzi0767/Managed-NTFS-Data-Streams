using System;
using System.IO;
using System.Text;

namespace Emzi0767.NtfsDataStreams.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var fpath = @"C:\Users\emzi0\Documents\test.txt";
            var fi = new FileInfo(fpath);
            var utf8 = new UTF8Encoding(false);

            foreach (var ads in fi.EnumerateDataStreams())
            {
                Console.WriteLine($"-------- DATA STREAM '{ads.Name}' ({ads.Length} bytes)");
                using (var sr = ads.OpenText(utf8))
                    Console.WriteLine(sr.ReadToEnd());
            }

            var sname = "emzi-wuz-here";
            var adsi = fi.GetNtfsDataStream(sname);
            using (var sw = adsi?.AppendText(utf8) ?? fi.CreateTextDataStream(sname, utf8))
                sw.WriteLine(DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffffzzz"));
        }
    }
}
