using System;
using System.Threading.Tasks;
using Bluehands.Hypermedia.Client.Hypermedia;
using Bluehands.Hypermedia.Client.Resolver;

namespace Bluehands.Hypermedia.Client.Extensions
{
    public static class NavigateExtension
    {
        public static async Task<TResult> NavigateAsync<TIn, TResult>(this Task<TIn> hco, Func<TIn, MandatoryHypermediaLink<TResult>> linkSelector)
            where TResult : HypermediaClientObject
            where TIn : HypermediaClientObject
        {
            return await linkSelector(await hco).ResolveAsync();
        }

        public static async Task<TResult> NavigateAsync<TIn, TResult>(this TIn hco, Func<TIn, MandatoryHypermediaLink<TResult>> linkSelector)
            where TResult : HypermediaClientObject
            where TIn : HypermediaClientObject
        {
            return await linkSelector(hco).ResolveAsync();
        }


        public static async Task<ResolverResult<TResult>> NavigateAsync<TIn, TResult>(this Task<TIn> hco, Func<TIn, HypermediaLink<TResult>> linkSelector)
            where TResult : HypermediaClientObject
            where TIn : HypermediaClientObject
        {
            return await linkSelector(await hco).TryResolveAsync();
        }

        public static async Task<ResolverResult<TResult>> NavigateAsync<TIn, TResult>(this TIn hco, Func<TIn, HypermediaLink<TResult>> linkSelector)
            where TResult : HypermediaClientObject
            where TIn : HypermediaClientObject
        {
            return await linkSelector(hco).TryResolveAsync();
        }
    }
}