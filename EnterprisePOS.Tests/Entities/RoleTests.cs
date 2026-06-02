using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class RoleTests : TestBase
{
    [Fact]
    public async Task CanCreateRole()
    {
        // Arrange
        var role = new Role
        {
            Name = "Admin",
            Description = "Administrator role",
            IsSystem = true
        };

        // Act
        Context.Roles.Add(role);
        await Context.SaveChangesAsync();

        // Assert
        var savedRole = await Context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        Assert.NotNull(savedRole);
        Assert.Equal("Admin", savedRole.Name);
        Assert.True(savedRole.IsSystem);
    }

    [Fact]
    public async Task CanUpdateRole()
    {
        // Arrange
        var role = new Role { Name = "Manager", Description = "Manager role" };
        Context.Roles.Add(role);
        await Context.SaveChangesAsync();

        // Act
        role.Description = "Updated description";
        await Context.SaveChangesAsync();

        // Assert
        var updatedRole = await Context.Roles.FirstOrDefaultAsync(r => r.Name == "Manager");
        Assert.Equal("Updated description", updatedRole?.Description);
    }

    [Fact]
    public async Task CanDeleteRole_SoftDelete()
    {
        // Arrange
        var role = new Role { Name = "TestRole" };
        Context.Roles.Add(role);
        await Context.SaveChangesAsync();

        // Act
        role.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedRole = await Context.Roles.FirstOrDefaultAsync(r => r.Name == "TestRole");
        Assert.Null(deletedRole); // Soft delete filter excludes it
    }

    [Fact]
    public async Task CanGetRoleWithPermissions()
    {
        // Arrange
        var role = new Role { Name = "Admin" };
        var permission = new Permission { Name = "CanEdit", Description = "Edit permission" };
        
        var rolePermission = new RolePermission
        {
            Role = role,
            Permission = permission
        };

        Context.Roles.Add(role);
        Context.Permissions.Add(permission);
        Context.RolePermissions.Add(rolePermission);
        await Context.SaveChangesAsync();

        // Act
        var roleWithPermissions = await Context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Name == "Admin");

        // Assert
        Assert.NotNull(roleWithPermissions);
        Assert.Single(roleWithPermissions.RolePermissions);
        Assert.Equal("CanEdit", roleWithPermissions.RolePermissions.First().Permission.Name);
    }
}
