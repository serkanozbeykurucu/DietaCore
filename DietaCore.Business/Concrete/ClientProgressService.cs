using DietaCore.Business.Abstract;
using DietaCore.DataAccess.Abstract;
using DietaCore.Dto.ClientProgressDTOs;
using DietaCore.Entities.Concrete;
using DietaCore.Shared.Common.Responses.ComplexTypes;
using DietaCore.Shared.Common.Responses.Concrete;

namespace DietaCore.Business.Concrete
{
    public class ClientProgressService : IClientProgressService
    {
        private readonly IClientProgressDal _progressDal;
        private readonly IClientDal _clientDal;

        public ClientProgressService(IClientProgressDal progressDal, IClientDal clientDal)
        {
            _progressDal = progressDal;
            _clientDal = clientDal;
        }

        public async Task<Response<List<ProgressDto>>> GetClientProgressByDietitianAsync(int clientId)
        {
            var records = await _progressDal.GetByClientIdAsync(clientId);
            var dtos = records.Select(MapToDto).ToList();
            return new Response<List<ProgressDto>>(ResponseCode.Success, dtos);
        }
        public async Task<Response<ProgressSummary>> GetClientProgressSummaryByDietitianAsync(int clientId)
        {
            var records = await _progressDal.GetByClientIdWithDetailsAsync(clientId);
            var client = records.FirstOrDefault()?.Client ?? await _clientDal.GetByIdAsync(clientId);

            var summary = new ProgressSummary
            {
                ClientId = clientId,
                ClientName = records.FirstOrDefault()?.Client?.User != null
                    ? $"{records.FirstOrDefault().Client.User.FirstName} {records.FirstOrDefault().Client.User.LastName}"
                    : "İsim bulunamadı",
                StartWeight = client.InitialWeight,
                CurrentWeight = records.FirstOrDefault()?.Weight ?? client.InitialWeight,
                TargetWeight = 70,
                WeightLoss = client.InitialWeight - (records.FirstOrDefault()?.Weight ?? client.InitialWeight),
                Recent = records.Take(5).Select(MapToDto).ToList()
            };

            return new Response<ProgressSummary>(ResponseCode.Success, summary);
        }
        public async Task<Response<ProgressDto>> CreateProgressByDietitianAsync(ProgressDto dto)
        {
            var dietitianId = await _clientDal.GetDietitianIdByClientId(dto.ClientId);
            if (dietitianId == null)
            {
                return new Response<ProgressDto>(ResponseCode.NotFound, "Diyetisyen bulunamadı");
            }

            var progress = new ClientProgress
            {
                ClientId = dto.ClientId,
                Weight = dto.Weight,
                BodyFatPercentage = dto.BodyFat,
                MuscleMass = dto.Muscle,
                WaistCircumference = dto.Waist,
                Notes = dto.Notes,
                RecordedDate = dto.Date,
                IsClientEntry = false,
                RecordedByDietitianId = dietitianId,
            };

            await _progressDal.AddAsync(progress);

            var client = await _clientDal.GetByIdAsync(dto.ClientId);
            client.CurrentWeight = dto.Weight;
            await _clientDal.UpdateAsync(client);

            dto.Id = progress.Id;
            return new Response<ProgressDto>(ResponseCode.Success, dto);
        }
        public async Task<Response<ProgressDto>> UpdateProgressByDietitianAsync(ProgressDto dto)
        {
            var progress = await _progressDal.GetByIdAsync(dto.Id);
            if (progress == null)
                return new Response<ProgressDto>(ResponseCode.NotFound, "Kayıt bulunamadı");

            progress.Weight = dto.Weight;
            progress.BodyFatPercentage = dto.BodyFat;
            progress.MuscleMass = dto.Muscle;
            progress.WaistCircumference = dto.Waist;
            progress.Notes = dto.Notes;
            progress.UpdatedAt = DateTime.UtcNow;

            await _progressDal.UpdateAsync(progress);
            return new Response<ProgressDto>(ResponseCode.Success, dto);
        }
        public async Task<Response<bool>> DeleteProgressByDietitianAsync(int id)
        {
            var progress = await _progressDal.GetByIdAsync(id);
            if (progress == null)
                return new Response<bool>(ResponseCode.NotFound, "Kayıt bulunamadı");

            await _progressDal.DeleteAsync(progress.Id);
            return new Response<bool>(ResponseCode.Success, true);
        }
        private ProgressDto MapToDto(ClientProgress progress)
        {
            return new ProgressDto
            {
                Id = progress.Id,
                ClientId = progress.ClientId,
                Weight = progress.Weight,
                BodyFat = progress.BodyFatPercentage,
                Muscle = progress.MuscleMass,
                Waist = progress.WaistCircumference,
                Notes = progress.Notes,
                Date = progress.RecordedDate
            };
        }
    }
}
