using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordChainGame.Data
{
    public class Class1
    {
        public Class1()
        {
        }

        public static void Main()
        {
            var client = new MongoClient("mongodb://localhost:27017");

            var db = client.GetDatabase("Test");



            
            Console.WriteLine("there");
        }
    }
}
