using System;
using System.Collections.Generic;

namespace FinalProject.Models
{
    public class DrinkModel: BaseDrinkModel
    {
        public List<BaseDrinkModel> drinks { get; set; }
        public DrinkModel()
        {
        }
    }
}
