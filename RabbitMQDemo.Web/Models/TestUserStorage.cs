using System.Collections.Generic;
namespace RabbitMQDemo.Web.Models
{
    public static class TestUserStorage
    {
        public static List<User> UserList { get; set; } = new List<User>() {
            new User { UserName = "RMQ1",Password = "112233"},
            new User { UserName = "RMQ2",Password = "112233"},
            new User { UserName = "RMQ3",Password = "112233"},
            new User { UserName = "RMQ4",Password = "112233"}
        };
    }

}
