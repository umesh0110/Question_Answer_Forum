using System.Text.Json.Serialization;

namespace QuestionsService.Models
{
    public class Question
    {
        [JsonIgnore]
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionTags { get; set; }

        public int Upvotes { get; set; }
    }

}
