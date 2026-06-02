using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class RecipeTests : TestBase
{
    [Fact]
    public async Task CanCreateRecipe()
    {
        // Arrange
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Kilogram", Abbreviation = "KG", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Burger", Category = category, Unit = unit };
        
        var recipe = new Recipe
        {
            Product = product,
            Name = "Classic Burger",
            IsActive = true
        };

        // Act
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.Recipes.Add(recipe);
        await Context.SaveChangesAsync();

        // Assert
        var savedRecipe = await Context.Recipes
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Product.Code == "PROD001");
        
        Assert.NotNull(savedRecipe);
        Assert.Equal("Burger", savedRecipe.Product.Name);
        Assert.True(savedRecipe.IsActive);
    }

    [Fact]
    public async Task CanUpdateRecipe()
    {
        // Arrange
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Kilogram", Abbreviation = "KG", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Burger", Category = category, Unit = unit };
        var recipe = new Recipe { Product = product, Name = "Old Recipe" };
        
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.Recipes.Add(recipe);
        await Context.SaveChangesAsync();

        // Act
        recipe.Name = "New Recipe";
        recipe.IsActive = false;
        await Context.SaveChangesAsync();

        // Assert
        var updatedRecipe = await Context.Recipes.FirstOrDefaultAsync(r => r.Product.Code == "PROD001");
        Assert.Equal("New Recipe", updatedRecipe?.Name);
        Assert.False(updatedRecipe?.IsActive);
    }

    [Fact]
    public async Task CanDeleteRecipe_SoftDelete()
    {
        // Arrange
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Kilogram", Abbreviation = "KG", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Burger", Category = category, Unit = unit };
        var recipe = new Recipe { Product = product };
        
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.Recipes.Add(recipe);
        await Context.SaveChangesAsync();

        // Act
        recipe.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedRecipe = await Context.Recipes.FirstOrDefaultAsync(r => r.Product.Code == "PROD001");
        Assert.Null(deletedRecipe);
    }

    [Fact]
    public async Task CanGetRecipeWithIngredients()
    {
        // Arrange
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Kilogram", Abbreviation = "KG", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Burger", Category = category, Unit = unit };
        var ingredientProduct = new Product { Code = "PROD002", Name = "Patty", Category = category, Unit = unit };
        
        var recipe = new Recipe { Product = product };
        var recipeIngredient = new RecipeIngredient
        {
            Recipe = recipe,
            IngredientProduct = ingredientProduct,
            Unit = unit,
            BaseUnit = unit,
            Quantity = 0.150m,
            BaseQuantity = 0.150m
        };

        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.Products.Add(ingredientProduct);
        Context.Recipes.Add(recipe);
        Context.RecipeIngredients.Add(recipeIngredient);
        await Context.SaveChangesAsync();

        // Act
        var recipeWithIngredients = await Context.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.IngredientProduct)
            .FirstOrDefaultAsync(r => r.Product.Code == "PROD001");

        // Assert
        Assert.NotNull(recipeWithIngredients);
        Assert.Single(recipeWithIngredients.RecipeIngredients);
        Assert.Equal("Patty", recipeWithIngredients.RecipeIngredients.First().IngredientProduct.Name);
    }
}
