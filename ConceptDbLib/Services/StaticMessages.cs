using CptClientShared;
using CptClientShared.Entities;
using CptClientShared.Scopes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Services
{
    internal static class StaticMessages
    {
        internal static ConceptDbResponse LibraryCreated(int id, string name) =>
            new (ConceptDbResponseId.Success,
                "Library Created: [Id,Name]",
                new () { id.ToString(), name });

        internal static ConceptDbResponse LibraryNameUnavailable(string name) =>
            new(ConceptDbResponseId.Error,
                "Library Name Unavailable: [Name]",
                new() { name });

        internal static ConceptDbResponse LibraryIndexRequested =>
            new(ConceptDbResponseId.Success,
                "Libraries Listed: [name1, name2, etc]",
                new());

        internal static ConceptDbResponse ObjectRenamed(string libName, string oldName, string newName) =>
            new(ConceptDbResponseId.Success,
                "Object Renamed : [in library, oldName, newName]",
                new() { libName, oldName, newName });

        internal static ConceptDbResponse ObjectNotFound(string libName, string oldName) =>
            new(ConceptDbResponseId.Error, "Object Not Found: [in library, object name]",
                new() { libName, oldName });

        internal static ConceptDbResponse ParentChildObjRelationshipAdded(string libName, string parentName, string childName) =>
            new(ConceptDbResponseId.Success, "Parent Child Object Relationship Added [in library, parent object, child object]",
                new() { libName, parentName, childName });

        internal static ConceptDbResponse LibraryDeleted(CptLibrary lib) =>
            new(ConceptDbResponseId.Success,
                "Library Deleted: [Id,Name]",
                new() { lib.Id.ToString(), lib.Name });

        internal static ConceptDbResponse ObjectNameUnavailable(string name) =>
            new(ConceptDbResponseId.Error,
                "Object Name Unavailable: [Name]",
                new() { name });

        internal static ConceptDbResponse ObjectCreated(int id, string name) =>
            new(ConceptDbResponseId.Success,
                "Object Created: [Id,Name]",
                new() { id.ToString(), name });

        internal static ConceptDbResponse LibraryNameChanged(string oldName, string newName) =>
            new(ConceptDbResponseId.Success,
                "Libary name changed: [oldName], [newName]",
                new() { oldName, newName });

        internal static ConceptDbResponse LibraryScopeRetrieved(string name, LibraryScope scoped) =>
            new(ConceptDbResponseId.Success,
                "Library Scope Retrieved [name, serialized scope]",
                new() { name, JsonConvert.SerializeObject(scoped, Formatting.Indented) });

        internal static ConceptDbResponse LibraryNotFound(string name) =>
            new(ConceptDbResponseId.Error,
                "Library Not Found: [name]",
                new() { name });
    }
}
