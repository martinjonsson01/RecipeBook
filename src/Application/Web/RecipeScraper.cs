using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

using HtmlAgilityPack;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Core.Domain.Units;

namespace RecipeBook.Core.Application.Web
{
    public class RecipeScraper : IRecipeScraper
    {
        private readonly HttpClient _http;

        public RecipeScraper(HttpClient http)
        {
            _http = http;
        }

        public async Task<Recipe?> Scrape(string fromUrl)
        {
            string html = await _http.GetStringAsync(fromUrl);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            try
            {
                HtmlNode? titleNode = doc.DocumentNode
                                         .Descendants("h1")
                                         .FirstOrDefault();
                string title = titleNode?.InnerText ?? "Ingen titel hittad";
                title = HttpUtility.HtmlDecode(title);

                HtmlNode? ratingNode = doc.DocumentNode
                                          .Descendants("div")
                                          .FirstOrDefault(node => node.HasClass("rating-wrapper"));
                string? ratingText = ratingNode?.GetAttributeValue("title", null);
                double.TryParse(ratingText, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleRating);
                var rating = (int) Math.Round(doubleRating * 2, 0);

                HtmlNode? imageNode = doc.DocumentNode
                                         .Descendants("img")
                                         .FirstOrDefault();
                string? imagePath = imageNode?.GetAttributeValue("src", null);

                var stepNumber = 1;
                List<Step> steps = doc.DocumentNode
                                      .Descendants("div")
                                      .Where(node => node.HasClass("cooking-steps-card"))
                                      .Select(node => new Step { Number = stepNumber++, Instruction =
                                          HttpUtility.HtmlDecode(node.InnerText.Trim())
                                      })
                                      .ToList();

                List<HtmlNode> ingredientNodes = doc.DocumentNode
                                                    .Descendants("div")
                                                    .Where(node => node.HasClass("ingredients-list-group__card"))
                                                    .Where(node => !node.HasClass("extra-content"))
                                                    .ToList();
                List<Ingredient> ingredients = ingredientNodes
                                               .Select(node => node.LastChild)
                                               .Select(node => new Ingredient
                                                   { Name = HttpUtility.HtmlDecode(node.InnerText.Replace("\n", "").Trim()) })
                                               .ToList();
                List<Unit> amounts = ingredientNodes
                                     .Select(node => node.FirstChild)
                                     .Select(UnitFromNode)
                                     .ToList();
                for (var i = 0; i < ingredients.Count; i++)
                {
                    ingredients[i].Amount = amounts[i];
                }
                
                return new Recipe
                {
                    Name = title,
                    Rating = rating,
                    ImagePath = imagePath,
                    Steps = steps,
                    Ingredients = ingredients
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Unit UnitFromNode(HtmlNode node)
        {
            string textValue = node.InnerText.Replace("\n", "").Trim();
            if (Unit.TryParseString(textValue, out Unit result, out string? errorMessage))
            {
                return result;
            }
            Console.WriteLine(errorMessage);
            return new Mass(0);
        }
    }
}