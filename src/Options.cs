using System;
using CommandLine;

namespace otpcli
{
	class Options
	{
		[Option('c', "config", 
			HelpText = "Use the specified configuration file.  If set to a non-absolute path, the file will be searched for using the current working directory first, and will fallback to the user's home directory.  If the file does not exist, the program will exit with an error code.")]
		public String ConfigurationFile { get; set; }

		[Option('w', "wait",
			HelpText = "Wait for a fresh token before returning.")]
		public Boolean Wait { get; set; }

		[Option('v', "verbose",
			HelpText = "Output will include informational messages.")]
		public Boolean Verbose { get; set; }
	}
}
