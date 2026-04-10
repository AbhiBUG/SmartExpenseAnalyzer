// File: Services/ExpenseManager.cs
// Handles loading, saving, and managing the list of expenses.
// Primary persistence: MongoDB. Fallback: JSON file.

using Newtonsoft.Json;
using SmartExpenseAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SmartExpenseAnalyzer.Services
{
    /// <summary>
    /// Manages expense data: add, retrieve, remove.
    /// Uses MongoDB as the primary store and JSON as a local fallback.
    /// </summary>
    public class ExpenseManager
    {
        // ── MongoDB ───────────────────────────────────────────────────────────
        private readonly MongoExpenseRepository _mongo;
        private readonly bool _mongoAvailable;

        // ── JSON Fallback ─────────────────────────────────────────────────────
        private readonly string _filePath;

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore
        };

        // ── In-memory cache ───────────────────────────────────────────────────
        private List<Expense> _expenses;

        // ── Constructor ───────────────────────────────────────────────────────
        /// <summary>
        /// Initialises the manager.
        /// Tries to connect to MongoDB first; falls back to JSON if unavailable.
        /// </summary>
        /// <param name="connectionString">MongoDB connection string.</param>
        /// <param name="filePath">JSON fallback file path.</param>
        public ExpenseManager(
            string connectionString = "mongodb://localhost:27017",
            string filePath = "Data/expenses.json")
        {
            _filePath = filePath;
            _expenses = new List<Expense>();

            // ── Try MongoDB ───────────────────────────────────────────────────
            try
            {
                _mongo = new MongoExpenseRepository(connectionString);
                _expenses = _mongo.GetAll();
                _mongoAvailable = true;
                Debug.WriteLine("✅ MongoDB connected. Loaded expenses from MongoDB.");
            }
            catch (Exception ex)
            {
                _mongoAvailable = false;
                Debug.WriteLine($"⚠️ MongoDB unavailable: {ex.Message}. Falling back to JSON.");

                // ── Fallback: JSON ────────────────────────────────────────────
                EnsureDataDirectory();
                LoadFromJson();
            }
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Returns a read-only snapshot of all expenses.</summary>
        public IReadOnlyList<Expense> GetAll() => _expenses.AsReadOnly();

        /// <summary>
        /// Adds a new expense and persists it (MongoDB or JSON).
        /// </summary>
        public void Add(Expense expense)
        {
            if (expense == null) throw new ArgumentNullException(nameof(expense));

            if (_mongoAvailable)
            {
                _mongo.Add(expense);        // persist to MongoDB
            }

            _expenses.Add(expense);         // update in-memory cache

            if (!_mongoAvailable)
            {
                SaveToJson();               // fallback: persist to JSON
            }
        }

        /// <summary>
        /// Removes an expense by its unique GUID and persists the change.
        /// </summary>
        /// <returns>True if removed; false if not found.</returns>
        public bool Remove(Guid id)
        {
            bool removed = false;

            if (_mongoAvailable)
            {
                removed = _mongo.Remove(id);   // remove from MongoDB
                if (removed)
                    _expenses.RemoveAll(e => e.Id == id);
            }
            else
            {
                int count = _expenses.RemoveAll(e => e.Id == id);
                removed = count > 0;
                if (removed) SaveToJson();     // persist to JSON fallback
            }

            return removed;
        }

        // ── JSON Fallback Helpers ─────────────────────────────────────────────

        private void EnsureDataDirectory()
        {
            string dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private void SaveToJson()
        {
            string json = JsonConvert.SerializeObject(_expenses, _jsonSettings);
            File.WriteAllText(_filePath, json);
        }

        private void LoadFromJson()
        {
            if (!File.Exists(_filePath))
                return;

            try
            {
                string json = File.ReadAllText(_filePath);
                _expenses = JsonConvert.DeserializeObject<List<Expense>>(json, _jsonSettings)
                              ?? new List<Expense>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Failed to load JSON: {ex.Message}");
                _expenses = new List<Expense>();
            }
        }
    }
}