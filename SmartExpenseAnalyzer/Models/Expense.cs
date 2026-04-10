using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartExpenseAnalyzer.Models
{
    public class Expense
    {
        /// <summary>Unique identifier for the expense.</summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]   // stores Guid as string in Mongo
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Amount spent (in currency units).</summary>
        public double Amount { get; set; }

        /// <summary>Category of the expense (e.g., Food, Travel, Shopping, Bills).</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Date when the expense occurred.</summary>
        public DateTime Date { get; set; }

        /// <summary>Optional note or description for the expense.</summary>
        public string Note { get; set; } = string.Empty;
    }
}
