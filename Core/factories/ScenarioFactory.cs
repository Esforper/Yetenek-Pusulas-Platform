// Core/Factories/ScenarioFactory.cs
using System;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Data; // ApplicationUser için (teacherId tipi için)

namespace YetenekPusulasi.Core.Factories
{
    public class ScenarioFactory
    {
        // Bu factory şimdilik basit olacak. Eğer DbContext'e veya başka servislere ihtiyacı olursa
        // constructor'a enjekte edilebilir. Örneğin, senaryo türüne göre varsayılan bir
        // açıklama şablonu veritabanından çekilecekse.

        public ScenarioFactory()
        {
            // Şimdilik constructor boş.
        }

        public Scenario Create(string title, string description, ScenarioType type, string teacherId, int classroomId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Senaryo başlığı boş olamaz.", nameof(title));
            if (string.IsNullOrWhiteSpace(teacherId))
                throw new ArgumentException("Öğretmen ID'si boş olamaz.", nameof(teacherId));
            if (classroomId <= 0)
                throw new ArgumentException("Geçerli bir Sınıf ID'si belirtilmelidir.", nameof(classroomId));

            var scenario = new Scenario
            {
                Title = title,
                Description = description,
                Type = type, // Gelen tip doğrudan atanıyor
                TeacherId = teacherId,
                ClassroomId = classroomId
                // CreatedDate zaten Scenario constructor'ında set ediliyor.
            };

            // İPUCU: Senaryo türüne göre burada ek mantıklar eklenebilir.
            // Örneğin, type ProblemSolving ise Description'a bir ön ek eklenebilir:
            // if (type == ScenarioType.ProblemSolving && !description.StartsWith("[Problem]:"))
            // {
            //     scenario.Description = "[Problem]: " + description;
            // }

            return scenario;
        }
    }
}