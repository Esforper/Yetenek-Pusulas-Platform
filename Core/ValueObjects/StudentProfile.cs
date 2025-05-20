namespace YetenekPusulasi.Core.Entities
{
    public class StudentProfile
    {
        public int StudentId { get; }
        public string TargetSkillPreference { get; } // Öğrencinin tercih ettiği yetenek
        public int PreferredDifficulty { get; }     // Öğrencinin tercih ettiği zorluk

        public StudentProfile(int studentId, string targetSkillPreference, int preferredDifficulty)
        {
            StudentId = studentId;
            TargetSkillPreference = targetSkillPreference;
            PreferredDifficulty = preferredDifficulty;
        }
    }
}