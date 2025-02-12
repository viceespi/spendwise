using Popoupa.Core.DomainModels;

namespace Popoupa.Core.DataBaseManipulation.Interfaces
{
    public interface ICategoryRepositoryInterface
    {
        public Task CategoryPipeLine();
        public Task<Guid> Add(Category newCategory);
        public Task Update(Category category);
        public Task Delete(Guid categoryId);
        public Task<Category?> Get(Guid categoryId);
        public Task<List<Category?>> GetAll(Guid userId);
        public Task CreateUserCategoryRelation(Guid userId, Guid categoryId);
    }
}