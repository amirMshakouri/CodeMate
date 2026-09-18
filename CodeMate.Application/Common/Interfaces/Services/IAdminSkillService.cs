using System;
using System.Collections.Generic;
using System.Text;

using CodeMate.Contracts.Skills.Requests;
using CodeMate.Contracts.Skills.Responses;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface IAdminSkillService
{
    Task<SkillResponse> CreateSkillAsync(CreateSkillRequest request);

    Task<SkillResponse> UpdateSkillAsync(Guid skillId, UpdateSkillRequest request);

    Task DeleteSkillAsync(Guid skillId);
}