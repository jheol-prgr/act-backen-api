using BakendApi.Data;
using BakendApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BakendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController(HeladeriaRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> ObtenerCategorias(CancellationToken cancellationToken)
    {
        return Ok(await repository.ObtenerCategoriasAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Categoria>> ObtenerCategoriaPorId(int id, CancellationToken cancellationToken)
    {
        var categoria = await repository.ObtenerCategoriaPorIdAsync(id, cancellationToken);

        if (categoria is null)
        {
            return NotFound(new { mensaje = "Categoria no encontrada" });
        }

        return Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult<Categoria>> CrearCategoria(CrearCategoriaRequest request, CancellationToken cancellationToken)
    {
        if (!EsCategoriaValida(request.Nombre, request.Descripcion))
        {
            return BadRequest(new { mensaje = "Complete los campos obligatorios de la categoria" });
        }

        try
        {
            var categoria = await repository.CrearCategoriaAsync(request, cancellationToken);
            return CreatedAtAction(nameof(ObtenerCategoriaPorId), new { id = categoria.Id }, categoria);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return Conflict(new { mensaje = "Ya existe una categoria con ese nombre" });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarCategoria(int id, ActualizarCategoriaRequest request, CancellationToken cancellationToken)
    {
        if (!EsCategoriaValida(request.Nombre, request.Descripcion))
        {
            return BadRequest(new { mensaje = "Complete los campos obligatorios de la categoria" });
        }

        try
        {
            var actualizada = await repository.ActualizarCategoriaAsync(id, request, cancellationToken);
            if (!actualizada)
            {
                return NotFound(new { mensaje = "Categoria no encontrada" });
            }

            return NoContent();
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return Conflict(new { mensaje = "Ya existe una categoria con ese nombre" });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarCategoria(int id, CancellationToken cancellationToken)
    {
        var eliminada = await repository.DesactivarCategoriaAsync(id, cancellationToken);
        if (!eliminada)
        {
            return NotFound(new { mensaje = "Categoria no encontrada" });
        }

        return NoContent();
    }

    private static bool EsCategoriaValida(string nombre, string descripcion)
    {
        return !string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(descripcion);
    }
}
