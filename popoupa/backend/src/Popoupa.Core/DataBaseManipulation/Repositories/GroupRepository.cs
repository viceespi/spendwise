using Popoupa.Core.DataBaseManipulation.Interfaces;
using Npgsql;
using Popoupa.Core.DomainModels;
using Dapper;
using System.Collections.Immutable;

namespace Popoupa.Core.DataBaseManipulation.Repositories
{
    public class GroupRepository : IGroupRepositoryInterface
    {

        private readonly string popoupaDB = "Server=192.168.0.21;Port=5432;Database=popoupa;User Id=postgres;Password=Nina100%";

        
        public async Task GroupPipeLine()
        {
            const string sqlOrder =
            @"
            INSERT INTO groups (group_id, name) 
            VALUES ('00000000-0000-0000-0000-000000000000', 'Unassigned')
            ON CONFLICT (group_id) 
            DO NOTHING;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during group pipeline query: {exception.Message}");
                }
            }
        }
        public async Task<Guid> Add(Group newGroup)
        {
            const string sqlOrder = @"
                INSERT INTO
                groups (name)
                VALUES
                (@GroupName)
                RETURNING group_id;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var groupId = await popoupaDBConnection.QuerySingleAsync<Guid>(sqlOrder, new
                    {
                        GroupName = newGroup.Name
                    });

                    return groupId;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during group creation query: {exception.Message}");
                }
            }
        }

        public async Task Delete(Guid groupId)
        {
            const string sqlOrder = @"
                DELETE
                FROM
                groups
                WHERE
                group_id = @GroupId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        GroupId = groupId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during group deletion query: {exception.Message}");
                }
            }
        }

        public async Task<Group?> Get(Guid groupId)
        {
            const string sqlOrder = @"
                SELECT
                groups.group_id as Id,
                groups.name as Name
                FROM
                groups
                WHERE
                group_id = @GroupId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var group = await popoupaDBConnection.QuerySingleAsync<Group?>(sqlOrder, new
                    {
                        GroupId = groupId
                    });

                    return group;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during group query: {exception.Message}");
                }
            }
        }

        public async Task<List<Group?>> GetAll(Guid userId)
        {
            const string sqlOrder = @"
                SELECT
                groups.group_id as Id,
                groups.name as Name
                FROM
                groups
                INNER JOIN
                user_groups
                ON
                groups.group_id = user_groups.group_id
                WHERE
                user_groups.user_id = @UserId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var groups = await popoupaDBConnection.QueryAsync<Group?>(sqlOrder, new
                    {
                        UserId = userId
                    });

                    return groups.ToList();
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during groups query: {exception.Message}");
                }
            }
        }

        public async Task Update(Group group)
        {
            const string sqlOrder = @"
                UPDATE
                groups
                SET
                group_id = @GroupId,
                name = @GroupName
                WHERE
                group_id = @GroupId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        GroupId = group.Id,
                        GroupName = group.Name 
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during group update query: {exception.Message}");
                }
            }
        }

        public async Task CreateUserGroupRelation(Guid userId, Guid groupId)
        {
            const string sqlOrder = @"
                INSERT INTO
                user_groups (user_id, group_id)
                VALUES
                (@UserId, @GroupId);
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new 
                    {
                        UserId = userId,
                        GroupId = groupId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during user group relation creation query: {exception.Message}");
                }
            }
        }

    }
}

