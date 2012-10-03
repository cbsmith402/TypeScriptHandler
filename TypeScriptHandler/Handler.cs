using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Diagnostics;

namespace TypeScriptHandler
{
	public class Handler : IHttpHandler
	{
		#region IHttpHandler Members

		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			string tsPath = context.Request.PhysicalPath;

			if (!File.Exists(tsPath))
			{
				context.Response.StatusCode = 404;
				context.Response.End();
				return;
			}

			string jsPath = context.Request.PhysicalPath.Replace(".ts", ".js");
			string tsDirectory = new FileInfo(context.Request.PhysicalPath).Directory.FullName;

			//Check if the file doesn't exist or has been changed since we compiled it
			if (!File.Exists(jsPath) || File.GetLastWriteTimeUtc(jsPath) < File.GetLastWriteTimeUtc(tsPath))
			{
				//Generate or regenerate the JS file
				//Whee, escaping!
				ExecuteCommandSync("\"tsc \"" + tsPath + "\"\"", tsDirectory);
			}

			//Write out the resulting file (we assume it exists if there were no errors written to stderr below)
			context.Response.WriteFile(jsPath);
		}

		public string ExecuteCommandSync(string command, string workingDirectory)
		{
			// create the ProcessStartInfo using "cmd" as the program to be run,
			// and "/c " as the parameters.
			// Incidentally, /c tells cmd that we want it to execute the command that follows,
			// and then exit.
			ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/s /c " + command);

			// The following commands are needed to redirect the standard output.
			// This means that it will be redirected to the Process.StandardOutput StreamReader.
			procStartInfo.RedirectStandardOutput = true;
			procStartInfo.RedirectStandardError = true;
			procStartInfo.UseShellExecute = false;
			// Do not create the black window.
			procStartInfo.CreateNoWindow = true;
			//Set the working directory
			procStartInfo.WorkingDirectory = workingDirectory;

			// Now we create a process, assign its ProcessStartInfo and start it
			Process proc = new Process();
			proc.StartInfo = procStartInfo;
			proc.Start();
			proc.WaitForExit();

			var output = proc.StandardOutput.ReadToEnd();
			var errors = proc.StandardError.ReadToEnd();

			if (!string.IsNullOrWhiteSpace(errors))
			{
				throw new Exception(errors);
			}

			return output;
		}

		#endregion
	}
}
