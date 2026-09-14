namespace CodeMate.Application.Common.Interfaces.Services;

public interface IProjectMembershipChecker
{
    Task<bool> IsProjectMemberAsync(
        Guid projectId,
        Guid userId);
}