using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public class RepositorioTransacciones : IRepositorioTrasacciones
    {
        private readonly string _connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>("TransaccionesInsertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;


        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(_connectionString);

            var fecha1 = modelo.FechaFin;
            var fecha2 = modelo.FechaInicio;
            var fecha3 = modelo.UsuarioId;
            var fecha4 = modelo.CuentaId;

            var query = @"SELECT 
	                t.Id,
                    t.Monto,
	                t.FechaTransaccion,
	                c.Nombre AS Categoria,
	                ct.Nombre AS Cuenta,
	                c.TipoOperacionId
                FROM transacciones t
	                INNER JOIN Categorias c On c.Id  = t.CategoriaId
	                INNER JOIN Cuentas ct ON ct.Id = t.CuentasId
                WHERE t.CuentasId = @CuentaId AND t.UsuarioId = @UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                ";
            var test = await connection.QueryAsync<Transaccion>(query
                , modelo);
            return test;
            
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnteriorId,int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync("TransaccionesAtualizar",
                new 
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnteriorId,
                    cuentaAnteriorId,
                },commandType: System.Data.CommandType.StoredProcedure);

        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                   @"SELECT t.*,c.TipoOperacionId 
                        FROM transacciones t
                        INNER JOIN Categorias c ON c.Id = t.CategoriaId
                        WHERE t.Id = @Id AND t.UsuarioId = @UsuarioId
                    ",new {id,usuarioId});
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("TransaccionesBorrar",
                new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUausarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(_connectionString);

            var query = @"SELECT 
	                t.Id,
                    t.Monto,
	                t.FechaTransaccion,
	                c.Nombre AS Categoria,
	                ct.Nombre AS Cuenta,
	                c.TipoOperacionId
                FROM transacciones t
	                INNER JOIN Categorias c On c.Id  = t.CategoriaId
	                INNER JOIN Cuentas ct ON ct.Id = t.CuentasId
                WHERE t.UsuarioId = @UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                ORDER BY t.FechaTransaccion DESC
                ";
            var test = await connection.QueryAsync<Transaccion>(query
                , modelo);
            return test;

        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana( ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryAsync<ResultadoObtenerPorSemana>(
                @"
                    
                    SELECT 
	                    datediff(d,@fechaInicio,FechaTransaccion) / 7 + 1 AS Semana,
	                    SUM(Monto) AS Monto, c.TipoOperacionId
	
                    FROM transacciones t
                    INNER JOIN Categorias c ON c.Id = t.CategoriaId
                    WHERE t.UsuarioId = @UsuarioId AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                    GROUP BY datediff(d,@FechaInicio,FechaTransaccion) / 7 + 1,c.TipoOperacionId
                ",modelo);
        }

    }
}
