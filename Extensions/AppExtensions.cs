using TerminalGPT.Constants;
using TerminalGPT.Options;

namespace TerminalGPT.Extensions;

public static class AppExtensions
{
    public static bool In<T>(this T obj, params T[] values)
    {
        return values.Contains(obj);
    }
    
    public static bool In(this object obj, params object[] values)
    {
        return values.Contains(obj);
    }

    public static string GetId(this GPTModel? model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        
        return model switch
        {
            GPTModel.GPT4 => AppConstants.ModelDictionary[GPTModel.GPT4],
            GPTModel.GPT4_32k => AppConstants.ModelDictionary[GPTModel.GPT4_32k],
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
        };
    }
}