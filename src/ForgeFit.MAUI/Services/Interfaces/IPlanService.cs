using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Plan;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IPlanService
{
    Task<ServiceResponse<PlanDto?>> GeneratePlanAsync(CancellationToken cancellationToken = default);
    Task<ServiceResponse<PlanDto>> ConfirmPlanAsync(PlanDto planDto, CancellationToken cancellationToken = default);
    Task<ServiceResponse<PlanDto?>> GetPlanAsync(CancellationToken cancellationToken = default);
}
