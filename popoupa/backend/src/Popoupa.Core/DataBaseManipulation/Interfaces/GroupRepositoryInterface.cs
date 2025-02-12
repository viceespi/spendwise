using Popoupa.Core.DomainModels;

namespace Popoupa.Core.DataBaseManipulation.Interfaces
{
    public interface IGroupRepositoryInterface
    {
        public Task GroupPipeLine();
        public Task<Guid> Add(Group newGroup);
        public Task Update(Group group);
        public Task Delete(Guid groupId);
        public Task<Group?> Get(Guid groupId);
        public Task<List<Group?>> GetAll(Guid userId);
        public Task CreateUserGroupRelation(Guid groupId, Guid userId);
    }
}
