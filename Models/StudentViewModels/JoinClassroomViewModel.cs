using System.ComponentModel.DataAnnotations;
namespace YetenekPusulasi.Models.StudentViewModels
{
    public class JoinClassroomViewModel
    {
        [Required(ErrorMessage = "Katılım kodu zorunludur.")]
        [Display(Name = "Sınıf Katılım Kodu")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Katılım kodu geçerli bir formatta değil.")] // Uzunluğu ClassroomService'teki ile tutarlı olmalı
        public string ParticipationCode { get; set; }
    }
}