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
        static String[] bitRate44100 = new String[] { "WM5_01B.nub", "WM5_02B.nub", "WM5_03B.nub", "WM5_04B.nub", "WM5_05B.nub", "WM5_06B.nub", "WM5_07B.nub", "WM5_08B.nub", "WMR_01B.nub", "WMR_02B.nub", "WMR_03B.nub", "WMR_04B.nub", "WMR_05B.nub", "WMR_06B.nub", "WMR_07B.nub", "WMR_08B.nub", "WMR_09B.nub", "WMR_10B.nub", "WMR_11B.nub", "WMR_12B.nub", "WM4_01B.nub" ,"WM4_02B.nub", "WM4_03B.nub", "WM4_04B.nub", "WM4_05B.nub", "WM4_06B.nub", "WM4_07B.nub", "WM4_08B.nub", "WM4_09B.nub", "WM4_10B.nub", "WM4_11B.nub", "WM4_12B.nub", "WM4_13B.nub", "WM4_14B.nub", "WM4_15B.nub", "WM4_16B.nub", "WM4_17B.nub", "WM3_01B.nub", "WM3_02B.nub", "WM3_03B.nub", "WM3_04B.nub", "WM3_05B.nub", "WM3_06B.nub", "WM3_07B.nub", "WM3_08B.nub", "WM3_09B.nub", "WM3_10B.nub", "WM3_11B.nub", "WM3_12B.nub", "WM3_13B.nub", "WM3_14B.nub", "WM3_15B.nub", "WM3_16B.nub", "WM3_17B.nub", "WM3_18B.nub", "WM3_19B.nub", "WM3_20B.nub", "WM2_09B.nub", "WM2_08B.nub", "WM2_18B.nub", "TEN_01B.nub", "TEN_02B.nub", "TEN_03B.nub", "TEN_04B.nub", "TEN_05B.nub", "TER_01B.nub", "MOV_01B.nub", "MOV_02B.nub", "MOV_A01.nub", "MOV_A04.nub", "MOV_A04C.nub", "MOV_A04E.nub", "MOV_A06.nub", "MOV_A06W.nub", "MOV_A07.nub", "MOV_A07W.nub", "MOV_B01.nub", "MOV_B01C.nub", "MOV_B01E.nub", "MOV_BPC.nub", "MOV_BPE.nub", "MOV_BPJ.nub", "MOV_END.nub", "MOV_TEAM.nub", "MOV_W5_D00.nub", "MOV_W5_D01.nub", "MOV_W5_D02.nub", "MOV_W5_T00.nub", "MOV_W5_T01.nub", "MOV_W5_T02.nub", "MOV_ZENICHI.nub", "NET_REMIX_BGM01.nub", "NET_REMIX_BGM02.nub", "SYS_01B.nub", "SYS_02B.nub", "SYS_03B.nub", "SYS_04B.nub", "SYS_05B.nub", "SYS_05_2B.nub", "SYS_06B.nub", "SYS_07B.nub" };
        static String[] bitRate28000 = new String[] { "WM2_01B.nub", "WM2_02B.nub", "WM2_03B.nub", "WM2_04B.nub", "WM2_05B.nub", "WM2_06B.nub", "WM2_07B.nub", "WM2_10B.nub", "WM2_11B.nub", "WM2_12B.nub", "WM2_13B.nub", "WM2_14B.nub", "WM2_15B.nub", "WM2_16B.nub", "WM2_17B.nub", "WM2_19B.nub", "WM2_20B.nub" };
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
                        Console.WriteLine("Bitrate is set to: " + bitrate);
                    }

                    //automatic bitrate detection, but only if custom bitrate isn't supplied
                    if (args.Length != 2)
                    {
                        String fileName = Path.GetFileName(args[0]);
                        Console.WriteLine("Checking bitrate...");

                        if (bitRate44100.Contains(fileName))
                        {
                            bitrate = 44100;
                            Console.WriteLine("Bitrate is set to: " + bitrate);
                        }
                        else if (bitRate28000.Contains(fileName))
                        {
                            bitrate = 28000;
                            Console.WriteLine("Bitrate is set to: " + bitrate);
                        }
                        else
                        {
                            Console.WriteLine(fileName + " is unrecognized, defaulting to " + bitrate);
                        }
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
