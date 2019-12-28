// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes {
	using System;
	using System.Text;

	public static class UnixFilePathModule {
		public static String ToLocalUnixFilePath (this Route route) {
			var path = new StringBuilder(128).Append('.');
			foreach (var node in route.nodes) path.Append('/').Append(node);
			return path.ToString();
		}

		public static String ToFullUnixFilePath (this Route route) {
			var nodeE = route.nodes.GetEnumerator();
			if (! nodeE.MoveNext()) return "/";
			else {
				var path = new StringBuilder(128);

				do path.Append('/').Append(nodeE.Current);
				while (nodeE.MoveNext());

				return path.ToString();
			}
		}
	}
}
