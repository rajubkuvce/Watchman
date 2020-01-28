﻿
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Watchman.BusinessLogic.Models.Data
{
    public interface IStore<TClass, TKey> : IDisposable
        where TClass : class, IIdentifiedEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<OperationResult> CreateAsync(TClass obj, CancellationToken cancellationToken);
        Task<OperationResult> UpdateAsync(TClass obj, CancellationToken cancellationToken);
        Task<OperationResult> DeleteAsync(TClass obj, CancellationToken cancellationToken);
        Task<TKey> GeTKeyAsync(TClass obj, CancellationToken cancellationToken);
        Task<TClass> FindByIdAsync(TKey id, CancellationToken cancellationToken);
        Task<IEnumerable<TClass>> FindByConditionAsync(Expression<Func<TClass, bool>> expression);
    }
}