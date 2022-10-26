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
        internal static ConceptDbResponse PropertyNameUnavailable(string libName, string propName) =>
            new(ConceptDbResponseId.Error,
                "Property name not available: [library, property]",
                new() { libName, propName });

        internal static ConceptDbResponse PropertyDeleted(string libName, string propName) =>
            new(ConceptDbResponseId.Success,
                "Property deleted : [library, property]",
                new() { libName, propName });

        internal static ConceptDbResponse PropertyAlreadyAssigned(string libName, string propName, string objName) =>
            new(ConceptDbResponseId.Error,
                "Property Already Assigned: [library, propName, objName]",
                new() { libName, propName, objName });

        internal static ConceptDbResponse PropertyUnassigned(string libName, string propName, string objName) =>
            new(ConceptDbResponseId.Success,
                "Property Unassigned: [library, propName, objName]",
                new() { libName, propName, objName });

        internal static ConceptDbResponse PropertyNameChanged(string libName, string oldName, string newName) =>
            new(ConceptDbResponseId.Success,
                "Property name changed: [library, oldName, newName]",
                new() { libName, oldName, newName });

        internal static ConceptDbResponse PropertyAssigned(string libName, string propName, string objName) =>
            new(ConceptDbResponseId.Success,
                "Property Assigned: [library, propName, objName]",
                new() { libName, propName, objName });

        internal static ConceptDbResponse PropertyCreated(string libName, string propName) =>
            new(ConceptDbResponseId.Success,
                "Property created: [library, property]",
                new() { libName, propName });

        internal static ConceptDbResponse PropertyNotFound(string libName, string propName) =>
            new(ConceptDbResponseId.Error,
                "Property not found: [library, property]",
                new() { libName, propName });

        internal static ConceptDbResponse ObjectDeleted(string libName, string objName) =>
            new (ConceptDbResponseId.Success,
                "Object removed from library: [library, object]",
                new() { libName, objName });

        internal static ConceptDbResponse ObjectRenamed(string libName, string oldName, string newName) =>
            new(ConceptDbResponseId.Success,
                "Object Renamed : [in library, oldName, newName]",
                new() { libName, oldName, newName });

        internal static ConceptDbResponse ObjPropValuesSet(string libName, string propName, string objName) =>
            new(ConceptDbResponseId.Success,
                "Object Property Values Set : [library, propName, objName] (request new scope for updated data)",
                new() { libName, propName, objName });

        internal static ConceptDbResponse ParentChildObjectRelationshipSevered(string libName, string pName, string childName) =>
            new(ConceptDbResponseId.Success, "Object Parent Child Relationship Severed: [library, parent object, child object]",
                new() { libName, pName, childName });

        internal static ConceptDbResponse ObjectNotFound(string libName, string objName) =>
            new(ConceptDbResponseId.Error, "Object Not Found: [in library, object name]",
                new() { libName, objName });

        internal static ConceptDbResponse ObjectHasNoParent(string libName, string childName) =>
            new(ConceptDbResponseId.Error, "Object Has No Parent: [library, object]",
                new() { libName, childName });

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
                new() { 
                    name,
                    JsonConvert.SerializeObject(scoped, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
                });

        internal static ConceptDbResponse LibraryNotFound(string name) =>
            new(ConceptDbResponseId.Error,
                "Library Not Found: [name]",
                new() { name });

        internal static ConceptDbResponse ObjectTypeNotFound(string libName, string typeName) =>
            new(ConceptDbResponseId.Error,
                "Object Type Not Found: [library, object type]",
                new() { libName, typeName });

        internal static ConceptDbResponse ObjectTypeNameUnavailable(string libName, string newType) =>
            new(ConceptDbResponseId.Error,
                "Object Type Already Exists: [library, object type]",
                new() { libName, newType });

        internal static ConceptDbResponse ObjectTypeAddedToLibrary(string libName, string parentType, string newType) =>
            new(ConceptDbResponseId.Success,
                "Object Type Created : [library, parentType, newType]",
                new () { libName, parentType, newType});

        internal static ConceptDbResponse ObjectTypeNameChanged(string libName, string oldName, string newName) =>
            new(ConceptDbResponseId.Success,
                "Object Type Name Changed : [library, oldName, newName]",
                new() { libName, oldName, newName });

        internal static ConceptDbResponse ObjectTypeDeleted(string libName, string objTypeName) =>
            new (ConceptDbResponseId.Success,
                "Object Type Deleted : [library, objectType]",
                new () { libName, objTypeName });

        internal static ConceptDbResponse ObjectTypeMoved(string libName, string objType, string newParent) =>
            new(ConceptDbResponseId.Success,
                "Object Type Moved : [library, objectType, newParent]",
                new() { libName, objType, newParent });
    }
}
