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
    public static class  Colors
    {
        public const int Black = 0;
        public const int White = 1;
        public const int Red = 2;
        public const int Green = 3;
        public const int Blue = 4;
        public const int Yellow = 5;
        public const int Magenta = 6;
        public const int Cyan = 7;
        public const int Gray = 8;
        public const int DarkGray = 9;
        public const int LightRed = 10;
        public const int LightGreen = 11;
        public const int LightBlue = 12;
        public const int LightYellow = 13;
        public const int LightMagenta = 14;
        public const int LightCyan = 15;
    }


    public static class Utils
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

        /// <summary>
    /// VB.NET-like DoEvents function - Processes pending UI events and messages
    /// This allows the UI to remain responsive during long-running operations
    /// </summary>
    public static void DoEvents()
    {
        try
        {
            // For GTK4, we use GLib.MainContext to process pending events
            var context = GLib.MainContext.Default();
            while (context.Pending())
            {
                context.Iteration(false);
            }
        }
        catch (Exception ex)
        {
            // Log the error but don't let it break the application
            Console.WriteLine($"DoEvents error: {ex.Message}");
        }
    }

        /// <summary>
        /// VB.NET-like DoEvents function with timeout - Processes pending UI events for a specified duration
        /// </summary>
        /// <param name="maxProcessingTimeMs">Maximum time to spend processing events in milliseconds</param>
        public static void DoEvents(int maxProcessingTimeMs)
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var context = GLib.MainContext.Default();

                while (context.Pending() && stopwatch.ElapsedMilliseconds < maxProcessingTimeMs)
                {
                    context.Iteration(false);
                }

                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                // Log the error but don't let it break the application
                Console.WriteLine($"DoEvents error: {ex.Message}");
            }
        }
    }
}


