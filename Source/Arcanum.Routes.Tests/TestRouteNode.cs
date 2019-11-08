// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes.Tests {
	using FluentAssertions;
	using Xunit;

	public class TestRouteNode {
		[Fact]
		public void CommonNodeIsEqualToItself () {
			var node1 = new RouteNode.Common("common_node_name");
			node1.Should().Be(node1);
		}

		[Fact]
		public void CommonNodeIsEqualToOtherCommonNodeWithSameName () {
			var node1 = new RouteNode.Common("common_node_name");
			var node2 = new RouteNode.Common("common_node_name");

			node1.Should().Be(node2);
		}

		[Fact]
		public void CommonNodeIsNotEqualToOtherCommonNodeWithAnotherName () {
			var node1 = new RouteNode.Common("common_node_name");
			var node2 = new RouteNode.Common("common_node_another_name");

			node1.Should().NotBe(node2);
		}

		[Fact]
		public void CommonNodeIsNotEqualToBackNode () {
			var commonNode = new RouteNode.Common("common_node_name, not back node, A COMMON!");
			commonNode.Should().NotBe(RouteNode.back);
		}

		[Fact]
		public void CommonNodeIsNotEqualToCurrentNode () {
			var commonNode = new RouteNode.Common("common_node_name, not current node, A COMMON!");
			commonNode.Should().NotBe(RouteNode.current);
		}

		[Fact]
		public void BackNodeIsEqualToItself () =>
			RouteNode.back.Should().Be(RouteNode.back);

		[Fact]
		public void BackNodeIsNotEqualToCurrentNode () =>
			RouteNode.back.Should().NotBe(RouteNode.current);

		[Fact]
		public void CurrentNodeIsEqualToItself () =>
			RouteNode.current.Should().Be(RouteNode.current);
	}
}
