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
                INSERT INTO History (Url, Title, VisitCount, LastVisitedUtc)
                VALUES ($url, $title, 1, $visitedAt)
                ON CONFLICT(Url) DO UPDATE SET
                    Title = excluded.Title,
                    VisitCount = History.VisitCount + 1,
                    LastVisitedUtc = excluded.LastVisitedUtc;
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
                SELECT Id, Url, Title, VisitCount, LastVisitedUtc
                FROM History
                ORDER BY LastVisitedUtc DESC
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
            await using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM History;";
            await command.ExecuteNonQueryAsync();
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

    CREATE INDEX IF NOT EXISTS IX_History_LastVisitedUtc
        ON History (LastVisitedUtc DESC);
    """;
                await command.ExecuteNonQueryAsync();
                _isInitialized = true;
            }
            finally
            {
                _initializationLock.Release();
            }
        }
    }
}
