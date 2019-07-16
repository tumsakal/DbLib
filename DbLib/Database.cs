using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace DbLib
{
    public class Database
    {
        private DbProviderFactory factory { get; set; }
        private DbConnection connection { get; set; }
        private Database()
        {

        }
        public static Database Of(string provider, string connectionString)
        {
            var db = new Database { factory = DbProviderFactories.GetFactory(provider) };
            db.connection = db.factory.CreateConnection();
            db.connection.ConnectionString = connectionString;
            return db;
        }
        public void OpenConnnection()
        {
            if (this.connection.State != System.Data.ConnectionState.Open)
                this.connection.Open();
        }
        public void CloseConnection()
        {
            this.connection.Close();
        }
        private DbParameter[] CreateParameter(string[] fields, object[] values)
        {
            if (fields.Length != values.Length) throw new Exception("Field and Values is not match.");
            DbParameter[] result = new DbParameter[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                var p = factory.CreateParameter();
                p.ParameterName = $"@{fields[i]}";
                p.Value = values[i];
                result[i] = p;
            }
            return result;
        }
        public T ExecuteScalar<T>(string sql, params DbParameter[] parameters)
        {
            var command = this.factory.CreateCommand();
            command.Connection = this.connection;
            command.CommandText = sql;
            command.Parameters.Clear();
            if (parameters.Length > 0)
                command.Parameters.AddRange(parameters);
            this.OpenConnnection();
            object result = command.ExecuteScalar();
            return (T)result;
        }
        public int ExecuteNoneQuery(string sql, params DbParameter[] parameters)
        {
            var cmd = this.factory.CreateCommand();
            cmd.Connection = this.connection;
            cmd.CommandText = sql;
            if (parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);
            this.OpenConnnection();
            return cmd.ExecuteNonQuery();
        }
        public IEnumerable<T> Query<T>(string sql, Func<IDataReader, T> adapter, params DbParameter[] parameters)
        {
            var command = this.factory.CreateCommand();
            command.Connection = this.connection;
            command.CommandText = sql;
            if (parameters.Length > 0)
                command.Parameters.AddRange(parameters);
            this.OpenConnnection();
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    yield return adapter(reader);
                }
            }
            reader.Close();
            yield break;
        }
        public int Insert(string table, string[] fields, object[] values)
        {
            string sql = $"INSERT INTO {table}({string.Join(",", fields.Select(f => $"[{f}]"))}) VALUES({string.Join(",", fields.Select(f => $"@{f}"))});";
            return ExecuteNoneQuery(sql, CreateParameter(fields, values));
        }
        public int Delete(string table, string key, object value)
        {
            string sql = $"DELETE FROM {table} WHERE [{key}] = @{key};";
            var param = this.factory.CreateParameter();
            param.ParameterName = $"@{key}";
            param.Value = value;
            return this.ExecuteNoneQuery(sql, param);
        }
        public int Update(string table, string[] fields, object[] values)
        {
            string sql = $"UPDATE {table} SET {string.Join(", ", fields.Skip(1).Select(f => $"[{f}]=@{f}"))} WHERE [{fields[0]}] = @{fields[0]};";
            return this.ExecuteNoneQuery(sql, this.CreateParameter(fields, values));
        }
    }
}