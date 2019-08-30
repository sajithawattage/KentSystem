using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DAO
{
    public class DBConnection
    {
        private SqlConnection _connection = null;
        private SqlDataAdapter _adapter = null;

        #region Constant

        private const string KEY_SERVER = "Server";
        private const string KEY_DATABASE = "Database";
        private const string KEY_USERNAME = "UserName";
        private const string KEY_PASSWORD = "Password";
        private const string MASTER = "ConnectionString";

        #endregion

        public SqlConnection OpenConnection()
        {
            try
            {
                string connectionstring = Config.GetConnectionString(MASTER);

                _connection = new SqlConnection(connectionstring);
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
            }
            catch (Exception)
            {
                _connection.Dispose();
                throw;
            }
            return _connection;
        }

        private void CloseConnection()
        {
            _connection.Close();
            _connection.Dispose();
        }

        public SqlTransaction CreateTransaction(SqlConnection conn)
        {
            return conn.BeginTransaction();
        }

        public string CreateConnectionString(string database, string username, string password, string server)
        {
            var connectionbuilder = new StringBuilder();
            try
            {
                return "Server=" + server + ";Database=" + database + ";Uid=" + username + ";Pwd=" + password;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        public object ExecuteScalerQuery(string query)
        {
            var command = new SqlCommand();
            _adapter = new SqlDataAdapter();

            try
            {
                command.Connection = OpenConnection();
                command.CommandText = query;
                return command.ExecuteScalar();
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        public object ExecuteScalerQuery(string query, SqlParameter[] sqlParameter)
        {
            SqlCommand Command = new SqlCommand();
            object _objReturn = new object();
            _adapter = new SqlDataAdapter();

            try
            {
                Command.Connection = OpenConnection();
                Command.CommandText = query;
                Command.Parameters.AddRange(sqlParameter);
                _objReturn = Command.ExecuteScalar();

                return _objReturn;
            }
            catch (SqlException) { throw; }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable ExecuteSelectQuery(string query, SqlParameter[] sqlParameter)
        {
            SqlCommand Command = new SqlCommand();
            DataTable dataTable = new DataTable();
            dataTable = null;
            DataSet ds = new DataSet();
            _adapter = new SqlDataAdapter();

            try
            {
                Command.Connection = OpenConnection();
                Command.CommandText = query;
                Command.Parameters.AddRange(sqlParameter);
                Command.ExecuteNonQuery();
                _adapter.SelectCommand = Command;
                _adapter.Fill(ds);
                dataTable = ds.Tables[0];
                return dataTable;
            }
            catch (SqlException) { throw; }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable ExecuteSelectQuery(string query)
        {
            SqlCommand Command = new SqlCommand();
            DataTable dataTable = new DataTable();
            dataTable = null;
            DataSet ds = new DataSet();
            _adapter = new SqlDataAdapter();

            try
            {
                Command.Connection = OpenConnection();
                Command.CommandText = query;

                Command.ExecuteNonQuery();

                _adapter.SelectCommand = Command;
                _adapter.Fill(ds);
                dataTable = ds.Tables[0];
                return dataTable;
            }
            catch (SqlException) { throw; }
            finally
            {
                CloseConnection();
            }
        }

        public SqlDataReader ExecuteSelectReader(string query)
        {
            SqlCommand Command = new SqlCommand();
            try
            {
                Command.Connection = OpenConnection();
                Command.CommandText = query;
                return Command.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        public SqlDataReader ExecuteSelectReader(string query, SqlParameter[] param)
        {
            var command = new SqlCommand();
            try
            {
                command.Connection = OpenConnection();
                command.CommandText = query;
                command.Parameters.AddRange(param);
                return command.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool ExecuteInsertQuery(string query, SqlParameter[] sqlParameter)
        {
            return ExecuteNonQueryStatment(query, sqlParameter);
        }

        public bool ExecuteInsertQuery(string query, SqlParameter[] sqlParameter, SqlTransaction transaction)
        {
            return ExecuteNonQueryStatment(query, sqlParameter);
        }
        
        public bool ExecuteUpdateQuery(string query, SqlParameter[] sqlParameter)
        {
            return ExecuteNonQueryStatment(query, sqlParameter);
        }

        public bool ExecuteDeleteQuery(string query, SqlParameter[] sqlParameter)
        {
            return ExecuteNonQueryStatment(query, sqlParameter);
        }

        public SqlDataReader ExecuteOutputProcedure(string query)
        {
            SqlCommand command = new SqlCommand();
            try
            {
                command.Connection = OpenConnection();
                command.CommandText = query;
                command.CommandType = CommandType.StoredProcedure;
                return command.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        public SqlDataReader ExecuteOutputProcedure(string query, SqlParameter[] sqlParameter)
        {

            SqlCommand command = new SqlCommand();
            try
            {
                command.Connection = OpenConnection();
                command.CommandText = query;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(sqlParameter);
                command.Parameters["@outparam"].Direction = ParameterDirection.Output;
                return command.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable ExecuteProcedure(string query)
        {
            SqlDataAdapter adapter = null;
            DataTable result = new DataTable();

            try
            {
                adapter = new SqlDataAdapter(query, OpenConnection());
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                adapter.Fill(result);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public DataTable ExecuteProcedure(string query, SqlParameter[] sqlParameter)
        {
            DataTable result = new DataTable();
            SqlDataAdapter adapter = null;

            try
            {
                adapter = new SqlDataAdapter(query, OpenConnection());
                adapter.SelectCommand.Parameters.AddRange(sqlParameter);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.Fill(result);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataSet ExecuteDatasetProcedure(string query)
        {
            SqlDataAdapter adapter = null;
            DataSet result = new DataSet();

            try
            {
                adapter = new SqlDataAdapter(query, OpenConnection());
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                adapter.Fill(result);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataSet ExecuteDatasetProcedure(string query, SqlParameter[] sqlParameter)
        {
            DataSet result = new DataSet();
            SqlDataAdapter adapter = null;

            try
            {
                adapter = new SqlDataAdapter(query, OpenConnection());
                adapter.SelectCommand.Parameters.AddRange(sqlParameter);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.Fill(result);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object ExecuteScalarProcedure(string query)
        {

            SqlCommand command = null;
            try
            {

                command = new SqlCommand(query, OpenConnection());
                command.CommandType = CommandType.StoredProcedure;

                return command.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sqlParamter"></param>
        /// <returns></returns>
        public object ExecuteScalarProcedure(string query, SqlParameter[] sqlParamter)
        {
            SqlCommand command = null;

            try
            {
                command = new SqlCommand(query, OpenConnection());
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddRange(sqlParamter);

                return command.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryList"></param>
        /// <returns></returns>
        public bool ExecuteTransactionQuery(SqlConnection connection, SqlTransaction tansaction,
                                                    string query, SqlParameter[] param)
        {
            SqlCommand Command = new SqlCommand();
            _adapter = new SqlDataAdapter();
            try
            {
                Command.Connection = connection;
                Command.Transaction = tansaction;
                Command.CommandText = query;
                Command.Parameters.AddRange(param);
                _adapter.InsertCommand = Command;
                Command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExecuteTransactionProcedure(SqlConnection connection, SqlTransaction tansaction,
                                                    string query, SqlParameter[] param)
        {
            SqlCommand Command = new SqlCommand();
            _adapter = new SqlDataAdapter();
            try
            {
                Command.Connection = connection;
                Command.Transaction = tansaction;
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = query;
                Command.Parameters.AddRange(param);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool ExecuteNonQueryStatment(string query, SqlParameter[] param)
        {
            SqlCommand Command = new SqlCommand();
            _adapter = new SqlDataAdapter();
            try
            {
                Command.Connection = OpenConnection();
                Command.CommandText = query;
                Command.Parameters.AddRange(param);
                _adapter.InsertCommand = Command;
                Command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

    }
}
