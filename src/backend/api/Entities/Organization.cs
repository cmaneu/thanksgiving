namespace api.Entities
{
    public class Organization
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public OrganizationVisibility Visibility { get; set; }
        public IEnumerable<Activity>? Activities { get; set; }
    }
    

    public enum OrganizationVisibility
    {
        Private = 1,
        ForMembers = 2,
        Public = 3
    }
}
