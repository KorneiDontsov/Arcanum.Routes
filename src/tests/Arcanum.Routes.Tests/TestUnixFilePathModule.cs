// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes.Tests {
	using FluentAssertions;
	using Xunit;

	public class TestUnixFilePathModule {
		[Fact]
		public void RouteConvertedToLocalPath () {
			Route route = "dir/nested-dir/file.ext";
			var path = route.ToLocalUnixFilePath();
			path.Should().Be("./dir/nested-dir/file.ext");
		}

		[Fact]
		public void EmptyRouteConvertedToLocalPath () {
			var route = Route.empty;
			var path = route.ToLocalUnixFilePath();
			path.Should().Be(".");
		}

		[Fact]
		public void RouteConvertedToFullPath () {
			Route route = "dir/nested-dir/file.ext";
			var path = route.ToFullUnixFilePath();
			path.Should().Be("/dir/nested-dir/file.ext");
		}

		[Fact]
		public void EmptyRouteConvertedToFullPath () {
			var route = Route.empty;
			var path = route.ToFullUnixFilePath();
			path.Should().Be("/");
		}
	}
}
