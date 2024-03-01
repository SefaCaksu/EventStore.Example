﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shared.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order = 0)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order = 0)]
        public string ProductName { get; set; }
        [BsonRepresentation(BsonType.Int64)]
        [BsonElement(Order = 0)]
        public int Count { get; set; }
        [BsonRepresentation(BsonType.Boolean)]
        [BsonElement(Order = 0)]
        public bool IsAvilable { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        [BsonElement(Order = 0)]
        public decimal Price { get; set; }
    }
}
