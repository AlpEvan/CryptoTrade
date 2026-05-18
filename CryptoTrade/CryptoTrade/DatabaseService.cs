using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CryptoTrade
{
    class MySQLDB
    {
        private string _connectionString;
        public MySQLDB(string server, uint port, string user, string password, string database)
        {
            _connectionString = $"server={server};database={database};user={user};password={password};port={port};";
        }

        public void ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null) foreach (var p in parameters) cmd.Parameters.AddWithValue(p.Key, p.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }

        public List<Dictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            var results = new List<Dictionary<string, object>>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null) foreach (var p in parameters) cmd.Parameters.AddWithValue(p.Key, p.Value);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++) row[reader.GetName(i)] = reader.GetValue(i);
                                results.Add(row);
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            return results;
        }
    }

    public class DatabaseService
    {
        private readonly MySQLDB _db;
        public DatabaseService(string password = "Super")
        {
            _db = new MySQLDB("127.0.0.1", 3306, "root", password, "cryptotrade");
        }

        public int? VerifyUser(string user, string pass)
        {
            var res = _db.ExecuteQuery("SELECT id FROM users WHERE username=@u AND password_hash=@p",
                new Dictionary<string, object> { { "@u", user }, { "@p", pass } });
            return res.Count > 0 ? Convert.ToInt32(res[0]["id"]) : (int?)null;
        }

        public bool CreateUser(string user, string pass)
        {
            try
            {
                _db.ExecuteNonQuery("INSERT INTO users (username, password_hash) VALUES (@u, @p)",
                    new Dictionary<string, object> { { "@u", user }, { "@p", pass } });
                return true;
            }
            catch { return false; }
        }

        public void InsertTrade(int uid, string type, string sym, decimal q, decimal p, decimal t)
        {
            _db.ExecuteNonQuery("INSERT INTO trades (user_id, executed_at, type, symbol, quantity, price, total_usdt) VALUES (@uid, NOW(), @t, @s, @q, @p, @tot)",
                new Dictionary<string, object> { { "@uid", uid }, { "@t", type }, { "@s", sym }, { "@q", q }, { "@p", p }, { "@tot", t } });
        }

        public (decimal walletUsdt, Dictionary<string, decimal> holdings) LoadLastPortfolio(int uid)
        {
            var res = _db.ExecuteQuery("SELECT id, wallet_usdt FROM portfolio WHERE user_id=@uid ORDER BY snapshot_at DESC LIMIT 1",
                new Dictionary<string, object> { { "@uid", uid } });

            if (res.Count == 0) return (10000m, new Dictionary<string, decimal>());

            decimal wallet = Convert.ToDecimal(res[0]["wallet_usdt"]);
            var holdings = new Dictionary<string, decimal>();
            var hRes = _db.ExecuteQuery("SELECT symbol, quantity FROM portfolio_holdings WHERE portfolio_id=@pid",
                new Dictionary<string, object> { { "@pid", res[0]["id"] } });

            foreach (var r in hRes) holdings[r["symbol"].ToString()] = Convert.ToDecimal(r["quantity"]);
            return (wallet, holdings);
        }

        public void SavePortfolioSnapshot(int uid, decimal wallet, decimal total, decimal gain, Dictionary<string, decimal> holdings)
        {
            _db.ExecuteNonQuery("INSERT INTO portfolio (user_id, snapshot_at, wallet_usdt, total_value, gain_pct) VALUES (@uid, NOW(), @w, @t, @g)",
                new Dictionary<string, object> { { "@uid", uid }, { "@w", wallet }, { "@t", total }, { "@g", gain } });

            var last = _db.ExecuteQuery("SELECT LAST_INSERT_ID() as id");
            long pid = Convert.ToInt64(last[0]["id"]);

            foreach (var h in holdings)
            {
                if (h.Value > 0)
                    _db.ExecuteNonQuery("INSERT INTO portfolio_holdings (portfolio_id, symbol, quantity, value_usdt) VALUES (@pid, @s, @q, 0)",
                        new Dictionary<string, object> { { "@pid", pid }, { "@s", h.Key }, { "@q", h.Value } });
            }
        }
    }
}