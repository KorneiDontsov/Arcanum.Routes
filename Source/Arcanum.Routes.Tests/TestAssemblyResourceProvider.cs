// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes.Tests {
	using FluentAssertions;
	using System.Reflection;
	using System.Threading.Tasks;
	using Xunit;

	public class TestAssemblyResourceProvider {
		Assembly assembly { get; } = Assembly.GetExecutingAssembly();

		[Fact]
		public async Task ResourceTextIsRead () {
			var resourceText = await assembly.ReadResourceText("resources/text_resource.txt");
			resourceText.Should().Be("Text resource content.");
		}
	}
}
