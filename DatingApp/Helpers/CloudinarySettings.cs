using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Helpers
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

    struct RolesEnum
    {
        public const string Member = "Member";
        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
    }

    struct PolicyEnum
    {
        public const string AdminRole = "RequireAdminRole";
        public const string ModeratorRole = "ModeratePhotoRole";
    }

    //public enum Tester
    //{
    //    [Description("Member")]
    //    Member,
    //    [Description("Admin")]
    //    Admin,
    //    [Description("Moderator")]
    //    Moderator
    //}

    //public static class EnumExtension
    //{
    //    public static string GetValue(this Tester value)
    //    {
    //        DescriptionAttribute[] attribute = (DescriptionAttribute[])value.GetType()
    //                                                .GetField(value.ToString())
    //                                                .GetCustomAttributes(typeof(DescriptionAttribute), false);
    //        return attribute.Length > 0 ? attribute[0].Description : string.Empty;
    //    }

    //    //how to use
    //    public static string useCase()
    //    {
    //        return Tester.Moderator.GetValue();
    //    }
    //}
}
