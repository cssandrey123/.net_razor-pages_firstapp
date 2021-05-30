using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;


namespace FinalProject.Pages.Drinks
{
    public class ViewDrinksModel : PageModel
    {
        [BindProperty]
        public List <BaseDrinkModel> filteredDrinks { get; set; }


        public List<string> availableCategories { get; set; }
        HttpClient client = new HttpClient();


        [BindProperty]
        public string filter { get; set; }
        [BindProperty]
        public string selectedCategory { get; set; }

        private readonly IMemoryCache _cache;

        public ViewDrinksModel(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnGet()
        {
            this.filteredDrinks = this.getInitialDrinks(15);
            this.availableCategories = this.getAllCategories(this.client);
        }

        public void OnPost()
        {
            this.filter = Request.Form["drinkName"];
            this.filteredDrinks = new List<BaseDrinkModel>();
            this.availableCategories = this.getAllCategories(this.client);
            this.selectedCategory = Request.Form["categories"];

            if (this.filter != "")
            {
                BaseDrinkModel matchedDrink = this.getSingleDrink(this.filter);
                if (matchedDrink != null)
                {
                    this.filteredDrinks.Add(matchedDrink);
                }
            } else
            {
                this.filteredDrinks = this.getDrinksByCategory();
            }

              
        }

        public List<BaseDrinkModel> getDrinks(int limit, string url)
        {
            List<BaseDrinkModel> tempDrinks = new List<BaseDrinkModel>();
            int index = 0;

            HttpClient client = new HttpClient();

            var resultFromCache = checkForCache(url);
            var json = "";

            if (resultFromCache == null || resultFromCache == "")
            {
                var response = client.GetAsync(url).Result;
                json = response.Content.ReadAsStringAsync().Result;
                this.saveInCache(url, json);
            }
            else
            {
                json = resultFromCache;
            }


            var allDrinks = JsonSerializer.Deserialize<DrinksResponse>(json);

            foreach (BaseDrinkModel drink in allDrinks.drinks)
            {
                if (index == limit)
                {
                    break;
                }
                tempDrinks.Add(new BaseDrinkModel
                {
                    idDrink = drink.idDrink,
                    strDrink = drink.strDrink,
                    strDrinkThumb = drink.strDrinkThumb
                });
                index++;
            }

            return tempDrinks;
        }

        public List<BaseDrinkModel> getInitialDrinks(int limit)
        {
            var url = "http://www.thecocktaildb.com/api/json/v1/1/filter.php?a=Alcoholic";
            return this.getDrinks(limit, url);
        }

        public BaseDrinkModel getSingleDrink(string drinkName)
        {
            var url = "http://www.thecocktaildb.com/api/json/v1/1/search.php?s=" + drinkName.ToLower();
            HttpClient client = new HttpClient();
            var resultFromCache = checkForCache(url);
            var json = "";

            if (resultFromCache == null || resultFromCache == "")
            {
                var response = client.GetAsync(url).Result;
                json = response.Content.ReadAsStringAsync().Result;
                this.saveInCache(url, json);
            }
            else
            {
                json = resultFromCache;
            }

            var matchedDrink = JsonSerializer.Deserialize<DrinksResponse>(json);

            if(matchedDrink.drinks == null)
            {
                return null;
            } else
            {
                return matchedDrink.drinks[0];
            };
        }

        public List<BaseDrinkModel> getDrinksByCategory()
        {
            var url = "";
            if(this.selectedCategory == "All")
            {
                url = "http://www.thecocktaildb.com/api/json/v1/1/filter.php?a=Alcoholic";
            }
            else
            {
                url = "http://www.thecocktaildb.com/api/json/v1/1/filter.php?c=" + this.selectedCategory;
            }
            return this.getDrinks(15, url);

        }

        private List<string> getAllCategories(HttpClient client)
        {
            List<string> allCategories = new List<string>();
            var url = "http://www.thecocktaildb.com/api/json/v1/1/list.php?c=list";

            var resultFromCache = checkForCache(url);
            var json = "";

            if(resultFromCache == null || resultFromCache == "")
            {
                var response = client.GetAsync(url).Result;
                json = response.Content.ReadAsStringAsync().Result;
                this.saveInCache(url, json);
            } else
            {
                json = resultFromCache;
            }

            
            CategoriesResponse myDeserializedClass = Newtonsoft.Json.JsonConvert.DeserializeObject<CategoriesResponse>(json);

            allCategories.Add("All");
            this.selectedCategory = "All";

            foreach (var category in myDeserializedClass.drinks)
            {

                allCategories.Add(category.strCategory);
        
            }

            return allCategories;
        }

        private dynamic checkForCache(string url)
        {
            dynamic result;
            if (_cache.TryGetValue(url, out result))
            {
                Console.WriteLine("result coming from cache: ");
                Console.WriteLine(result);
                return result;
            } else
            {
                Console.WriteLine("cache returning null ");
                return null;
            }
        }

        private void saveInCache(string url,  string value)
        {
            Console.WriteLine("saving result in cache: ");
            Console.WriteLine(value);

            _cache.Set(url, value);
        }
     }
}