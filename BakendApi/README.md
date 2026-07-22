# La Dolce Vita - Heladeria Artesanal

*Proyecto realizado por el grupo 404*

API REST backend para la gestion de una heladeria artesanal, desarrollada en ASP.NET Core 10.

## Tecnologias

- **.NET 10** / ASP.NET Core
- **C#** con Minimal API y Controllers
- **OpenAPI/Swagger** para documentacion

## Estructura del proyecto

```
BakendApi/
├── Controllers/
│   ├── CategoriasController.cs    # CRUD de categorias
│   ├── ComprasController.cs       # Gestion de compras
│   ├── ProductosController.cs     # CRUD de productos
│   └── UsuariosController.cs      # Consulta de usuarios
├── Program.cs                      # Configuracion y arranque
├── appsettings.json
└── BakendApi.csproj
```

## Endpoints

### Categorias (`/api/categorias`)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/categorias` | Obtener todas las categorias |
| GET | `/api/categorias/{id}` | Obtener categoria por ID |

### Productos (`/api/productos`)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/productos` | Obtener todos los productos |
| GET | `/api/productos/activos` | Obtener productos activos |
| GET | `/api/productos/{id}` | Obtener producto por ID |
| POST | `/api/productos` | Crear producto |
| PUT | `/api/productos/{id}` | Actualizar producto |
| DELETE | `/api/productos/{id}` | Desactivar producto (soft delete) |

### Compras (`/api/compras`)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/compras` | Obtener todas las compras |
| GET | `/api/compras/{id}` | Obtener compra por ID |
| POST | `/api/compras` | Crear compra (descuenta stock automaticamente) |
| DELETE | `/api/compras/{id}` | Eliminar compra |

### Usuarios (`/api/usuarios`)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/usuarios` | Obtener todos los usuarios |
| GET | `/api/usuarios/{id}` | Obtener usuario por ID |

## Modelos de datos

- **Categoria**: Id, Nombre, Descripcion
- **Producto**: Id, Nombre, Descripcion, Precio, Stock, IdCategoria, Activo
- **Compra**: Id, IdUsuario, Total, MetodoPago, Observaciones, FechaCompra
- **DetalleCompra**: Id, IdCompra, IdProducto, Cantidad, PrecioUnitario, Subtotal
- **Usuario**: Id, Nombre, Correo

## Ejecutar el proyecto

```bash
dotnet run
```

La API estara disponible en `http://localhost:5147`.

## Datos de prueba

El proyecto incluye datos precargados en memoria:

- 5 categorias (Helados, Paletas, Postres, Bebidas, Acompanamientos)
- 15 productos
- 3 compras con sus detalles
- 3 usuarios
