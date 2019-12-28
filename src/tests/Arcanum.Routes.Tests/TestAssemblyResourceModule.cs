// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes.Tests {
	using FluentAssertions;
	using System;
	using System.IO;
	using System.Reflection;
	using System.Threading.Tasks;
	using Xunit;

	public class TestAssemblyResourceModule {
		Assembly assembly { get; } = Assembly.GetExecutingAssembly();

		[Fact]
		public void FileNotFoundExceptionIsThrownWhereNoResourceFound () {
			Action action = () => assembly.OpenResourceStream("resources/nonexistent_resource.whatever");
			action.Should().Throw<FileNotFoundException>();
		}

		[Fact]
		public async Task ResourceTextIsRead () {
			var resourceText = await assembly.ReadResourceText("resources/text_resource.txt");
			resourceText.Should().Be("Text resource content.");
		}
	}
}
