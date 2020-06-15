using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CoreApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CoreApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }        

        // Additions table
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Whitelist> Whitelists { get; set; }
        public DbSet<CustomProperty> CustomProperties { get; set; }

        public DbSet<Form> Forms { get; set; }
        public DbSet<FormStep> FormSteps { get; set; }        


        public DbSet<UserForm> UserForms { get; set; }
        public DbSet<UserFormValue> UserFormValues { get; set; }
        public DbSet<UserFormStepValues> UserFormStepValues { get; set; }
        public DbSet<UserFormLog> UserFormLogs { get; set; }
        public DbSet<UserFormAssign> UserFormAssigns { get; set; }
        public DbSet<UserFormValueStorage> UserFormValueStorages { get; set; }

        public DbSet<Comment> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Rename Identity Table to friendly name
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");

            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            // Builder for UserDevice table
            builder.Entity<UserDevice>().ToTable("UserDevices").HasKey(x => new
            {
                x.DeviceId,
                x.UserId
            });
            builder.Entity<UserDevice>().HasIndex(x => x.UserId).IsUnique(false).ForSqlServerIsClustered(false);
            builder.Entity<UserDevice>().HasIndex(x => x.DeviceId).IsUnique(false).ForSqlServerIsClustered(false);
            builder.Entity<UserDevice>().HasIndex(x => x.Token).IsUnique().ForSqlServerIsClustered(false);

            // Builder for UserGroup
            builder.Entity<UserGroup>().ToTable("UserGroups").HasKey(x => new
            {
                x.GroupId,
                x.UserId
            });
            builder.Entity<UserGroup>().HasIndex(x => x.UserId).IsUnique(false).ForSqlServerIsClustered(false);
            builder.Entity<UserGroup>().HasIndex(x => x.GroupId).IsUnique(false).ForSqlServerIsClustered(false);

            // Add Index for Device
            builder.Entity<Device>().HasIndex(x => x.Uuid).IsUnique().ForSqlServerIsClustered(false);
            builder.Entity<Device>().HasIndex(x => x.Id).IsUnique();

            // Token
            builder.Entity<Token>().HasIndex(x => x.Value).IsUnique(false).ForSqlServerIsClustered(false);

            // Options
            builder.Entity<Option>().HasIndex(x => x.Key).IsUnique().ForSqlServerIsClustered(false);
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // Update DateTime for insert and updated
            foreach (var changedEntity in ChangeTracker.Entries())
            {
                var entity = changedEntity.Entity as ITimespan;
                if (entity == null) continue;

                switch (changedEntity.State)
                {
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        break;
                    case EntityState.Modified:
                        entity.DateUpdated = DateTimeOffset.UtcNow;
                        break;
                    case EntityState.Added:
                        entity.DateUpdated = DateTimeOffset.UtcNow;
                        entity.DateCreated = DateTimeOffset.UtcNow;
                        break;
                }
            }

            // Save
            return base.SaveChangesAsync(cancellationToken);
        }


        /// <summary>
        /// Execute SQL Query Text
        /// </summary>
        /// <param name="queryText"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(string queryText, IDictionary<string, object> parameters, CommandType commandType = CommandType.Text)
        {
            using (var cnn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection") ?? throw new NullReferenceException("Can't get Database Connection String")))
            {
                using (var cmd = new SqlCommand())
                {
                    // Build command text
                    cmd.CommandText = queryText;
                    cmd.CommandType = commandType;
                    cmd.Connection = cnn;

                    // Build parameters
                    if (parameters?.Count > 0)
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

                    // Open Connection
                    await cnn.OpenAsync();

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }
      

        /// <summary>
        /// Execute SQL Query Text and return DataReader.
        /// </summary>
        /// <param name="queryText"></param>
        /// <param name="parameters"></param>
        /// <param name="function"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<IList<T>> ExecuteQueryAsync<T>(string queryText, IDictionary<string, object> parameters, Func<IDataRecord, T> function, CommandType commandType = CommandType.Text)
        {
            var items = new List<T>();

            using (var cnn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection") ?? throw new NullReferenceException("Can't get Database Connection String")))
            {
                using (var cmd = new SqlCommand())
                {
                    // Build command text
                    cmd.CommandText = queryText;
                    cmd.CommandType = commandType;
                    cmd.Connection = cnn;

                    // Build parameters
                    if (parameters?.Count > 0)
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

                    // Open Connection
                    await cnn.OpenAsync();

                    // Execute Reader
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                items.Add(function(reader));
                            }
                        }

                        return items;
                    }
                }
            }
        }

        public async Task<T> ExecuteQueryAsync<T>(string queryText, IDictionary<string, object> parameters, Func<DataTable, Task<T>> function, CommandType commandType = CommandType.Text)
        {
            using (var cnn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection") ?? throw new NullReferenceException("Can't get Database Connection String")))
            {
                using (var cmd = new SqlCommand())
                {
                    // Build command text
                    cmd.CommandText = queryText;
                    cmd.CommandType = commandType;
                    cmd.Connection = cnn;                    

                    // Build parameters
                    if (parameters?.Count > 0)
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

                    // Open Connection
                    await cnn.OpenAsync();

                    // Execute Reader
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        using (var dt = new DataTable("Test"))
                        {
                            dt.Load(reader);
                            return await function(dt);
                        }
                    }
                }
            }
        }

        public async Task<T> ExecuteQueryAndReturnSingleAsync<T>(string queryText, IDictionary<string, object> parameters, Func<IDataRecord, T> function, CommandType commandType = CommandType.Text)
        {
            using (var cnn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection") ?? throw new NullReferenceException("Can't get Database Connection String")))
            {
                using (var cmd = new SqlCommand())
                {
                    // Build command text
                    cmd.CommandText = queryText;
                    cmd.CommandType = commandType;
                    cmd.Connection = cnn;

                    // Build parameters
                    if (parameters?.Count > 0)
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

                    // Open Connection
                    await cnn.OpenAsync();

                    // Execute Reader
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                return function(reader);
                            }
                        }

                        return default(T);
                    }
                }
            }
        }
    }
}

