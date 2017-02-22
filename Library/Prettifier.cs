using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace CSharpCpp
{
	public class Prettifier
	{
		private const string UncrustifyCommandName = "uncrustify";

		private const string UncrustifyConfigId = "CSharpCpp.Configs.uncrustify.cfg";

		private readonly string _uncrustifyFileName;

		private readonly string _uncrustifyConfigFileName;

		public Prettifier()
		{
			try
			{
				_uncrustifyFileName = FileUtils.FindExecutable(UncrustifyCommandName);
			}
			catch (FileNotFoundException)
			{
				throw new FileNotFoundException(string.Format("Can't find '{0}' in system", UncrustifyCommandName));
			}

			_uncrustifyConfigFileName = FileUtils.CopyResourceToTmpFolder(UncrustifyConfigId);

			// TODO move this code when application starts
			// TODO add cleanup after application exited
			// Note: cleanup in destructor doesn't work with multiple threads.
		}

		public void Prettify(TFile file)
		{
			using (var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _uncrustifyFileName,
					Arguments = "-q -l cpp -c " + _uncrustifyConfigFileName,
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				}
			})
			{
				process.Start();

				using (var input = process.StandardInput)
				{
					input.WriteLine(file.Content);
				}

				process.WaitForExit();

				file.Content = process.StandardOutput.ReadToEnd();
			}
		}
	}
}
