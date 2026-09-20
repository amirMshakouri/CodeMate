namespace CodeMate.Contracts.Skills.Requests
{
    public sealed class AddUserSkillRequest
    {
        public required Guid SkillId { get; set; }

        public int Level { get; set; }

        public int? YearsOfExperience { get; set; }
    }
}