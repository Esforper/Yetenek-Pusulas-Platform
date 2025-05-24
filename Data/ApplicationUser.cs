using Microsoft.AspNetCore.Identity;

namespace YetenekPusulasi.Data // Veya YetenekPusulasi.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        // Öğrenci veya Öğretmene özel ek alanlar buraya eklenebilir.
        // Örneğin:
        // public string FullName { get; set; }
        // public DateTime DateOfBirth { get; set; } // Sadece öğrenci için

        // Rolü doğrudan burada tutmak yerine Identity'nin Rol mekanizmasını kullanacağız.
    }
}