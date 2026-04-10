// File: Services/ExpenseManager.cs
// Handles loading, saving, and managing the list of expenses via JSON persistence.
// Uses Newtonsoft.Json (Json.NET) for serialization.

using Newtonsoft.Json;                // ← Newtonsoft.Json
using SmartExpenseAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SmartExpenseAnalyzer.Services
{
    /// <summary>
    /// Manages expense data: add, retrieve, persist (load/save) to JSON.
    /// </summary>
    public class ExpenseManager
    {
        // ── Configuration ────────────────────────────────────────────────────
        // Path to the JSON data file. Stored in the application directory.
        private readonly string _filePath;

        // In-memory list of all expenses.
        private List<Expense> _expenses;

        // ── JSON settings ──────────────────────────────────────────────────────
        // Newtonsoft.Json equivalent of JsonSerializerOptions.
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,  // Pretty-print JSON for readability
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore
        };

        // ── Constructor ───────────────────────────────────────────────────────
        /// <summary>
        /// Initialises the manager with an optional custom file path.
        /// Automatically loads existing expenses from disk.
        /// </summary>
        /// <param name="filePath">Path to expenses.json (defaults to Data/expenses.json).</param>
        public ExpenseManager(string filePath = "Data/expenses.json")
        {
            _filePath = filePath;
            _expenses = new List<Expense>();

            // Ensure the Data directory exists so saving never fails.
            string? directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            Load(); // Load saved expenses on startup.
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Returns a read-only snapshot of all expenses.</summary>
        public IReadOnlyList<Expense> GetAll() => _expenses.AsReadOnly();

        /// <summary>
        /// Adds a new expense to the in-memory list and persists immediately.
        /// </summary>
        /// <param name="expense">The expense to add.</param>
        public void Add(Expense expense)
        {
            if (expense == null) throw new ArgumentNullException(nameof(expense));
            _expenses.Add(expense);
            Save(); // Auto-save after every addition.
        }

        /// <summary>
        /// Removes an expense by its unique ID and persists the change.
        /// </summary>
        /// <param name="id">The GUID of the expense to remove.</param>
        /// <returns>True if an expense was removed; false if not found.</returns>
        public bool Remove(Guid id)
        {
            int removed = _expenses.RemoveAll(e => e.Id == id);
            if (removed > 0) Save();
            return removed > 0;
        }

        // ── Persistence ───────────────────────────────────────────────────────

        /// <summary>Serialises the current list to JSON and writes it to disk.</summary>
        public void Save()
        {
            // JsonConvert.SerializeObject replaces JsonSerializer.Serialize
            string json = JsonConvert.SerializeObject(_expenses, _jsonSettings);
            File.WriteAllText(_filePath, json);
        }

        /// <summary>
        /// Reads the JSON file from disk and populates the in-memory list.
        /// If the file doesn't exist or is corrupt, starts with an empty list.
        /// </summary>
        public void Load()
        {
            if (!File.Exists(_filePath))
                return; // No data yet — start fresh.

            try
            {
                string json = File.ReadAllText(_filePath);

                // JsonConvert.DeserializeObject replaces JsonSerializer.Deserialize
                _expenses = JsonConvert.DeserializeObject<List<Expense>>(json, _jsonSettings)
                            ?? new List<Expense>();
            }
            catch (Exception ex)
            {
                // If deserialization fails (e.g., corrupt file), reset gracefully.
                System.Diagnostics.Debug.WriteLine($"Failed to load expenses: {ex.Message}");
                _expenses = new List<Expense>();
            }
        }
    }
}