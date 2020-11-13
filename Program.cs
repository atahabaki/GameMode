// Copyright 2020 A. Taha Baki
//
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject
// to the following conditions:
//
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace GameMode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GameMode by @atahabaki");

            if (args.Length >= 1)
            {
                GameConfig gameConfig = Setup(args[0]);
                for (; ; )
                {
                    Loop(gameConfig);
                    Sleep(gameConfig.Timeout);
                }
            }
            else
            {
                Usage();
            }
        }

        static GameConfig Setup(String fileName)
        {
            GameConfig gameConfig = ReadConfigFileProperties(fileName);
            Console.WriteLine($"Starting GameMode for \"{gameConfig.Title}...\"");
            return gameConfig;
        }

        static void Loop(GameConfig gameConfig)
        {
            gameConfig.Processes.ForEach(process_name =>
            {
                foreach (Process proc in Process.GetProcessesByName(process_name))
                {
                    Console.WriteLine($"Killing {proc.ProcessName} ({proc.Id})");
                    proc.Kill();
                }
            });
        }

        static String[] ReadAllLinesFromFile(String fileName)
        {
            try
            {
                String[] lines = File.ReadAllLines(fileName);
                return lines;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found. Quiting...");
                System.Environment.Exit(1);
            }
            return null;
        }
        static GameConfig ReadConfigFileProperties(String fileName)
        {
            GameConfig gcf = new GameConfig();
            String[] lines = ReadAllLinesFromFile(fileName);
            var mode = 0; //0:general 1:process
            gcf.Processes = new List<String>();
            foreach (String line in lines)
            {
                String xline = line.Trim().ToUpper();
                if (xline.Equals("[GENERAL]"))
                {
                    mode = 0;
                }
                else if (xline.Equals("[PROCESS]"))
                {
                    mode = 1;
                }

                if (mode == 0 && xline.StartsWith("TITLE"))
                {
                    gcf.Title = line.Trim().Substring(6);
                }
                else if (mode == 0 && xline.StartsWith("TIMEOUT"))
                {
                    gcf.Timeout = Convert.ToInt32(xline.Substring(8)) * 1000; //1000 for s to ms...
                }
                else if (mode == 1 && !xline.Equals("[PROCESS]"))
                {
                    //add  (line) to processes list...
                    gcf.Processes.Add(line);
                }
            }
            return gcf;
        }

        static void Usage()
        {
            Console.WriteLine("USAGE:\ngame_mode.exe <config_file>");
        }

        static void Sleep(int ms)
        {
            Console.WriteLine($"Sleeping {ms}ms...");
            System.Threading.Thread.Sleep(ms);
        }

        static void KillByName(String name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            foreach (Process proc in processes)
            {
                Console.WriteLine($"Killing {proc.ProcessName} ({proc.Id})");
                proc.Kill();
            }
        }
    }
}
