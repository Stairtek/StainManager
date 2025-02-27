namespace StainManager.Application.Common.Helpers;

public interface ICodeGenerator
{
    string GenerateCode(int length);
}

public class CodeGenerator
    : ICodeGenerator
{
    private readonly char[] _chars = "ABCDEFGHIJKMNPQRSTUVWXYZ23456789".ToCharArray();

    public string GenerateCode(int length) 
    {
        if (length < 5)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than 5.");
        
        var random = new Random();
        var codeChars = random.GetItems(_chars, length);
        
        return new string(codeChars);
    }
}