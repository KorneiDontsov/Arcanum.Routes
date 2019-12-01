// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes {
	using System;

	static class HashUtils {
		public static Int32 Combine (Int32 h1, Int32 h2) {
			var rol5 = ((UInt32) h1 << 5) | ((UInt32) h1 >> 27);
			return ((Int32) rol5 + h1) ^ h2;
		}

		public static Int32 Combine (Int32 h1, Int32 h2, Int32 h3) =>
			Combine(Combine(h1, h2), h3);
	}
}
