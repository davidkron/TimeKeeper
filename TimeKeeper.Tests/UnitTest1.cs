using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimeKeeper.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetTime()
        {
            String CmdText = @"/c quser";

            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = CmdText,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                }
            };

            proc.Start();
            proc.WaitForExit();
            String line = proc.StandardOutput.ReadToEnd();
        }
    }
}
