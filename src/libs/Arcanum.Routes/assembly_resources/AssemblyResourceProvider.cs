// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes {
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading.Tasks;

	public static class AssemblyResourceProvider {
		/// <param name = "resourcePath"> Must be local. </param>
		/// <exception cref = "FileNotFoundException"> <paramref name = "resourcePath" /> was not found. </exception>
		/// <exception cref = "FileLoadException"> A file that was found could not be loaded. </exception>
		/// <exception cref = "BadImageFormatException"> <paramref name = "assembly" /> is not valid. </exception>
		/// <exception cref = "NotImplementedException">
		///     Resource length is greater than <see cref = "Int64.MaxValue" />.
		/// </exception>
		public static Stream OpenResourceStream (this Assembly assembly, Route resourcePath) {
			if (! resourcePath.isLocal)
				throw
					new ArgumentException(
						$"Failed to stream resource of assembly {assembly} because the specified resource path '{resourcePath}' is not local.");
			else {
				var resourceName =
					resourcePath.Select(node => ((RouteNode.Common) node).name)
						.Prepend(assembly.GetName().Name)
						.Join(".");

				return assembly.GetManifestResourceStream(resourceName);
			}
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
