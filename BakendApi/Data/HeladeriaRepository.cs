using System.Data;
using BakendApi.Models;
using Microsoft.Data.SqlClient;

namespace BakendApi.Data;

public sealed class HeladeriaRepository
{
    private readonly string _connectionString;

    public HeladeriaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Heladeria")
            ?? throw new InvalidOperationException("Falta la cadena de conexion 'Heladeria'.");
    }

    public async Task<List<Categoria>> ObtenerCategoriasAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Nombre, Descripcion
            FROM dbo.Categorias
            WHERE Activo = 1
            ORDER BY Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var categorias = new List<Categoria>();
        while (await reader.ReadAsync(cancellationToken))
        {
            categorias.Add(MapCategoria(reader));
        }

        return categorias;
    }

    public async Task<Categoria?> ObtenerCategoriaPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Nombre, Descripcion
            FROM dbo.Categorias
            WHERE Id = @Id AND Activo = 1;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? MapCategoria(reader) : null;
    }

    public async Task<Categoria> CrearCategoriaAsync(CrearCategoriaRequest request, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO dbo.Categorias (Nombre, Descripcion, Activo)
            OUTPUT INSERTED.Id
            VALUES (@Nombre, @Descripcion, 1);
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        AgregarParametrosCategoria(command, request.Nombre, request.Descripcion);

        await connection.OpenAsync(cancellationToken);
        var id = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));

        var categoria = await ObtenerCategoriaPorIdAsync(id, cancellationToken);
        return categoria!;
    }

    public async Task<bool> ActualizarCategoriaAsync(int id, ActualizarCategoriaRequest request, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE dbo.Categorias
            SET Nombre = @Nombre,
                Descripcion = @Descripcion
            WHERE Id = @Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        AgregarParametrosCategoria(command, request.Nombre, request.Descripcion);

        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DesactivarCategoriaAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Categorias SET Activo = 0 WHERE Id = @Id;";

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<List<Producto>> ObtenerProductosAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Nombre, Descripcion, Precio, Stock, IdCategoria, Activo
            FROM dbo.Productos
            ORDER BY Id;
            """;

        return await ObtenerProductosConSqlAsync(sql, cancellationToken);
    }

    public async Task<List<Producto>> ObtenerProductosActivosAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Nombre, Descripcion, Precio, Stock, IdCategoria, Activo
            FROM dbo.Productos
            WHERE Activo = 1
            ORDER BY Id;
            """;

        return await ObtenerProductosConSqlAsync(sql, cancellationToken);
    }

    public async Task<Producto?> ObtenerProductoPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Nombre, Descripcion, Precio, Stock, IdCategoria, Activo
            FROM dbo.Productos
            WHERE Id = @Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? MapProducto(reader) : null;
    }

    public async Task<Producto> CrearProductoAsync(CrearProductoRequest request, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO dbo.Productos (Nombre, Descripcion, Precio, Stock, IdCategoria, Activo)
            OUTPUT INSERTED.Id
            VALUES (@Nombre, @Descripcion, @Precio, @Stock, @IdCategoria, 1);
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        AgregarParametrosProducto(command, request.Nombre, request.Descripcion, request.Precio, request.Stock, request.IdCategoria);

        await connection.OpenAsync(cancellationToken);
        var id = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));

        var producto = await ObtenerProductoPorIdAsync(id, cancellationToken);
        return producto!;
    }

    public async Task<bool> ActualizarProductoAsync(int id, ActualizarProductoRequest request, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE dbo.Productos
            SET Nombre = @Nombre,
                Descripcion = @Descripcion,
                Precio = @Precio,
                Stock = @Stock,
                IdCategoria = @IdCategoria
            WHERE Id = @Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        AgregarParametrosProducto(command, request.Nombre, request.Descripcion, request.Precio, request.Stock, request.IdCategoria);

        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DesactivarProductoAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Productos SET Activo = 0 WHERE Id = @Id;";

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<List<Usuario>> ObtenerUsuariosAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Nombre, Email, Rol, Activo
            FROM dbo.Usuarios
            ORDER BY Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var usuarios = new List<Usuario>();
        while (await reader.ReadAsync(cancellationToken))
        {
            usuarios.Add(MapUsuario(reader));
        }

        return usuarios;
    }

    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Nombre, Email, Rol, Activo
            FROM dbo.Usuarios
            WHERE Id = @Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? MapUsuario(reader) : null;
    }

    public async Task<Usuario> CrearUsuarioAsync(CrearUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO dbo.Usuarios (Nombre, Email, PasswordHash, Rol, Activo)
            OUTPUT INSERTED.Id
            VALUES (@Nombre, @Email, @PasswordHash, @Rol, 1);
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        AgregarParametrosUsuario(command, request.Nombre, request.Email, request.Rol);
        command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = request.PasswordHash.Trim();

        await connection.OpenAsync(cancellationToken);
        var id = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));

        var usuario = await ObtenerUsuarioPorIdAsync(id, cancellationToken);
        return usuario!;
    }

    public async Task<bool> ActualizarUsuarioAsync(int id, ActualizarUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE dbo.Usuarios
            SET Nombre = @Nombre,
                Email = @Email,
                Rol = @Rol,
                Activo = @Activo
            WHERE Id = @Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        AgregarParametrosUsuario(command, request.Nombre, request.Email, request.Rol);
        command.Parameters.Add("@Activo", SqlDbType.Bit).Value = request.Activo;

        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DesactivarUsuarioAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Usuarios SET Activo = 0 WHERE Id = @Id;";

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    private async Task<List<Producto>> ObtenerProductosConSqlAsync(string sql, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var productos = new List<Producto>();
        while (await reader.ReadAsync(cancellationToken))
        {
            productos.Add(MapProducto(reader));
        }

        return productos;
    }

    private static void AgregarParametrosProducto(SqlCommand command, string nombre, string descripcion, decimal precio, int stock, int idCategoria)
    {
        command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 120).Value = nombre.Trim();
        command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value = (descripcion ?? string.Empty).Trim();

        var precioParameter = command.Parameters.Add("@Precio", SqlDbType.Decimal);
        precioParameter.Precision = 10;
        precioParameter.Scale = 2;
        precioParameter.Value = precio;

        command.Parameters.Add("@Stock", SqlDbType.Int).Value = stock;
        command.Parameters.Add("@IdCategoria", SqlDbType.Int).Value = idCategoria;
    }

    private static void AgregarParametrosCategoria(SqlCommand command, string nombre, string descripcion)
    {
        command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 80).Value = nombre.Trim();
        command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value = (descripcion ?? string.Empty).Trim();
    }

    private static void AgregarParametrosUsuario(SqlCommand command, string nombre, string email, string rol)
    {
        command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 120).Value = nombre.Trim();
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 160).Value = email.Trim();
        command.Parameters.Add("@Rol", SqlDbType.NVarChar, 20).Value = rol.Trim();
    }

    private static Categoria MapCategoria(SqlDataReader reader) => new(
        reader.GetInt32(0),
        reader.GetString(1),
        reader.GetString(2));

    private static Producto MapProducto(SqlDataReader reader) => new(
        reader.GetInt32(0),
        reader.GetString(1),
        reader.GetString(2),
        reader.GetDecimal(3),
        reader.GetInt32(4),
        reader.GetInt32(5),
        reader.GetBoolean(6));

    private static Usuario MapUsuario(SqlDataReader reader) => new(
        reader.GetInt32(0),
        reader.GetString(1),
        reader.GetString(2),
        reader.GetString(3),
        reader.GetBoolean(4));
}
