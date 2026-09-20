using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Skills.Requests;
using CodeMate.Contracts.Skills.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class AdminSkillService : IAdminSkillService
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public AdminSkillService(ISkillRepository skillRepository, IMapper mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    public async Task<SkillResponse> CreateSkillAsync(CreateSkillRequest request)
    {
        var existing = await _skillRepository.GetByNameAsync(request.Name);

        if (existing is not null)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Name"] = new[] { "A skill with this name already exists." }
            });
        }

        var skill = _mapper.Map<Skill>(request);

        await _skillRepository.AddSkillAsync(skill);
        await _skillRepository.SaveChangesAsync();

        return _mapper.Map<SkillResponse>(skill);
    }

    public async Task<SkillResponse> UpdateSkillAsync(Guid skillId, UpdateSkillRequest request)
    {
        var skill = await _skillRepository.GetByIdAsync(skillId);

        if (skill is null)
        {
            throw new NotFoundException("Skill not found.");
        }

        skill.Name = request.Name;

        await _skillRepository.UpdateAsync(skill);
        await _skillRepository.SaveChangesAsync();

        return _mapper.Map<SkillResponse>(skill);
    }

    public async Task DeleteSkillAsync(Guid skillId)
    {
        var skill = await _skillRepository.GetByIdAsync(skillId);

        if (skill is null)
        {
            throw new NotFoundException("Skill not found.");
        }

        await _skillRepository.DeleteAsync(skillId);
        await _skillRepository.SaveChangesAsync();
    }
}