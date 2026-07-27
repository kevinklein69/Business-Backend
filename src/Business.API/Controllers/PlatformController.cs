using Business.Application.Features.Auth.RegisterCompany;
using Business.Application.Features.Platform.DeleteCompany;
using Business.Application.Features.Platform.ListCompanies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Business.API.Controllers;

/// Internal platform-admin surface: manage tenants over HTTP instead of SSH, gated by the
/// shared secret in PLATFORM_ADMIN_KEY. NOT customer-facing.
///   GET    /admin                        -> self-contained form + company list (same origin)
///   GET    /api/platform/companies       -> list companies (name, user count, created)
///   POST   /api/platform/companies       -> create a company + first Admin
///   DELETE /api/platform/companies/{id}  -> delete a company and all its data (irreversible)
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

    [HttpGet("/api/platform/companies")]
    public async Task<IActionResult> ListCompanies(
        [FromHeader(Name = "X-Platform-Key")] string? key,
        CancellationToken cancellationToken)
    {
        if (!KeyValid(key))
        {
            return Unauthorized();
        }

        var companies = await sender.Send(new ListCompaniesQuery(), cancellationToken);
        return Ok(companies);
    }

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

    [HttpDelete("/api/platform/companies/{id:guid}")]
    public async Task<IActionResult> DeleteCompany(
        [FromHeader(Name = "X-Platform-Key")] string? key,
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!KeyValid(key))
        {
            logger.LogWarning("Platform admin: rejected company-delete from {IP} (bad/missing key)",
                HttpContext.Connection.RemoteIpAddress);
            return Unauthorized();
        }

        await sender.Send(new DeleteCompanyCommand(id), cancellationToken);
        logger.LogWarning("Platform admin: company {Id} DELETED (all tenant data removed)", id);
        return NoContent();
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
<title>Plattform-Admin</title>
<style>
  :root { color-scheme: light dark; }
  body { font-family: system-ui, sans-serif; max-width: 42rem; margin: 2.5rem auto; padding: 0 1rem; }
  h1 { font-size: 1.35rem; }
  h2 { font-size: 1.05rem; margin-top: 2rem; }
  label { display: block; margin: .75rem 0 .25rem; font-size: .9rem; }
  input { width: 100%; padding: .5rem; font-size: 1rem; box-sizing: border-box; }
  button { padding: .5rem .9rem; font-size: .95rem; cursor: pointer; }
  form button { margin-top: 1.25rem; }
  hr { margin: 2rem 0; border: none; border-top: 1px solid #8884; }
  table { width: 100%; border-collapse: collapse; margin-top: .75rem; font-size: .9rem; }
  th, td { text-align: left; padding: .45rem .5rem; border-bottom: 1px solid #8883; }
  td.actions { text-align: right; }
  button.del { background: #b3261e; color: #fff; border: none; border-radius: .3rem; }
  #out { margin-top: 1rem; padding: .75rem; border-radius: .375rem; white-space: pre-wrap; display: none; }
  .ok { background: #e6f4ea; color: #1e4620; }
  .err { background: #fce8e6; color: #5c1a15; }
  .muted { color: #8a8a8a; font-size: .85rem; }
</style>
</head>
<body>
<h1>Plattform-Admin</h1>

<label>Dev-Key</label>
<input id="key" type="password" autocomplete="off" placeholder="PLATFORM_ADMIN_KEY">
<p class="muted">Wird für Laden, Anlegen und Löschen verwendet.</p>

<h2>Firmen <button id="load" type="button">laden</button></h2>
<table id="list">
  <thead><tr><th>Firma</th><th>User</th><th>Erstellt</th><th></th></tr></thead>
  <tbody><tr><td colspan="4" class="muted">Noch nicht geladen.</td></tr></tbody>
</table>

<hr>

<h2>Neue Firma anlegen</h2>
<form id="f">
  <label>Firmenname</label><input id="company" required>
  <label>Admin Vorname</label><input id="first" required>
  <label>Admin Nachname</label><input id="last" required>
  <label>Admin E-Mail</label><input id="email" type="email" required>
  <label>Admin Passwort (min. 8)</label><input id="password" type="text" minlength="8" required>
  <button type="submit">Anlegen</button>
</form>

<div id="out"></div>

<script>
const val = (id) => document.getElementById(id).value;
const out = document.getElementById('out');
function showOut(cls, msg) { out.className = cls; out.textContent = msg; out.style.display = 'block'; }
function keyHeader() { return { 'X-Platform-Key': val('key') }; }

async function loadList() {
  const tbody = document.querySelector('#list tbody');
  let res;
  try {
    res = await fetch('/api/platform/companies', { headers: keyHeader() });
  } catch (err) {
    showOut('err', 'Netzwerkfehler: ' + err);
    return;
  }
  if (!res.ok) {
    showOut('err', res.status === 401 ? 'Falscher oder fehlender Dev-Key.' : ('Laden fehlgeschlagen (' + res.status + ')'));
    return;
  }
  const rows = await res.json();
  tbody.innerHTML = '';
  if (rows.length === 0) {
    tbody.innerHTML = '<tr><td colspan="4" class="muted">Noch keine Firmen.</td></tr>';
    return;
  }
  for (const c of rows) {
    const tr = document.createElement('tr');
    for (let i = 0; i < 4; i++) tr.appendChild(document.createElement('td'));
    tr.children[0].textContent = c.name;
    tr.children[1].textContent = c.userCount;
    tr.children[2].textContent = new Date(c.createdAt).toLocaleDateString('de-DE');
    tr.children[3].className = 'actions';
    const btn = document.createElement('button');
    btn.textContent = 'Löschen';
    btn.className = 'del';
    btn.onclick = () => del(c.id, c.name);
    tr.children[3].appendChild(btn);
    tbody.appendChild(tr);
  }
}

async function del(id, name) {
  if (!confirm('Firma "' + name + '" und ALLE zugehörigen Daten unwiderruflich löschen?')) return;
  let res;
  try {
    res = await fetch('/api/platform/companies/' + id, { method: 'DELETE', headers: keyHeader() });
  } catch (err) {
    showOut('err', 'Netzwerkfehler: ' + err);
    return;
  }
  if (res.ok) {
    showOut('ok', 'Firma "' + name + '" gelöscht.');
    loadList();
  } else if (res.status === 401) {
    showOut('err', 'Falscher oder fehlender Dev-Key.');
  } else if (res.status === 404) {
    showOut('err', 'Firma nicht gefunden (schon gelöscht?).');
  } else {
    showOut('err', 'Löschen fehlgeschlagen (' + res.status + ')');
  }
}

document.getElementById('load').addEventListener('click', loadList);

document.getElementById('f').addEventListener('submit', async (e) => {
  e.preventDefault();
  out.style.display = 'none';
  const btn = e.target.querySelector('button');
  btn.disabled = true;
  try {
    const res = await fetch('/api/platform/companies', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...keyHeader() },
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
      showOut('ok', (body && body.message ? body.message : 'Firma angelegt.') +
        (body && body.adminEmail ? '\nAdmin-Login: ' + body.adminEmail : ''));
      e.target.reset();
      loadList();
    } else if (res.status === 401) {
      showOut('err', 'Falscher oder fehlender Dev-Key.');
    } else {
      let msg = (body && (body.detail || body.title)) || ('Fehler ' + res.status);
      if (body && body.errors) msg = Object.values(body.errors).flat().join('\n');
      showOut('err', msg);
    }
  } catch (err) {
    showOut('err', 'Netzwerkfehler: ' + err);
  } finally {
    btn.disabled = false;
  }
});
</script>
</body>
</html>
""";
}
