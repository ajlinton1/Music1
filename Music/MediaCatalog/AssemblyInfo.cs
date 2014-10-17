using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General assembly blurb
[assembly: AssemblyTitle("Media Catalog")]
[assembly: AssemblyCopyright("Copyright (C) 2004 Microsoft Corporation. All rights reserved.")]
[assembly: AssemblyVersion("1.0.0.0")]

// Makes no sense for this to be COM-accessible, since
// the assembly itself is a wrapper around COM 
// functionality...
[assembly: ComVisible(false)]

// Unsigned integral values used.
// TODO: Revisit this to improve compatibility with VB.NET
[assembly: CLSCompliant(false)]