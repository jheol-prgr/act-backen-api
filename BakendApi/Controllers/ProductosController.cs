using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace BakendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Producto>> ObtenerProductos()
    {
        return Ok(DatosHeladeria.Productos);
    }

    [HttpGet("activos")]
    public ActionResult<IEnumerable<Producto>> ObtenerProductosActivos()
    {
        return Ok(DatosHeladeria.Productos.Where(producto => producto.Activo));
    }

    [HttpGet("{id:int}")]
    public ActionResult<Producto> ObtenerProductoPorId(int id)
    {
        var producto = DatosHeladeria.Productos.FirstOrDefault(producto => producto.Id == id);

        if (producto is null)
        {
            return NotFound(new { mensaje = "Producto no encontrado" });
        }

        return Ok(producto);
    }

    [HttpPost]
    public ActionResult<Producto> CrearProducto(CrearProductoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || request.Precio <= 0 || request.Stock < 0)
        {
            return BadRequest(new { mensaje = "Complete los campos obligatorios del producto" });
        }

        var producto = new Producto(
            DatosHeladeria.Productos.Max(producto => producto.Id) + 1,
            request.Nombre,
            request.Descripcion,
            request.Precio,
            request.Stock,
            request.IdCategoria,
            true);

        DatosHeladeria.Productos.Add(producto);

        return CreatedAtAction(nameof(ObtenerProductoPorId), new { id = producto.Id }, producto);
    }

    [HttpPut("{id:int}")]
    public IActionResult ActualizarProducto(int id, Producto productoActualizado)
    {
        var index = DatosHeladeria.Productos.FindIndex(producto => producto.Id == id);

        if (index == -1)
        {
            return NotFound(new { mensaje = "Producto no encontrado" });
        }

        if (string.IsNullOrWhiteSpace(productoActualizado.Nombre) || productoActualizado.Precio <= 0 || productoActualizado.Stock < 0)
        {
            return BadRequest(new { mensaje = "Complete los campos obligatorios del producto" });
        }

        DatosHeladeria.Productos[index] = productoActualizado with { Id = id };

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult EliminarProducto(int id)
    {
        var index = DatosHeladeria.Productos.FindIndex(producto => producto.Id == id);

        if (index == -1)
        {
            return NotFound(new { mensaje = "Producto no encontrado" });
        }

        DatosHeladeria.Productos[index] = DatosHeladeria.Productos[index] with { Activo = false };

        return NoContent();
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

    public static readonly List<Venta> Ventas = [];

    public static readonly List<DetalleVenta> DetalleVenta = [];

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

public record Categoria(int Id, string Nombre, string Descripcion);

public record Producto(
    int Id,
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    [property: JsonPropertyName("id_categoria")] int IdCategoria,
    bool Activo);

public record Venta(
    int Id,
    [property: JsonPropertyName("id_usuario")] int IdUsuario,
    decimal Total,
    [property: JsonPropertyName("metodo_pago")] string MetodoPago,
    string Observaciones,
    [property: JsonPropertyName("fecha_venta")] DateTime FechaVenta);

public record DetalleVenta(
    int Id,
    [property: JsonPropertyName("id_venta")] int IdVenta,
    [property: JsonPropertyName("id_producto")] int IdProducto,
    int Cantidad,
    [property: JsonPropertyName("precio_unitario")] decimal PrecioUnitario,
    decimal Subtotal);

public record CrearProductoRequest(
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    [property: JsonPropertyName("id_categoria")] int IdCategoria);
