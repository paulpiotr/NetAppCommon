#region using

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

#endregion

namespace NetAppCommon.Helpers.Shell
{
    public static class ShellHelper
    {
        /// <summary>
        ///     Original function
        /// </summary>
        /// <param name="cmd">
        ///     Command do execute as string run as "cmd".Bash();
        /// </param>
        /// <returns>
        ///     Execute result as string
        /// </returns>
        public static string Bash(this string cmd)
        {
            try
            {
                var escapedArgs = cmd.Replace("\"", "\\\"");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{escapedArgs}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        ///     Get arguments from string command
        /// </summary>
        /// <param name="cmd">
        ///     String command
        /// </param>
        /// <returns>
        ///     Array with command string elements
        /// </returns>
        public static string[] GetArgs(string cmd)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    var argsList = cmd.Split(' ').ToList();
                    if (argsList.Count > 0)
                    {
                        argsList.RemoveAll(r => string.IsNullOrWhiteSpace(r.Trim()) || r.StartsWith("--"));
                        if (argsList.Count > 0)
                        {
                            return argsList.ToArray();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        ///     Get options from string command
        /// </summary>
        /// <param name="cmd">
        ///     String command
        /// </param>
        /// <returns>
        ///     Array with string options elements
        /// </returns>
        public static string[] GetOptions(string cmd)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    var argsList = cmd.Split(' ').ToList();
                    if (argsList.Count > 0)
                    {
                        argsList.RemoveAll(r => string.IsNullOrWhiteSpace(r.Trim()) || !r.StartsWith("--"));
                        if (argsList.Count > 0)
                        {
                            return argsList.ToArray();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        ///     Run bin command from .bin directory
        /// </summary>
        /// <param name="cmd">
        ///     Command to execute as string
        /// </param>
        public static void Bin(this string cmd)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    var bin = cmd.Split(' ').ToList().FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(bin))
                    {
                        var args = GetArgs(cmd);
                        var options = GetOptions(cmd);
                        var path86 = string.Empty;
                        var path64 = string.Empty;
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                            (RuntimeInformation.OSArchitecture == Architecture.Arm ||
                             RuntimeInformation.OSArchitecture == Architecture.X86))
                        {
                            //Path for 86
                            path86 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".bin", "win86", bin);
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            //Path for 64
                            path64 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".bin", "win64", bin);
                        }

                        var path = !string.IsNullOrWhiteSpace(path64) && File.Exists(path64) ? path64 :
                            !string.IsNullOrWhiteSpace(path86) && File.Exists(path86) ? path86 : null;

                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            var process = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = path,
                                    Arguments = $"{string.Join(" ", options).Trim()} {string.Join(" ", args).Trim()}"
                                        .Trim(),
                                    RedirectStandardOutput = true,
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                }
                            };

                            process.Start();
                            //var result = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();
                            //Console.WriteLine(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Run bin command from .bin directory async
        /// </summary>
        /// <param name="cmd">
        ///     Command to execute as string
        /// </param>
        public static void BinAsync(this string cmd)
        {
            Task.Run(cmd.Bin);
        }
    }
}