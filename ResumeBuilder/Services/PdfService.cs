using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using ResumeBuilder.Models;
using System.Reflection.Metadata;

namespace ResumeBuilder.Services
{
    public class PdfService
    {
        public async Task<byte[]> GenerateResumePdf(Resume resume)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new iText.Layout.Document(pdf);
                        // Header
                        document.Add(new Paragraph(resume.FirstName + " " + resume.LastName)
                            .SetFontSize(20));

                        document.Add(new Paragraph(resume.Email + " | " + resume.Phone + " | " + resume.Address)
                            .SetFontSize(12));

                        // Summary
                        if (!string.IsNullOrEmpty(resume.Summary))
                        {
                            document.Add(new Paragraph("Summary")
                                .SetFontSize(16));
                            document.Add(new Paragraph(resume.Summary)
                                .SetFontSize(12));
                        }

                        // Experience
                        if (resume.Experience.Count > 0)
                        {
                            document.Add(new Paragraph("Experience")
                                .SetFontSize(16));

                            foreach (var exp in resume.Experience)
                            {
                                document.Add(new Paragraph(exp.Title + " at " + exp.Company)
                                    .SetFontSize(14));

                                string dateRange = exp.StartDate.ToString("MMM yyyy") + " - " +
                                    (exp.IsCurrentPosition ? "Present" : exp.EndDate?.ToString("MMM yyyy"));
                                document.Add(new Paragraph(dateRange + " | " + exp.Location)
                                    .SetFontSize(12));

                                document.Add(new Paragraph(exp.Description)
                                    .SetFontSize(12));
                            }
                        }

                        // Education
                        if (resume.Education.Count > 0)
                        {
                            document.Add(new Paragraph("Education")
                                .SetFontSize(16));

                            foreach (var edu in resume.Education)
                            {
                                document.Add(new Paragraph(edu.Degree + " in " + edu.FieldOfStudy)
                                    .SetFontSize(14));

                                document.Add(new Paragraph(edu.Institution)
                                    .SetFontSize(12));

                                string dateRange = edu.StartDate.ToString("MMM yyyy") + " - " +
                                    (edu.IsCurrentlyStudying ? "Present" : edu.EndDate?.ToString("MMM yyyy"));
                                document.Add(new Paragraph(dateRange)
                                    .SetFontSize(12));

                                if (!string.IsNullOrEmpty(edu.Description))
                                {
                                    document.Add(new Paragraph(edu.Description)
                                        .SetFontSize(12));
                                }
                            }
                        }

                        // Skills
                        if (resume.Skills.Count > 0)
                        {
                            document.Add(new Paragraph("Skills")
                                .SetFontSize(16));

                            foreach (var skill in resume.Skills)
                            {
                                document.Add(new Paragraph(skill.Name + " (" + skill.Proficiency + "/5)")
                                    .SetFontSize(12));
                            }
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}
