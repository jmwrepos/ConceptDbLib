using CptClientShared;
using CptClientShared.Entities;
using CptClientShared.Entities.Accounting;
using CptClientShared.Scopes;
using System.Net.Mail;

namespace ConceptDbLib.Services
{
    public class BuilderService
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        private readonly CptAcctUser _user;
        private Action SaveDb;
        private readonly ValidationService _validation;
        public BuilderService(CptDbProvider dbProvider, CptAcctUser user)
        {
            _user = user;
            SaveDb = () => { dbProvider.Context.SaveChanges(); };
            _validation = dbProvider.ValidationService;
        }

        internal ConceptDbResponse DeleteUser(string email)
        {
            bool userExists = _user.Account.Users.Any(e => e.Email.ToLower() == email.ToLower());
            if (userExists)
            {
                CptAcctUser targetUser = _user.Account.Users.Where(e => e.Email.ToLower() == email.ToLower()).First();
                if(targetUser != _user)
                {
                    _user.Account.Users.Remove(targetUser);
                    SaveDb();
                    return new(ConceptDbResponseId.Success, "User Deleted [email]", new() { email });
                }
                else
                {
                    return new(ConceptDbResponseId.Error, "Cannot delete self.", new() { email });
                }
            }
            else
            {
                return StaticMessages.UserNotFound(email);
            }
        }
        internal ConceptDbResponse InactivateUser(string email)
        {
            bool userExists = _user.Account.Users.Any(e => e.Email.ToLower() == email.ToLower());
            if (userExists)
            {
                CptAcctUser targetUser = _user.Account.Users.Where(e => e.Email.ToLower() == email.ToLower()).First();
                foreach(CptAcctUser listedUser in _user.Account.Users)
                {
                    if(listedUser != targetUser && listedUser.Active)
                    {
                        targetUser.Active = false;
                        SaveDb();
                        return new(ConceptDbResponseId.Success, "User Inactivated [user]", new() { targetUser.Email });
                    }
                }
                return new(ConceptDbResponseId.Error, "Cannot inactivate only active user.", new() { email });
            }
            else
            {
                return StaticMessages.UserNotFound(email);
            }
        }
            internal ConceptDbResponse UpdateAccountName(string newName)
        {
            if (CptRegex.Name.IsMatch(newName))
            {
                _user.Account.AccountName = newName;
                SaveDb();
                return new(ConceptDbResponseId.Success, "Account Name Updated [new name]", new() { newName });
            }
            else
            {
                return new(ConceptDbResponseId.Error, "Account Name Invalid", new() { newName });
            }
        }
        internal ConceptDbResponse RetrieveUserScope(string email)
        {
            bool userExists = _user.Account.Users.Any(e => e.Email.ToLower() == email.ToLower());
            if (userExists)
            {
                string msg = string.Empty;
                CptAcctUser user = _user.Account.Users.Where(e => e.Email.ToLower() == email.ToLower()).First();
                UserScope scope = new(user);
                return StaticMessages.UserScopeRetrieved(email, scope);
            }
            else
            {
                return StaticMessages.UserNotFound(email);
            }
        }

        internal ConceptDbResponse RetrieveAccountScope()
        {
            return StaticMessages.AccountScopeRetrieved(new AccountScope(_user.Account));
        }
        internal ConceptDbResponse UpdateUser(string currentEmail, string? firstName, string? lastName, string? newEmail, string? password)
        {
            bool userExists = _user.Account.Users.Any(e => e.Email.ToLower() == currentEmail.ToLower());
            if (userExists)
            {
                string msg = string.Empty;
                CptAcctUser user = _user.Account.Users.Where(e => e.Email.ToLower() == currentEmail.ToLower()).First();
                bool fnValid = true;
                bool lnValid = true;
                bool emailValid = true;
                bool pwValid = true;
                if (firstName != null)
                {
                    fnValid = CptRegex.Name.IsMatch(firstName);
                    if (!fnValid)
                    {
                        msg += "First Name Invalid";
                    }
                }
                if(lastName != null)
                {
                    lnValid = CptRegex.Name.IsMatch(lastName);
                    if (!lnValid)
                    {
                        msg += ", Last Name Invalid";
                    }
                }
                if(newEmail != null)
                {
                    bool dupe = !_validation.UsernameAvailable(newEmail);
                    if (!dupe)
                    {
                        try
                        {
                            MailAddress emailObj = new(newEmail);
                        }
                        catch
                        {
                            emailValid = false;
                        }
                        if (!emailValid)
                        {
                            msg += ", Email Invalid";
                        }
                    }
                    else
                    {
                        emailValid = false;
                        msg += ", Email Duplicate";
                    }
                }
                if(password != null)
                {
                    pwValid = CptRegex.Password.IsMatch(password);
                    if (!pwValid)
                    {
                        msg += ", Password Too Weak or Invalid";
                    }
                }
                if(fnValid && lnValid && emailValid && pwValid)
                {
                    if(firstName != null)
                    {
                        user.FirstName = firstName;
                        msg += "User First Name Changed";
                    }
                    if(lastName != null)
                    {
                        user.LastName = lastName;
                        msg += ", User Last Name Changed";
                    }
                    if(newEmail != null)
                    {
                        user.Email = newEmail;
                        msg += ", User Email Changed";
                    }
                    if(password != null)
                    {
                        user.Password = ApiEncryption.Encrypt(user.Account.EncryptionKey, user.UserIV, password);
                        msg += ", User Password Changed";
                    }
                    SaveDb();
                    return new(ConceptDbResponseId.Success, msg, new() { firstName ?? String.Empty, lastName ?? String.Empty, newEmail ?? String.Empty, password ?? String.Empty });
                }
                else
                {
                    return new(ConceptDbResponseId.Error, msg, new() { currentEmail, firstName ?? String.Empty, lastName ?? String.Empty, newEmail ?? String.Empty, password ?? String.Empty });
                }
            }
            else
            {
                return StaticMessages.UserNotFound(currentEmail);
            }
        }
        internal ConceptDbResponse NewUser(string? firstName, string? lastName, string? email, string? password)
        {
            firstName ??= string.Empty;
            lastName ??= string.Empty;
            email ??= string.Empty;
            password ??= string.Empty;
            bool firstNameValid = CptRegex.Name.IsMatch(firstName);
            bool lastNameValid = CptRegex.Name.IsMatch(lastName);
            bool emailValid;
            try
            {
                MailAddress emailObj = new(email);
                emailValid = emailObj.Address == email;
            }
            catch
            {
                emailValid = false;
            }            
            bool passwordValid = CptRegex.Password.IsMatch(password);
            if (!firstNameValid)
            {
                return new(ConceptDbResponseId.Error, "First Name Invalid", new() { firstName });
            }
            if (!lastNameValid)
            {
                return new(ConceptDbResponseId.Error, "Last Name Invalid", new() { lastName });
            }
            if (!emailValid)
            {
                return new(ConceptDbResponseId.Error, "Email/Username Invalid", new() { email });
            }
            if (!passwordValid)
            {
                return new(ConceptDbResponseId.Error, "Password Too Weak or Invalid", new() { password });
            }
            bool usernameAvail = _validation.UsernameAvailable(email);
            if (usernameAvail)
            {
                CptAccount acct = _user.Account;
                CptAcctUser newUser = new()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email.ToLower(),
                    UserIV = ApiEncryption.NewIV(),
                    Active = true
                };
                newUser.Password = ApiEncryption.Encrypt(acct.EncryptionKey, newUser.UserIV, password);
                acct.Users.Add(newUser);
                SaveDb();
                return new(ConceptDbResponseId.Success, "User Added: [Email/Username]", new() { email });
            }
            else
            {
                return new(ConceptDbResponseId.Error, "Username Not Available: [email]", new() { email });
            }
        }
        internal ConceptDbResponse SetPropertyValue(string libName, string objName, string propName, string stringVals, string objNameVals, string numVals)
        {
            List<string> sVals = stringVals.Split("_").ToList();
            List<string> onVals = objNameVals.Split("_").ToList();
            List<double> nVals = new();
            foreach(string entry in numVals.Split("_"))
            {
                if(entry != string.Empty && entry != null)
                {
                    nVals.Add(double.Parse(entry));
                }
            }

            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult objSearch = SearchObject(lib, objName);
                if (objSearch.Found)
                {
                    CptObject obj = objSearch.Objects[0];
                    foreach(CptObjectProperty objProp in obj.ObjectProperties)
                    {
                        if(objProp.Property.Name == propName)
                        {
                            objProp.StringValues.Clear();
                            objProp.ObjNameValues.Clear();
                            objProp.NumberValues.Clear();
                            foreach (string entry in sVals)
                            {
                                if(entry.Length > 0 && entry != null)
                                {
                                    objProp.StringValues.Add(new(entry));
                                }
                            }
                            foreach (string entry in onVals)
                            {
                                if (entry.Length > 0 && entry != null)
                                {
                                    objProp.ObjNameValues.Add(new(entry));
                                }
                            }
                            foreach (double entry in nVals)
                            {
                                objProp.NumberValues.Add(new(entry));
                            }
                            SaveDb();
                            return StaticMessages.ObjPropValuesSet(libName, propName, objName);
                        }
                    }
                    return StaticMessages.PropertyNotFound(libName, propName);
                }
                else
                {
                    return StaticMessages.ObjectNotFound(libName, objName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        //PROPERTY CRUD
        internal ConceptDbResponse UnassignProperty(string libName, string propName, string objName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult propSearch = SearchProperty(lib, propName);
                if (propSearch.Found)
                {
                    DbSearchResult objSearch = SearchObject(lib, objName);
                    if (objSearch.Found)
                    {
                        CptObject obj = objSearch.Objects[0];
                        CptProperty prop = propSearch.Properties[0];
                        for(int i = 0; i < obj.ObjectProperties.Count; i++)
                        {
                            CptObjectProperty objProp = obj.ObjectProperties[i];
                            if (objProp.Property == prop)
                            {
                                obj.ObjectProperties.Remove(objProp);
                                prop.ObjectProperties.Remove(objProp);
                            }

                        }
                        SaveDb();
                        return StaticMessages.PropertyUnassigned(libName, propName, objName);
                    }
                    else
                    {
                        return StaticMessages.ObjectNotFound(libName, objName);
                    }
                }
                else
                {
                    return StaticMessages.PropertyNotFound(libName, propName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }

        }
        internal ConceptDbResponse AssignProperty(string libName, string propName, string objName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult propSearch = SearchProperty(lib, propName);
                if (propSearch.Found)
                {
                    DbSearchResult objSearch = SearchObject(lib, objName);
                    if (objSearch.Found)
                    {
                        CptObject obj = objSearch.Objects[0];
                        CptProperty prop = propSearch.Properties[0];
                        foreach(CptObjectProperty objProp in obj.ObjectProperties)
                        {
                            if(objProp.Property == prop)
                            {
                                return StaticMessages.PropertyAlreadyAssigned(libName, propName, objName);
                            }
                        }
                        CptObjectProperty newObjProp = new();
                        obj.ObjectProperties.Add(newObjProp);
                        prop.ObjectProperties.Add(newObjProp);
                        SaveDb();
                        return StaticMessages.PropertyAssigned(libName, propName, objName);
                    }
                    else
                    {
                        return StaticMessages.ObjectNotFound(libName, objName);
                    }
                }
                else
                {
                    return StaticMessages.PropertyNotFound(libName, propName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse DeleteProperty(string libName, string propName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult propSearch = SearchProperty(lib, propName);
                if (propSearch.Found)
                {
                    CptProperty prop = propSearch.Properties[0];
                    lib.Properties.Remove(prop);
                    SaveDb();
                    return StaticMessages.PropertyDeleted(libName, propName);
                }
                else
                {
                    return StaticMessages.PropertyNotFound(libName, propName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse RenameProperty(string libName, string oldName, string newName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult propSearch = SearchProperty(lib, oldName);
                if (propSearch.Found)
                {
                    DbSearchResult existing = SearchProperty(lib, newName);
                    if (!existing.Found)
                    {
                        CptProperty prop = propSearch.Properties[0];
                        prop.Name = newName;
                        SaveDb();
                        return StaticMessages.PropertyNameChanged(libName, oldName, newName);
                    }
                    else
                    {
                        return StaticMessages.PropertyNameUnavailable(libName, newName);
                    }                    
                }
                else
                {
                    return StaticMessages.PropertyNotFound(libName, oldName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse NewProperty(string libName, string propName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult propSearch = SearchProperty(lib, propName);
                if (propSearch.Found)
                {
                    return StaticMessages.PropertyNameUnavailable(libName, propName);
                }
                else
                {
                    CptProperty newProp = new()
                    {
                        Name = propName
                    };
                    lib.Properties.Add(newProp);
                    SaveDb();
                    return StaticMessages.PropertyCreated(libName, propName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }

        private static DbSearchResult SearchProperty(CptLibrary lib, string propName)
        {
            foreach(CptProperty property in lib.Properties)
            {
                if(property.Name == propName)
                {
                    return DbSearchResult.PropFound(property);
                }
            }
            return DbSearchResult.PropNotFound;
        }

        //OBJECT CRUD
        internal ConceptDbResponse DeleteObject(string libName, string objName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            DbSearchResult objSearch = SearchObject(libSearch.Libraries[0], objName);
            if (objSearch.Found)
            {
                CptObject obj = objSearch.Objects[0];
                CptLibrary lib = libSearch.Libraries[0];
                lib.Objects.Remove(obj);
                SaveDb();
                return StaticMessages.ObjectDeleted(libName, objName);
            }
            else
            {
                return objSearch.ResultId switch
                {
                    ResultId.Unspecified => throw new InvalidOperationException("Result not populated."),
                    ResultId.Success => throw new InvalidOperationException("Unexpected operation."),
                    ResultId.LibNotFound => StaticMessages.LibraryNotFound(libName),
                    ResultId.ObjNotInLib => StaticMessages.ObjectNotFound(libName, objName),
                    _ => throw new InvalidOperationException("Invalid Value in Switch Case."),
                };
            }
        }
        internal ConceptDbResponse MoveObject(string libName, string? parentName, string childName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                DbSearchResult childObjSearch = SearchObject(libSearch.Libraries[0], childName);
                if (childObjSearch.Found)
                {
                    if (parentName != null && parentName != string.Empty)
                    {
                        DbSearchResult parentObjSearch = SearchObject(libSearch.Libraries[0], parentName);
                        if (parentObjSearch.Found)
                        {
                            CptObject parent = parentObjSearch.Objects[0];
                            CptObject child = childObjSearch.Objects[0];
                            child.Parent = parent;
                            SaveDb();
                            return StaticMessages.ParentChildObjRelationshipAdded(libName, parentName, childName);
                        }
                        else
                        {
                            return StaticMessages.ObjectNotFound(libName, parentName);
                        }
                    }
                    else
                    {
                        CptObject child = childObjSearch.Objects[0];
                        CptObject? parent = childObjSearch.Objects[0].Parent;
                        if (parent != null)
                        {
                            string pName = parent.Name;
                            child.Parent = null;
                            SaveDb();
                            return StaticMessages.ParentChildObjectRelationshipSevered(libName, pName, childName);

                        }
                        else
                        {
                            return StaticMessages.ObjectHasNoParent(libName, childName);
                        }
                    }
                }
                else
                {
                    return StaticMessages.ObjectNotFound(libName, childName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse RenameObject(string libName, string oldName, string newName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                DbSearchResult objSearch = SearchObject(libSearch.Libraries[0], oldName);
                if (objSearch.Found)
                {
                    CptObject obj = objSearch.Objects[0];
                    obj.Name = newName;
                    SaveDb();
                    return StaticMessages.ObjectRenamed(libName, oldName, newName);
                }
                else
                {
                    return StaticMessages.ObjectNotFound(libName, oldName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }

        }
        internal ConceptDbResponse NewObject(string libName, string objName, string parentObjName, string objTypes)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                return NewObject(libSearch.Libraries[0], objName, parentObjName, objTypes);
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse NewObject(CptLibrary lib, string objName, string parentObjName, string objTypes)
        {
            string libName = lib.Name;
            List<string> objTypeNames = objTypes == string.Empty ? new() { "root" } : objTypes.Split("_").ToList();
            DbSearchResult objSearch = SearchObject(lib, objName);
            if (objSearch.Found)
            {
                return StaticMessages.ObjectNameUnavailable(objName);
            }
            else
            {
                DbSearchResult libSearch = SearchLibrary(libName);
                if (libSearch.Found)
                {
                    CptObject newObj = new()
                    {
                        Name = objName
                    };
                    libSearch.Libraries[0].Objects.Add(newObj);
                    if(parentObjName != string.Empty)
                    {
                        DbSearchResult parentObjSearch = SearchObject(libSearch.Libraries[0], parentObjName);
                        if (parentObjSearch.Found)
                        {
                            newObj.Parent = parentObjSearch.Objects[0];
                        }
                        else
                        {
                            return StaticMessages.ObjectNotFound(libName, parentObjName);
                        }
                    }
                    foreach(string objTypeName in objTypeNames)
                    {
                        DbSearchResult objTypeSearch = SearchObjType(libSearch.Libraries[0], objTypeName);
                        if (objTypeSearch.Found)
                        {
                            newObj.ObjectTypes.Add(objTypeSearch.ObjectTypes[0]);
                        }
                        else
                        {
                            return StaticMessages.ObjectTypeNotFound(libName, objTypeName);
                        }
                    }
                    SaveDb();
                    return StaticMessages.ObjectCreated(newObj.Id, newObj.Name);
                }
                else
                {
                    return StaticMessages.LibraryNotFound(libName);
                }
            }
        }

        private static CptObject? RetrieveObjectFromLibrary(CptLibrary library, string objName)
            => library.Objects.Where(o => o.Name == objName).FirstOrDefault();
        private DbSearchResult SearchObject(CptLibrary lib, string objectName)
        {
            CptObject? result = RetrieveObjectFromLibrary(lib, objectName);
            if(result != null)
            {
                return new(true, ResultId.Success, new(), new() { result });
            }
            else
            {
                return DbSearchResult.ObjNotInLib;
            }
        }

        //OBJECT-TYPE CRUD
        internal ConceptDbResponse MoveObjectType(string libName, string objType, string newParent)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult objTypeSearch = SearchObjType(lib, objType);
                if (objTypeSearch.Found)
                {
                    DbSearchResult newParentSearch = SearchObjType(lib, newParent);
                    if (newParentSearch.Found)
                    {
                        CptObjectType parent = newParentSearch.ObjectTypes[0];
                        CptObjectType child = objTypeSearch.ObjectTypes[0];
                        child.ParentType = parent;
                        SaveDb();
                        return StaticMessages.ObjectTypeMoved(libName, objType, newParent);
                    }
                    else
                    {
                        return StaticMessages.ObjectTypeNotFound(libName, newParent);
                    }
                }
                else
                {
                    return StaticMessages.ObjectTypeNotFound(libName, objType);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse DeleteObjectType(string libName, string objTypeName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult objTypeSearch = SearchObjType(lib, objTypeName);
                if (objTypeSearch.Found)
                {
                    CptObjectType objType = objTypeSearch.ObjectTypes[0];
                    lib.ObjectTypes.Remove(objType);
                    SaveDb();
                    return StaticMessages.ObjectTypeDeleted(libName, objTypeName);
                }
                else
                {
                    return StaticMessages.ObjectTypeNotFound(libName, objTypeName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse RenameObjectType(string libName, string oldName, string newName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult existingType = SearchObjType(lib, newName);
                if (!existingType.Found)
                {
                    DbSearchResult editedTypeSearch = SearchObjType(lib, oldName);
                    if (editedTypeSearch.Found)
                    {
                        CptObjectType objType = editedTypeSearch.ObjectTypes[0];
                        objType.Name = newName;
                        SaveDb();
                        return StaticMessages.ObjectTypeNameChanged(libName, oldName, newName);

                    }
                    else
                    {
                        return StaticMessages.ObjectTypeNotFound(libName, oldName);
                    }
                }
                else
                {
                    return StaticMessages.ObjectTypeNameUnavailable(libName, newName);
                }

            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse NewObjectType(string libName, string parentType, string newType)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult existingType = SearchObjType(lib, newType);
                if (!existingType.Found)
                {
                    parentType = parentType == String.Empty ? "root" : parentType;
                    DbSearchResult parentObjTypeSearch = SearchObjType(lib, parentType);
                    if (parentObjTypeSearch.Found)
                    {
                        CptObjectType objType = new(newType);
                        lib.ObjectTypes.Add(objType);
                        parentObjTypeSearch.ObjectTypes[0].Children.Add(objType);
                        SaveDb();
                        return StaticMessages.ObjectTypeAddedToLibrary(libName, parentType, newType);
                    }
                    else
                    {
                        return StaticMessages.ObjectTypeNotFound(libName, parentType);
                    }
                }
                else
                {
                    return StaticMessages.ObjectTypeNameUnavailable(libName, newType);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }

        //LIBRARY CRUD

        internal ConceptDbResponse DeleteLibrary(string libName)
        {
            DbSearchResult search = SearchLibrary(libName);
            if (search.Found)
            {
                CptLibrary result = search.Libraries[0];
                _user.Account.Libraries.Remove(result);
                SaveDb();
                return StaticMessages.LibraryDeleted(result);
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse RenameLibrary(string oldName, string newName)
        {
            DbSearchResult search = SearchLibrary(oldName);
            if (search.Found)
            {
                DbSearchResult newSearch = SearchLibrary(newName);
                if (newSearch.Found)
                {
                    return StaticMessages.LibraryNameUnavailable(newName);
                }
                else
                {
                    CptLibrary lib = search.Libraries[0];
                    lib.Name = newName;
                    SaveDb();
                    return StaticMessages.LibraryNameChanged(oldName, newName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(oldName);
            }
        }
        internal ConceptDbResponse RetrieveScopedLibrary(string name)
        {
            DbSearchResult search = SearchLibrary(name);
            if (search.Found)
            {
                LibraryScope scoped = new(search.Libraries[0]);
                ConceptDbResponse response = StaticMessages.LibraryScopeRetrieved(name, scoped);
                return response;
            }
            else
            {
                return StaticMessages.LibraryNotFound(name);
            }
        }
        internal ConceptDbResponse ViewLibraryIndex()
        {
            List<CptLibrary> libraryList = _user.Account.Libraries.ToList();
            ConceptDbResponse response = StaticMessages.LibraryIndexRequested;
            foreach(CptLibrary library in libraryList)
            {
                response.Values.Add(library.Name);
            }
            return response;
        }
        internal ConceptDbResponse CreateLibrary(string name)
        {
            DbSearchResult search0 = SearchLibrary(name);
            if (search0.Found)
            {
                return StaticMessages.LibraryNameUnavailable(name);
            }
            else
            {
                CptLibrary newLibrary = new();
                newLibrary.Name = name;
                _user.Account.Libraries.Add(newLibrary);
                SaveDb();
                return StaticMessages.LibraryCreated(newLibrary.Id, name);
            }
        }
        private CptLibrary? RetrieveLibrary(string name) => _user.Account.Libraries.Where(e => e.Name == name).FirstOrDefault();
        private DbSearchResult SearchLibrary(string name)
        {
            CptLibrary? result = RetrieveLibrary(name);
            return result == null 
                ? DbSearchResult.LibNotFound 
                : new(true, ResultId.Success, new() { result }, new());
        }
        private static DbSearchResult SearchObjType(CptLibrary lib, string name)
        {
            foreach(CptObjectType objType in lib.ObjectTypes)
            {
                if(objType.Name == name)
                {
                    return new(true, ResultId.Success, new() { objType });
                }
            }
            return DbSearchResult.ObjTypeNotFound;
        }
    }
}
