using System;

namespace Gaucho
{

    /// <summary>
    /// Gambas-style helper functions for type conversion and string manipulation
    /// </summary>
    public static class Gb
    {
        #region Type Conversion Functions

        /// <summary>
        /// Converts a string to an integer (similar to VB.NET CInt)
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>The integer value</returns>
        /// <exception cref="FormatException">Thrown when the string is not a valid integer</exception>
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

        /// <summary>
        /// Converts a string to a double (similar to VB.NET CDbl)
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>The double value</returns>
        /// <exception cref="FormatException">Thrown when the string is not a valid double</exception>
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

        /// <summary>
        /// Returns the absolute value of an integer
        /// </summary>
        /// <param name="i">The integer value</param>
        /// <returns>The absolute value</returns>
        public static int Abs(int i)
        {
            return Math.Abs(i);
        }

        #endregion

        #region Math Conversion Functions

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="degrees">The angle in degrees</param>
        /// <returns>The angle in radians</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="radians">The angle in radians</param>
        /// <returns>The angle in degrees</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        #endregion

        #region VB.NET-like String Functions

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

        /// <summary>
        /// VB.NET-like InStr function - Returns the position of the first occurrence of a substring within a string
        /// </summary>
        /// <param name="sourceString">The string to search in</param>
        /// <param name="searchString">The string to search for</param>
        /// <param name="startPosition">Optional starting position (1-based index like VB.NET, default: 1)</param>
        /// <param name="compareMethod">Optional comparison method (default: case-sensitive)</param>
        /// <returns>The 1-based position of the first occurrence, or 0 if not found</returns>
        public static int InStr(string sourceString, string searchString, int startPosition = 1, StringComparison compareMethod = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(sourceString) || string.IsNullOrEmpty(searchString))
                return 0;
                
            if (startPosition < 1 || startPosition > sourceString.Length)
                return 0;
                
            // Convert 1-based position to 0-based for C#
            int zeroBasedStart = startPosition - 1;
            
            int result = sourceString.IndexOf(searchString, zeroBasedStart, compareMethod);
            
            // Convert back to 1-based indexing (VB.NET style), return 0 if not found
            return result == -1 ? 0 : result + 1;
        }

        /// <summary>
        /// VB.NET-like InStr function overload - Simplified version with just source and search strings
        /// </summary>
        /// <param name="sourceString">The string to search in</param>
        /// <param name="searchString">The string to search for</param>
        /// <returns>The 1-based position of the first occurrence, or 0 if not found</returns>
        public static int InStr(string sourceString, string searchString)
        {
            return InStr(sourceString, searchString, 1, StringComparison.Ordinal);
        }

        /// <summary>
        /// VB.NET-like InStr function overload - With starting position only
        /// </summary>
        /// <param name="startPosition">The starting position (1-based index)</param>
        /// <param name="sourceString">The string to search in</param>
        /// <param name="searchString">The string to search for</param>
        /// <returns>The 1-based position of the first occurrence, or 0 if not found</returns>
        public static int InStr(int startPosition, string sourceString, string searchString)
        {
            return InStr(sourceString, searchString, startPosition, StringComparison.Ordinal);
        }

        /// <summary>
        /// VB.NET-like InStr function overload - Case-insensitive version
        /// </summary>
        /// <param name="sourceString">The string to search in</param>
        /// <param name="searchString">The string to search for</param>
        /// <param name="ignoreCase">If true, performs case-insensitive comparison</param>
        /// <returns>The 1-based position of the first occurrence, or 0 if not found</returns>
        public static int InStr(string sourceString, string searchString, bool ignoreCase)
        {
            var compareMethod = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return InStr(sourceString, searchString, 1, compareMethod);
        }

        #endregion

        #region File Path Helper Functions

        /// <summary>
        /// Returns the file name part after the last '/' in the path.
        /// If no '/' is found, returns the original string.
        /// Mirrors the behavior of the provided VB FileFromPath.
        /// </summary>
        /// <param name="sPath">The file path</param>
        /// <returns>The file name portion</returns>
        public static string? FileFromPath(string? sPath)
        {
            if (string.IsNullOrEmpty(sPath)) return sPath;
            int last = sPath.LastIndexOf('/');
            if (last == -1) return sPath;
            return sPath.Substring(last + 1);
        }

        /// <summary>
        /// Returns the path portion including the trailing '/' up to the last '/'.
        /// If no '/' is found, returns an empty string.
        /// Mirrors the behavior of the provided VB PathFromFile.
        /// </summary>
        /// <param name="sPath">The file path</param>
        /// <returns>The path portion with trailing slash</returns>
        public static string? PathFromFile(string? sPath)
        {
            if (sPath == null) return null;
            int last = sPath.LastIndexOf('/');
            if (last == -1) return string.Empty;
            // include the trailing slash, matching VB's Left(..., p2 - 1) behavior
            return sPath.Substring(0, last + 1);
        }

        /// <summary>
        /// Returns the file name without its extension (the part before the last '.').
        /// If there is no '.' in the file name, returns the file name unchanged.
        /// Mirrors the behavior of the provided VB FileWithoutExtension.
        /// </summary>
        /// <param name="sPath">The file path</param>
        /// <returns>The file name without extension</returns>
        public static string? FileWithoutExtension(string? sPath)
        {
            if (sPath == null) return null;
            string? fileName = FileFromPath(sPath);
            if (fileName == null) return null;
            int idx = fileName.LastIndexOf('.');
            if (idx == -1) return fileName;
            // if '.' is the first character, this returns an empty string (matches VB behavior)
            return fileName.Substring(0, idx);
        }

        #endregion

        #region Bit Manipulation Functions

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

        #endregion
    }
}