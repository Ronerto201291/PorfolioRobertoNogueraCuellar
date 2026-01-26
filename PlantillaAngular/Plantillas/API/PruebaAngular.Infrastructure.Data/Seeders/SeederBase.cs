using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace PruebaAngular.Infrastructure.Seeders
{
    public abstract class SeederBase<TData>
    {
        public List<TData> Data { get; set; }

        public SeederBase()
        {
            InitData();
        }

        private void InitData()
        {
            var seedName = typeof(TData).Name;
            String json = File.ReadAllText($"./Seed/{seedName}.json");
            if (!String.IsNullOrWhiteSpace(json))
                Data = JsonSerializer.Deserialize<List<TData>>(json);
        }

        public abstract void Execute(DbContext dbContext);
    }
}
