using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Security;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Auth.Requests;
using CodeMate.Contracts.Auth.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
    IUserRepository userRepository,
    IMapper mapper,
    IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByUserNameAsync(request.UserName))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = _mapper.Map<User>(request);
        

        // در Phase 4 رمز عبور هش خواهد شد.
        user.PasswordHash = _passwordHasher.Hash(request.Password);

        // ذخیره کاربر
        await _userRepository.AddAsync(user);

        // تبدیل Entity به Response
        var response = _mapper.Map<RegisterResponse>(user);

        response.Message = "User registered successfully.";

        return response;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUserNameOrEmailAsync(request.UserNameOrEmail);

        if (user is null)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        // Phase 4
        // بررسی رمز عبور
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
             throw new InvalidOperationException("Invalid username or password.");
        }

        // Phase 4
        // تولید JWT
        // var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            UserName = user.UserName,

            Token = string.Empty,

            Expiration = DateTimeOffset.MinValue
        };
    }


}