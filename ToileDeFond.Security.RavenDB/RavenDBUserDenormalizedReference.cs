namespace ToileDeFond.Security.RavenDB
{
    public class RavenDBUserDenormalizedReference
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static implicit operator RavenDBUserDenormalizedReference(User user)
        {
            return new RavenDBUserDenormalizedReference
            {
                Id = user.Id,
                Name = user.Name
            };
        }
    }
}
