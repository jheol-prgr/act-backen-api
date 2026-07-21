using Microsoft.AspNetCore.Mvc;

namespace BakendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> ObtenerCategorias()
    {
        return Ok(DatosHeladeria.Categorias);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Categoria> ObtenerCategoriaPorId(int id)
    {
        var categoria = DatosHeladeria.Categorias.FirstOrDefault(categoria => categoria.Id == id);

        if (categoria is null)
        {
            return NotFound(new { mensaje = "Categoria no encontrada" });
        }

        return Ok(categoria);
    }
}
