using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class BranchTests : TestBase
{
    [Fact]
    public async Task CanCreateBranch()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Main Branch",
            Code = "MAIN",
            Address = "123 Main St",
            Phone = "555-1234",
            Email = "main@example.com"
        };

        // Act
        Context.Branches.Add(branch);
        await Context.SaveChangesAsync();

        // Assert
        var savedBranch = await Context.Branches.FirstOrDefaultAsync(b => b.Code == "MAIN");
        Assert.NotNull(savedBranch);
        Assert.Equal("Main Branch", savedBranch.Name);
        Assert.Equal("MAIN", savedBranch.Code);
    }

    [Fact]
    public async Task CanUpdateBranch()
    {
        // Arrange
        var branch = new Branch { Name = "Branch 1", Code = "B1" };
        Context.Branches.Add(branch);
        await Context.SaveChangesAsync();

        // Act
        branch.Address = "Updated Address";
        branch.Phone = "555-9999";
        await Context.SaveChangesAsync();

        // Assert
        var updatedBranch = await Context.Branches.FirstOrDefaultAsync(b => b.Code == "B1");
        Assert.Equal("Updated Address", updatedBranch?.Address);
        Assert.Equal("555-9999", updatedBranch?.Phone);
    }

    [Fact]
    public async Task CanDeleteBranch_SoftDelete()
    {
        // Arrange
        var branch = new Branch { Name = "Test Branch", Code = "TEST" };
        Context.Branches.Add(branch);
        await Context.SaveChangesAsync();

        // Act
        branch.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedBranch = await Context.Branches.FirstOrDefaultAsync(b => b.Code == "TEST");
        Assert.Null(deletedBranch);
    }

    [Fact]
    public async Task CanGetBranchWithUsers()
    {
        // Arrange
        var branch = new Branch { Name = "Main Branch", Code = "MAIN" };
        var role = new Role { Name = "Admin" };
        var user = new User { Username = "user1", Email = "user1@example.com", Role = role, Branch = branch };

        Context.Branches.Add(branch);
        Context.Roles.Add(role);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var branchWithUsers = await Context.Branches
            .Include(b => b.Users)
            .FirstOrDefaultAsync(b => b.Code == "MAIN");

        // Assert
        Assert.NotNull(branchWithUsers);
        Assert.Single(branchWithUsers.Users);
        Assert.Equal("user1", branchWithUsers.Users.First().Username);
    }
}
