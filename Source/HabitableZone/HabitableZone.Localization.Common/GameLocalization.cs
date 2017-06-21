using System;
using System.Collections.Generic;

namespace HabitableZone.Localization.Common
{
	public sealed class GameLocalization : Dictionary<String, LocalizationString>
	{
		public const Char KeysSeparator = '.';

		public GameLocalization() { }
		public GameLocalization(Int32 capacity) : base(capacity) { }
		public GameLocalization(IEqualityComparer<String> comparer) : base(comparer) { }
		public GameLocalization(Int32 capacity, IEqualityComparer<String> comparer) : base(capacity, comparer) { }
		public GameLocalization(IDictionary<String, LocalizationString> dictionary) : base(dictionary) { }

		public GameLocalization(IDictionary<String, LocalizationString> dictionary,
			IEqualityComparer<String> comparer) : base(dictionary, comparer) { }
	}
}