using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string _connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        
        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Categorias(Nombre,TipoOperacionId,UsuarioId)
                  VALUES(@Nombre,@TipoOperacionId,@UsuarioId)
                  SELECT SCOPE_IDENTITY();
                ",categoria);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = $@"SELECT * FROM Categorias WHERE UsuarioId = {id}";
            
            return await connection.QueryAsync<Categoria>(query);

        }
         public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var connection = new SqlConnection(_connectionString);
            //var query = $@"SELECT * FROM Categorias WHERE UsuarioId = {id} AND TipoOperacionId = {tipoOperacion}";
            //var query = $@"SELECT * FROM Categorias WHERE UsuarioId = @usuarioId AND TipoOperacionId = @tipoOperacionId",new { usuarioId, tipoOperacionId};
            
            return await connection.QueryAsync<Categoria>(
                @"SELECT * FROM Categorias 
                    WHERE UsuarioId = @usuarioId AND TipoOperacionId = @tipoOperacionId", 
                new { usuarioId, tipoOperacionId });

        }


        public async Task<Categoria> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(@"
                SELECT * FROM Categorias WHERE Id = @Id AND UsuarioId = @UsuarioId
            ",new { id,usuarioId});
        }
        
        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
                UPDATE Categorias 
                    SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionID 
                    WHERE Id = @ID", categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("DELETE Categorias Where Id = @Id", new { id });
        }
    }
}
