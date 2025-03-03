﻿using System.Data;
using Dapper;

namespace EventSourcing.Projections.Database.Repositories
{
    public class OpenBoxRepository(ReadStoreConnection dbConnection)
    {
        private IDbConnection Connection => dbConnection.GetConnection();
        private IDbTransaction Transaction => dbConnection.GetTransaction();

        public void CreateOpenBox(Guid boxId, int capacity)
        {
            const string insertCommand = """
                                     INSERT INTO [dbo].[OpenBoxes]
                                     (BoxId, Capacity) VALUES (@BoxId, @Capacity)
                                     """;

            Connection.Execute(
                insertCommand,
                new { BoxId = boxId, Capacity = capacity },
                Transaction);
        }

        public void AddBottleToBox(Guid boxId)
        {
            const string updateCommand = """
                                     UPDATE [dbo].[OpenBoxes]
                                     SET [NumberOfBottles] = [NumberOfBottles] + 1
                                     WHERE BoxId = @BoxId
                                     """;

            Connection.Execute(
                updateCommand,
                new { BoxId = boxId },
                Transaction);
        }

        public void RemoveOpenBox(Guid boxId)
        {
            const string deleteCommand = """
                                     DELETE FROM [dbo].[OpenBoxes]
                                     WHERE BoxId = @BoxId
                                     """;

            Connection.Execute(
                deleteCommand,
                new { BoxId = boxId },
                Transaction);
        }
    }
}
