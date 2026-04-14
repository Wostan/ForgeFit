using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Aggregates.FoodAggregate;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class CustomFoodService(
    IFoodProductRepository foodProductRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : ICustomFoodService
{
    public async Task<CustomFoodDto> CreateAsync(Guid userId, CustomFoodCreateRequest request)
    {
        if (!await userRepository.ExistsAsync(userId)) throw new NotFoundException("User not found");

        var foodProduct = FoodProduct.Create(
            null,
            userId,
            request.Name,
            request.Brand,
            request.Barcode,
            request.Calories,
            request.Carbs,
            request.Protein,
            request.Fat,
            request.Fiber,
            request.Sugar,
            request.SaturatedFat,
            request.Sodium,
            request.ServingSize,
            request.ServingUnit
        );

        await foodProductRepository.AddAsync(foodProduct);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<CustomFoodDto>(foodProduct);
    }

    public async Task<CustomFoodDto> GetByIdAsync(Guid userId, Guid foodId)
    {
        var foodProduct = await foodProductRepository.GetByIdAsync(foodId);

        if (foodProduct == null) throw new NotFoundException("Custom food not found");

        if (userId != foodProduct.UserId) throw new UnauthorizedAccessException("You do not own this custom food");

        return mapper.Map<CustomFoodDto>(foodProduct);
    }

    public async Task<List<CustomFoodDto>> GetAllForUserAsync(Guid userId)
    {
        var foodProducts = await foodProductRepository.GetAllByUserIdAsync(userId);
        return mapper.Map<List<CustomFoodDto>>(foodProducts);
    }

    public async Task<CustomFoodDto> UpdateAsync(Guid userId, Guid foodId, CustomFoodUpdateRequest request)
    {
        var foodProduct = await foodProductRepository.GetByIdAsync(foodId);

        if (foodProduct == null) throw new NotFoundException("Custom food not found");

        if (userId != foodProduct.UserId) throw new UnauthorizedAccessException("You do not own this custom food");

        foodProduct.UpdateDetails(request.Name, request.Brand, request.Barcode);
        foodProduct.UpdateNutrients(
            request.Calories,
            request.Carbs,
            request.Protein,
            request.Fat,
            request.Fiber,
            request.Sugar,
            request.SaturatedFat,
            request.Sodium
        );
        foodProduct.UpdateServingInfo(request.ServingSize, request.ServingUnit);

        await unitOfWork.SaveChangesAsync();

        return mapper.Map<CustomFoodDto>(foodProduct);
    }

    public async Task DeleteAsync(Guid userId, Guid foodId)
    {
        var foodProduct = await foodProductRepository.GetByIdAsync(foodId);

        if (foodProduct == null) throw new NotFoundException("Custom food not found");

        if (userId != foodProduct.UserId) throw new UnauthorizedAccessException("You do not own this custom food");

        foodProductRepository.Remove(foodProduct);
        await unitOfWork.SaveChangesAsync();
    }
}
