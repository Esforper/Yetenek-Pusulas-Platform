namespace YetenekPusulasi.Core.ValueObjects
{
    public class StudentProfile // Senaryo kişiselleştirme için basit veri taşıyıcı
    {
        public string TargetSkillPreference { get; }
        public int PreferredDifficulty { get; }
        // Öğrenciye dair diğer bilgiler eklenebilir (yaş, seviye vb.)

        public StudentProfile(string targetSkillPreference, int preferredDifficulty)
        {
            TargetSkillPreference = targetSkillPreference;
            PreferredDifficulty = preferredDifficulty;
        }
    }
}