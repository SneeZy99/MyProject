using System.ComponentModel.DataAnnotations;

namespace kkkk11.ViewModels  // ✅ แก้จาก HotelBooking.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "กรุณากรอกชื่อ-นามสกุล")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "กรุณากรอก Email")]
        [EmailAddress(ErrorMessage = "รูปแบบ Email ไม่ถูกต้อง")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "กรุณากรอกเบอร์โทร")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [MinLength(6, ErrorMessage = "รหัสผ่านต้องมีอย่างน้อย 6 ตัวอักษร")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "กรุณายืนยันรหัสผ่าน")]
        [Compare("Password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}