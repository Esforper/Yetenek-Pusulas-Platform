// Core/Entities/Scenarios/GenericScenario.cs (YENİ DOSYA)
namespace YetenekPusulasi.Core.Entities.Scenarios
{
    public class GenericScenario : Scenario // Scenario'dan türüyor
    {
        // GenericScenario'ya özgü ek property'ler olabilir veya olmayabilir.

        // Constructor, base class'ın constructor'ını çağırır.
        // Gelen 'type' parametresi, bu jenerik senaryonun hangi spesifik olmayan
        // tür için oluşturulduğunu saklamak için kullanılabilir.
        public GenericScenario(ScenarioType type) : base(type)
        {
            // Eğer type Undefined değilse ve biz bu GenericScenario'yu bir fallback
            // olarak kullanıyorsak, loglama yapılabilir veya type'ı Undefined'a set edebiliriz.
            // Şimdilik, gelen type'ı olduğu gibi kabul edelim.
            // Eğer Scenario abstract sınıfındaki Type property'sinin set'i protected ise
            // ve constructor'da atanıyorsa, burada tekrar Type = type; yapmaya gerek yok.
        }

        // Abstract GetSystemPrompt metodunu implemente ediyoruz.
        public override string GetSystemPrompt()
        {
            string specificInstruction = $"Bu genel bir senaryodur. Öğrencinin verdiği cevaptaki temel anlama, ifade yeteneği ve mantıksal akışını değerlendir. Cevabın senaryo konusuyla ne kadar ilgili olduğuna odaklan.";

            // İsteğe bağlı olarak, this.Type (yani constructor'dan gelen type) enum değerini
            // string'e çevirip prompt'a ekleyebiliriz.
            if (this.Type != ScenarioType.Undefined)
            {
                specificInstruction += $" Bu senaryo '{this.Type.ToString()}' türü için genel bir değerlendirme çerçevesindedir.";
            }

            return FormatBasePrompt(specificInstruction); // Base class'taki helper metodu kullanıyoruz
        }

        // GenericScenario'ya özel başka metotlar veya property'ler eklenebilir.
        // Örneğin, bu senaryonun gerçekten "jenerik" olduğunu belirten bir flag:
        public bool IsTrulyGeneric => this.Type == ScenarioType.Undefined;
    }
}