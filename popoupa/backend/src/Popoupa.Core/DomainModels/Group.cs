using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SurrealDb.Net.Models;

namespace Popoupa.Core.DomainModels
{
    public class Group
    {
        public Group(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
