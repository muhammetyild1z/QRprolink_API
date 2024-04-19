using Aspose.Pdf;
using Microsoft.AspNetCore.Mvc;


namespace ImageGenius_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesConverterController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FilesConverterController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost("c-pdf-doc")]
        public IActionResult ConvertToDoc(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return BadRequest();
            }

            // PDF dosyasını DOC dönüştür
            Document pdfDocument = new Document(path);
            DocSaveOptions saveOptions = new DocSaveOptions
            {
                Format = DocSaveOptions.DocFormat.Doc,
                Mode = DocSaveOptions.RecognitionMode.Flow,
                RelativeHorizontalProximity = 2.5f,
                RecognizeBullets = true
            };
            pdfDocument.Save(path + ".doc", saveOptions);


            //  indir
            string fileName = Path.GetFileName(path) + ".docx";
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, fileName);

            return PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }

        [HttpPost("c-pdf-docx")]
        public IActionResult ConvertToDocx(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return BadRequest();
            }
            Document pdfDocument = new Document(path);
            DocSaveOptions saveOptions = new DocSaveOptions
            {
                // Specify the output format as DOCX
                Format = DocSaveOptions.DocFormat.DocX,
                // Set other DocSaveOptions params
                Mode = DocSaveOptions.RecognitionMode.EnhancedFlow
            };
            // Save document in docx format
            pdfDocument.Save(path + ".docx", saveOptions);
            //  indir
            string fileName = Path.GetFileName(path) + ".docx";
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, fileName);

            return PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }

        [HttpPost("c-pdf-xlsx")]
        public IActionResult ConvertToxlsx(string path)
        {
            
            if (!System.IO.File.Exists(path))
            {
                return BadRequest("File not found");
            }

            // Load PDF document
            Document pdfDocument = new Document(path);

            
            Aspose.Pdf.ExcelSaveOptions excelsave = new ExcelSaveOptions();

            
            string outputPath = path + ".xlsx";
            pdfDocument.Save(outputPath, excelsave);

 //  indir
            byte[] fileBytes = System.IO.File.ReadAllBytes(outputPath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "output.xlsx");
        }

    }
}
