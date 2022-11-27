using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Models
{
    public class Group
    {
        public Group() { }

        public Group(string groupName)
        {
            Name = groupName;
        }

        [Key]
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}
