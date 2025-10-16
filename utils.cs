using Gio;
using Gtk;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using OpenTK;
using Gaucho;


using System.Diagnostics;

namespace Gaucho
{

    public class Utils
    {
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

}


