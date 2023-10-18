using System.Text;
using Newtonsoft.Json;
using Spectre.Console;
using TerminalGPT.Constants;
using TerminalGPT.Options;

namespace TerminalGPT.Extensions;

public static class AppExtensions
{
    public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);
    public static T FromJson<T>(this string json) => JsonConvert.DeserializeObject<T>(json);
    
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

    // generic extension method to get description from enum
    public static string GetDescription<T>(this T enumerationValue)
        where T : struct
    {
        var type = enumerationValue.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
        }

        var memberInfo = type.GetMember(enumerationValue.ToString());
        if (memberInfo.Length > 0)
        {
            var attrs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

            if (attrs.Length > 0)
            {
                return ((System.ComponentModel.DescriptionAttribute) attrs[0]).Description;
            }
        }

        return enumerationValue.ToString();
    }

    public static string GetInputWithCursor(this string input, string caret, int cursorPosition, bool isCommand = false,
        bool escapeMarkup = true)
    {
        StringBuilder sb = new StringBuilder();

        return cursorPosition switch
        {
            _ when cursorPosition == 0 && !isCommand => sb
                .Append(caret)
                .Append(input.GetEscapedMarkup(escapeMarkup))
                .ToString(),

            _ when cursorPosition == 0 && isCommand => sb
                .Append(caret)
                .Append("[gold3][bold]")
                .Append(input.GetEscapedMarkup(escapeMarkup))
                .Append("[/][/]")
                .ToString(),

            _ when cursorPosition == input.Length && !isCommand => sb
                .Append(input.GetEscapedMarkup(escapeMarkup))
                .Append(caret)
                .ToString(),

            _ when cursorPosition == input.Length && isCommand => sb
                .Append("[gold3][bold]")
                .Append(input.GetEscapedMarkup(escapeMarkup))
                .Append("[/][/]")
                .Append(caret)
                .ToString(),

            _ => SandwichCursor(input, caret, cursorPosition, escapeMarkup, sb, isCommand)
        };
    }

    private static string SandwichCursor(string input, string caret, int cursorPosition, bool escapeMarkup,
        StringBuilder sb, bool isCommand)
    {
        if (!isCommand)
        {
            return sb.Append(input[..cursorPosition].GetEscapedMarkup(escapeMarkup))
                .Append(caret)
                .Append(input[cursorPosition..].GetEscapedMarkup(escapeMarkup))
                .ToString();
        }
        else
        {
            return sb.Append("[gold3][bold]")
                .Append(input[..cursorPosition].GetEscapedMarkup(escapeMarkup))
                .Append("[/][/]")
                .Append(caret)
                .Append("[gold3][bold]")
                .Append(input[cursorPosition..].GetEscapedMarkup(escapeMarkup))
                .Append("[/][/]")
                .ToString();
        }
    }


    public static string GetEscapedMarkup(this string input, bool escapeMarkup = true) =>
        escapeMarkup ? Markup.Escape(input) : input;

    public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);
}