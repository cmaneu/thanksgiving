namespace api.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        public Organization? Organization { get; set; }
        public int OrganizationId { get; set; }
        public User? Volunteer { get; set; }
        public int VolunteerId { get; set; }
        public required string Name { get; set; }
        public required DateTime Date { get; set; }
        public int? Duration { get; set; }
        public required ActivityStatus Status { get; set; }
    }

    public enum ActivityStatus
    {
        Submitted = 1,
        Approved = 2,
        Rejected = 3
    }
}
