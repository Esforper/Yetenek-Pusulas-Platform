// Models/StudentViewModels/SubmitAnswerViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace YetenekPusulasi.WebApp.Models.StudentViewModels // Namespace'inizi kontrol edin
{
    public class SubmitAnswerViewModel
    {
        [Required]
        public int ScenarioId { get; set; }

        [Required(ErrorMessage = "Cevap alanı boş bırakılamaz.")]
        [MinLength(10, ErrorMessage = "Cevabınız en az 10 karakter olmalıdır.")]
        [DataType(DataType.MultilineText)]
        public string AnswerText { get; set; }
    }
}