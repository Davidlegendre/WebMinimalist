using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebBrowserMinimalist.Models;

namespace WebBrowserMinimalist.DBA
{
    /// <summary>Persistent application-owned browser history.</summary>
    public sealed class HIstorialDBA
    {
        private readonly string _connectionString;
        private readonly SemaphoreSlim _initializationLock = new(1, 1);
        private bool _isInitialized;

        public HIstorialDBA()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WebBrowserMinimalist");
            Directory.CreateDirectory(folder);
            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(folder, "history.db"),
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();
        }

        public async Task RecordVisitAsync(string url, string title)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return;

            await InitializeAsync();
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO HistoryVisits (Url, Title, VisitCount, LastVisitedUtc)
                VALUES ($url, $title, 1, $visitedAt);
                """;
            command.Parameters.AddWithValue("$url", uri.AbsoluteUri);
            command.Parameters.AddWithValue("$title", string.IsNullOrWhiteSpace(title) ? uri.Host : title.Trim());
            command.Parameters.AddWithValue("$visitedAt", DateTime.UtcNow.ToString("O"));
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<HistoryModel>> GetRecentAsync(int limit = 500)
        {
            await InitializeAsync();
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT
                    (SELECT latest.Id
                     FROM HistoryVisits AS latest
                     WHERE latest.Url = visits.Url
                     ORDER BY julianday(latest.LastVisitedUtc) DESC, latest.Id DESC
                     LIMIT 1),
                    visits.Url,
                    (SELECT latest.Title
                     FROM HistoryVisits AS latest
                     WHERE latest.Url = visits.Url
                     ORDER BY julianday(latest.LastVisitedUtc) DESC, latest.Id DESC
                     LIMIT 1),
                    SUM(visits.VisitCount),
                    (SELECT latest.LastVisitedUtc
                     FROM HistoryVisits AS latest
                     WHERE latest.Url = visits.Url
                     ORDER BY julianday(latest.LastVisitedUtc) DESC, latest.Id DESC
                     LIMIT 1)
                FROM HistoryVisits AS visits
                GROUP BY visits.Url
                ORDER BY julianday(
                    (SELECT latest.LastVisitedUtc
                     FROM HistoryVisits AS latest
                     WHERE latest.Url = visits.Url
                     ORDER BY julianday(latest.LastVisitedUtc) DESC, latest.Id DESC
                     LIMIT 1)
                ) DESC
                LIMIT $limit;
                """;
            command.Parameters.AddWithValue("$limit", limit);

            var history = new List<HistoryModel>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                history.Add(new HistoryModel
                {
                    id = reader.GetInt64(0),
                    url = reader.GetString(1),
                    title = reader.GetString(2),
                    visitCount = reader.GetInt64(3),
                    lastVisitedUtc = DateTime.Parse(reader.GetString(4), null, System.Globalization.DateTimeStyles.RoundtripKind)
                });
            }
            return history;
        }

        public async Task DeleteAllAsync()
        {
            await InitializeAsync();
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();
            await using var command = connection.CreateCommand();
            command.Transaction = (SqliteTransaction)transaction;
            command.CommandText = "DELETE FROM HistoryVisits; DELETE FROM History;";
            await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }

        public async Task DeleteBetweenAsync(DateTime startUtc, DateTime endUtc)
        {
            if (startUtc >= endUtc)
                throw new ArgumentException("The start time must be earlier than the end time.", nameof(startUtc));

            await InitializeAsync();
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();
            await using var command = connection.CreateCommand();
            command.Transaction = (SqliteTransaction)transaction;
            command.CommandText = """
                DELETE FROM HistoryVisits
                WHERE julianday(LastVisitedUtc) >= julianday($start)
                  AND julianday(LastVisitedUtc) < julianday($end);
                DELETE FROM History
                WHERE julianday(LastVisitedUtc) >= julianday($start)
                  AND julianday(LastVisitedUtc) < julianday($end);
                """;
            command.Parameters.AddWithValue("$start", startUtc.ToUniversalTime().ToString("O"));
            command.Parameters.AddWithValue("$end", endUtc.ToUniversalTime().ToString("O"));
            await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }

        private async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            await _initializationLock.WaitAsync();
            try
            {
                if (_isInitialized)
                    return;

                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = """
                    CREATE TABLE IF NOT EXISTS History (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Url TEXT NOT NULL UNIQUE,
                        Title TEXT NOT NULL,
                        VisitCount INTEGER NOT NULL DEFAULT 1,
                        LastVisitedUtc TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS HistoryVisits (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Url TEXT NOT NULL,
                        Title TEXT NOT NULL,
                        VisitCount INTEGER NOT NULL DEFAULT 1,
                        LastVisitedUtc TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS HistoryMetadata (
                        Key TEXT PRIMARY KEY,
                        Value TEXT NOT NULL
                    );

                    CREATE INDEX IF NOT EXISTS IX_HistoryVisits_LastVisitedUtc
                        ON HistoryVisits (LastVisitedUtc DESC);

                    CREATE INDEX IF NOT EXISTS IX_HistoryVisits_Url
                        ON HistoryVisits (Url);
                    """;
                await command.ExecuteNonQueryAsync();

                await using var migrationTransaction = await connection.BeginTransactionAsync();
                await using var migrationCommand = connection.CreateCommand();
                migrationCommand.Transaction = (SqliteTransaction)migrationTransaction;
                migrationCommand.CommandText = """
                    INSERT INTO HistoryVisits (Url, Title, VisitCount, LastVisitedUtc)
                    SELECT Url, Title, VisitCount, LastVisitedUtc
                    FROM History
                    WHERE NOT EXISTS (
                        SELECT 1 FROM HistoryMetadata WHERE Key = 'LegacyHistoryMigrated'
                    );

                    INSERT OR IGNORE INTO HistoryMetadata (Key, Value)
                    VALUES ('LegacyHistoryMigrated', '1');
                    """;
                await migrationCommand.ExecuteNonQueryAsync();
                await migrationTransaction.CommitAsync();
                _isInitialized = true;
            }
            finally
            {
                _initializationLock.Release();
            }
        }
    }
}
