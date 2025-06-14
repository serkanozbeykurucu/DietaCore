using DietaCore.Business.Abstract;
using DietaCore.Business.Utilities.Abstract;
using DietaCore.DataAccess.Abstract;
using DietaCore.Dto.DietitianDTOs;
using DietaCore.Entities.Concrete;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;
using DietaCore.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace DietaCore.Business.Concrete
{
    public class DietitianService : IDietitianService
    {
        private readonly IDietitianDal _dietitianDal;
        private readonly UserManager<User> _userManager;
        private readonly IUserContextHelper _userContextHelper;

        public DietitianService(IDietitianDal dietitianDal, UserManager<User> userManager, IUserContextHelper userContextHelper)
        {
            _dietitianDal = dietitianDal;
            _userManager = userManager;
            _userContextHelper = userContextHelper;
        }

        public async Task<Response<IList<DietitianResponseDto>>> GetAllAsync()
        {
            var dietitians = await _dietitianDal.GetAllWithUsersAsync();

            var dietitianDtos = dietitians.Select(d => new DietitianResponseDto
            {
                Id = d.Id,
                FirstName = d.User.FirstName,
                LastName = d.User.LastName,
                Email = d.User.Email,
                PhoneNumber = d.User.PhoneNumber,
                Specialization = d.Specialization,
                LicenseNumber = d.LicenseNumber,
                Education = d.Education,
                Biography = d.Biography,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            }).ToList();

            return new Response<IList<DietitianResponseDto>>(ResponseCode.Success, dietitianDtos, "Dietitians retrieved successfully.");
        }

        public async Task<Response<DietitianResponseDto>> GetByIdAsync(int id)
        {
            var dietitian = await _dietitianDal.GetByIdWithUsersAsync(id);
            if (dietitian == null)
            {
                return new Response<DietitianResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            var dietitianDto = new DietitianResponseDto
            {
                Id = dietitian.Id,
                FirstName = dietitian.User.FirstName,
                LastName = dietitian.User.LastName,
                Email = dietitian.User.Email,
                PhoneNumber = dietitian.User.PhoneNumber,
                Specialization = dietitian.Specialization,
                LicenseNumber = dietitian.LicenseNumber,
                Education = dietitian.Education,
                Biography = dietitian.Biography,
                CreatedAt = dietitian.CreatedAt,
                UpdatedAt = dietitian.UpdatedAt
            };

            return new Response<DietitianResponseDto>(ResponseCode.Success, dietitianDto, "Dietitian retrieved successfully.");
        }

        public async Task<Response<DietitianResponseDto>> GetByUserIdAsync()
        {
            var userId = _userContextHelper.GetCurrentUserId();

            var dietitian = await _dietitianDal.GetByUserIdAsync(userId);
            if (dietitian == null)
            {
                return new Response<DietitianResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            var dietitianDto = new DietitianResponseDto
            {
                Id = dietitian.Id,
                FirstName = dietitian.User.FirstName,
                LastName = dietitian.User.LastName,
                Email = dietitian.User.Email,
                PhoneNumber = dietitian.User.PhoneNumber,
                Specialization = dietitian.Specialization,
                LicenseNumber = dietitian.LicenseNumber,
                Education = dietitian.Education,
                Biography = dietitian.Biography,
                CreatedAt = dietitian.CreatedAt,
                UpdatedAt = dietitian.UpdatedAt
            };

            return new Response<DietitianResponseDto>(ResponseCode.Success, dietitianDto, "Dietitian retrieved successfully.");
        }

        public async Task<Response<DietitianResponseDto>> CreateAsync(DietitianRequestDto dietitianDto)
        {
            var user = new User
            {
                UserName = dietitianDto.Email,
                Email = dietitianDto.Email,
                FirstName = dietitianDto.FirstName,
                LastName = dietitianDto.LastName,
                PhoneNumber = dietitianDto.PhoneNumber,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dietitianDto.Password);
            if (!result.Succeeded)
            {
                return new Response<DietitianResponseDto>(ResponseCode.BadRequest, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, RoleConstants.Dietitian);

            var dietitian = new Dietitian
            {
                UserId = user.Id,
                Specialization = dietitianDto.Specialization,
                LicenseNumber = dietitianDto.LicenseNumber,
                Education = dietitianDto.Education,
                Biography = dietitianDto.Biography
            };

            await _dietitianDal.AddAsync(dietitian);

            var createdDietitian = await _dietitianDal.GetByUserIdAsync(user.Id);

            var responseDto = new DietitianResponseDto
            {
                Id = createdDietitian.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Specialization = createdDietitian.Specialization,
                LicenseNumber = createdDietitian.LicenseNumber,
                Education = createdDietitian.Education,
                Biography = createdDietitian.Biography,
                CreatedAt = createdDietitian.CreatedAt,
                UpdatedAt = createdDietitian.UpdatedAt
            };

            return new Response<DietitianResponseDto>(ResponseCode.Success, responseDto, "Dietitian created successfully.");
        }

        public async Task<Response<DietitianResponseDto>> UpdateAsync(DietitianUpdateDto dietitianDto)
        {
            var dietitian = await _dietitianDal.GetByIdWithUsersAsync(dietitianDto.Id);
            if (dietitian == null)
            {
                return new Response<DietitianResponseDto>(ResponseCode.NotFound, "Dietitian not found.");
            }

            var user = dietitian.User;
            user.FirstName = dietitianDto.FirstName;
            user.LastName = dietitianDto.LastName;
            user.PhoneNumber = dietitianDto.PhoneNumber;

            if (user.Email != dietitianDto.Email)
            {
                user.Email = dietitianDto.Email;
                user.UserName = dietitianDto.Email;
                user.EmailConfirmed = false;
            }

            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                return new Response<DietitianResponseDto>(ResponseCode.BadRequest, string.Join(", ", userUpdateResult.Errors.Select(e => e.Description)));
            }

            dietitian.Specialization = dietitianDto.Specialization;
            dietitian.LicenseNumber = dietitianDto.LicenseNumber;
            dietitian.Education = dietitianDto.Education;
            dietitian.Biography = dietitianDto.Biography;

            await _dietitianDal.UpdateAsync(dietitian);

            var responseDto = new DietitianResponseDto
            {
                Id = dietitian.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Specialization = dietitian.Specialization,
                LicenseNumber = dietitian.LicenseNumber,
                Education = dietitian.Education,
                Biography = dietitian.Biography,
                CreatedAt = dietitian.CreatedAt,
                UpdatedAt = dietitian.UpdatedAt
            };

            return new Response<DietitianResponseDto>(ResponseCode.Success, responseDto, "Dietitian updated successfully.");
        }

        public async Task<Response<bool>> DeleteAsync(int id)
        {
            var dietitian = await _dietitianDal.GetByIdWithUsersAsync(id);
            if (dietitian == null)
            {
                return new Response<bool>(ResponseCode.NotFound, "Dietitian not found.");
            }
            await _dietitianDal.DeleteAsync(id);

            var user = dietitian.User;
            await _userManager.UpdateAsync(user);

            return new Response<bool>(ResponseCode.Success, true, "Dietitian deleted successfully.");
        }

    }
}
