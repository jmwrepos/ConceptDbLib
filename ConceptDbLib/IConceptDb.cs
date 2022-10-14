namespace ConceptDbLib
{
    public interface IConceptDb
    {
        ConceptDbResponse AddObjectToLibrary(string builderId, string objName, string libName);
        Task<ConceptDbResponse> AddObjectToLibraryAsync(string builderId, string objName, string libName);
        ConceptDbResponse AddObjectToNetwork(string builderId, string objName, string networkName);
        Task<ConceptDbResponse> AddObjectToNetworkAsync(string builderId, string objName, string networkName);
        ConceptDbResponse CheckDbConnection(string key);
        Task<ConceptDbResponse> CheckDbConnectionAsync(string key);
        ConceptDbResponse CreateNetwork(string builderId, string name);
        Task<ConceptDbResponse> CreateNetworkAsync(string builderId, string name);
        ConceptDbResponse CreateNetworkMembership(string builderId, string networkName, string parentObjName, string childObjName);
        Task<ConceptDbResponse> CreateNetworkMembershipAsync(string builderId, string networkName, string parentObjName, string childObjName);
        ConceptDbResponse CreateObject(string builderId, string name);
        Task<ConceptDbResponse> CreateObjectAsync(string builderId, string name);
        ConceptDbResponse CreateObjectLibrary(string builderId, string name);
        Task<ConceptDbResponse> CreateObjectLibraryAsync(string builderId, string name);
        ConceptDbResponse DbInitialize(string key);
        Task<ConceptDbResponse> DbInitializeAsync(string key);
        ConceptDbResponse NewBuilder(string secKey);
        Task<ConceptDbResponse> NewBuilderAsync(string secKey);
        ConceptDbResponse RequestDbInitialize(string key);
        Task<ConceptDbResponse> RequestDbInitializeAsync(string key);
    }
}