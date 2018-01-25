using System.Reflection;
using System.Runtime.InteropServices;
using Jarvis.Addin.Processes;
using Jarvis.Core;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Jarvis.Addin.Processes")]
[assembly: AssemblyDescription("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("AE5C446D-B1D3-4E98-941D-A751B80CA74F")]

// The addin definition.
[assembly: Addin(typeof(ProcessAddin))]