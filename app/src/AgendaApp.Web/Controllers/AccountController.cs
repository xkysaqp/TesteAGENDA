using AgendaApp.Dominio.Constants;
using AgendaApp.Infraestrutura.Identity;
using AgendaApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApp.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
                return View(model);
            }

            if (!user.Ativo)
            {
                ModelState.AddModelError(string.Empty, "Sua conta está desativada. Entre em contato com o administrador.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuário {Email} fez login com sucesso.", model.Email);
                
                user.RegistrarAcesso();
                await _userManager.UpdateAsync(user);

                return RedirectToAction("Index", "Dashboard");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Conta do usuário {Email} foi bloqueada.", model.Email);
                ModelState.AddModelError(string.Empty, "Conta bloqueada devido a muitas tentativas de login incorretas.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login do usuário {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "Ocorreu um erro interno. Tente novamente.");
            return View(model);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Nome = model.Nome,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var role = model.LojaId.HasValue ? Roles.Loja : Roles.Admin;
                await _userManager.AddToRoleAsync(user, role);

                _logger.LogInformation("Novo usuário {Email} criado pelo admin.", model.Email);
                TempData["SuccessMessage"] = "Usuário criado com sucesso!";
                return RedirectToAction("Register");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "Ocorreu um erro interno. Tente novamente.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var userEmail = User.Identity?.Name;
        await _signInManager.SignOutAsync();
        
        _logger.LogInformation("Usuário {Email} fez logout.", userEmail);
        TempData["InfoMessage"] = "Você foi desconectado com sucesso.";
        
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new ProfileViewModel
        {
            Nome = user.Nome,
            Email = user.Email!,
            Telefone = user.PhoneNumber ?? "",
            CNPJ = user.CNPJ,
            AreaAtuacao = user.AreaAtuacao,
            UltimoAcesso = user.UltimoAcesso,
            EhAdmin = user.EhAdmin
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            user.Nome = model.Nome;
            user.PhoneNumber = model.Telefone;
            user.CNPJ = model.CNPJ;
            user.AreaAtuacao = model.AreaAtuacao;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar perfil do usuário");
            ModelState.AddModelError(string.Empty, "Erro interno. Tente novamente.");
        }

        return View(model);
    }
} 