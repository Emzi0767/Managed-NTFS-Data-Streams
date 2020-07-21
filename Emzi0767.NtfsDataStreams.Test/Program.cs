// This file is part of Managed NTFS Data Streams project
//
// Copyright 2020 Emzi0767
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
            var adsi = fi.GetDataStream(sname);
            using (var sw = adsi?.AppendText(utf8) ?? fi.CreateTextDataStream(sname, utf8))
                sw.WriteLine(DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffffzzz"));
        }
    }
}
