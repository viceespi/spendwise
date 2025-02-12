using Popoupa.Core.DomainModels;

namespace Popoupa.Core.DataBaseManipulation.Interfaces
{
    public interface IUserRepositoryInterface
    {
        public Task UserPipeLine();
        public Task<Guid> Add(User newUser);
        public Task Update(User user);
        public Task Delete(Guid userId);
        public Task<User?> Get(Guid userId);
        public Task<IEnumerable<User?>> GetAll(Guid userId);
        public Task CreateUserFriendRelation(Guid userId, Guid friendId);
    }
}