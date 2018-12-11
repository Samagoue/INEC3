using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace INEC3.Models
{
    public class UserModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public UserProfile UserProfile { get; set; }
        //public HttpPostedFile UserImage { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {

        public int UserProfileId { get; set; }
        public Guid AspNetUsersId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Isactive { get; set; }
        public string Createdate { get; set; }
        public string Updatedate { get; set; }
    }

    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUser
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userid { get; set; }
        public string displayname { get; set; }
        public string profileimg { get; set; }
        public string role { get; set; }
        public string issued { get; set; }
        public string expires { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }

    public class UserDisplay
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public bool Isactive { get; set; }
        public bool EmailConfirmed { get; set; }

    }

    [Table("UserPolStation")]
    public class UserPolStation
    {
        [Key]
        public int ID_UserPolStation { get; set; }
        public string UserID { get; set; }
        public string Createdate { get; set; }
        public int AssignID { get; set; }//One Of Province/Territoire/Commune/BureauVote
        public string AssignRole { get; set; }

        //public string Province { get; set; }
        //public string Territoire { get; set; }
        //public string CommuneName { get; set; }
        //public int ID_Bureauvote { get; set; }
        //public string PolStationName { get; set; }
        //public int ID_Province { get; set; }
        //public int ID_Territoire { get; set; }
    }

    public class ForgotPasswordModel
    {
        public Guid UserId { get; set; }
        public string SecurityCode { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}