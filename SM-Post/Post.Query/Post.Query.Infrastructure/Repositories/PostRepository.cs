using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContextFactory _contextFactory;

        public PostRepository(DatabaseContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateAsync(PostEntity entity)
        {
            using var context = _contextFactory.CreateDbContext();

            context.Posts.Add(entity);
            _ = await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid postId)
        {
            using var context = _contextFactory.CreateDbContext();

            var post = await GetByIdAsync(postId);

            if (post is null)
            {
                return;
            }

            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }

        public async Task<PostEntity> GetByIdAsync(Guid postId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Posts
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.PostId == postId);
        }

        public async Task<List<PostEntity>> ListAllAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Posts
                .AsNoTracking()
                .Include(x => x.Comments)
                .ToListAsync();
        }

        public async Task<List<PostEntity>> ListByAuthorAsync(string author)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Posts
                .AsNoTracking()
                .Include(x => x.Comments)
                .Where(x => x.Author.Contains(author))
                .ToListAsync();
        }

        public async Task<List<PostEntity>> ListWhithCommentsAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Posts
                .AsNoTracking()
                .Include(x => x.Comments)
                .Where(x => x.Comments != null && x.Comments.Count > 0)
                .ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Posts
              .AsNoTracking()
              .Include(x => x.Comments)
              .Where(x => x.Likes >= numberOfLikes)
              .ToListAsync();
        }

        public async Task UpdateAsync(PostEntity entity)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Update(entity);

            await context.SaveChangesAsync();
        }
    }
}
