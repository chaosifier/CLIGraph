using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLIGraphPlotter
{
    class Program
    {
        static void Main(string[] args)
        {
            CLIGraphPlotter pltr = new CLIGraphPlotter(70, 20);

            PerformanceCounter cpuCounter;
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            while (true)
            {
                pltr.Update(cpuCounter.NextValue());
                Thread.Sleep(1000);
            }
        }

        public class CLIGraphPlotter
        {
            private int _width;
            private int _height;
            private GraphPixel[] _data;
            private ConsoleColor _defaultForegroundColor = ConsoleColor.White;
            private int _startPositionX;
            private int _startPositionY;
            public char EmptyChar = ' ';
            public char PixelChar = '■';

            public CLIGraphPlotter()
            {
                _width = Console.BufferWidth - 4;
                _height = 20;

                _data = new GraphPixel[_width];
            }

            public CLIGraphPlotter(int width, int height, int startPositionX = 0, int startPositionY = 0)
            {
                if (width > Console.BufferWidth - 4)
                {
                    width = Console.BufferWidth - 4;
                }

                _width = width;
                _height = height;
                _startPositionX = startPositionX;
                _startPositionY = startPositionY;
                _data = new GraphPixel[_width];
            }

            public void Update(double newValueInPercent)
            {
                Array.Copy(_data, 1, _data, 0, _data.Length - 1);

                int pixels = (int)Math.Floor((newValueInPercent / 100 * (_height - 1)));

                var pixelData = new GraphPixel();
                pixelData.Pixels = pixels;

                if (newValueInPercent > 95)
                {
                    pixelData.ForegroundColor = ConsoleColor.DarkRed;
                }
                else if (newValueInPercent > 75)
                {
                    pixelData.ForegroundColor = ConsoleColor.Red;
                }
                else if (newValueInPercent > 50)
                {
                    pixelData.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    pixelData.ForegroundColor = ConsoleColor.DarkGreen;
                }

                _data[_data.Length - 1] = pixelData;

                PlotData();
            }

            private void PlotData()
            {
                for (int x = _width; x >= 0; x--)
                {
                    for (int y = _height; y >= 0; y--)
                    {
                        Console.ForegroundColor = _defaultForegroundColor;

                        if (y == 0 && x == 0)
                        {
                            SetPosition(x + 1, _height + 1);
                            Console.Write("+");
                        }
                        else if (x == 0)
                        {
                            SetPosition(x + 1, y);
                            Console.Write("|");
                        }
                        else if (y == 0)
                        {
                            SetPosition(x + 2, _height + 1);
                            Console.Write("-");
                        }
                        else
                        {
                            SetPosition(x + 2, y);

                            if ((_height - y) < _data[x - 1].Pixels)
                            {
                                Console.ForegroundColor = _data[x - 1].ForegroundColor;
                                Console.WriteLine(PixelChar);
                            }
                            else
                            {
                                Console.WriteLine(EmptyChar);
                            }
                        }
                    }
                }
            }

            private void SetPosition(int x, int y)
            {
                try
                {
                    Console.SetCursorPosition(x, y);
                }
                catch { }
            }

            public struct GraphPixel
            {
                public int Pixels { get; set; }
                public ConsoleColor ForegroundColor { get; set; }
                public ConsoleColor BackgroundColor { get; set; }
            }
        }
    }
}
