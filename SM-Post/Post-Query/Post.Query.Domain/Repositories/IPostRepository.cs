using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories
{
    public interface IPostRepository
    {
        Task CreateAsync(PostEntity post);
        Task UpdateAsync(PostEntity post);
        Task DeleteAsync(Guid postId);
        Task<PostEntity> GetByIdAsync(Guid postId);
        Task<List<PostEntity>> ListAllAsync();
        Task<List<PostEntity>> ListByAuthorAsync(string author);
        Task<List<PostEntity>> ListWithLikeAsync(int numberOfLikes);
        Task<List<PostEntity>> ListWithCommentAsync();
    }
}