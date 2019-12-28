// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

#pragma warning disable 8509

namespace Arcanum.Routes {
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Text;
	using static System.String;
	using static Arcanum.Routes.HashFunctions;

	public readonly struct Route: IEquatable<Route> {
		public ImmutableArray<String> nodes { get; }

		Route (ImmutableArray<String> nodes) => this.nodes = nodes;

		public Boolean isDefault => nodes.IsDefault;

		public Boolean isEmpty => nodes.IsEmpty;

		public Boolean isDefaultOrEmpty => nodes.IsDefaultOrEmpty;

		public static Route empty { get; } = new Route(nodes: ImmutableArray<String>.Empty);

		public static Route Unit (String node) {
			if (IsNullOrEmpty(node))
				return empty;
			else if (node.IndexOf('/') is var slashPos && slashPos >= 0)
				throw new Exception($"Node '{node}' contain '/' at {slashPos}.");
			else
				return new Route(ImmutableArray.Create(node));
		}

		public static Route Join (IEnumerable<String> nodes) {
			var join = ImmutableArray.CreateBuilder<String>();
			var nodeOrder = 0u;
			foreach (var node in nodes) {
				if (! IsNullOrEmpty(node)) {
					if (node.IndexOf('/') is var slashPos && slashPos >= 0)
						throw new Exception($"Node {nodeOrder} '{node}' contain '/' at {slashPos}.");
					else
						join.Add(node);
				}
				nodeOrder = checked(nodeOrder + 1);
			}
			return new Route(nodes: join.ToImmutable());
		}

		public static Route Join (params String[] nodes) => Join((IEnumerable<String>) nodes);

		public static Route Parse (String routeStr) {
			var nodes = ImmutableArray.CreateBuilder<String>();

			var nodeStrStartPos = 0;
			while (nodeStrStartPos < routeStr.Length) {
				var slashPos =
					routeStr.IndexOf('/', nodeStrStartPos) is var maybeSlashPos && maybeSlashPos >= 0
						? maybeSlashPos
						: routeStr.Length;
				var nodeStrLength = slashPos - nodeStrStartPos;

				if (nodeStrLength > 0) {
					var node = routeStr.Substring(nodeStrStartPos, nodeStrLength);
					nodes.Add(node);
				}

				nodeStrStartPos = slashPos + 1;
			}

			return new Route(nodes.ToImmutable());
		}

		public static implicit operator Route (String routeStr) => Parse(routeStr);

		/// <inheritdoc />
		public override String ToString () {
			if (nodes.IsDefaultOrEmpty) return "";
			else {
				var str = new StringBuilder(128);
				foreach (var node in nodes) str.Append(node).Append('/');
				return str.RemoveLast().ToString();
			}
		}

		public Route Add (Route route) => new Route(nodes: nodes.AddRange(route.nodes));

		public static Route operator + (Route left, Route right) => left.Add(right);

		public Route Add (String node) {
			if (IsNullOrEmpty(node))
				return this;
			else if (node.IndexOf('/') is var slashPos && slashPos >= 0)
				throw new Exception($"Node '{node}' contain '/' at {slashPos}.");
			else
				return new Route(nodes: nodes.Add(node));
		}

		public static Boolean operator == (Route first, Route second) {
			if (first.nodes.Length != second.nodes.Length)
				return false;
			else {
				var firstEnum = first.nodes.GetEnumerator();
				var secondEnum = second.nodes.GetEnumerator();
				while (firstEnum.MoveNext()) {
					secondEnum.MoveNext();
					if (firstEnum.Current != secondEnum.Current) return false;
				}

				return true;
			}
		}

		public static Boolean operator != (Route first, Route second) => ! (first == second);

		/// <inheritdoc />
		public Boolean Equals (Route other) => this == other;

		/// <inheritdoc />
		public override Int32 GetHashCode () {
			if (nodes.IsDefault) return 0;
			else {
				var hash = 0;
				foreach (var node in nodes) hash = CombineHashes(hash, node.GetHashCode());
				return hash;
			}
		}

		/// <inheritdoc />
		public override Boolean Equals (Object? obj) =>
			! nodes.IsDefault && obj is Route other && ! other.nodes.IsDefault && this == other;
	}
}
