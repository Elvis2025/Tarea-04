using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string _connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                insert into Cuentas (Nombre,TipoCuentaId,Balance,descripcion)
                values (@Nombre, @TipoCuentaId, @Balance, @Descripcion)
                select SCOPE_IDENTITY();", cuenta);
            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connetion = new SqlConnection(_connectionString);
            return await connetion.QueryAsync<Cuenta>(@"
                select Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta
                from Cuentas
                inner join TiposCuentas tc
                on tc.Id = Cuentas.TipoCuentaId
                where tc.UsuarioId = @UsuarioId
                Order by tc.Orden", new { usuarioId });


        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            var test = await connection.QueryFirstOrDefaultAsync<Cuenta>(
                @"
                    SELECT  
	                    O.Id, 
	                    O.Nombre ,
	                    Balance,
                        O.Descripcion,
                        TipoCuentaId
                    From Cuentas O 
                    Inner Join TiposCuentas tc on tc.Id = O.TipoCuentaId
                    where tc.UsuarioId = @UsuarioId and O.Id = @Id
                "
                , new { id, usuarioId });
            return test;
        }


        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"                
                UPDATE Cuentas 
	                SET 
	                Nombre = @Nombre, 
	                Balance =@Balance, 
	                descripcion = @Descripcion,
	                TipoCuentaId = @TipoCuentaId
                where Id = @Id;                      
            ",cuenta);
                
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var query  = $@"                
                DELETE Cuentas Where Id = {id}                     
            ";

            await connection.ExecuteAsync(query);

            
        }

/*

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connetion = new SqlConnection(_connectionString);
            return await connetion.QueryFirstOrDefaultAsync<Cuenta>(@"
                SELECT 
                    O.Id,
                    O.Nombre,
                    O.Balance,
                    O.Descripción,
                    Tc.Id
                From Cuentas
                INNER JOIN TiposCuentas Tc
                ON Tc.Id = O.TipoCuentaId
                WHERE Tc.TiposCuentasId = @TiposCuentasId AND O.Id = @Id
                ", new { id, usuarioId });
        }   */
    } 
}
