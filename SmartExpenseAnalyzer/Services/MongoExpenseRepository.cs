// File: Services/MongoExpenseRepository.cs
using MongoDB.Driver;
using SmartExpenseAnalyzer.Models;
using System;
using System.Collections.Generic;

namespace SmartExpenseAnalyzer.Services
{
    public class MongoExpenseRepository
    {
        private readonly IMongoCollection<Expense> _collection;

        public MongoExpenseRepository(
            string connectionString = "mongodb://localhost:27017",
            string databaseName = "SmartExpenseDB",
            string collectionName = "Expenses")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Expense>(collectionName);
        }

        /// <summary>Returns all expenses from MongoDB.</summary>
        public List<Expense> GetAll()
            => _collection.Find(_ => true).ToList();

        /// <summary>Inserts a new expense document.</summary>
        public void Add(Expense expense)
        {
            Console.WriteLine("add method called");
            if (expense == null) throw new ArgumentNullException(nameof(expense));
            _collection.InsertOne(expense);
        }

        /// <summary>Removes an expense by its GUID Id.</summary>
        public bool Remove(Guid id)
        {
            var filter = Builders<Expense>.Filter.Eq(e => e.Id, id);
            var result = _collection.DeleteOne(filter);
            return result.DeletedCount > 0;
        }

        /// <summary>Replaces an existing expense document.</summary>
        public bool Update(Expense expense)
        {
            var filter = Builders<Expense>.Filter.Eq(e => e.Id, expense.Id);
            var result = _collection.ReplaceOne(filter, expense);
            return result.ModifiedCount > 0;
        }
    }
}
