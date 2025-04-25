namespace ResumeBuilder.Models.LinkedInModels
{
    public class LinkedInProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfileImageUrl { get; set; }
        public List<LinkedInEducation> Education { get; set; } = new List<LinkedInEducation>();
        public List<LinkedInExperience> Experience { get; set; } = new List<LinkedInExperience>();
        public List<LinkedInSkill> Skills { get; set; } = new List<LinkedInSkill>();
    }
}
