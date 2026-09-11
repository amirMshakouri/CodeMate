namespace CodeMate.Contracts.Teams.Requests;

public class AddProjectSkillRequest
{
    public Guid ProjectId { get; set; }

    public Guid SkillId { get; set; }

    public int RequiredLevel { get; set; }

    public bool IsMandatory { get; set; }
}