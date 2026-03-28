using System.ComponentModel.DataAnnotations;

namespace kkkk11.ViewModels
{
    public class AddUserViewModel
    {
        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public string Email { get; set; } = "";

        public string? Phone { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";
    }
}
