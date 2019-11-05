// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

#pragma warning disable 8509

namespace Arcanum.Routes {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq;
	using static Arcanum.Routes.ImmutableCollectionUtils;

	public sealed class Route: IReadOnlyList<RouteNode>, IEquatable<Route> {
		ImmutableList<RouteNode> nodes { get; }

		public Boolean isEmpty => nodes.IsEmpty;

		public Boolean isLocal => nodes.IsEmpty || nodes[0] is RouteNode.Common;

		/// <inheritdoc />
		public Int32 Count => nodes.Count;

		/// <inheritdoc />
		public RouteNode this [Int32 index] => nodes[index];

		Route (ImmutableList<RouteNode> nodes) => this.nodes = nodes;

		public Route (RouteNode routeNode): this(nodes: ImmutableList.Create(routeNode)) { }

		public static implicit operator Route (RouteNode routeNode) => new Route(routeNode);

		/// <inheritdoc />
		public IEnumerator<RouteNode> GetEnumerator () => nodes.GetEnumerator();

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

		public static Boolean operator == (Route? first, Route? second) =>
			first is { }
				? second is { } && first.nodes.SequenceEqual(second.nodes)
				: second is null;

		public static Boolean operator != (Route? first, Route? second) =>
			! (first == second);

		/// <inheritdoc />
		public Boolean Equals (Route? other) => this == other;

		/// <inheritdoc />
		public override Int32 GetHashCode () =>
			nodes.Aggregate(seed: 0, (hash, node) => HashUtils.Combine(hash, node.GetHashCode()));

		/// <inheritdoc />
		public override Boolean Equals (Object? obj) => obj is Route other && this == other;

		public static Route Parse (String routeString) {
			var nodes =
				routeString.Split('/')
					.Aggregate(
						seed: ImmutableList.CreateBuilder<RouteNode>(),
						(nodeList, nodeString) => {
							if (nodeString != "..")
								nodeList.Add(new RouteNode.Common(nodeString));
							else if (nodeList.LastOrDefault() is RouteNode.Common)
								nodeList.RemoveLast();
							else
								nodeList.Add(RouteNode.back);

							return nodeList;
						})
					.ToImmutable();

			return new Route(nodes);
		}

		public static implicit operator Route (String routeString) => Parse(routeString);

		/// <inheritdoc />
		public override String ToString () {
			var nodeStrings =
				nodes.Select(
					node =>
						node switch {
							RouteNode.Common commonNode => commonNode.name,
							RouteNode.Back _ => ".."
						});
			return String.Join("/", nodeStrings);
		}

		public Route Add (Route route) {
			static ImmutableList<RouteNode> ConcatNodes (
			ImmutableList<RouteNode> leftNodes, ImmutableList<RouteNode> rightNodes) {
				var redundantNodeCount =
					leftNodes.Reverse()
						.Zip(rightNodes, (left, right) => (left, right))
						.TakeWhile(pair => pair.left is RouteNode.Common && pair.right is RouteNode.Back)
						.Count();
				return
					leftNodes.Take(leftNodes.Count - redundantNodeCount)
						.Concat(rightNodes.Skip(redundantNodeCount))
						.ToImmutableList();
			}

			return
				(this, route) switch {
					(var first, { isEmpty: true }) => first,
					({ isEmpty: true }, var second) => second,
					var (first, second) => new Route(nodes: ConcatNodes(first.nodes, second.nodes))
				};
		}

		public static Route operator + (Route left, Route right) => left.Add(right);

		public Route CreateParent () =>
			nodes.LastOrDefault() switch {
				null => RouteNode.back,
				RouteNode.Common _ => new Route(nodes: nodes.RemoveLast()),
				RouteNode.Back _ => new Route(nodes: nodes.Add(RouteNode.back))
			};
	}
}
