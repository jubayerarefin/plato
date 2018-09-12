﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Users.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly UserManager<User> _userManager;

        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder,
            UserManager<User> userManager)
        {
            _schemaBuilder = schemaBuilder;
            _userManager = userManager;
        }

        #region "Implementation"

        public override async Task SetUp(SetUpContext context, Action<string, string> reportError)
        {

            // build schemas
            
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // user schema
                Users(builder);

                // userphoto schema
                UserPhoto(builder);

                // UserData schema
                UserData(builder);
                
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Message, error.StackTrace);
                    }

                }

            }

            // create super user

            try
            {
                var result =  await _userManager.CreateAsync(new User()
                {
                    Email = context.AdminEmail,
                    UserName = context.AdminUsername,
                    EmailConfirmed = true
                }, context.AdminPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Code, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                reportError(ex.Message, ex.StackTrace);
            }

        }

        #endregion

        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = base.ModuleId;
                    options.Version = "1.0.0";
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void Users(ISchemaBuilder builder)
        {

            var users = new SchemaTable()
            {
                Name = "Users",
                Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "PrimaryRoleId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "UserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedUserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Email",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedEmail",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "EmailConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "DisplayName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "FirstName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "LastName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Alias",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "SamAccountName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordHash",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "SecurityStamp",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumber",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumberConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TwoFactorEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnd",
                        Nullable = true,
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "AccessFailedCount",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ResetToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ConfirmationToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ApiKey",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "TimeZone",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ObserveDst",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "Culture",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "LastLoginDate",
                        DbType = DbType.DateTimeOffset,
                    }
                }
            };

            builder
                .CreateTable(users)
                .CreateDefaultProcedures(users)

            // Overwrite our SelectEntityById created via CreateDefaultProcedures
            // above to also return all EntityData within a second result set
            .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectUserById",
                            @" SELECT * FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   Id = @Id
                                )
                                SELECT * FROM {prefix}_UserData WITH (nolock) 
                                WHERE (
                                   UserId = @Id
                                )")
                        .ForTable(users)
                        .WithParameter(users.PrimaryKeyColumn))

                .CreateProcedure(new SchemaProcedure("SelectUserByEmail", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   Email = @Email
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(users)
                    .WithParameter(new SchemaColumn() {Name = "Email", DbType = DbType.String, Length = "255"}))


                .CreateProcedure(new SchemaProcedure("SelectUserByEmailNormalized", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   NormalizedEmail = @NormalizedEmail
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(users)
                    .WithParameter(new SchemaColumn() { Name = "NormalizedEmail", DbType = DbType.String, Length = "255" }))


                .CreateProcedure(new SchemaProcedure("SelectUserByUserName", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   UserName = @UserName
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(users)
                    .WithParameter(new SchemaColumn() {Name = "UserName", DbType = DbType.String, Length = "255"}))
              
                .CreateProcedure(
                    new SchemaProcedure("SelectUserByUserNameNormalized", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   NormalizedUserName = @NormalizedUserName
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(users)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "NormalizedUserName",
                            DbType = DbType.String,
                            Length = "255"
                        }))


                .CreateProcedure(new SchemaProcedure("SelectUserByApiKey", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   ApiKey = @ApiKey
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(users)
                    .WithParameter(new SchemaColumn() { Name = "ApiKey", DbType = DbType.String, Length = "255" }))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByEmailAndPassword", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   Email = @Email AND PasswordHash = @PasswordHash
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(users)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "Email",
                                DbType = DbType.String,
                                Length = "255"
                            },
                            new SchemaColumn()
                            {
                                Name = "PasswordHash",
                                DbType = DbType.String,
                                Length = "255"
                            }
                        }))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByResetToken", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   ResetToken = @ResetToken
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(users)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "ResetToken",
                                DbType = DbType.String,
                                Length = "255"
                            }
                        }))

                    .CreateProcedure(
                    new SchemaProcedure("SelectUserByConfirmationToken", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   ConfirmationToken = @ConfirmationToken
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(users)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "ConfirmationToken",
                                DbType = DbType.String,
                                Length = "255"
                            }
                        }))
                        
                .CreateProcedure(new SchemaProcedure("SelectUsersPaged", StoredProcedureType.SelectPaged)
                    .ForTable(users)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Id",
                            DbType = DbType.Int32
                        },
                        new SchemaColumn()
                        {
                            Name = "UserName",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "Email",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "DisplayName",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "FirstName",
                            DbType = DbType.String,
                            Length = "255"
                        }
                        ,
                        new SchemaColumn()
                        {
                            Name = "LastName",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));
            

        }

        void UserPhoto(ISchemaBuilder builder)
        {

            var userPhoto = new SchemaTable()
            {
                Name = "UserPhoto",
                Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "UserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Name]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentBlob",
                        Nullable = true,
                        DbType = DbType.Binary
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentType",
                        Length = "75",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentLength",
                        DbType = DbType.Int64
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    }
                }
            };

            builder
                // Create tables
                .CreateTable(userPhoto)
                // Create basic default CRUD procedures
                .CreateDefaultProcedures(userPhoto)
                .CreateProcedure(new SchemaProcedure("SelectUserPhotoByUserId", StoredProcedureType.SelectByKey)
                    .ForTable(userPhoto)
                    .WithParameter(new SchemaColumn() {Name = "UserId", DbType = DbType.Int32}));

        }

        void UserData(ISchemaBuilder builder)
        {

            var userData = new SchemaTable()
            {
                Name = "UserData",
                Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "UserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Key]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Value]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTime2
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    }
                }
            };

            builder
                .CreateTable(userData)
                .CreateDefaultProcedures(userData)
                .CreateProcedure(new SchemaProcedure("SelectUserDatumByUserId", StoredProcedureType.SelectByKey)
                    .ForTable(userData)
                    .WithParameter(new SchemaColumn() {Name = "UserId", DbType = DbType.Int32}))
                .CreateProcedure(new SchemaProcedure("SelectUserDatumByKeyAndUserId", StoredProcedureType.SelectByKey)
                    .ForTable(userData)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "[Key]",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }
                    }));
            
            builder.CreateProcedure(new SchemaProcedure("SelectUserDatumPaged", StoredProcedureType.SelectPaged)
                .ForTable(userData)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "[Key]",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }));


        }

        #endregion

    }

}
