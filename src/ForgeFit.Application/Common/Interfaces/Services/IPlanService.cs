using ForgeFit.Application.DTOs.Plan;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IPlanService
{
    Task<PlanDto> GeneratePlanAsync(GeneratePlanRequest request);
    Task ConfirmPlanAsync(Guid userId, PlanDto plan);
    
    Task<PlanDto> GetPlanAsync(Guid userId);
    Task UpdatePlanAsync(Guid userId, PlanDto plan);
}