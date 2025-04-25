using ResumeBuilder.Models.LinkedInModels;
using ResumeBuilder.Models;

namespace ResumeBuilder.Services
{
    public class ResumeService
    {
        private static Dictionary<string, Resume> _resumes = new Dictionary<string, Resume>();

        public Task<Resume> GetResumeForUser(string userId)
        {
            if (_resumes.TryGetValue(userId, out var resume))
            {
                return Task.FromResult(resume);
            }

            return Task.FromResult(new Resume { UserId = userId });
        }

        public Task SaveResume(Resume resume)
        {
            _resumes[resume.UserId] = resume;
            return Task.CompletedTask;
        }

        public Resume MapFromLinkedInProfile(LinkedInProfile profile)
        {
            var resume = new Resume
            {
                UserId = profile.Id,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.Email,
                Education = new List<Education>(),
                Experience = new List<Experience>(),
                Skills = new List<Skill>()
            };

            // Map education
            foreach (var edu in profile.Education)
            {
                resume.Education.Add(new Education
                {
                    Institution = edu.SchoolName,
                    Degree = edu.Degree,
                    FieldOfStudy = edu.FieldOfStudy,
                    StartDate = edu.StartDate,
                    EndDate = edu.EndDate,
                    IsCurrentlyStudying = !edu.EndDate.HasValue,
                    Description = edu.Description
                });
            }

            // Map experience
            foreach (var exp in profile.Experience)
            {
                resume.Experience.Add(new Experience
                {
                    Company = exp.CompanyName,
                    Title = exp.Title,
                    Location = exp.Location,
                    StartDate = exp.StartDate,
                    EndDate = exp.EndDate,
                    IsCurrentPosition = !exp.EndDate.HasValue,
                    Description = exp.Description
                });
            }

            // Map skills
            foreach (var skill in profile.Skills)
            {
                resume.Skills.Add(new Skill
                {
                    Name = skill.Name,
                    Proficiency = Math.Min(5, Math.Max(1, skill.Endorsements / 10 + 1)) // Scale endorsements to 1-5
                });
            }

            return resume;
        }
    }
}
