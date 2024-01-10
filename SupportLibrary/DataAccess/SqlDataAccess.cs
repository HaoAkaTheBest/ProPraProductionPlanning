using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary.DataAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;

        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = _config.GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                return rows.ToList();
            }
        }

        public async Task SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = _config.GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {

                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                //try
                //{
                //    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                //}
                //catch (SqlException ex)
                //{
                //    if (ex.Number == 2627 || ex.Number ==2601 ||ex.Number ==50000)
                //    {
                //        throw new Exception("Duplicate Entries");
                //    }
                //}  
            }
        }
    }
}
