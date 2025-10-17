
using API_Classes;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataWebAPI.Models
{
    public sealed class DataServer
    {
        private static readonly Lazy<DataServer> _instance = new(() => new DataServer());
        public static DataServer Instance => _instance.Value;

        public List<DataIntermed> Data { get; }

        private static readonly string[] FirstNames = { "John", "Jane", "Alice", "Bob", "Eve", "Charlie" };
        private static readonly string[] LastNames = { "Doe", "Smith", "Johnson", "Brown", "Taylor", "Adams" };
        private static readonly int[] Balances = { 900, 1500, 2750, 3100, 4800, 5200 };
        private static readonly Random RandomGen = new();

        private DataServer()
        {
            Data = SeedData(1000); 
        }

        private List<DataIntermed> SeedData(int count)
        {
      
            var images = new byte[6][];
            for (int i = 0; i < 6; i++)
            {
                string path = Path.Combine("Images", $"{i + 1}.jpg");
                images[i] = File.Exists(path) ? File.ReadAllBytes(path) : Array.Empty<byte>();
            }

            var list = new List<DataIntermed>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(new DataIntermed
                {
                    acct = (uint)(1000 + i), 
                    pin = (uint)(1000 + RandomGen.Next(9000)), 
                    fname = FirstNames[RandomGen.Next(FirstNames.Length)],
                    lname = LastNames[RandomGen.Next(LastNames.Length)],
                    bal = Balances[RandomGen.Next(Balances.Length)],
                    image = images[i % 6] 
                });
            }

            return list;
        }
    }
}
