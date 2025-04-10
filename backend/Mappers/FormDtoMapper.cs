using backend.Dto;


namespace backend.Mappers;
public static class FormDtoMapper
{
    public static Form DtoToForm(this FormDto dto)
    {
        var NameOfForm = dto.FormName;
        var Questions = dto.Questions;
        var NumberOfAnswers = dto.NumberOfAnswers;
        var Answers = dto.Answers;
        // Firstly, chack if number validation of arguments
        if (Questions.Count() != NumberOfAnswers.Count())
            throw new InvalidDataException("Number of questions and collection of number of answers for it have different sizes!");
        if (!CheckIfNumberOfAnswersIsSameAsDeclared(NumberOfAnswers, Answers))
            throw new InvalidDataException("Number of answers isn't proper to number of declared answers!");
        // After, check if Form does already exist in a database.
        var form = new Form 
        {
            NameOfForm = NameOfForm,
            Obligatory = Questions.Select(q => new ObligatoryPreference
            {
                Name = q
            }).ToList(),
        };  
        int sum = 0;
        foreach (var number in NumberOfAnswers)
            sum += number;
        var arrAnswers = Answers.ToArray();
        var arrNumbers = NumberOfAnswers.ToArray();
        int numberOfPassedOptions = 0;
        for (int i = 0; i < NumberOfAnswers.Count(); i++)
        {
            form.Obligatory[i].FormForWhichCorrespond = form;
            form.Obligatory[i].Options = arrAnswers[numberOfPassedOptions..(numberOfPassedOptions+arrNumbers[i])]
                .Select(a => new OptionForObligatoryPreference
            {
                Preference = form.Obligatory[i],
                OptionForPreference = a 
            }).ToList();
            numberOfPassedOptions += arrNumbers[i];
        }
        return form;
    }

    
    public static bool CheckIfNumberOfAnswersIsSameAsDeclared(IEnumerable<int> NumberOfAnswers, IEnumerable<string> Answers)
    {
        int sum  = 0;
        foreach (int number in NumberOfAnswers)
        {
            sum += number;
        }
        return Answers.Count() == sum;
    }

}