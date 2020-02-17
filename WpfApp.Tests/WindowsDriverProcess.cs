using System;
using System.Diagnostics;
using System.Text;

namespace WpfApp.Tests
{
    internal class WindowsDriverProcess
    {
        private readonly Uri uri;
        private Process process;

        private string previouslyReceivedMessage = null;
        private bool newLinePreviouslyWritten;

        public WindowsDriverProcess(Uri uri)
        {
            this.uri = uri;
        }

        internal void Start()
        {
            if (this.IsRunning())
            {
                return;
            }

            string exeFilePath = @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";
            string commandLineArguments = $"{this.uri.Host} {this.uri.Port}{this.uri.PathAndQuery} /forcequit";

            ProcessStartInfo processStartInfo = new ProcessStartInfo(
                exeFilePath,
                commandLineArguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            Log($"Launching \"{processStartInfo.FileName}\" {processStartInfo.Arguments}");

            this.process = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            this.process.ErrorDataReceived += this.OnDataReceived;
            this.process.OutputDataReceived += this.OnDataReceived;

            this.process.Start();

            this.process.BeginOutputReadLine();
            this.process.BeginErrorReadLine();
        }

        internal void Stop()
        {
            if ((this.process != null) && this.IsRunning())
            {
                Log($"Stopping process \"{this.process.StartInfo.FileName}\".");

                this.process.Kill();

                this.process.WaitForExit();
            }
        }

        private bool IsRunning()
        {
            return (this.process != null)
                && (!this.process.HasExited);
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            string utf8Message = e.Data.TrimStart('\0');

            if ((this.previouslyReceivedMessage == null)
                && (utf8Message == string.Empty)
                && !this.newLinePreviouslyWritten)
            {
                this.Log(string.Empty);
                this.previouslyReceivedMessage = utf8Message;
                this.newLinePreviouslyWritten = true;
                return;
            }
            else if ((this.previouslyReceivedMessage == string.Empty)
                && (utf8Message == string.Empty))
            {
                // do not write output
                this.previouslyReceivedMessage = null;
                return;
            }
            else if (utf8Message == string.Empty)
            {
                // do not write output
                this.previouslyReceivedMessage = utf8Message;
                return;
            }

            this.previouslyReceivedMessage = utf8Message;
            this.newLinePreviouslyWritten = false;

            byte[] bytes = Encoding.UTF8.GetBytes(utf8Message);
            string unicodeMessage = Encoding.Unicode.GetString(bytes);

            this.Log(unicodeMessage);
        }

        private void Log(string unicodeMessage)
        {
            Console.WriteLine(unicodeMessage);
        }
    }
}
