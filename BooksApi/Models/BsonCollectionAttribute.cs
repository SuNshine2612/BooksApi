﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BsonCollectionAttribute : Attribute
    {
        public string CollectionName { get;}
        public BsonCollectionAttribute(string name)
        {
            CollectionName = name;
        }
    }
}
