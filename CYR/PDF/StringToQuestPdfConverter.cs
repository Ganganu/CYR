using QuestPDF.Infrastructure;

namespace CYR.PDF;

public static class StringToQuestPdfConverter
{
    public static FontWeight ToFontWeight(string str)
    {
        var fontWeight = str switch
        {
            "Bold" => FontWeight.Bold,
            _ => FontWeight.Normal
        };
        return fontWeight;
    }
   
}
