﻿
namespace CRUDRepository.Repository.Base
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the specified domain.
        /// </summary>
        /// <typeparam name="TDomain">The type of the domain.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="domain">The domain.</param>
        /// <returns>A mapped item</returns>
        TItem Map<TDomain, TItem>(TDomain domain) where TItem : class;
    }
}