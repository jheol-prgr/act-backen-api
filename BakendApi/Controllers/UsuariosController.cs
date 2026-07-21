using Microsoft.AspNetCore.Mvc;

namespace BakendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private static readonly List<Usuario> Usuarios =
    [
        new(1, "Ana Martinez", "ana.martinez@example.com"),
        new(2, "Carlos Lopez", "carlos.lopez@example.com"),
        new(3, "Maria Garcia", "maria.garcia@example.com")
    ];

    [HttpGet]
    public ActionResult<IEnumerable<Usuario>> ObtenerUsuarios()
    {
        return Ok(Usuarios);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Usuario> ObtenerUsuarioPorId(int id)
    {
        var usuario = Usuarios.FirstOrDefault(usuario => usuario.Id == id);

        if (usuario is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado" });
        }

        return Ok(usuario);
    }
}

public record Usuario(int Id, string Nombre, string Correo);
