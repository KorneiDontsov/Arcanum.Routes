// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

#pragma warning disable 8601 //disable nullable warnings

namespace Arcanum.Routes {
	using System.Collections.Immutable;

	static class ImmutableCollectionUtils {
		public static T LastOrDefault<T> (this ImmutableList<T> list) =>
			list.IsEmpty ? default : list[list.Count - 1];

		public static ImmutableList<T> RemoveLast<T> (this ImmutableList<T> list) =>
			list.RemoveAt(list.Count - 1);

		public static T LastOrDefault<T> (this ImmutableList<T>.Builder listBuilder) =>
			listBuilder.Count > 0 ? listBuilder[listBuilder.Count - 1] : default;

		public static void RemoveLast<T> (this ImmutableList<T>.Builder listBuilder) =>
			listBuilder.RemoveAt(listBuilder.Count - 1);
	}
}
