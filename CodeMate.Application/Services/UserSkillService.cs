using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Skills.Requests;
using CodeMate.Contracts.Skills.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Shared.Exceptions;
using CodeMate.Domain.Enums;

namespace CodeMate.Application.Services;

public sealed class UserSkillService : IUserSkillService
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public UserSkillService(
        ISkillRepository skillRepository,
        IMapper mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SkillResponse>> GetMySkillsAsync(Guid userId)
    {
        var userSkills = await _skillRepository.GetUserSkillsAsync(userId);

        return _mapper.Map<IEnumerable<SkillResponse>>(userSkills);
    }

    public async Task<SkillResponse> AddSkillAsync(
      Guid userId,
      AddUserSkillRequest request)
    {
        var skill = await _skillRepository.GetByIdAsync(request.SkillId);

        if (skill is null)
        {
            throw new NotFoundException("Skill not found. Contact an admin to add it.");
        }

        if (await _skillRepository.HasUserSkillAsync(userId, skill.Id))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["SkillId"] = new[] { "This skill has already been added." }
            });
        }

        var userSkill = new UserSkill
        {
            UserId = userId,
            SkillId = skill.Id,
            Level = (SkillLevel)request.Level,
            YearsOfExperience = request.YearsOfExperience
        };

        await _skillRepository.AddUserSkillAsync(userSkill);
        await _skillRepository.SaveChangesAsync();

        return _mapper.Map<SkillResponse>(skill);
    }
    public async Task RemoveSkillAsync(
        Guid userId,
        Guid skillId)
    {
        await _skillRepository.RemoveUserSkillAsync(userId, skillId);
        await _skillRepository.SaveChangesAsync();
    }
    public async Task<IEnumerable<SkillResponse>> SearchSkillsAsync(string? term)
    {
        var skills = await _skillRepository.SearchAsync(term);

        return _mapper.Map<IEnumerable<SkillResponse>>(skills);
    }
}