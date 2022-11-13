namespace DatingApp.Models
{
    public class UserLike
    {
        public User LoggedInUser { get; set; }

        public int LoggedInUserId { get; set; }

        public User LikedUser { get; set; }

        public int LikedUserId { get; set; }

        //one loggedinuser can like so many users
        //so many users can like the loggedInUser
    }
}
