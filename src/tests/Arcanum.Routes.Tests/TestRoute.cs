// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes.Tests {
	using FluentAssertions;
	using FluentAssertions.Execution;
	using System;
	using Xunit;

	public class TestRoute {
		[Fact]
		public void DefaultValueIsNotValid () {
			Route route = default;
			route.isDefault.Should().BeTrue();
		}

		[Fact]
		public void EmptyRouteIsCreated () {
			var route = Route.empty;

			using var assertionScope = new AssertionScope();
			route.Should().Be((Route) "");
			route.isDefault.Should().BeFalse();
			route.isEmpty.Should().BeTrue();
			route.nodes.Should().BeEmpty();
		}

		[Fact]
		public void UnitRouteIsCreatedFromCommonNode () {
			var route = Route.Unit("the node");

			using var assertionScope = new AssertionScope();
			route.Should().Be((Route) "the node");
			route.isDefault.Should().BeFalse();
			route.isEmpty.Should().BeFalse();
			route.nodes.Should().Equal("the node");
		}

		[Fact]
		public void UnitRouteIsCreatedFromEmptyNode () {
			var route = Route.Unit("");

			using var assertionScope = new AssertionScope();
			route.Should().Be((Route) "");
			route.isDefault.Should().BeFalse();
			route.isEmpty.Should().BeTrue();
			route.nodes.Should().BeEmpty();
		}

		[Fact]
		public void UnitRouteIsNotCreatedFromNodeContainedSlash () {
			Action action = () => Route.Unit("node/with/slashes");
			action.Should().ThrowExactly<Exception>();
		}

		[Fact]
		public void RouteIsJoined () {
			var route = Route.Join("first", "", "second", "third");

			using var assertionScope = new AssertionScope();
			route.Should().Be((Route) "first/second/third");
			route.isDefault.Should().BeFalse();
			route.isEmpty.Should().BeFalse();
			route.nodes.Should().Equal("first", "second", "third");
		}

		[Fact]
		public void RouteIsNotJoinedBecauseOfNodeContainedSlash () {
			Action action = () => Route.Join("first", "second/but third", "fourth");
			action.Should().ThrowExactly<Exception>();
		}

		[Fact]
		public void EmptyRouteIsJoined () {
			var route = Route.Join("", "", "");

			using var assertionScope = new AssertionScope();
			route.Should().Be((Route) "");
			route.isDefault.Should().BeFalse();
			route.isEmpty.Should().BeTrue();
			route.nodes.Should().BeEmpty();
		}

		[Fact]
		public void RoutesAreConcatenated () {
			Route leftPart = "first/second";
			Route rightPart = "third/fourth";

			var route = leftPart + rightPart;

			route.Should().Be((Route) "first/second/third/fourth");
		}

		[Fact]
		public void EmptyRoutesAreConcatenated () {
			Route leftPart = "";
			Route rightPart = "";

			var route = leftPart + rightPart;

			route.Should().Be((Route) "");
		}

		[Theory]
		[InlineData("", new String[0])]
		[InlineData("/", new String[0])]
		[InlineData("//", new String[0])]
		[InlineData("///", new String[0])]
		[InlineData("first", new[] { "first" })]
		[InlineData("/first", new[] { "first" })]
		[InlineData("first/", new[] { "first" })]
		[InlineData("/first/", new[] { "first" })]
		[InlineData("//first//", new[] { "first" })]
		[InlineData("first/second", new[] { "first", "second" })]
		[InlineData("/first/second", new[] { "first", "second" })]
		[InlineData("first/second/", new[] { "first", "second" })]
		[InlineData("/first/second/", new[] { "first", "second" })]
		[InlineData("first//second", new[] { "first", "second" })]
		[InlineData("first/second/third", new[] { "first", "second", "third" })]
		[InlineData("first/second/third/fourth", new[] { "first", "second", "third", "fourth" })]
		public void RouteStringIsParsed (String routeStr, String[] expectedNodes) {
			Route route = routeStr;
			route.nodes.Should().Equal(expectedNodes);
		}

		[Fact]
		public void RouteConvertedToString () {
			Route route = "first/second";
			var routeStr = route.ToString();
			routeStr.Should().Be("first/second");
		}

		[Fact]
		public void EmptyRouteConvertedToString () {
			Route route = "";
			var routeStr = route.ToString();
			routeStr.Should().Be("");
		}

		[Theory]
		[InlineData("", "")]
		[InlineData("/", "")]
		[InlineData("//", "")]
		[InlineData("///", "")]
		[InlineData("first", "first")]
		[InlineData("/first", "first")]
		[InlineData("first/", "first")]
		[InlineData("/first/", "first")]
		[InlineData("//first//", "first")]
		[InlineData("first/second", "first/second")]
		[InlineData("/first/second", "first/second")]
		[InlineData("first/second/", "first/second")]
		[InlineData("/first/second/", "first/second")]
		[InlineData("first//second", "first/second")]
		[InlineData("first/second/third", "first/second/third")]
		[InlineData("first/second/third/fourth", "first/second/third/fourth")]
		public void RoutesAreEqual (Route first, Route second) =>
			first.Should().Be(second);
	}
}
