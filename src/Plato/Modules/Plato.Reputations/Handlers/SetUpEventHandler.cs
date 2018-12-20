﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Reputations.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        public string Version { get; } = "1.0.0";

        private readonly SchemaTable _userReputations = new SchemaTable()
        {
            Name = "UserReputations",
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
                    Name = "[Name]",
                    DbType = DbType.String,
                    Length = "255"
                },
                new SchemaColumn()
                {
                    Name = "Points",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "CreatedUserId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "CreatedDate",
                    DbType = DbType.DateTimeOffset
                },
            }
        };
        
        private readonly ISchemaBuilder _schemaBuilder;

        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
     
        }

        #region "Implementation"

        public override async Task SetUp(SetUpContext context, Action<string, string> reportError)
        {

            // build schemas
            
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // User reputations
                UserReputations(builder);
                
                var result = await builder.ApplySchemaAsync();
                if (result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        reportError(error.Message, error.StackTrace);
                    }

                }

            }
            
        }

        #endregion

        #region "Private Methods"
        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = ModuleId;
                    options.Version = Version;
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void UserReputations(ISchemaBuilder builder)
        {

            builder
                .CreateTable(_userReputations)
                .CreateDefaultProcedures(_userReputations);

            builder.CreateProcedure(new SchemaProcedure("SelectUserReputationsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_userReputations)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "Keywords",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }));

        }
        
        #endregion

    }

}