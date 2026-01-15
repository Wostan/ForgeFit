namespace ForgeFit.MAUI.Helpers;

public class RegistrationStepTemplateSelector : DataTemplateSelector
{
    public DataTemplate Step1Credentials { get; set; }
    public DataTemplate Step2Personal { get; set; }
    public DataTemplate Step3Measurements { get; set; }
    public DataTemplate Step4Goal { get; set; }
    public DataTemplate Step5Commitment { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            "Credentials" => Step1Credentials,
            "Personal" => Step2Personal,
            "Measurements" => Step3Measurements,
            "Goal" => Step4Goal,
            "Commitment" => Step5Commitment,
            _ => Step1Credentials
        };
    }
}
