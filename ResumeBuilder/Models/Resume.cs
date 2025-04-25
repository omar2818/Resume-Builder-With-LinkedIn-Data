namespace ResumeBuilder.Models
{
    public class Resume
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Summary { get; set; }
        public List<Education> Education { get; set; } = new List<Education>();
        public List<Experience> Experience { get; set; } = new List<Experience>();
        public List<Skill> Skills { get; set; } = new List<Skill>();
    }
}
