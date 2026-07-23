# La Dolce Vita - Backend API

Actividad: Complementar metodos GET, PUT, POST y DELETE.

## Ejecucion

1. Abrir SQL Server y verificar que exista la base de datos `Heladeria`.
2. Ejecutar el backend desde la carpeta `BakendApi`:

```bash
dotnet run
```

3. Abrir Swagger en el navegador:

```text
http://localhost:5147/swagger
```

## Endpoints complementados

| Controlador | Metodo | Endpoint | Resultado esperado |
| --- | --- | --- | --- |
| Categorias | GET | `/api/categorias` | Lista categorias activas |
| Categorias | GET | `/api/categorias/{id}` | Consulta una categoria |
| Categorias | POST | `/api/categorias` | Crea una categoria |
| Categorias | PUT | `/api/categorias/{id}` | Actualiza una categoria |
| Categorias | DELETE | `/api/categorias/{id}` | Desactiva una categoria |
| Productos | GET | `/api/productos` | Lista productos |
| Productos | GET | `/api/productos/activos` | Lista productos activos |
| Productos | GET | `/api/productos/{id}` | Consulta un producto |
| Productos | POST | `/api/productos` | Crea un producto |
| Productos | PUT | `/api/productos/{id}` | Actualiza un producto |
| Productos | DELETE | `/api/productos/{id}` | Desactiva un producto |
| Usuarios | GET | `/api/usuarios` | Lista usuarios |
| Usuarios | GET | `/api/usuarios/{id}` | Consulta un usuario |
| Usuarios | POST | `/api/usuarios` | Crea un usuario |
| Usuarios | PUT | `/api/usuarios/{id}` | Actualiza un usuario |
| Usuarios | DELETE | `/api/usuarios/{id}` | Desactiva un usuario |

## Evidencias de prueba en Swagger

Ruta validada: `http://localhost:5147/swagger/index.html`

| Prueba | Metodo | Endpoint | Estado HTTP | Evidencia |
| --- | --- | --- | --- | --- |
| Swagger UI | GET | `/swagger/index.html` | 200 | Swagger carga correctamente |
| Listar categorias | GET | `/api/categorias` | 200 | Retorna categorias desde SQL Server |
| Crear categoria | POST | `/api/categorias` | 201 | Categoria creada con id `6` |
| Actualizar categoria | PUT | `/api/categorias/6` | 204 | Categoria actualizada correctamente |
| Eliminar categoria | DELETE | `/api/categorias/6` | 204 | Categoria desactivada correctamente |
| Listar productos | GET | `/api/productos` | 200 | Retorna productos desde SQL Server |
| Crear producto | POST | `/api/productos` | 201 | Producto creado con id `16` |
| Actualizar producto | PUT | `/api/productos/16` | 204 | Producto actualizado correctamente |
| Eliminar producto | DELETE | `/api/productos/16` | 204 | Producto desactivado correctamente |
| Listar usuarios | GET | `/api/usuarios` | 200 | Retorna usuarios desde SQL Server |
| Crear usuario | POST | `/api/usuarios` | 201 | Usuario creado con id `5` |
| Actualizar usuario | PUT | `/api/usuarios/5` | 204 | Usuario actualizado correctamente |
| Eliminar usuario | DELETE | `/api/usuarios/5` | 204 | Usuario desactivado correctamente |

## Ejemplos de payload usados en Swagger

Categoria:

```json
{
  "nombre": "Prueba Swagger",
  "descripcion": "Categoria creada desde Swagger"
}
```

Producto:

```json
{
  "nombre": "Producto Swagger",
  "descripcion": "Producto creado desde Swagger",
  "precio": 2.75,
  "stock": 8,
  "id_categoria": 1
}
```

Usuario:

```json
{
  "nombre": "Usuario Swagger",
  "email": "swagger@example.com",
  "passwordHash": "prueba123",
  "rol": "empleado"
}
```
