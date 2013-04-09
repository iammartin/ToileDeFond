namespace ToileDeFond.Security.RavenDB
{

    //public class AuthorizationUser
    //{
    //    public string Name { get; set; }
    //    public string Id { get; set; }
    //    public List<string> Roles { get; set; }
    //    public List<OperationPermission> Permissions { get; set; }
    //}

    public abstract class RavenDBUser : User/* : AuthorizationUser*/
    {
        //    public List<string> Roles { get; set; }
        //    public List<OperationPermission> Permissions { get; set; }
    }
}
