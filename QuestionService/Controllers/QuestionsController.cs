using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using QuestionsService.Database;
using QuestionsService.Models;
using System.Data;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        private IConfiguration _configuration { get; set; }

        public QuestionsController(MySqlDatabase mySqlDatabase, IConfiguration configuration)
        {
            this.MySqlDatabase = mySqlDatabase;
            this._configuration = configuration;
        }


        [HttpPost]
        [Route("add")]
        public IActionResult AddUser(Question question)
        {

            try
            {
                var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"INSERT INTO QuestionsTable(UserId,QuestionText,QuestionTags) VALUES (@UserId,@QuestionText,@QuestionTags);";
                cmd.Parameters.AddWithValue("@UserId", question.UserId);
                cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                cmd.Parameters.AddWithValue("@QuestionTags", question.QuestionTags);
                cmd.ExecuteNonQuery();

            }
            catch
            {
                return BadRequest();
            }

            return Ok("Question added Successfully.");

        }


        [HttpGet]
        [Route("upvote/{questionId}")]
        public IActionResult UpvoteQuestion(int questionId)
        {

            try
            {
                var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"Update QuestionsTable Set Upvotes = Upvotes + 1 Where QuestionId = @questionId;";
                cmd.Parameters.AddWithValue("@questionId", questionId);

                cmd.ExecuteNonQuery();

                MySqlCommand mySqlCommand = new MySqlCommand("SELECT UserId FROM QuestionsTable Where QuestionId = @questionId", this.MySqlDatabase.Connection);
                mySqlCommand.Parameters.AddWithValue("@questionId", questionId);
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                mySqlDataReader.Read();
                int userIdPointIncrease = mySqlDataReader.GetInt32(0);
                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

                    using (var client = new HttpClient(httpClientHandler))
                    {

                        var result = client.PostAsync(_configuration.GetValue<string>("UserServiceUrl") + userIdPointIncrease, null).Result;
                    }
                }
            }
            catch
            {
                return BadRequest();
            }
            return Ok("Upvoted Successfully. QuestionId - " + questionId);
            
        }

    }
}



