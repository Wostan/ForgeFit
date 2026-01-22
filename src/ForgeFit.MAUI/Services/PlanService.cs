using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Plan;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class PlanService(IApiService apiService) : IPlanService
{
    public async Task<ServiceResponse<PlanDto?>> GeneratePlanAsync(CancellationToken cancellationToken)
    {
        return await apiService.GetAsync<PlanDto?>("api/plan/generate", cancellationToken);
    }

    public async Task<ServiceResponse<PlanDto>> ConfirmPlanAsync(PlanDto planDto, CancellationToken cancellationToken)
    {
        return await apiService.PostAsync<PlanDto, PlanDto>("api/plan/confirm", planDto, cancellationToken);
    }

    public async Task<ServiceResponse<PlanDto?>> GetPlanAsync(CancellationToken cancellationToken)
    {
        return await apiService.GetAsync<PlanDto?>("api/plan", cancellationToken);
    }
}
