namespace clsoc;

public sealed class LineCounter
{
    public LineCountResult CountFile(string filePath)
    {
        return this.CountFile(filePath, LanguageRegistry.Legacy);
    }

    public LineCountResult CountFile(string filePath, LanguageDefinition language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(language);

        CommentBlockDefinition? activeBlockComment = null;
        int totalLines = 0;
        int codeLines = 0;
        int blankLines = 0;
        int commentLines = 0;

        foreach (string rawLine in File.ReadLines(filePath))
        {
            totalLines++;
            LineClassification classification = ClassifyLine(rawLine, language, ref activeBlockComment);

            if (classification.HasCode)
            {
                codeLines++;
            }
            else if (classification.HasComment)
            {
                commentLines++;
            }
            else
            {
                blankLines++;
            }
        }

        return new LineCountResult(1, totalLines, codeLines, blankLines, commentLines);
    }

    private static LineClassification ClassifyLine(
        string line,
        LanguageDefinition language,
        ref CommentBlockDefinition? activeBlockComment)
    {
        bool hasCode = false;
        bool hasComment = false;
        bool isInsideString = false;
        bool isInsideChar = false;
        bool isInsideVerbatimString = false;
        bool hasPendingVerbatimPrefix = false;

        for (int index = 0; index < line.Length; index++)
        {
            char current = line[index];
            char? next = index + 1 < line.Length ? line[index + 1] : null;

            if (activeBlockComment is not null)
            {
                hasComment = true;

                if (StartsWith(line, index, activeBlockComment.EndToken))
                {
                    index += activeBlockComment.EndToken.Length - 1;
                    activeBlockComment = null;
                }

                continue;
            }

            if (isInsideVerbatimString)
            {
                hasCode = true;

                if (current == '"' && next == '"')
                {
                    index++;
                    continue;
                }

                if (current == '"')
                {
                    isInsideVerbatimString = false;
                }

                continue;
            }

            if (isInsideString)
            {
                hasCode = true;

                if (current == '\\')
                {
                    index++;
                    continue;
                }

                if (current == '"')
                {
                    isInsideString = false;
                }

                continue;
            }

            if (isInsideChar)
            {
                hasCode = true;

                if (current == '\\')
                {
                    index++;
                    continue;
                }

                if (current == '\'')
                {
                    isInsideChar = false;
                }

                continue;
            }

            if (char.IsWhiteSpace(current))
            {
                hasPendingVerbatimPrefix = false;
                continue;
            }

            CommentBlockDefinition? blockComment = FindBlockCommentAt(line, index, language);
            if (blockComment is not null)
            {
                hasComment = true;
                activeBlockComment = blockComment;
                index += blockComment.StartToken.Length - 1;
                hasPendingVerbatimPrefix = false;
                continue;
            }

            string? lineCommentToken = FindLineCommentTokenAt(line, index, language);
            if (lineCommentToken is not null)
            {
                hasComment = true;
                break;
            }

            if (current == '\'' && !language.LineCommentTokens.Contains("'", StringComparer.Ordinal))
            {
                isInsideChar = true;
                hasCode = true;
                hasPendingVerbatimPrefix = false;
                continue;
            }

            if (current == '"')
            {
                hasCode = true;

                if (hasPendingVerbatimPrefix)
                {
                    isInsideVerbatimString = true;
                    hasPendingVerbatimPrefix = false;
                    continue;
                }

                isInsideString = true;
                continue;
            }

            if (current == '@')
            {
                hasCode = true;
                hasPendingVerbatimPrefix = true;
                continue;
            }

            hasCode = true;
            hasPendingVerbatimPrefix = false;
        }

        return new LineClassification(hasCode, hasComment);
    }

    private static CommentBlockDefinition? FindBlockCommentAt(string line, int index, LanguageDefinition language)
    {
        return language.BlockComments.FirstOrDefault(blockComment => StartsWith(line, index, blockComment.StartToken));
    }

    private static string? FindLineCommentTokenAt(string line, int index, LanguageDefinition language)
    {
        return language.LineCommentTokens.FirstOrDefault(token => StartsWith(line, index, token));
    }

    private static bool StartsWith(string text, int startIndex, string token)
    {
        return startIndex + token.Length <= text.Length
            && string.CompareOrdinal(text, startIndex, token, 0, token.Length) == 0;
    }

    private readonly record struct LineClassification(bool HasCode, bool HasComment);
}
