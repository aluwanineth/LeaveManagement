

using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Identity.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LeaveManagement.Application.DTOs.Settings;
using LeaveManagement.Application.Wrappers;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Application.DTOs.Account;

namespace LeaveManagement.Identity.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JWTSettings _jwtSettings;
    private readonly IEmployeeRepositoryAsync _employeeRepository;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JWTSettings> jwtSettings,
        IEmployeeRepositoryAsync employeeRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
        _employeeRepository = employeeRepository;
    }

    public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new Response<AuthenticationResponse>("User not found.");
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return new Response<AuthenticationResponse>("Invalid credentials.");
        }

        var employee = await _employeeRepository.GetByEmailAsync(request.Email);
        if (employee != null)
        {
            user.EmployeeId = employee.EmployeeId;
            await _userManager.UpdateAsync(user);
        }

        var jwtSecurityToken = await GenerateJWToken(user);
        var response = new AuthenticationResponse
        {
            Id = user.Id,
            JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email,
            UserName = user.UserName,
            EmployeeId = user.EmployeeId
        };

        return new Response<AuthenticationResponse>(response, "Authentication successful.");
    }

    public async Task<Response<string>> RegisterAsync(LeaveManagement.Application.DTOs.Account.RegisterRequest request)
    {
        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            return new Response<string>("Username already exists.");
        }

        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail != null)
        {
            return new Response<string>("Email already exists.");
        }

        var employee = await _employeeRepository.GetByEmailAsync(request.Email);
        if (employee == null)
        {
            return new Response<string>("Employee not found with this email.");
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            EmployeeId = employee.EmployeeId
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            // Assign role based on employee type
            var roleName = GetRoleNameByEmployeeType(employee.EmployeeType);
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            await _userManager.AddToRoleAsync(user, roleName);

            return new Response<string>(user.Id, "User registered successfully.");
        }
        else
        {
            return new Response<string>(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role));
        }

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("EmployeeId", user.EmployeeId?.ToString() ?? "")
            }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    private string GetRoleNameByEmployeeType(EmployeeType employeeType)
    {
        return employeeType switch
        {
            EmployeeType.CEO => "CEO",
            EmployeeType.Manager => "Manager",
            EmployeeType.TeamLead => "TeamLead",
            EmployeeType.Employee => "Employee",
            _ => "Employee"
        };
    }
}