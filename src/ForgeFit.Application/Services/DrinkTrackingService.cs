using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Aggregates.FoodAggregate;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class DrinkTrackingService(
    IDrinkEntryRepository drinkEntryRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IDrinkTrackingService
{
    public async Task<DrinkEntryResponse> LogDrinkEntryAsync(Guid userId, DrinkEntryCreateRequest request)
    {
        if (!await userRepository.ExistsAsync(userId))
        {
            throw new NotFoundException("User not found");
        }

        var drinkEntry = DrinkEntry.Create(
            userId, 
            request.VolumeMl, 
            request.Date);
        
        await drinkEntryRepository.AddAsync(drinkEntry);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<DrinkEntryResponse>(drinkEntry);
    }

    public async Task<DrinkEntryResponse> UpdateDrinkEntryAsync(Guid userId, Guid entryId,
        DrinkEntryCreateRequest request)
    {
        var drinkEntry = await drinkEntryRepository.GetByIdAsync(entryId);
        
        if (drinkEntry == null)
        {
            throw new NotFoundException("Drink entry not found");
        }

        if (userId != drinkEntry.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this drink entry");
        }

        drinkEntry.Update(request.VolumeMl, request.Date);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<DrinkEntryResponse>(drinkEntry);
    }

    public async Task DeleteDrinkEntryAsync(Guid userId, Guid entryId)
    {
        var drinkEntry = await drinkEntryRepository.GetByIdAsync(entryId);
        
        if (drinkEntry == null)
        {
            throw new NotFoundException("Drink entry not found");
        }

        if (userId != drinkEntry.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this drink entry");
        }

        drinkEntryRepository.Remove(drinkEntry);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<DrinkEntryResponse> GetDrinkEntryAsync(Guid userId, Guid entryId)
    {
        var drinkEntry = await drinkEntryRepository.GetByIdAsync(entryId);
        
        if (drinkEntry == null)
        {
            throw new NotFoundException("Drink entry not found");
        }

        if (userId != drinkEntry.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this drink entry");
        }

        return mapper.Map<DrinkEntryResponse>(drinkEntry);
    }

    public async Task<List<DrinkEntryResponse>> GetDrinkEntriesByDateAsync(Guid userId, DateTime date)
    {
        var drinkEntries = await drinkEntryRepository.GetAllByUserIdAndDateAsync(userId, date);
        return mapper.Map<List<DrinkEntryResponse>>(drinkEntries);
    }

    public async Task<List<DrinkEntryResponse>> GetDrinkEntriesByDateAsync(Guid userId, DateTime from, DateTime to)
    {
        var drinkEntries = await drinkEntryRepository.GetAllByUserIdAndDateRangeAsync(userId, from, to);
        return mapper.Map<List<DrinkEntryResponse>>(drinkEntries);
    }
}