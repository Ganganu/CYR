using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace CYR.PDF
{
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
}
