// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jarvis.Addin.Files;
using Jarvis.Core;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Jarvis.Addin.Applications")]
[assembly: AssemblyDescription("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6e2c9a93-08d3-42c8-9822-9c17d118e646")]

// Expose internal types for testing.
[assembly: InternalsVisibleTo("Jarvis.Tests")]

// The addin definition.
[assembly: Addin(typeof(FileAddin))]
