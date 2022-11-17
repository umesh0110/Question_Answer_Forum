using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using UserService.Database;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public UserController(MySqlDatabase mySqlDatabase)
        {
            this.MySqlDatabase = mySqlDatabase;
        }


        [HttpPost]
        [Route("add")]
        public IActionResult AddUser(User user)
        {
            try
            {
                var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"INSERT INTO UserTable(Name) VALUES (@Name);";
                cmd.Parameters.AddWithValue("@Name", user.Name);
                var recs = cmd.ExecuteNonQuery();
            }
            catch
            {
                return BadRequest();
            }
            return Ok("User added successfully");

        }


        [HttpGet]
        [Route("points/add/{userId}")]
        public IActionResult AddPointsToUser(int userId)
        {
           
            try
            {
                var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"Update UserTable Set Points = Points + 1 Where UserId = @userId;";
                cmd.Parameters.AddWithValue("@userId", userId.ToString());
                var recs = cmd.ExecuteNonQuery();
            }
            catch
            {
                return BadRequest();
            }

            return Ok("Added 50 points to user - " + userId);
        }

        [HttpGet]
        [Route("points/{userId}")]
        public IActionResult GetUserPoints(int userId)
        {
            int points;
            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand("SELECT Points FROM UserTable Where UserId = @userId", this.MySqlDatabase.Connection);
                mySqlCommand.Parameters.AddWithValue("@questionId", userId);
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                mySqlDataReader.Read();
                points = mySqlDataReader.GetInt32(0);
            }
            catch
            {
                return BadRequest();
            }

            return Ok("User - " +  userId + ", Points - " + points);
        }


    }
}
