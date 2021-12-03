using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CallingCRM.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        //[Required]
        //[Display(Name = "Email")]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }

    public class RegisterViewModel
    {
        //[Required]
        [Display(Name = "Manager's Email *")]
        public string SponsorId { get; set; }

        //[Required]
        [Display(Name = "Team Leader's Email *")]
        public string ParentId { get; set; }

        //[Required]
        //[StringLength(8, ErrorMessage = "The username must be at least 8 characters long.", MinimumLength = 8)]
        //[Display(Name = "Telecaller Agent (3 to 20 characters 'A-Z0-9' without space) *")]
        //public string Username { get; set; }

        //[Required] 
        [EmailAddress]
        [Display(Name = "Agent's Email *")]
        public string Email { get; set; }

        //[Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "CRM Password *")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm CRM password *")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Agent's Name")]
        public string FullName { get; set; }

        [StringLength(10, ErrorMessage = "Invalid mobile number.", MinimumLength = 10)]
        [Display(Name = "Mobile Number")]
        public string Mobile { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Date Of Birth")]
        public string DOB { get; set; }

        public string SponsorDisplay { get; set; }
        public string ParentDisplay { get; set; }
        public string PositionDisplay { get; set; }
        public string CurrentEmail { get; set; }
        public int Step { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
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

        public string Code { get; set; }
    }

    public class UpdateUserInfoViewModel
    { 
        public UpdatePasswordViewModel passwordView { get; set;  }
        public UpdateUsernameViewModel usernameView { get; set; }
        public UpdateProfileViewModel profileView { get; set; }
    }

    public class UpdatePasswordViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class UpdateUsernameViewModel
    {
        [Required]
        [Display(Name = "Old Username")]
        public string OldUsername { get; set; }

        [Required]
        [Display(Name = "New Username")]
        public string NewUsername { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Username")]
        [Compare("NewUsername", ErrorMessage = "The new username and confirmation new username do not match.")]
        public string ConfirmNewUsername { get; set; }
    }

    public class UpdateProfileViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Mobile")]
        public string Mobile { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class UserUpdateViewModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [StringLength(10, ErrorMessage = "Invalid mobile number.", MinimumLength = 10)]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Display(Name = "ZIP Code")]
        public string ZipCode { get; set; }
    }

    public enum LegPosition
    {
        L,
        R
    }

    public enum SiteVersion
    {
        Version_1,
        Version_2
    }

    public class VerifyFamilyModel
    { 
        public int chekcedLevels { get; set; }
        public int FoundAtLevel { get; set; }
        public string MatchStatus { get; set; }

    }
}
