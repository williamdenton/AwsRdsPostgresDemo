using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using Npgsql.NameTranslation;

namespace WilliamDenton.AwsRdsPostgresDemo.Models
{
    public static class ModelBuilderExtensions
    {
        static readonly Regex _keysRegex = new Regex("^(PK|FK|IX)_", RegexOptions.Compiled);

        /*
            Postgres is case sensitive, any string defined inside quotes would be treated as cased sensitive otherwise it will converted
            to lower case; This makes Pascal case bit undesirable to follow as a conventions.
            It would make things easier if we follow lower case conventions in naming our tables/columns, etc..
            inspired https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL/issues/21

            Maybe replaced by https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL/issues/933
        */
        public static void UseSnakeCaseNames(this ModelBuilder modelBuilder)
        {
            var mapper = new NpgsqlSnakeCaseNameTranslator();

            foreach (var table in modelBuilder.Model.GetEntityTypes())
            {

                ConvertToSnake(mapper, table);

                foreach (var property in table.GetProperties())
                {
                    ConvertToSnake(mapper, property);
                }

                foreach (var primaryKey in table.GetKeys())
                {
                    ConvertToSnake(mapper, primaryKey);
                }

                foreach (var foreignKey in table.GetForeignKeys())
                {
                    ConvertToSnake(mapper, foreignKey);
                }

                foreach (var indexKey in table.GetIndexes())
                {
                    ConvertToSnake(mapper, indexKey);
                }
            }
        }

        static void ConvertToSnake(INpgsqlNameTranslator mapper, object entity)
        {
            switch (entity)
            {
                case IMutableEntityType table:
                    table.SetTableName(ConvertGeneralToSnake(mapper, table.GetTableName()));
                    break;
                case IMutableProperty property:
                    property.SetColumnName(ConvertGeneralToSnake(mapper, property.GetColumnName()));
                    break;
                case IMutableKey primaryKey:
                    primaryKey.SetName(ConvertKeyToSnake(mapper, primaryKey.GetName()));
                    break;
                case IMutableForeignKey foreignKey:
                    foreignKey.SetConstraintName(ConvertKeyToSnake(mapper, foreignKey.GetConstraintName()));
                    break;
                case IMutableIndex indexKey:
                    indexKey.SetName(ConvertKeyToSnake(mapper, indexKey.GetName()));
                    break;
                default:
                    throw new NotImplementedException("Unexpected type was provided to snake case converter");
            }
        }

        static string ConvertKeyToSnake(INpgsqlNameTranslator mapper, string keyName) =>
            ConvertGeneralToSnake(mapper, _keysRegex.Replace(keyName, match => match.Value.ToLower()));

        static string ConvertGeneralToSnake(INpgsqlNameTranslator mapper, string entityName) =>
            mapper.TranslateMemberName(entityName);
    }
}
