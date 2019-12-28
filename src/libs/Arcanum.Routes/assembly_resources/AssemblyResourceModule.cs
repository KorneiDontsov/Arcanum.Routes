// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes {
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading.Tasks;
	using static System.String;

	public static class AssemblyResourceModule {
		/// <exception cref = "FileNotFoundException"> <paramref name = "resourcePath" /> was not found. </exception>
		/// <exception cref = "FileLoadException"> A file that was found could not be loaded. </exception>
		/// <exception cref = "BadImageFormatException"> <paramref name = "assembly" /> is not valid. </exception>
		/// <exception cref = "NotImplementedException">
		///     Resource length is greater than <see cref = "Int64.MaxValue" />.
		/// </exception>
		public static Stream OpenResourceStream (this Assembly assembly, Route resourcePath) {
			var resourceNameBuilder = new StringBuilder(128).Append(assembly.GetName().Name);
			foreach (var node in resourcePath.nodes) resourceNameBuilder.Append('.').Append(node);
			var resourceName = resourceNameBuilder.ToString();
			return
				assembly.GetManifestResourceStream(resourceName)
				?? throw new FileNotFoundException("Resource not found.", resourceName);
		}

		/// <inheritdoc cref = "OpenResourceStream" />
		public static TextReader OpenResourceText (this Assembly assembly, Route resourcePath) =>
			new StreamReader(OpenResourceStream(assembly, resourcePath));

		/// <inheritdoc cref = "OpenResourceStream" />
		public static TextReader OpenResourceText (this Assembly assembly, Route resourcePath, Encoding encoding) =>
			new StreamReader(OpenResourceStream(assembly, resourcePath), encoding);

		/// <inheritdoc cref = "OpenResourceStream" />
		public static async ValueTask<Byte[]> ReadResourceBytes (this Assembly assembly, Route resourcePath) {
			using var resStream = assembly.OpenResourceStream(resourcePath);
			using var memStream = new MemoryStream();
			await resStream.CopyToAsync(memStream, 81920).ConfigureAwait(false);
			return memStream.ToArray();
		}

		/// <inheritdoc cref = "OpenResourceStream" />
		public static async ValueTask<String> ReadResourceText (this Assembly assembly, Route resourcePath) {
			using var resourceTextReader = assembly.OpenResourceText(resourcePath);
			return await resourceTextReader.ReadToEndAsync().ConfigureAwait(false);
		}
	}
}
