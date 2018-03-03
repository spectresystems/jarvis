using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jarvis.Addin.Windows;
using Jarvis.Core;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Jarvis.Addin.Windows")]
[assembly: AssemblyDescription("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5e357761-af5a-4a88-b997-dd2e0d405866")]


// The addin definition.
[assembly: Addin(typeof(WindowsAddin))]