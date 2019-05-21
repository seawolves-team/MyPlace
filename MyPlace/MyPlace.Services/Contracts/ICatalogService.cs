﻿
namespace MyPlace.Services.Contracts
{
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICatalogService
    {
        Task<IQueryable<TSource>> ReadAll<TSource>();

        Task<TSource> GetById<TSource>(int Id);

        Task CreateReply(int Id, string text);
    }
}

