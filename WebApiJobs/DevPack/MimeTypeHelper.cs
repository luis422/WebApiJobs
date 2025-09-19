using System.Net.Mime;

namespace WebApiJobs.DevPack
{
    public class MimeTypeHelper
    {
        public static string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".bmp" => MediaTypeNames.Image.Bmp,
                ".gif" => MediaTypeNames.Image.Gif,
                ".svg" => MediaTypeNames.Image.Svg,
                ".md" => MediaTypeNames.Text.Markdown,
                ".ico" => "image/vnd.microsoft.icon",
                ".jar" => "application/java-archive",
                ".js" or ".mjs" => MediaTypeNames.Text.JavaScript,
                ".htm" or ".html" => MediaTypeNames.Text.Html,
                ".xhtml" => "application/xhtml+xml",
                ".txt" or ".log" => MediaTypeNames.Text.Plain,
                ".css" => MediaTypeNames.Text.Css,
                ".csv" => MediaTypeNames.Text.Csv,
                ".rtf" => MediaTypeNames.Application.Rtf,
                ".zip" => MediaTypeNames.Application.Zip,
                ".rar" => "application/vnd.rar",
                ".7z" => "application/x-7z-compressed",
                ".xml" => MediaTypeNames.Text.Xml,
                ".json" => MediaTypeNames.Application.Json,
                ".pdf" => MediaTypeNames.Application.Pdf,
                ".jpg" or ".jpeg" => MediaTypeNames.Image.Jpeg,
                ".png" => MediaTypeNames.Image.Png,
                #region Microsoft Office MIME types
                ".doc" => "application/msword",
                ".dot" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".dotx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
                ".docm" => "application/vnd.ms-word.document.macroEnabled.12",
                ".dotm" => "application/vnd.ms-word.template.macroEnabled.12",
                ".xls" => "application/vnd.ms-excel",
                ".xlt" => "application/vnd.ms-excel",
                ".xla" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xltx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.template",
                ".xlsm" => "application/vnd.ms-excel.sheet.macroEnabled.12",
                ".xltm" => "application/vnd.ms-excel.template.macroEnabled.12",
                ".xlam" => "application/vnd.ms-excel.addin.macroEnabled.12",
                ".xlsb" => "application/vnd.ms-excel.sheet.binary.macroEnabled.12",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pot" => "application/vnd.ms-powerpoint",
                ".pps" => "application/vnd.ms-powerpoint",
                ".ppa" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".potx" => "application/vnd.openxmlformats-officedocument.presentationml.template",
                ".ppsx" => "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
                ".ppam" => "application/vnd.ms-powerpoint.addin.macroEnabled.12",
                ".pptm" => "application/vnd.ms-powerpoint.presentation.macroEnabled.12",
                ".potm" => "application/vnd.ms-powerpoint.template.macroEnabled.12",
                ".ppsm" => "application/vnd.ms-powerpoint.slideshow.macroEnabled.12",
                ".mdb" => "application/vnd.ms-access",
                #endregion
                _ => MediaTypeNames.Application.Octet // Tipo genérico se não reconhecido
            };
        }
    }
}
