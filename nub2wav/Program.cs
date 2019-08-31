using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace nub2wav
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("nub2wav");
            Console.WriteLine("Created by nzgamer41");

            if (args.Length < 1)
            {
                Console.WriteLine("Usage: nub2wav.exe <path to nub file> <optional bitrate>");
            }
            else
            {
                try
                {
                    int bitrate = 44100;
                    if (args.Length == 2)
                    {
                        bitrate = int.Parse(args[1]);
                    }
                    var path = args[0];
                    byte[] file = File.ReadAllBytes(path);
                    //hopefully should convert big endian to little endian for converting to wav
                    Array.Reverse(file);

                    var s = new RawSourceWaveStream(new MemoryStream(file), new WaveFormat(bitrate, 16, 2));
                    var outpath = Path.ChangeExtension(path, ".wav");
                    int blockAlign = s.WaveFormat.BlockAlign;
                    using (WaveFileWriter writer = new WaveFileWriter(outpath, s.WaveFormat))
                    {
                        byte[] buffer = new byte[blockAlign];
                        long samples = s.Length / blockAlign;
                        for (long sample = samples - 1; sample >= 0; sample--)
                        {
                            s.Position = sample * blockAlign;
                            s.Read(buffer, 0, blockAlign);
                            writer.WriteData(buffer, 0, blockAlign);
                        }
                    }
                    Console.WriteLine(args[0] + " converted to " + outpath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
