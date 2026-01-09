using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Plan;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IPlanService
{
    Task<ServiceResponse<PlanDto?>> GeneratePlanAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<PlanDto>> ConfirmPlanAsync(PlanDto planDto, CancellationToken cancellationToken);
    Task<ServiceResponse<PlanDto?>> GetPlanAsync(CancellationToken cancellationToken);
}
