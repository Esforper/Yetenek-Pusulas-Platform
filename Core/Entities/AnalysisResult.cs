// Core/Entities/AnalysisResult.cs
using System;
using System.Collections.Generic;
using YetenekPusulasi.Data; // ApplicationUser için (analizi yapan veya yapılan)
// StudentAnswer ve Scenario için using'ler eklenecek

namespace YetenekPusulasi.Core.Entities
{
    public class AnalysisResult
    {
        public int Id { get; set; }

        public int StudentAnswerId { get; set; } // Hangi cevabın analizi
        // public virtual StudentAnswer StudentAnswer { get; set; } // Navigasyon

        public string AiModelUsed { get; set; } // Hangi AI modeli kullanıldı (örn: IAIModelAdapter.ModelIdentifier)
        public string? Summary { get; set; } // Analizin özeti
        public double? OverallScore { get; set; } // Genel bir skor (0-1 arası veya 1-100)
        public List<string>? DetectedSkills { get; set; } // Tespit edilen yetenekler
        public string? RawAiResponse { get; set; } // AI'dan gelen ham cevap (debug/log için)
        public DateTime AnalysisDate { get; set; }
        public string? ErrorMessage { get; set; } // Analiz sırasında bir hata oluştuysa
        public virtual StudentAnswer StudentAnswer { get; set; }

        public AnalysisResult()
        {
            AnalysisDate = DateTime.UtcNow;
            DetectedSkills = new List<string>();
        }
    }
}