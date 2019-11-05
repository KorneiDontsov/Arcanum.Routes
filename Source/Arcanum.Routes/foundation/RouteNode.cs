// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes {
	using System;

	public abstract class RouteNode: IEquatable<RouteNode> {
		/// <inheritdoc />
		public virtual Boolean Equals (RouteNode? other) => GetType() == other?.GetType();

		/// <inheritdoc />
		public override Int32 GetHashCode () => GetType().GetHashCode();

		/// <inheritdoc />
		public override sealed Boolean Equals (Object? obj) => obj is RouteNode other && Equals(other);

		public static Boolean operator == (RouteNode? first, RouteNode? second) =>
			first?.Equals(second) ?? second is null;

		public static Boolean operator != (RouteNode? first, RouteNode? second) =>
			! (first == second);

		#region cases
		public sealed class Common: RouteNode {
			public String name { get; }

			public Common (String name) => this.name = name;

			/// <inheritdoc />
			public override Boolean Equals (RouteNode? other) => other is Common another && another.name == name;

			/// <inheritdoc />
			public override Int32 GetHashCode () => name.GetHashCode();
		}

		public sealed class Back: RouteNode { }

		public static Back back { get; } = new Back();
		#endregion
	}
}
