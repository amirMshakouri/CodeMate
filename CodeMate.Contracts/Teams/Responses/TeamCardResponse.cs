namespace CodeMate.Contracts.Teams.Responses;

public sealed class TeamCardResponse
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}