using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

public class WorkoutExercise : ValueObject
{
    public WorkoutExercise(
        string externalId,
        string name,
        Uri? gifUrl,
        IReadOnlyCollection<Muscle> targetMuscles,
        IReadOnlyCollection<BodyPart> bodyParts,
        IReadOnlyCollection<Equipment> equipment,
        string? instructions)
    {
        SetExternalId(externalId);
        SetName(name);
        SetGifUrl(gifUrl);
        SetTargetMuscles(targetMuscles);
        SetBodyParts(bodyParts);
        SetEquipment(equipment);
        SetInstructions(instructions);
    }

    private WorkoutExercise()
    {
    }

    public string ExternalId { get; private set; }
    public string Name { get; private set; }
    public Uri? GifUrl { get; private set; }
    public IReadOnlyCollection<Muscle> TargetMuscles { get; private set; }
    public IReadOnlyCollection<BodyPart> BodyParts { get; private set; }
    public IReadOnlyCollection<Equipment> Equipment { get; private set; }
    public string? Instructions { get; private set; }

    private void SetExternalId(string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new DomainValidationException("ExternalId cannot be null or whitespace");

        ExternalId = externalId;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Name cannot be null or whitespace");

        if (name.Length > 50)
            throw new DomainValidationException("Name must be less than 50 characters long");

        Name = name;
    }

    private void SetGifUrl(Uri? gifUrl)
    {
        if (gifUrl is not null && !gifUrl.IsAbsoluteUri)
            throw new DomainValidationException("GifUrl must be an absolute URI.");

        GifUrl = gifUrl;
    }

    private void SetTargetMuscles(IReadOnlyCollection<Muscle> targetMuscles)
    {
        if (targetMuscles.Count < 1)
            throw new DomainValidationException("TargetMuscles must contain at least one muscle.");

        TargetMuscles = targetMuscles ?? throw new DomainValidationException("TargetMuscles cannot be null");
    }

    private void SetBodyParts(IReadOnlyCollection<BodyPart> bodyParts)
    {
        if (bodyParts.Count < 1)
            throw new DomainValidationException("BodyParts must contain at least one body part.");

        BodyParts = bodyParts ?? throw new DomainValidationException("BodyParts cannot be null");
    }

    private void SetEquipment(IReadOnlyCollection<Equipment> equipment)
    {
        Equipment = equipment ?? throw new DomainValidationException("Equipment cannot be null");
    }

    private void SetInstructions(string? instructions)
    {
        if (instructions is not null && instructions.Length > 500)
            throw new DomainValidationException("Instructions must be less than 500 characters long");

        Instructions = instructions;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ExternalId;
    }
}