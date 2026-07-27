using Business.Application.Features.Auth.RegisterCompany;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Business.API.Controllers;

/// Internal platform-admin surface: onboard a new tenant (company + first Admin) over HTTP
/// instead of SSH, gated by the shared secret in PLATFORM_ADMIN_KEY. NOT customer-facing.
///   GET  /admin                   -> tiny self-contained form (same origin as the API)
///   POST /api/platform/companies  -> validates the key, then runs RegisterCompanyCommand
[ApiController]
[AllowAnonymous]
public class PlatformController(ISender sender, IConfiguration config, ILogger<PlatformController> logger)
    : ControllerBase
{
    public record CreateCompanyRequest(
        string CompanyName,
        string AdminFirstName,
        string AdminLastName,
        string AdminEmail,
        string AdminPassword);

    [HttpPost("/api/platform/companies")]
    public async Task<IActionResult> CreateCompany(
        [FromHeader(Name = "X-Platform-Key")] string? key,
        CreateCompanyRequest request,
        CancellationToken cancellationToken)
    {
        if (!KeyValid(key))
        {
            logger.LogWarning("Platform admin: rejected company-create from {IP} (bad/missing key)",
                HttpContext.Connection.RemoteIpAddress);
            return Unauthorized();
        }

        // Field validation (email format/uniqueness, min password length) lives in the
        // RegisterCompany validator/handler and is surfaced by ExceptionHandlingMiddleware
        // as 400/409 — no need to duplicate it here.
        await sender.Send(new RegisterCompanyCommand(
            request.CompanyName,
            request.AdminFirstName,
            request.AdminLastName,
            request.AdminEmail,
            request.AdminPassword), cancellationToken);

        logger.LogInformation("Platform admin: company '{Company}' created (admin {Email})",
            request.CompanyName, request.AdminEmail);

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = $"Firma '{request.CompanyName}' angelegt.",
            adminEmail = request.AdminEmail,
        });
    }

    // Fails closed: if PLATFORM_ADMIN_KEY is unset/empty, every request is rejected.
    // Constant-time compare so a wrong key can't be guessed byte-by-byte via timing.
    private bool KeyValid(string? provided)
    {
        var expected = config["PLATFORM_ADMIN_KEY"];
        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(provided))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(provided),
            Encoding.UTF8.GetBytes(expected));
    }

    [HttpGet("/admin")]
    public ContentResult Admin() => Content(AdminHtml, "text/html; charset=utf-8");

    private const string AdminHtml = """
<!doctype html>
<html lang="de">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta name="robots" content="noindex">
<title>Plattform-Admin – Firma anlegen</title>
<style>
  :root { color-scheme: light dark; }
  body { font-family: system-ui, sans-serif; max-width: 30rem; margin: 3rem auto; padding: 0 1rem; }
  h1 { font-size: 1.25rem; }
  label { display: block; margin: .75rem 0 .25rem; font-size: .9rem; }
  input { width: 100%; padding: .5rem; font-size: 1rem; box-sizing: border-box; }
  button { margin-top: 1.25rem; padding: .6rem 1rem; font-size: 1rem; cursor: pointer; }
  #out { margin-top: 1rem; padding: .75rem; border-radius: .375rem; white-space: pre-wrap; display: none; }
  .ok { background: #e6f4ea; color: #1e4620; }
  .err { background: #fce8e6; color: #5c1a15; }
</style>
</head>
<body>
<h1>Firma anlegen</h1>
<form id="f">
  <label>Dev-Key</label><input id="key" type="password" autocomplete="off" required>
  <label>Firmenname</label><input id="company" required>
  <label>Admin Vorname</label><input id="first" required>
  <label>Admin Nachname</label><input id="last" required>
  <label>Admin E-Mail</label><input id="email" type="email" required>
  <label>Admin Passwort (min. 8)</label><input id="password" type="text" minlength="8" required>
  <button type="submit">Anlegen</button>
</form>
<div id="out"></div>
<script>
const f = document.getElementById('f'), out = document.getElementById('out');
const val = (id) => document.getElementById(id).value;
f.addEventListener('submit', async (e) => {
  e.preventDefault();
  out.style.display = 'none';
  const btn = f.querySelector('button');
  btn.disabled = true;
  try {
    const res = await fetch('/api/platform/companies', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'X-Platform-Key': val('key') },
      body: JSON.stringify({
        companyName: val('company'),
        adminFirstName: val('first'),
        adminLastName: val('last'),
        adminEmail: val('email'),
        adminPassword: val('password'),
      }),
    });
    const body = await res.json().catch(() => null);
    if (res.ok) {
      out.className = 'ok';
      out.textContent = (body && body.message ? body.message : 'Firma angelegt.') +
        (body && body.adminEmail ? '\nAdmin-Login: ' + body.adminEmail : '');
      f.reset();
    } else if (res.status === 401) {
      out.className = 'err';
      out.textContent = 'Falscher oder fehlender Dev-Key.';
    } else {
      out.className = 'err';
      let msg = (body && (body.detail || body.title)) || ('Fehler ' + res.status);
      if (body && body.errors) {
        msg = Object.values(body.errors).flat().join('\n');
      }
      out.textContent = msg;
    }
  } catch (err) {
    out.className = 'err';
    out.textContent = 'Netzwerkfehler: ' + err;
  } finally {
    out.style.display = 'block';
    btn.disabled = false;
  }
});
</script>
</body>
</html>
""";
}
