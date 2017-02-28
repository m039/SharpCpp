using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace SharpCpp
{
    public class Prettifier : IDisposable
    {
        private const string UncrustifyCommandName = "uncrustify";

        private const string UncrustifyConfigId = "SharpCpp.Configs.uncrustify.cfg";

        private readonly string _uncrustifyFileName;

        private readonly string _uncrustifyConfigFileName;

        private bool _disposed = false;

        public Prettifier()
        {
            try {
                _uncrustifyFileName = FileUtils.FindExecutable(UncrustifyCommandName);
            } catch (FileNotFoundException) {
                throw new TFatalException(string.Format("Can't find '{0}' in system", UncrustifyCommandName));
            }

            _uncrustifyConfigFileName = FileUtils.CopyResourceToTmpFolder(UncrustifyConfigId);
        }

        public void Dispose()
        {
            File.Delete(_uncrustifyConfigFileName);
            _disposed = true;
        }

        public void Prettify(GeneratedFile file)
        {
            if (_disposed)
                throw new TException("This object was disposed");

            using (var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = _uncrustifyFileName,
                    Arguments = "-q -l cpp -c " + _uncrustifyConfigFileName,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            }) {
                process.Start();

                using (var input = process.StandardInput) {
                    input.WriteLine(file.Content);
                }

                process.WaitForExit();

                file.Content = process.StandardOutput.ReadToEnd();
            }
        }
    }
}
