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
        public Colors()
        {
        }
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
    /// Extension methods for bit manipulation operations
    /// </summary>
    
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


        public static int CInt(string str)
        {

            int result;
            if (int.TryParse(str, out result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"'{str}' is not a valid integer.");
            }
        }

        public static double CDbl(string str)
        {

            double result;
            if (double.TryParse(str, out result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"'{str}' is not a valid double.");
            }
        }

        public static int Abs(int i)
        {
            return Math.Abs(i);
        }
       
        
    

        
    
    
        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        public static double RadiansToDegrees(double radians)
{
    return radians * (180.0 / Math.PI);
}

        /// <summary>
        /// VB.NET-like Left function - Returns a string containing the leftmost characters from a string
        /// </summary>
        /// <param name="str">The source string</param>
        /// <param name="length">The number of characters to return from the left</param>
        /// <returns>A string containing the leftmost characters</returns>
        public static string Left(string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
                
            if (length <= 0)
                return string.Empty;
                
            if (length >= str.Length)
                return str;
                
            return str.Substring(0, length);
        }

        /// <summary>
        /// VB.NET-like Mid function - Returns a substring from the middle of a string
        /// </summary>
        /// <param name="str">The source string</param>
        /// <param name="start">The starting position (1-based index like VB.NET)</param>
        /// <param name="length">The number of characters to return (optional)</param>
        /// <returns>A substring from the specified position</returns>
        public static string Mid(string str, int start, int? length = null)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
                
            // Convert 1-based index to 0-based
            int zeroBasedStart = start - 1;
            
            if (zeroBasedStart < 0 || zeroBasedStart >= str.Length)
                return string.Empty;
                
            if (length.HasValue)
            {
                if (length.Value <= 0)
                    return string.Empty;
                    
                int actualLength = Math.Min(length.Value, str.Length - zeroBasedStart);
                return str.Substring(zeroBasedStart, actualLength);
            }
            else
            {
                return str.Substring(zeroBasedStart);
            }
        }

        /// <summary>
        /// VB.NET-like LTrim function - Removes leading whitespace characters
        /// </summary>
        /// <param name="str">The source string</param>
        /// <returns>A string with leading whitespace removed</returns>
        public static string LTrim(string str)
        {
            return str?.TrimStart() ?? string.Empty;
        }

        /// <summary>
        /// VB.NET-like RTrim function - Removes trailing whitespace characters
        /// </summary>
        /// <param name="str">The source string</param>
        /// <returns>A string with trailing whitespace removed</returns>
        public static string RTrim(string str)
        {
            return str?.TrimEnd() ?? string.Empty;
        }

        /// <summary>
        /// VB.NET-like Trim function - Removes leading and trailing whitespace characters
        /// </summary>
        /// <param name="str">The source string</param>
        /// <returns>A string with leading and trailing whitespace removed</returns>
        public static string Trim(string str)
        {
            return str?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// VB.NET-like Split function - Splits a string into an array using the specified separator
        /// </summary>
        /// <param name="str">The source string to split</param>
        /// <param name="separator">The separator character or string</param>
        /// <param name="removeEmpty">Whether to remove empty entries (default: false)</param>
        /// <returns>An array of strings</returns>
        public static string[] Split(string str, string separator, bool removeEmpty = false)
        {
            if (string.IsNullOrEmpty(str))
                return new string[0];
                
            if (string.IsNullOrEmpty(separator))
                return new string[] { str };
                
            var options = removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            return str.Split(new string[] { separator }, options);
        }

        /// <summary>
        /// VB.NET-like Split function overload - Splits a string using a single character separator
        /// </summary>
        /// <param name="str">The source string to split</param>
        /// <param name="separator">The separator character</param>
        /// <param name="removeEmpty">Whether to remove empty entries (default: false)</param>
        /// <returns>An array of strings</returns>
        public static string[] Split(string str, char separator, bool removeEmpty = false)
        {
            if (string.IsNullOrEmpty(str))
                return new string[0];
                
            var options = removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            return str.Split(new char[] { separator }, options);
        }

        /// <summary>
        /// VB.NET-like Replace function - Replaces all occurrences of a substring with another substring
        /// </summary>
        /// <param name="str">The source string</param>
        /// <param name="oldValue">The substring to be replaced</param>
        /// <param name="newValue">The substring to replace with</param>
        /// <param name="ignoreCase">Whether to ignore case when searching (default: false)</param>
        /// <returns>A string with all occurrences of oldValue replaced with newValue</returns>
        public static string Replace(string str, string oldValue, string newValue, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(oldValue))
                return str ?? string.Empty;
                
            if (newValue == null)
                newValue = string.Empty;
                
            if (!ignoreCase)
            {
                return str.Replace(oldValue, newValue);
            }
            else
            {
                // Case-insensitive replacement
                var comparison = StringComparison.OrdinalIgnoreCase;
                var result = str;
                int index = 0;
                
                while ((index = result.IndexOf(oldValue, index, comparison)) != -1)
                {
                    result = result.Substring(0, index) + newValue + result.Substring(index + oldValue.Length);
                    index += newValue.Length;
                }
                
                return result;
            }
        }

        /// <summary>
        /// VB.NET-like Replace function overload - Replaces all occurrences of a character with another character
        /// </summary>
        /// <param name="str">The source string</param>
        /// <param name="oldChar">The character to be replaced</param>
        /// <param name="newChar">The character to replace with</param>
        /// <returns>A string with all occurrences of oldChar replaced with newChar</returns>
        public static string Replace(string str, char oldChar, char newChar)
        {
            if (string.IsNullOrEmpty(str))
                return str ?? string.Empty;
                
            return str.Replace(oldChar, newChar);
        }
    

}
}


