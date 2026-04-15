using System;
using System.Collections.Generic;
using System.Linq;

namespace DunGen.Weighting
{
	/// <summary>
	/// A collection of weighted entries from which random selections can be made using complex weights
	/// based on the tile placement context (main path, branch path, depth, etc).
	/// Replaces the older <see cref="DunGen.GameObjectChanceTable"/> class in new code.
	/// </summary>
	[Serializable]
	public class WeightedTable<T>
	{
		public sealed class SelectionContext
		{
			public bool IsOnMainPath = true;
			public float NormalizedPathDepth = 0f;
			public float NormalizedBranchDepth = 0f;
			public bool AllowNullSelection = false;
			public bool AllowImmediateRepeats = true;
			public T PreviouslyChosen;
		}

		public List<WeightedEntry<T>> Entries = new List<WeightedEntry<T>>();


		public WeightedTable() { }

		public WeightedTable(WeightedTable<T> original)
		{
			Entries = new List<WeightedEntry<T>>(original.Entries);
		}

		public WeightedTable(params WeightedTable<T>[] tables)
		{
			foreach (var table in tables)
				foreach (var entry in table.Entries)
					Entries.Add(entry);
		}

		public float GetTotalWeight(SelectionContext context)
		{
			float totalWeight = 0;

			foreach (var w in Entries)
			{
				if (w == null)
					continue;
				if (!context.AllowNullSelection && w.Value == null)
					continue;
				if (!(context.AllowImmediateRepeats || context.PreviouslyChosen == null || !w.Value.Equals(context.PreviouslyChosen)))
					continue;

				totalWeight += w.GetEffectiveWeight(context.IsOnMainPath, context.NormalizedPathDepth, context.NormalizedBranchDepth);
			}

			return totalWeight;
		}

		public bool TrySelectRandomEntry(out WeightedEntry<T> result, RandomStream random, SelectionContext context, bool removeFromTable = false)
		{
			float totalWeight = 0;
			foreach (var w in Entries)
			{
				if (w == null)
					continue;

				if (!context.AllowNullSelection && w.Value == null)
					continue;

				if (!(context.AllowImmediateRepeats || context.PreviouslyChosen == null || !w.Value.Equals(context.PreviouslyChosen)))
					continue;

				totalWeight += w.GetEffectiveWeight(context.IsOnMainPath, context.NormalizedPathDepth, context.NormalizedBranchDepth);
			}

			float randomNumber = (float)(random.NextDouble() * totalWeight);

			foreach (var w in Entries)
			{
				if (w == null)
					continue;

				if (!context.AllowNullSelection && w.Value == null)
					continue;

				if (w.Value.Equals(context.PreviouslyChosen) && Entries.Count() > 1 && !context.AllowImmediateRepeats)
					continue;

				float weight = w.GetEffectiveWeight(context.IsOnMainPath, context.NormalizedPathDepth, context.NormalizedBranchDepth);

				if (randomNumber < weight)
				{
					if (removeFromTable)
						Entries.Remove(w);

					result = w;
					return true;
				}

				randomNumber -= weight;
			}

			result = default;
			return false;
		}

		public bool TrySelectRandom(out T result, RandomStream random, SelectionContext context, bool removeFromTable = false)
		{
			if (TrySelectRandomEntry(out var selectedEntry, random, context, removeFromTable))
			{
				result = selectedEntry.Value;
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}

		public bool ContainsValue(T value)
		{
			foreach (var entry in Entries)
			{
				if (entry == null)
					return false;

				if (entry.Value != null && entry.Value.Equals(value))
					return true;
			}

			return false;
		}

		public bool HasAnyValidEntries(SelectionContext context)
		{
			if (context.AllowNullSelection)
				return true;

			bool hasValidEntries = false;

			foreach (var entry in Entries)
			{
				if (entry.Value != null)
				{
					float weight = entry.GetEffectiveWeight(context.IsOnMainPath, context.NormalizedPathDepth, context.NormalizedBranchDepth);

					if (weight > 0f && (context.AllowImmediateRepeats || !context.PreviouslyChosen.Equals(entry.Value)))
					{
						hasValidEntries = true;
						break;
					}
				}
			}

			return hasValidEntries;
		}
	}
}