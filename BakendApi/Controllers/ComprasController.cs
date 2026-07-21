using Microsoft.AspNetCore.Mvc;

namespace BakendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Compra>> ObtenerCompras()
    {
        return Ok(DatosHeladeria.Compras);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Compra> ObtenerCompraPorId(int id)
    {
        var compra = DatosHeladeria.Compras.FirstOrDefault(compra => compra.Id == id);

        if (compra is null)
        {
            return NotFound(new { mensaje = "Compra no encontrada" });
        }

        return Ok(compra);
    }

    [HttpPost]
    public ActionResult<Compra> CrearCompra(CrearCompraRequest request)
    {
        if (request.IdUsuario <= 0 || request.Items is null || request.Items.Count == 0)
        {
            return BadRequest(new { mensaje = "Complete los campos obligatorios de la compra" });
        }

        var compra = new Compra(
            DatosHeladeria.Compras.Count > 0
                ? DatosHeladeria.Compras.Max(c => c.Id) + 1
                : 1,
            request.IdUsuario,
            0,
            request.MetodoPago,
            request.Observaciones,
            DateTime.Now);

        decimal total = 0;

        foreach (var item in request.Items)
        {
            var producto = DatosHeladeria.Productos.FirstOrDefault(p => p.Id == item.IdProducto);

            if (producto is null)
            {
                return NotFound(new { mensaje = $"Producto con id {item.IdProducto} no encontrado" });
            }

            if (producto.Stock < item.Cantidad)
            {
                return BadRequest(new { mensaje = $"Stock insuficiente para {producto.Nombre}" });
            }

            var subtotal = producto.Precio * item.Cantidad;
            total += subtotal;

            var detalle = new DetalleCompra(
                DatosHeladeria.DetalleCompra.Count > 0
                    ? DatosHeladeria.DetalleCompra.Max(d => d.Id) + 1
                    : 1,
                compra.Id,
                item.IdProducto,
                item.Cantidad,
                producto.Precio,
                subtotal);

            DatosHeladeria.DetalleCompra.Add(detalle);

            var indexProducto = DatosHeladeria.Productos.FindIndex(p => p.Id == item.IdProducto);
            DatosHeladeria.Productos[indexProducto] = producto with { Stock = producto.Stock - item.Cantidad };
        }

        compra = compra with { Total = total };

        var indexCompra = DatosHeladeria.Compras.FindIndex(c => c.Id == compra.Id);
        DatosHeladeria.Compras[indexCompra] = compra;

        return CreatedAtAction(nameof(ObtenerCompraPorId), new { id = compra.Id }, compra);
    }

    [HttpDelete("{id:int}")]
    public IActionResult EliminarCompra(int id)
    {
        var index = DatosHeladeria.Compras.FindIndex(compra => compra.Id == id);

        if (index == -1)
        {
            return NotFound(new { mensaje = "Compra no encontrada" });
        }

        DatosHeladeria.Compras.RemoveAt(index);

        return NoContent();
    }
}

public record Compra(
    int Id,
    [property: System.Text.Json.Serialization.JsonPropertyName("id_usuario")] int IdUsuario,
    decimal Total,
    [property: System.Text.Json.Serialization.JsonPropertyName("metodo_pago")] string MetodoPago,
    string Observaciones,
    [property: System.Text.Json.Serialization.JsonPropertyName("fecha_compra")] DateTime FechaCompra);

public record DetalleCompra(
    int Id,
    [property: System.Text.Json.Serialization.JsonPropertyName("id_compra")] int IdCompra,
    [property: System.Text.Json.Serialization.JsonPropertyName("id_producto")] int IdProducto,
    int Cantidad,
    [property: System.Text.Json.Serialization.JsonPropertyName("precio_unitario")] decimal PrecioUnitario,
    decimal Subtotal);

public record CrearCompraRequest(
    [property: System.Text.Json.Serialization.JsonPropertyName("id_usuario")] int IdUsuario,
    [property: System.Text.Json.Serialization.JsonPropertyName("metodo_pago")] string MetodoPago,
    string Observaciones,
    List<ItemCompraRequest> Items);

public record ItemCompraRequest(
    [property: System.Text.Json.Serialization.JsonPropertyName("id_producto")] int IdProducto,
    int Cantidad);
