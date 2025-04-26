using backend.Dto;

namespace backend.Mappers;
public static class FormDtoMapper
{
    public static Form DtoToForm(this FormCreateDto dto)
    {
        var NameOfForm = dto.FormName;
        var Questions = dto.Questions;
        var Obligatoriness = dto.Obligatoriness;
        var NumberOfAnswers = dto.NumberOfOptions;
        var Answers = dto.Options;
        // Firstly, chack if number validation of arguments
        if (Questions.Count() != NumberOfAnswers.Count())
            throw new InvalidDataException("Number of questions and collection of number of answers for it have different sizes!");
        if (Questions.Count() != Obligatoriness.Count())
            throw new InvalidDataException("Number of questions and obligatoriness for it have different sizes!");
        if (!CheckIfNumberOfAnswersIsSameAsDeclared(NumberOfAnswers, Answers))
            throw new InvalidDataException("Number of answers isn't proper to number of declared answers!");
        // After, check if Form does already exist in a database.
        var form = new Form 
        {
            NameOfForm = NameOfForm,
            Questions = Questions.Select(q => new Question
            {
                Name = q,
            }).ToList(),
        };  
        int sum = 0;
        foreach (var number in NumberOfAnswers)
            sum += number;
        var arrAnswers = Answers.ToArray();
        var arrNumbers = NumberOfAnswers.ToArray();
        var arrObligatoriness = Obligatoriness.ToArray();
        int numberOfPassedOptions = 0;
        for (int i = 0; i < NumberOfAnswers.Count(); i++)
        {
            form.Questions[i].IsObligatory = arrObligatoriness[i];
            form.Questions[i].FormForWhichCorrespond = form;
            form.Questions[i].Options = arrAnswers[numberOfPassedOptions..(numberOfPassedOptions+arrNumbers[i])]
                .Select(answ => new OptionForQuestion
            {
                Question = form.Questions[i],
                Name = answ
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