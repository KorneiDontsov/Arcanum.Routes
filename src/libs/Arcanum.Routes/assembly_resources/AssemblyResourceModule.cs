// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Routes {
	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Threading.Tasks;

	public static class AssemblyResourceModule {
		/// <param name = "namespace">
		///     The namespace of the resource. If null specified then the assembly name is
		///     used.
		/// </param>
		/// <exception cref = "FileNotFoundException"> <paramref name = "resourcePath" /> was not found. </exception>
		/// <exception cref = "FileLoadException"> A file that was found could not be loaded. </exception>
		/// <exception cref = "BadImageFormatException"> <paramref name = "assembly" /> is not valid. </exception>
		/// <exception cref = "NotImplementedException">
		///     Resource length is greater than <see cref = "Int64.MaxValue" />.
		/// </exception>
		public static Stream OpenResourceStream
		(this Assembly assembly,
		 Route resourcePath,
		 String? @namespace = null) {
			var resourceNameBuilder = new StringBuilder(128).Append(@namespace ?? assembly.GetName().Name);
			foreach (var node in resourcePath.nodes) resourceNameBuilder.Append('.').Append(node);
			var resourceName = resourceNameBuilder.ToString();
			return
				assembly.GetManifestResourceStream(resourceName)
				?? throw new FileNotFoundException("Resource not found.", resourceName);
		}

		/// <inheritdoc cref = "OpenResourceStream" />
		/// <param name = "encoding"> Encoding. If null specified then UTF8 is used. </param>
		public static TextReader OpenResourceText
		(this Assembly assembly,
		 Route resourcePath,
		 String? @namespace = null,
		 Encoding? encoding = null) =>
			new StreamReader(OpenResourceStream(assembly, resourcePath, @namespace), encoding ?? Encoding.UTF8);

		/// <inheritdoc cref = "OpenResourceStream" />
		public static async ValueTask<Byte[]> ReadResourceBytes
		(this Assembly assembly,
		 Route resourcePath,
		 String? @namespace = null) {
			using var resStream = assembly.OpenResourceStream(resourcePath, @namespace);
			using var memStream = new MemoryStream();
			await resStream.CopyToAsync(memStream, 81920).ConfigureAwait(false);
			return memStream.ToArray();
		}

		/// <inheritdoc cref = "OpenResourceText" />
		public static async ValueTask<String> ReadResourceText
		(this Assembly assembly,
		 Route resourcePath,
		 String? @namespace = null,
		 Encoding? encoding = null) {
			using var resourceTextReader = assembly.OpenResourceText(resourcePath, @namespace, encoding);
			return await resourceTextReader.ReadToEndAsync().ConfigureAwait(false);
		}
	}
}
