using System.Runtime.InteropServices;
using NUnit.Framework;

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c0445534-55d9-4874-931c-dd73986e0bbb")]

// Parallelize test cases at fixture level
[assembly: Parallelizable(ParallelScope.Fixtures)]