﻿using System;
using System.Collections.Generic;

namespace Watchman.BusinessLogic.Models.Users
{
    public abstract class WatchmanProfile : WatchmanProfile<Guid> { }
    public abstract class WatchmanProfile<TKey> : IIdentifiedEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public ICollection<WatchmanPatient<TKey, TKey>> WatchmanPatients { get; set; } = new List<WatchmanPatient<TKey, TKey>>();
    }
}
