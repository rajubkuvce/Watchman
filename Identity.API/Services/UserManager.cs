﻿using Identity.API.Models;
using Identity.API.Services.PasswordHashing;

using System;
using System.Threading.Tasks;

using Watchman.BusinessLogic.Models.Data;
using Watchman.BusinessLogic.Models.Users;
using Watchman.BusinessLogic.Services;

namespace Identity.API.Services
{
    public class UserManager : IUserManager<IdentityUser, Guid>
    {
        private readonly IPersonalInformationRepository<PersonalInfo, Guid> _personalInfoRepository;
        private readonly IUserRepository<IdentityUser> _userRepository;
        private readonly ICustomPasswordHasher _hasher;

        public UserManager(IUserRepository<IdentityUser> userRepository, ICustomPasswordHasher hasher, IPersonalInformationRepository<PersonalInfo, Guid> personalInformationRepository)
        {
            this._userRepository = userRepository;
            this._hasher = hasher;
            this._personalInfoRepository = personalInformationRepository;
        }

        public async Task CreateUserWithPersonalInformationAsync(PersonalInformation<Guid> personalInformation, string clearPassword)
        {
            if (_personalInfoRepository.GetByEmailAsync(personalInformation.Email).Result == null)
            {
                personalInformation.Id = personalInformation.Id == Guid.Empty ? Guid.NewGuid() : personalInformation.Id;
                personalInformation.HashedPassword = _hasher.Hash(clearPassword);

                var user = new IdentityUser
                {
                    PersonalInformationId = personalInformation.Id
                };

                await _userRepository.CreateAsync(user);
                await _userRepository.SaveChangesAsync();

                await _personalInfoRepository.CreateAsync(personalInformation as PersonalInfo);
                await _personalInfoRepository.SaveChangesAsync();
            }
        }

        public async Task<IdentityUser> FindByWatchman(Guid watchmanId, string token = null)
        {
            return await _userRepository.GetByWatchmanId(watchmanId);
        }

        public async Task<IdentityUser> FindByPatient(Guid patientId, string token = null)
        {
            return await _userRepository.GetByPatientId(patientId);
        }

        public async Task<IdentityUser> FindByEmailAsync(string email, string token = null)
        {
            var personalnfo = await _personalInfoRepository.GetByEmailAsync(email);
            return personalnfo != null ? await _userRepository.GetByPersonalInformationId(personalnfo.Id) : null;
        }
        public async Task<IdentityUser> FindByIdAsync(Guid key, string token = null)
        {
            return await _userRepository.RetrieveAsync(key);
        }
    }
}
