﻿using HealthService.API.Models.Infrastructure.Repositories;
using HealthService.API.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Watchman.BusinessLogic.Models.Analysis;
using Watchman.BusinessLogic.Models.Signs;
using Watchman.BusinessLogic.Models.Users;
using Watchman.BusinessLogic.Services;

namespace HealthService.API.Services
{
    public class WatchmanPatientService : IWatchmanPatientService<Guid>
    {
        private readonly IWatchmanPatientUnitOfWork db;
        private readonly IHealthAnalyzer analyzer;
        public WatchmanPatientService(IWatchmanPatientUnitOfWork unitOfWork, IHealthAnalyzer analyzer)
        {
            this.db = unitOfWork;
            this.analyzer = analyzer;
        }

        public async Task AddIgnorableSignToPatientAsync(Guid patientId, Sign<Guid> sign)
        {
            var patient = db.PatientRepository.RetrieveAsync(patientId);
            if (patient != null)
            {
                await db.PatientRepository.AddIgnorableSignAsync(patientId, sign);
                await db.SaveAsync();
            }
        }

        public async Task<IAnalysisResult> AnalyzeLastMeasurementAsync(Guid patientId)
        {
            var patient = await db.PatientRepository.RetrieveAsync(patientId);
            if (patient != null)
            {
                var patientWithProps = await db.PatientRepository.RetrieveWithAllPropertiesAsync(patientId);
                analyzer.AnalyzeLast(patientWithProps);
                return analyzer.AnalysisResult as IAnalysisResult;
            }
            return null;
        }

        public async Task<HealthMeasurement<Guid, Guid>> GetLastHealthMeasurementAsync(Guid patientId)
        {
            var patient = await db.PatientRepository.RetrieveAsync(patientId);
            if (patient != null)
            {
                return await db.PatientRepository.GetLastHealthMeasurementAsync(patientId);
            }
            return null;
        }
        public async Task<IEnumerable<HealthMeasurement<Guid, Guid>>> GetLastHealthMeasurementsAsync(Guid patientId, int count)
        {
            var patient = await db.PatientRepository.RetrieveAsync(patientId);
            if (patient != null)
            {
                return await db.PatientRepository.GetLastHealthMeasurementsAsync(patientId, count);
            }
            return null;
        }
        public async Task AddHealthMeasurementAsync(Guid patientId, HealthMeasurement<Guid, Guid> healthMeasurement)
        {
            var patient = db.PatientRepository.RetrieveAsync(patientId);
            if (patient != null && healthMeasurement != null)
            {
                await db.PatientRepository.AddHealthMeasurementAsync(patientId, healthMeasurement);
                await db.SaveAsync();
            }
        }

        public async Task AddPatientToWatchmanAsync(Guid watchmanId, Guid patientId)
        {
            var watchman = db.WatchmanRepository.RetrieveAsync(watchmanId);
            var patient = db.PatientRepository.RetrieveAsync(patientId);

            if (watchman != null && patient != null)
            {
                await db.AddWatchmanToPatientAsync(watchmanId, patientId);
                await db.SaveAsync();
            }
        }
        public async Task RemovePatientFromWatchmanAsync(Guid watchmanId, Guid patientId)
        {
            var watchman = await db.WatchmanRepository.RetrieveAsync(watchmanId);
            if (watchman != null)
            {
                var patients = new List<Patient<Guid>>(await db.WatchmanRepository.GetPatientsAsync(watchman));
                if (patients.FirstOrDefault(patient => patient.Id.Equals(patientId)) != null)
                {
                    db.RemoveWatchmanFromPatient(watchmanId, patientId);
                    await db.SaveAsync();
                }
            }
        }

        public void RemoveAllWatchmenFromPatient(Guid patientId)
        {
            var patient = db.PatientRepository.RetrieveAsync(patientId);
            if (patient != null)
            {
                db.RemoveAllWatchmen(patientId);
                db.SaveAsync();
            }
        }
        public void RemoveAllPatientFromWatchman(Guid watchmanId)
        {
            var watchman = db.WatchmanRepository.RetrieveAsync(watchmanId);
            if (watchman != null)
            {
                db.RemoveAllPatients(watchmanId);
                db.SaveAsync();
            }
        }

        public async Task CreatePatient(Patient<Guid> patient, string token = null)
        {
            await db.PatientRepository.CreateAsync(patient as PatientProfile);
            await db.SaveAsync();
        }

        public Task CreateWatchman(WatchmanProfile<Guid> watchman)
        {
            throw new NotImplementedException();
        }
    }
}
