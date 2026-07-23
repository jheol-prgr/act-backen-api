using System.Text.Json.Serialization;

namespace BakendApi.Models;

public record Categoria(int Id, string Nombre, string Descripcion);

public record Producto(
    int Id,
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    [property: JsonPropertyName("id_categoria")] int IdCategoria,
    bool Activo);

public record Usuario(int Id, string Nombre, string Email, string Rol, bool Activo);

public record CrearCategoriaRequest(string Nombre, string Descripcion);

public record ActualizarCategoriaRequest(string Nombre, string Descripcion);

public record CrearProductoRequest(
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    [property: JsonPropertyName("id_categoria")] int IdCategoria);

public record ActualizarProductoRequest(
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    [property: JsonPropertyName("id_categoria")] int IdCategoria);

public record CrearUsuarioRequest(string Nombre, string Email, string PasswordHash, string Rol);

public record ActualizarUsuarioRequest(string Nombre, string Email, string Rol, bool Activo);
