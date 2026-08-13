using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Security;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Auth.Requests;
using CodeMate.Contracts.Auth.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Shared.Exceptions;

namespace CodeMate.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByUserNameAsync(request.UserName))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["UserName"] = new[] { "Username already exists." }
            });
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Email"] = new[] { "Email already exists." }
            });
        }

        var user = _mapper.Map<User>(request);
        user.PasswordHash = _passwordHasher.Hash(request.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync(); 

        var response = _mapper.Map<RegisterResponse>(user);
        response.Message = "User registered successfully.";
        return response;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUserNameOrEmailAsync(request.UserNameOrEmail);

       
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var jwt = _jwtService.GenerateToken(user);
        return new LoginResponse
        {
            UserName = user.UserName,
            Token = jwt.Token,
            Expiration = jwt.Expiration
        };
    }
}