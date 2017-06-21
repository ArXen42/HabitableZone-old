using Fclp;
using System;
using System.IO;
using HabitableZone.Common;

namespace HabitableZone.Localization.Aggregator
{
	internal class Program
	{
		public static void Main(String[] args)
		{
			var parser = new FluentCommandLineParser<ApplicationArguments>();

			parser.Setup(arg => arg.TargetRootPath)
				.As('t', "target")
				.WithDescription("Directory in which localization srings will be aggregated.")
				.Required();

			parser.Setup(arg => arg.OutputPath)
				.As('o', "output")
				.WithDescription("Directory in which output localization files will be stored.")
				.Required();

			parser.SetupHelp("h", "help")
				.WithHeader(
					"Scans given directory and aggregates localization strings in it into language localization files.")
				.Callback(t => Console.WriteLine(t))
				.UseForEmptyArgs();

			var result = parser.Parse(args);

			if (result.HasErrors)
			{
				Console.WriteLine($"Invalid arguments:{Environment.NewLine}{result.ErrorText}");
				return;
			}

			String targetRootPath = parser.Object.TargetRootPath;
			String outputPath = parser.Object.OutputPath;

			if (!Directory.Exists(targetRootPath))
			{
				Console.WriteLine($"Invalid arguments: {targetRootPath} does not exist.");
				return;
			}

			if (!Directory.Exists(outputPath))
			{
				Console.WriteLine($"Invalid arguments: {outputPath} does not exist.");
				return;
			}

			var scanner = new LocalizationSourcesScanner(targetRootPath);
			var localizations = scanner.GetLocalizations();

			foreach (var localization in localizations)
			{
				String outputFileName = Path.Combine(outputPath, localization.Key.ToString()) + "Localization.json";

				using (var stream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
				{
					Serialization.SerializeDataToJson(localization.Value, stream);
				}
			}
		}
	}

	public class ApplicationArguments
	{
		public String TargetRootPath { get; set; }
		public String OutputPath { get; set; }
	}
}