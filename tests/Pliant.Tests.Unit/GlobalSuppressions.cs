// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Unit tests are not performance critical code", Scope = "module")]
[assembly: SuppressMessage("Performance", "HAA0502:Explicit new reference type allocation", Justification = "Unit tests are not performance critical code", Scope = "module")]
