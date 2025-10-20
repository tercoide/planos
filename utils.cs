using Gio;
using Gtk;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using OpenTK;
using Gaucho;


using System.Diagnostics;
using Graphene.Internal;

namespace Gaucho
{
    public struct Colors
    {
        int Black = 0;
        int White = 1;
        int Red = 2;
        int Green = 3;
        int Blue = 4;
        int Yellow = 5;
        int Magenta = 6;
        int Cyan = 7;
        int Gray = 8;
        int DarkGray = 9;
        int LightRed = 10;
        int LightGreen = 11;
        int LightBlue = 12;
        int LightYellow = 13;
        int LightMagenta = 14;
        int LightCyan = 15;
        public  Colors()
        {
        }
    }


    public class Utils
    {


        public static void Swap<T>(ref T a, ref T b)
{
    T temp = a;
    a = b;
    b = temp;
}
        public static Gtk.Image? LoadSvgToImage(string svgPath, int width, int height)
        {
            if (!System.IO.File.Exists(svgPath))
            {
                Console.WriteLine($"File not found: {svgPath}");
                return null;
            }

            try
            {
                return new Gtk.Image()
                {
                    File = svgPath,
                    WidthRequest = width,
                    HeightRequest = height
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading SVG: {ex.Message}");
                return null;
            }
        }

        public static Gtk.Button CreateButton(string iconPath, int iconSize = 24, string? tooltip = null)
        {
            var image = null as Gtk.Image;

            image = LoadSvgToImage(iconPath, iconSize, iconSize);


            var button = new Gtk.Button();

            if (image != null)
            {
                button.Child = image;
            }

            if (tooltip != null)
            {
                button.TooltipText = tooltip;
            }

            return button;
        }


        public static void Shell(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine("Output: " + output);
            Console.WriteLine("Error: " + error);
        }
    }

    /// <summary>
    /// Extension methods for bit manipulation operations
    /// </summary>
    public static class BitExtensions
    {
        /// <summary>
        /// Tests if a specific bit is set in an integer value
        /// </summary>
        /// <param name="value">The integer value to test</param>
        /// <param name="bitPosition">The bit position to test (0-based)</param>
        /// <returns>True if the bit is set, false otherwise</returns>
        public static bool IsBitSet(this int value, int bitPosition)
        {
            return (value & (1 << bitPosition)) != 0;
        }

        /// <summary>
        /// Sets a specific bit in an integer value
        /// </summary>
        /// <param name="value">The integer value to modify</param>
        /// <param name="bitPosition">The bit position to set (0-based)</param>
        /// <returns>The modified integer with the bit set</returns>
        public static int SetBit(this int value, int bitPosition)
        {
            return value | (1 << bitPosition);
        }

        /// <summary>
        /// Clears a specific bit in an integer value
        /// </summary>
        /// <param name="value">The integer value to modify</param>
        /// <param name="bitPosition">The bit position to clear (0-based)</param>
        /// <returns>The modified integer with the bit cleared</returns>
        public static int ClearBit(this int value, int bitPosition)
        {
            return value & ~(1 << bitPosition);
        }

        /// <summary>
        /// Toggles a specific bit in an integer value
        /// </summary>
        /// <param name="value">The integer value to modify</param>
        /// <param name="bitPosition">The bit position to toggle (0-based)</param>
        /// <returns>The modified integer with the bit toggled</returns>
        public static int ToggleBit(this int value, int bitPosition)
        {
            return value ^ (1 << bitPosition);
        }
    }

}


