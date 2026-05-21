namespace clsoc;

public sealed class LineCounter
{
    public LineCountResult CountFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        bool isInsideBlockComment = false;
        int totalLines = 0;
        int codeLines = 0;
        int blankLines = 0;
        int commentLines = 0;

        foreach (string rawLine in File.ReadLines(filePath))
        {
            totalLines++;
            LineClassification classification = ClassifyLine(rawLine, ref isInsideBlockComment);

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

    private static LineClassification ClassifyLine(string line, ref bool isInsideBlockComment)
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

            if (isInsideBlockComment)
            {
                hasComment = true;

                if (current == '*' && next == '/')
                {
                    isInsideBlockComment = false;
                    index++;
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

            if (current == '/' && next == '/')
            {
                hasComment = true;
                break;
            }

            if (current == '/' && next == '*')
            {
                hasComment = true;
                isInsideBlockComment = true;
                index++;
                hasPendingVerbatimPrefix = false;
                continue;
            }

            if (current == '\'')
            {
                if (hasCode)
                {
                    isInsideChar = true;
                    hasPendingVerbatimPrefix = false;
                    continue;
                }

                hasComment = true;
                break;
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

    private readonly record struct LineClassification(bool HasCode, bool HasComment);
}
