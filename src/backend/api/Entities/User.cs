namespace api.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<OrganizationMembership>? Memberships { get; set; }
    }

    public class OrganizationMembership
    {
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Organization Organization { get; set; }
        public OrganizationMembershipType Type { get; set; }
    }

    public enum OrganizationMembershipType
    {
        Member = 1,
        Admin = 2
    }
}
