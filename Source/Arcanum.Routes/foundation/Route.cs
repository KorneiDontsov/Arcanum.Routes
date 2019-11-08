// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

#pragma warning disable 8509

namespace Arcanum.Routes {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq;
	using System.Text.RegularExpressions;
	using static Arcanum.Routes.ImmutableCollectionUtils;

	public sealed class Route: IReadOnlyList<RouteNode>, IEquatable<Route> {
		ImmutableList<RouteNode> nodes { get; }

		Route (ImmutableList<RouteNode> nodes) => this.nodes = nodes;

		#region construction & deconstruction
		public static Route root { get; } = new Route(nodes: ImmutableList<RouteNode>.Empty);

		public static Route Unit (RouteNode routeNode) =>
			routeNode is RouteNode.Current ? root : new Route(ImmutableList.Create(routeNode));

		public static implicit operator Route (RouteNode routeNode) => Unit(routeNode);

		public static Route Join (IEnumerable<RouteNode> routeNodes) {
			var nodeList = ImmutableList.CreateBuilder<RouteNode>();
			foreach (var node in routeNodes)
				switch (node) {
					case RouteNode.Common commonNode:
						nodeList.Add(commonNode);
						break;
					case RouteNode.Current _: break;
					case RouteNode.Back _ when nodeList.LastOrDefault() is RouteNode.Common:
						nodeList.RemoveLast();
						break;
					case RouteNode.Back backNode:
						nodeList.Add(backNode);
						break;
					case var unknownNode:
						throw new Exception($"Route node {unknownNode.GetType()} is not expected.");
				}

			return new Route(nodes: nodeList.ToImmutable());
		}

		static Regex escapeRegex { get; } = new Regex(@"(\\|^\.$|^\.\.$)");

		static String EscapeNodeName (String nodeName) => escapeRegex.Replace(nodeName, @"\$1");

		static Regex unescapeRegex { get; } = new Regex(@"\\(.)", RegexOptions.Singleline);

		static String UnescapeNodeName (String escapedNodeName) => unescapeRegex.Replace(escapedNodeName, "$1");

		public static Route Parse (String routeString) {
			static IEnumerable<RouteNode> EnumerateNodes (String routeString) {
				static Int32 FindSlashPos (String routeString, Int32 startPos) {
					var searchPos = startPos;
					while (routeString.IndexOf('/', searchPos) is var slashPos && slashPos != -1)
						if (slashPos != 0 && routeString[slashPos - 1] == '\\')
							searchPos = slashPos + 1;
						else
							return slashPos;

					return routeString.Length;
				}

				var nodeStartPos = 0;
				do {
					var slashPos = FindSlashPos(routeString, nodeStartPos);
					var nodeString = routeString.Substring(nodeStartPos, slashPos - nodeStartPos);
					yield return
						nodeString switch {
							"." => (RouteNode) RouteNode.current,
							".." => RouteNode.back,
							_ => new RouteNode.Common(name: UnescapeNodeName(nodeString))
						};

					nodeStartPos = slashPos + 1;
				} while (nodeStartPos < routeString.Length);
			}

			var nodes = EnumerateNodes(routeString);
			return Join(nodes);
		}

		public static implicit operator Route (String routeString) => Parse(routeString);

		/// <inheritdoc />
		public override String ToString () =>
			nodes
				.Select(
					node =>
						node switch {
							RouteNode.Common commonNode => EscapeNodeName(commonNode.name),
							RouteNode.Back _ => ".."
						})
				.DefaultIfEmpty(".")
				.Join("/");

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
					(var first, { nodes: { IsEmpty: true } }) => first,
					({ nodes: { IsEmpty: true } }, var second) => second,
					var (first, second) => new Route(nodes: ConcatNodes(first.nodes, second.nodes))
				};
		}

		public static Route operator + (Route left, Route right) => left.Add(right);
		#endregion

		#region enumeration
		/// <inheritdoc />
		public Int32 Count => nodes.Count;

		/// <inheritdoc />
		public RouteNode this [Int32 index] => nodes[index];

		public struct Enumerator: IEnumerator<RouteNode> {
			ImmutableList<RouteNode>.Enumerator decorated;

			/// <inheritdoc />
			public RouteNode Current => decorated.Current;

			/// <inheritdoc />
			Object IEnumerator.Current => Current;

			internal Enumerator (Route source) => decorated = source.nodes.GetEnumerator();

			/// <inheritdoc />
			public void Dispose () => decorated.Dispose();

			/// <inheritdoc />
			public Boolean MoveNext () => decorated.MoveNext();

			/// <inheritdoc />
			public void Reset () => decorated.Reset();
		}

		public Enumerator GetEnumerator () => new Enumerator(this);

		/// <inheritdoc />
		IEnumerator<RouteNode> IEnumerable<RouteNode>.GetEnumerator () => nodes.GetEnumerator();

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
		#endregion

		#region comparison
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
		#endregion

		#region tools
		public Boolean isRoot => nodes.IsEmpty;

		public Boolean isLocal => nodes.IsEmpty || nodes[0] is RouteNode.Common;

		public Route CreateParent () =>
			nodes.LastOrDefault() switch {
				null => RouteNode.back,
				RouteNode.Common _ => new Route(nodes: nodes.RemoveLast()),
				RouteNode.Back _ => new Route(nodes: nodes.Add(RouteNode.back))
			};
		#endregion
	}
}
