using System;
using System.Collections.Generic;

namespace FinalProject.Models
{
    public class CategoriesResponse
    {
        public List<Drink> drinks { get; set; }
        public CategoriesResponse()
        {
        }
    }

    public class Drink
    {
        public string strCategory { get; set; }
    }
}
