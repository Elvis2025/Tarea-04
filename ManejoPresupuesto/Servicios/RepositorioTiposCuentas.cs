using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {
        private readonly string _connectionStrings;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            _connectionStrings = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionStrings);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar", 
                                                            new { nombre = tipoCuenta.Nombre, usuarioId = tipoCuenta.UsuarioId},
                                                            commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;
        }

        public async Task<bool> existe(string nombre,int usuarioId)
        {
            using var connection = new SqlConnection(_connectionStrings);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                @"Select 1 from TiposCuentas
                where Nombre = @Nombre AND UsuarioId = @UsuarioId;", new {nombre,usuarioId});

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connect = new SqlConnection(_connectionStrings);
            return await connect.QueryAsync<TipoCuenta>(
                @"select Id,Nombre, Orden 
                    from TiposCuentas
                    where UsuarioId = @UsuarioId
                    Order by Orden",new {usuarioId});
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionStrings);
            await connection.ExecuteAsync(@"update TiposCuentas
                                            set Nombre = @Nombre
                                            where Id = @Id",tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId (int id, int usuarioId)
        {
            using var connection = new SqlConnection (_connectionStrings);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"
                                      select Id, Nombre, Orden
                                      from TiposCuentas
                                      where Id = @Id and UsuarioId = @UsuarioId",new {id,usuarioId});
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionStrings);
            await connection.ExecuteAsync("Delete TiposCuentas where Id = @Id", new {id});
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentaOrdenados)
        {
            var query = @"update TiposCuentas 
                        set Orden = @OrdenId where Id= @Id";
            using var connection = new SqlConnection(_connectionStrings);
            await connection.ExecuteAsync(query,tipoCuentaOrdenados);
        }
    }
}
