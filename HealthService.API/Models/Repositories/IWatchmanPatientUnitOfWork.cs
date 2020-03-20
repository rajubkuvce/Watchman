﻿using HealthService.API.Models.Users;

using System;
using System.Threading.Tasks;

using Watchman.BusinessLogic.Models.Data;

namespace HealthService.API.Models.Repositories
{
    public interface IWatchmanPatientUnitOfWork : IDisposable
    {
        IWatchmanRepository<WatchmanProfileHealth, Guid> WatchmanRepository { get; }
        IPatientRepository<PatientProfile, Guid> PatientRepository { get; }
        void AddWatchmanToPatient(Guid watchmanId, Guid patientId);
        void RemoveWatchmanFromPatient(Guid watchmanId, Guid patientId);

        void RemoveAllWatchmen(Guid patientId);
        void RemoveAllPatients(Guid watchmanId);

        void Save();
        Task SaveAsync();
    }
}
