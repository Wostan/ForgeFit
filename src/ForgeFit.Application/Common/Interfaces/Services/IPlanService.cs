using ForgeFit.Application.DTOs.Plan;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IPlanService
{
    Task<PlanDto> GeneratePlanAsync(Guid userId);
    Task<PlanDto> ConfirmPlanAsync(Guid userId, PlanDto plan);

    Task<PlanDto> GetPlanAsync(Guid userId);
}
