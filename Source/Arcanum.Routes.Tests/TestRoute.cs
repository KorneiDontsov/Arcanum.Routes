// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes.Tests {
	using FluentAssertions;
	using FluentAssertions.Execution;
	using System;
	using Xunit;

	public class TestRoute {
		[Fact]
		public void RootRouteIsCreated () {
			var route = Route.root;

			using (new AssertionScope()) {
				route.As<Object>().Should().Be(route).And.Be((Route) ".");
				route.Should().BeEmpty();
				route.Count.Should().Be(0);
				route.isRoot.Should().BeTrue();
				route.isLocal.Should().BeTrue();
			}
		}

		[Fact]
		public void UnitRouteIsCreatedFromCommonNode () {
			var routeNode = new RouteNode.Common("single_common_node");

			Route route = routeNode;

			using (new AssertionScope()) {
				route.As<Object>().Should().Be(route).And.Be((Route) "single_common_node");
				route.Should().Equal(routeNode);
				route.Count.Should().Be(1);
				route.isRoot.Should().BeFalse();
				route.isLocal.Should().BeTrue();
			}
		}

		[Fact]
		public void UnitRouteIsCreateFromBackNode () {
			var routeNode = RouteNode.back;

			Route route = routeNode;

			using (new AssertionScope()) {
				route.As<Object>().Should().Be(route).And.Be((Route) "..");
				route.Should().Equal(routeNode);
				route.Count.Should().Be(1);
				route.isRoot.Should().BeFalse();
				route.isLocal.Should().BeFalse();
			}
		}

		[Fact]
		public void UnitRouteIsCreatedFromCurrentNode () {
			var routeNode = RouteNode.current;

			Route route = routeNode;

			using (new AssertionScope()) {
				route.As<Object>().Should().Be(route).And.Be((Route) ".");
				route.Should().BeEmpty();
				route.Count.Should().Be(0);
				route.isRoot.Should().BeTrue();
				route.isLocal.Should().BeTrue();
			}
		}

		[Fact]
		public void LocalRouteIsJoined () {
			var route =
				Route.Join(
					new RouteNode.Common("first"),
					RouteNode.current,
					new RouteNode.Common("first_missed"),
					RouteNode.back,
					new RouteNode.Common("second"));

			using (new AssertionScope()) {
				route.As<Object>().Should().Be(route).And.Be((Route) "first/second");
				route.Should().Equal(new RouteNode.Common("first"), new RouteNode.Common("second"));
				route.Count.Should().Be(2);
				route.isRoot.Should().BeFalse();
				route.isLocal.Should().BeTrue();
			}
		}

		[Fact]
		public void OuterRouteIsJoined () {
			var route =
				Route.Join(
					RouteNode.back,
					RouteNode.current,
					new RouteNode.Common("neighborhood"));

			using (new AssertionScope()) {
				route.As<Object>().Should().Be(route).And.Be((Route) "../neighborhood");
				route.Should().Equal(RouteNode.back, new RouteNode.Common("neighborhood"));
				route.Count.Should().Be(2);
				route.isRoot.Should().BeFalse();
				route.isLocal.Should().BeFalse();
			}
		}

		[Fact]
		public void RootRouteIsJoined () {
			var route =
				Route.Join(
					RouteNode.current,
					new RouteNode.Common("forget"),
					RouteNode.back,
					RouteNode.current);

			using (new AssertionScope()) {
				route.As<Object>().Should().Be(route).And.Be((Route) ".");
				route.Should().BeEmpty();
				route.Count.Should().Be(0);
				route.isRoot.Should().BeTrue();
				route.isLocal.Should().BeTrue();
			}
		}

		[Fact]
		public void RoutesConcatenatedWithoutCompression () {
			Route leftPart = "first/second";
			Route rightPart = "third/fourth";

			var route = leftPart + rightPart;

			route.As<Object>().Should().Be((Route) "first/second/third/fourth");
		}

		[Fact]
		public void RoutesConcatenatedWithCompression () {
			Route leftPart = "first/doesn't matter";
			Route rightPart = "../second";

			var route = leftPart + rightPart;

			route.As<Object>().Should().Be((Route) "first/second");
		}

		[Fact]
		public void RoutesConcatenatedToRoot () {
			Route leftPart = "first/second";
			Route rightPart = "../..";

			var route = leftPart + rightPart;

			route.As<Object>().Should().Be((Route) ".");
		}

		[Fact]
		public void RouteStringIsParsed () {
			var routeString = "./../first/doesn't matter/../second/any stuff/../.";

			Route route = routeString;

			route.As<Object>().Should().Be((Route) "../first/second");
		}

		[Fact]
		public void RouteConvertedToString () {
			Route route = "../first/second";

			var routeString = route.ToString();

			routeString.Should().Be("../first/second");
		}

		[Fact]
		public void RootRouteConvertedToString () {
			Route route = ".";

			var routeString = route.ToString();

			routeString.Should().Be(".");
		}
	}
}
