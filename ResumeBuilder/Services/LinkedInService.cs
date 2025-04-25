using Microsoft.AspNetCore.Authentication;
using ResumeBuilder.Models.LinkedInModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ResumeBuilder.Services
{
    public class LinkedInService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkedInService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LinkedInProfile> GetUserProfile()
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new InvalidOperationException("Access token not found");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Get basic profile information
            var basicProfileResponse = await _httpClient.GetAsync("https://api.linkedin.com/v2/me");
            basicProfileResponse.EnsureSuccessStatusCode();
            var basicProfile = await JsonDocument.ParseAsync(await basicProfileResponse.Content.ReadAsStreamAsync());

            // Get email address
            var emailResponse = await _httpClient.GetAsync("https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))");
            emailResponse.EnsureSuccessStatusCode();
            var emailData = await JsonDocument.ParseAsync(await emailResponse.Content.ReadAsStreamAsync());

            // Get education data
            var educationResponse = await _httpClient.GetAsync("https://api.linkedin.com/v2/educations?q=members&projection=(elements*(schoolName,degreeName,fieldOfStudy,startDate,endDate,activities,description))");
            educationResponse.EnsureSuccessStatusCode();
            var educationData = await JsonDocument.ParseAsync(await educationResponse.Content.ReadAsStreamAsync());

            // Get experience data
            var experienceResponse = await _httpClient.GetAsync("https://api.linkedin.com/v2/positions?q=members&projection=(elements*(company,title,location,startDate,endDate,description))");
            experienceResponse.EnsureSuccessStatusCode();
            var experienceData = await JsonDocument.ParseAsync(await experienceResponse.Content.ReadAsStreamAsync());

            // Get skills data
            var skillsResponse = await _httpClient.GetAsync("https://api.linkedin.com/v2/skills?q=members&projection=(elements*(name,endorsements))");
            skillsResponse.EnsureSuccessStatusCode();
            var skillsData = await JsonDocument.ParseAsync(await skillsResponse.Content.ReadAsStreamAsync());

            return MapToLinkedInProfile(basicProfile, emailData, educationData, experienceData, skillsData);
        }

        private LinkedInProfile MapToLinkedInProfile(JsonDocument basicProfile, JsonDocument emailData, JsonDocument educationData, JsonDocument experienceData, JsonDocument skillsData)
        {
            var profile = new LinkedInProfile
            {
                Id = basicProfile.RootElement.GetProperty("id").GetString(),
                FirstName = basicProfile.RootElement.GetProperty("localizedFirstName").GetString(),
                LastName = basicProfile.RootElement.GetProperty("localizedLastName").GetString(),
                Email = emailData.RootElement.GetProperty("elements")[0].GetProperty("handle~").GetProperty("emailAddress").GetString()
            };

            // Parse education data
            if (educationData.RootElement.TryGetProperty("elements", out var educationElements))
            {
                foreach (var edu in educationElements.EnumerateArray())
                {
                    profile.Education.Add(new LinkedInEducation
                    {
                        SchoolName = edu.GetProperty("schoolName").GetString(),
                        Degree = edu.TryGetProperty("degreeName", out var degree) ? degree.GetString() : string.Empty,
                        FieldOfStudy = edu.TryGetProperty("fieldOfStudy", out var field) ? field.GetString() : string.Empty,
                        StartDate = ParseLinkedInDate(edu.GetProperty("startDate")),
                        EndDate = edu.TryGetProperty("endDate", out var endDate) ? ParseLinkedInDate(endDate) : null,
                        Activities = edu.TryGetProperty("activities", out var activities) ? activities.GetString() : string.Empty,
                        Description = edu.TryGetProperty("description", out var description) ? description.GetString() : string.Empty
                    });
                }
            }

            // Parse experience data
            if (experienceData.RootElement.TryGetProperty("elements", out var experienceElements))
            {
                foreach (var exp in experienceElements.EnumerateArray())
                {
                    profile.Experience.Add(new LinkedInExperience
                    {
                        CompanyName = exp.GetProperty("company").GetProperty("name").GetString(),
                        Title = exp.GetProperty("title").GetString(),
                        Location = exp.TryGetProperty("location", out var location) ? location.GetString() : string.Empty,
                        StartDate = ParseLinkedInDate(exp.GetProperty("startDate")),
                        EndDate = exp.TryGetProperty("endDate", out var endDate) ? ParseLinkedInDate(endDate) : null,
                        Description = exp.TryGetProperty("description", out var description) ? description.GetString() : string.Empty
                    });
                }
            }

            // Parse skills data
            if (skillsData.RootElement.TryGetProperty("elements", out var skillElements))
            {
                foreach (var skill in skillElements.EnumerateArray())
                {
                    profile.Skills.Add(new LinkedInSkill
                    {
                        Name = skill.GetProperty("name").GetString(),
                        Endorsements = skill.TryGetProperty("endorsements", out var endorsements) ? endorsements.GetInt32() : 0
                    });
                }
            }

            return profile;
        }

        private DateTime ParseLinkedInDate(JsonElement dateElement)
        {
            int year = dateElement.GetProperty("year").GetInt32();
            int month = dateElement.TryGetProperty("month", out var monthElement) ? monthElement.GetInt32() : 1;
            int day = dateElement.TryGetProperty("day", out var dayElement) ? dayElement.GetInt32() : 1;

            return new DateTime(year, month, day);
        }
    }
}
