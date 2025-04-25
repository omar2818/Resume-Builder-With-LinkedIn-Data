namespace ResumeBuilder.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentPosition { get; set; }
        public string Description { get; set; }
    }
}
