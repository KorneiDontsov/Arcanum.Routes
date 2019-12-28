// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes {
	using System;
	using System.Collections.Generic;

	static class StringUtils {
		public static String Join (this IEnumerable<String> values, String separator) =>
			String.Join(separator, values);
	}
}
