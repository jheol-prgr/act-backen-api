using BakendApi.Data;
using BakendApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BakendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(HeladeriaRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> ObtenerProductos(CancellationToken cancellationToken)
    {
        return Ok(await repository.ObtenerProductosAsync(cancellationToken));
    }

    [HttpGet("activos")]
    public async Task<ActionResult<IEnumerable<Producto>>> ObtenerProductosActivos(CancellationToken cancellationToken)
    {
        return Ok(await repository.ObtenerProductosActivosAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> ObtenerProductoPorId(int id, CancellationToken cancellationToken)
    {
        var producto = await repository.ObtenerProductoPorIdAsync(id, cancellationToken);

        if (producto is null)
        {
            return NotFound(new { mensaje = "Producto no encontrado" });
        }

        return Ok(producto);
    }

    [HttpPost]
    public async Task<ActionResult<Producto>> CrearProducto(CrearProductoRequest request, CancellationToken cancellationToken)
    {
        if (!EsProductoValido(request.Nombre, request.Precio, request.Stock, request.IdCategoria))
        {
            return BadRequest(new { mensaje = "Complete los campos obligatorios del producto" });
        }

        if (await repository.ObtenerCategoriaPorIdAsync(request.IdCategoria, cancellationToken) is null)
        {
            return BadRequest(new { mensaje = "Categoria no encontrada" });
        }

        var producto = await repository.CrearProductoAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObtenerProductoPorId), new { id = producto.Id }, producto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarProducto(int id, ActualizarProductoRequest productoActualizado, CancellationToken cancellationToken)
    {
        if (!EsProductoValido(productoActualizado.Nombre, productoActualizado.Precio, productoActualizado.Stock, productoActualizado.IdCategoria))
        {
            return BadRequest(new { mensaje = "Complete los campos obligatorios del producto" });
        }

        if (await repository.ObtenerCategoriaPorIdAsync(productoActualizado.IdCategoria, cancellationToken) is null)
        {
            return BadRequest(new { mensaje = "Categoria no encontrada" });
        }

        var actualizado = await repository.ActualizarProductoAsync(id, productoActualizado, cancellationToken);
        if (!actualizado)
        {
            return NotFound(new { mensaje = "Producto no encontrado" });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarProducto(int id, CancellationToken cancellationToken)
    {
        var eliminado = await repository.DesactivarProductoAsync(id, cancellationToken);
        if (!eliminado)
        {
            return NotFound(new { mensaje = "Producto no encontrado" });
        }

        return NoContent();
    }

    private static bool EsProductoValido(string nombre, decimal precio, int stock, int idCategoria)
    {
        return !string.IsNullOrWhiteSpace(nombre) && precio > 0 && stock >= 0 && idCategoria > 0;
    }
}

public static class DatosHeladeria
{
    public static readonly List<Categoria> Categorias =
    [
        new(1, "Helados", "Helados artesanales en cono, pocillo y tubo"),
        new(2, "Paletas", "Paletas de fruta y crema"),
        new(3, "Postres", "Postres helados especiales"),
        new(4, "Bebidas", "Batidos, malteadas y refrescos"),
        new(5, "Acompanamientos", "Conos, toppings y adicionales")
    ];

    public static readonly List<Producto> Productos =
    [
        new(1, "Helado de Vainilla", "Helado cremoso de vainilla natural", 2.50m, 100, 1, true),
        new(2, "Helado de Chocolate", "Helado intenso de cacao premium", 2.50m, 100, 1, true),
        new(3, "Helado de Fresa", "Helado de fresa fresca", 2.50m, 80, 1, true),
        new(4, "Helado de Menta", "Helado refrescante de menta con chispas de chocolate", 3.00m, 60, 1, true),
        new(5, "Helado de Mango", "Helado tropical de mango", 2.75m, 70, 1, true),
        new(6, "Paleta de Limon", "Paleta natural de limon", 1.50m, 120, 2, true),
        new(7, "Paleta de Sandia", "Paleta refrescante de sandia", 1.50m, 90, 2, true),
        new(8, "Paleta de Mora", "Paleta de mora artesanal", 1.75m, 85, 2, true),
        new(9, "Sundae de Chocolate", "Sundae con salsa de chocolate y nata", 4.50m, 50, 3, true),
        new(10, "Banana Split", "Banana split clasico con tres sabores", 5.00m, 40, 3, true),
        new(11, "Batido de Fresa", "Batido cremoso de fresa", 3.50m, 60, 4, true),
        new(12, "Malteada de Vainilla", "Malteada de vainilla con crema batida", 4.00m, 55, 4, true),
        new(13, "Cono Clasico", "Cono crujiente para helado", 0.50m, 200, 5, true),
        new(14, "Sprinkles de Colores", "Grageas de colores para decorar", 0.30m, 300, 5, true),
        new(15, "Salsa de Chocolate", "Salsa de chocolate para topping", 0.75m, 150, 5, true)
    ];

    public static readonly List<Compra> Compras =
    [
        new(1, 1, 7.00m, "Efectivo", "Compra de helados variados", new DateTime(2025, 7, 20, 14, 30, 0)),
        new(2, 2, 5.50m, "Tarjeta", "Paletas y batido", new DateTime(2025, 7, 20, 16, 0, 0)),
        new(3, 3, 12.00m, "Efectivo", "Pedido grande para evento", new DateTime(2025, 7, 21, 10, 15, 0))
    ];

    public static readonly List<DetalleCompra> DetalleCompra =
    [
        new(1, 1, 1, 2, 2.50m, 5.00m),
        new(2, 1, 3, 1, 2.50m, 2.50m),
        new(3, 2, 6, 2, 1.50m, 3.00m),
        new(4, 2, 11, 1, 3.50m, 3.50m),
        new(5, 3, 9, 1, 4.50m, 4.50m),
        new(6, 3, 10, 1, 5.00m, 5.00m),
        new(7, 3, 12, 1, 4.00m, 4.00m)
    ];
}
