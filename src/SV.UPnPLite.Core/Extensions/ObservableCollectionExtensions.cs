﻿
namespace SV.UPnPLite.Core
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

    internal static class ObservableCollectionExtensions
	{
		public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> elements)
		{
			collection.EnsureNotNull("collection");

			if (elements != null)
			{
				foreach (var element in elements)
				{
					collection.Add(element);
				}
			}
		}
	}
}
