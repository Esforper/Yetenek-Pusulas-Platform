using System.ComponentModel.DataAnnotations;
namespace YetenekPusulasi.Models.ClassroomViewModels
{
    public class CreateClassroomViewModel
    {
        [Required(ErrorMessage = "Sınıf adı zorunludur.")]
        [Display(Name = "Sınıf Adı")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Sınıf adı 3-100 karakter arasında olmalıdır.")]
        public string Name { get; set; }

        [Display(Name = "Açıklama (Opsiyonel)")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }
    }
}