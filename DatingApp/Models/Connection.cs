namespace DatingApp.Models
{
    public class Connection
    {
        public Connection() { } //default constructor. If not here, entity framework core will throw an error
        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}