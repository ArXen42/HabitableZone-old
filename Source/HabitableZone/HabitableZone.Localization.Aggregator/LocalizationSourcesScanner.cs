using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HabitableZone.Common;
using HabitableZone.Localization.Common;
using UnityEngine;

namespace HabitableZone.Localization.Aggregator
{
	public sealed class LocalizationSourcesScanner
	{
		public LocalizationSourcesScanner(String rootPath)
		{
			_rootPath = rootPath;
		}

		public Dictionary<SystemLanguage, GameLocalization> GetLocalizations()
		{
			_result = new Dictionary<SystemLanguage, GameLocalization>();
			_relativePath = new List<String>();

			Deeper();

			return _result;
		}

		private void Deeper()
		{
			String currentPath = Path.Combine(new[] {_rootPath}.Concat(_relativePath).ToArray());

			var directories = Directory.GetDirectories(currentPath);
			var files = Directory.GetFiles(currentPath);

			if (directories.Length > 0)
			{
				if (files.Length > 0)
					throw new InvalidOperationException("There should be no files in intermediate directories.");

				foreach (String directory in directories)
				{
					var directoryInfo = new DirectoryInfo(directory);

					_relativePath.Add(directoryInfo.Name);
					Deeper();
					_relativePath.RemoveAt(_relativePath.Count - 1);
				}
			}
			else
			{
				foreach (String file in files)
				{
					String filename = Path.GetFileName(file);

					SystemLanguage language;
					Boolean parsingSuccesful = Enum.TryParse(Path.GetFileNameWithoutExtension(file), out language);

					if (parsingSuccesful)
						using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
						{
							var localizationString = Serialization.DeserializeDataFromJson<LocalizationString>(stream);

							var key = new StringBuilder();
							_relativePath.ForEach(p => key.Append(p + GameLocalization.KeysSeparator));
							key.Remove(key.Length - 1, 1);

							if (!_result.ContainsKey(language))
								_result.Add(language, new GameLocalization());

							_result[language].Add(key.ToString(), localizationString);
						}
					else
						Console.WriteLine($"SystemLanguage was not recognized, skipping \"{filename}\"");
				}
			}
		}

		private readonly String _rootPath;
		private List<String> _relativePath;

		private Dictionary<SystemLanguage, GameLocalization> _result;
	}
}