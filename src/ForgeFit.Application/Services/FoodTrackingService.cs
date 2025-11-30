using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Domain.ValueObjects.FoodValueObjects;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class FoodTrackingService(
    IFoodEntryRepository foodEntryRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IFoodTrackingService
{
    public async Task<FoodEntryDto> LogFoodEntryAsync(Guid userId, FoodEntryCreateRequest request)
    {
        if (!await userRepository.ExistsAsync(userId))
        {
            throw new NotFoundException("User not found");
        }

        var foodItems = GetFoodItemsFromDto(request.FoodItems);

        var foodEntry = FoodEntry.Create(
            userId,
            request.DayTime,
            request.Date,
            foodItems
        );

        await foodEntryRepository.AddAsync(foodEntry);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<FoodEntryDto>(foodEntry);
    }

    public async Task<FoodEntryDto> UpdateFoodEntryAsync(Guid userId, Guid entryId, FoodEntryCreateRequest request)
    { 
        var foodEntry = await foodEntryRepository.GetByIdAsync(entryId);
        
        if (foodEntry == null)
        {
            throw new NotFoundException("Food entry not found");
        }

        if (userId != foodEntry.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this food entry");
        }

        var foodItems = GetFoodItemsFromDto(request.FoodItems);

        foodEntry.Update(request.DayTime, request.Date, foodItems);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<FoodEntryDto>(foodEntry);
    }

    public async Task DeleteFoodEntryAsync(Guid userId, Guid entryId)
    {
        var foodEntry = await foodEntryRepository.GetByIdAsync(entryId);
        
        if (foodEntry == null)
        {
            throw new NotFoundException("Food entry not found");
        }

        if (userId != foodEntry.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this food entry");
        }

        foodEntryRepository.Remove(foodEntry);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<FoodEntryDto> GetFoodEntryAsync(Guid userId, Guid entryId)
    {
        var foodEntry = await foodEntryRepository.GetByIdWithNavigationsAsync(entryId);
        
        if (foodEntry == null)
        {
            throw new NotFoundException("Food entry not found");
        }

        if (userId != foodEntry.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this food entry");
        }

        return mapper.Map<FoodEntryDto>(foodEntry);
    }

    public async Task<List<FoodEntryDto>> GetFoodEntriesByDateAsync(Guid userId, DateTime date)
    {
        var foodEntries = await foodEntryRepository.GetAllByUserIdAndDateAsync(userId, date);
        return mapper.Map<List<FoodEntryDto>>(foodEntries);
    }

    public async Task<List<FoodEntryDto>> GetFoodEntriesByDateAsync(Guid userId, DateTime from, DateTime to)
    {
        var foodEntries = await foodEntryRepository.GetAllByUserIdAndDateRangeAsync(userId, from, to);
        return mapper.Map<List<FoodEntryDto>>(foodEntries);
    }
    
    private static HashSet<FoodItem> GetFoodItemsFromDto(List<FoodItemDto> foodItemDtos)
    {
        return foodItemDtos.Select(dto => new FoodItem(
            dto.ExternalId,
            dto.Label,
            dto.Calories,
            dto.Carbs,
            dto.Protein,
            dto.Fat,
            dto.ServingUnit,
            dto.Amount
        )).ToHashSet();
    }
}