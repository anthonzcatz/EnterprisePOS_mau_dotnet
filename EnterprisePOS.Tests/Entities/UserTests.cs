using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class UserTests : TestBase
{
    [Fact]
    public async Task CanCreateUser()
    {
        // Arrange
        var role = new Role { Name = "Admin" };
        var branch = new Branch { Name = "Main Branch", Code = "MAIN" };
        
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            FullName = "Test User",
            Role = role,
            Branch = branch
        };

        // Act
        Context.Roles.Add(role);
        Context.Branches.Add(branch);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Assert
        var savedUser = await Context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.NotNull(savedUser);
        Assert.Equal("testuser", savedUser.Username);
        Assert.Equal("Test User", savedUser.FullName);
    }

    [Fact]
    public async Task CanUpdateUser()
    {
        // Arrange
        var role = new Role { Name = "Admin" };
        var user = new User { Username = "testuser", Email = "test@example.com", Role = role };
        
        Context.Roles.Add(role);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        user.FullName = "Updated Name";
        user.LastLoginAt = DateTime.UtcNow;
        await Context.SaveChangesAsync();

        // Assert
        var updatedUser = await Context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.Equal("Updated Name", updatedUser?.FullName);
        Assert.NotNull(updatedUser?.LastLoginAt);
    }

    [Fact]
    public async Task CanDeleteUser_SoftDelete()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        user.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedUser = await Context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task CanGetUserWithRoleAndBranch()
    {
        // Arrange
        var role = new Role { Name = "Manager" };
        var branch = new Branch { Name = "Branch 1", Code = "B1" };
        var user = new User { Username = "manager1", Email = "manager@example.com", Role = role, Branch = branch };

        Context.Roles.Add(role);
        Context.Branches.Add(branch);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var userWithDetails = await Context.Users
            .Include(u => u.Role)
            .Include(u => u.Branch)
            .FirstOrDefaultAsync(u => u.Username == "manager1");

        // Assert
        Assert.NotNull(userWithDetails);
        Assert.Equal("Manager", userWithDetails.Role?.Name);
        Assert.Equal("Branch 1", userWithDetails.Branch?.Name);
    }
}
