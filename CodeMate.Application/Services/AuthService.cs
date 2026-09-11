using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Security;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Auth.Requests;
using CodeMate.Contracts.Auth.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Infrastructure.Services;
using CodeMate.Shared.Exceptions;
using System.Security.Cryptography;

namespace CodeMate.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private static readonly TimeSpan PasswordResetTokenLifetime = TimeSpan.FromMinutes(15);

    private readonly IEmailService _emailService;


    public AuthService(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _emailService = emailService;


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

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // برای جلوگیری از User Enumeration، حتی اگر Email پیدا نشود همان پیام موفقیت برگردانده می‌شود.
        if (user is null)
        {
            return new ForgotPasswordResponse
            {
                Message = "If an account with this email exists, a reset code has been sent."
            };
        }

        var resetToken = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        var expiresAt = DateTimeOffset.UtcNow.Add(PasswordResetTokenLifetime);

        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiresAt = expiresAt;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken, expiresAt);

        return new ForgotPasswordResponse
        {
            Message = "If an account with this email exists, a reset code has been sent."
        };
    }

    public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(request.Token);

        if (user is null ||
            !string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase) ||
            user.PasswordResetTokenExpiresAt is null ||
            user.PasswordResetTokenExpiresAt < DateTimeOffset.UtcNow)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Token"] = new[] { "Reset token is invalid or has expired." }
            });
        }

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

        // بعد از استفاده موفق، Token باید Invalidate شود تا دوباره قابل استفاده نباشد.
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return new ResetPasswordResponse
        {
            Message = "Password has been reset successfully."
        };
    }

    public async Task<ChangePasswordResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["CurrentPassword"] = new[] { "Current password is incorrect." }
            });
        }

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return new ChangePasswordResponse
        {
            Message = "Password has been changed successfully."
        };
    }

    public Task LogoutAsync()
    {
        // Stateless JWT — چیزی برای Invalidate کردن نداریم.
        // Client مسئول حذف Token از Storage خودشه.
        return Task.CompletedTask;
    }
}