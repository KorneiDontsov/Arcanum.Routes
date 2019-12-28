// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes {
	using System.Text;

	static class StringBuilderFunctions {
		public static StringBuilder RemoveLast (this StringBuilder stringBuilder) =>
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
	}
}
