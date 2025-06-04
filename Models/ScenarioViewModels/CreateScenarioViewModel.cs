// Models/ScenarioViewModels/CreateScenarioViewModel.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // SelectList için
using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem için
using YetenekPusulasi.Core.Entities; // ScenarioType enum'ı için

namespace YetenekPusulasi.WebApp.Models.ScenarioViewModels // Namespace'i projenize göre ayarlayın
{
    public class CreateScenarioViewModel
    {
        [Required(ErrorMessage = "Senaryo başlığı zorunludur.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Başlık 3-200 karakter arasında olmalıdır.")]
        [Display(Name = "Senaryo Başlığı")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Senaryo açıklaması zorunludur.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Senaryo Açıklaması")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Senaryo türü seçmek zorunludur.")]
        [Display(Name = "Senaryo Türü")]
        public ScenarioType Type { get; set; }

        // Bu ViewModel, belirli bir sınıf için senaryo oluşturma sayfasında kullanılacak,
        // bu yüzden ClassroomId'yi hidden field olarak veya route'dan alacağız.
        // Controller'da set edilecek.
        public int ClassroomId { get; set; }
        public string? ClassroomName { get; set; } // Sadece göstermek için

        // Senaryo türlerini dropdown'da göstermek için
        public IEnumerable<SelectListItem>? ScenarioTypes { get; set; }

        public CreateScenarioViewModel()
        {
            ScenarioTypes = Enum.GetValues(typeof(ScenarioType))
                                .Cast<ScenarioType>()
                                .Select(e => new SelectListItem
                                {
                                    Value = ((int)e).ToString(),
                                    Text = e.ToString() // Enum ismini gösterir, daha kullanıcı dostu isimler için bir helper metot yazılabilir.
                                }).ToList();
        }


        [Display(Name = "Özel Başlangıç Metni (Opsiyonel)")]
        [DataType(DataType.MultilineText)]
        public string? TeacherProvidedInitialPrompt { get; set; }

        [Display(Name = "Başlangıç Metnini Yapay Zeka Oluştursun Mu?")]
        public bool GenerateInitialPromptWithAI { get; set; }
    }
}