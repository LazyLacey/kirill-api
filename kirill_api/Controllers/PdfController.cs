using System.Drawing;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Mvc;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace kirill_api.Controllers;

[ApiController]
[Route("/")]
public class PdfController : ControllerBase
{
    private static PdfDocument _document = new PdfDocument();
    
    public class Req
    {
        public IFormFile image { get; set; }
    }
    
    public static string FixBase64ForImage(string image) {
        StringBuilder sbText = new StringBuilder(image, image.Length);
        sbText.Replace("\r\n", String.Empty);
        sbText.Replace(" ", String.Empty);
        return sbText.ToString();
    }

    [HttpGet]
    [Route("create_doc")]
    public ActionResult CreateNewDoc()
    {
        _document = new PdfDocument();
        return Ok();
    }
    
    [HttpPost]
    [Route("add_image")]
    public async Task<ActionResult> AddImagePage([FromForm]Req request)
    {

        // Create an empty page
        PdfPage page = _document.AddPage();
        page.Size = PageSize.A4;
        page.Orientation = PageOrientation.Landscape;
        


        // Get an XGraphics object for drawing
        XGraphics gfx = XGraphics.FromPdfPage(page);
        
       
        XImage imageToPdf;
        
        var ms = new MemoryStream();
        request.image.CopyTo(ms);
        imageToPdf = XImage.FromStream(ms);
        
        double x = (imageToPdf.PixelWidth / imageToPdf.PixelHeight) * page.Width;

        gfx.DrawImage(imageToPdf, 0, 0, x , page.Height);
        
        return Ok();
    }
    
    
    [HttpGet]
    [Route("save_doc")]
    public FileStreamResult SavePdf()
    {
        
        var path = "static";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        _document.Save("static\\Презенташка.pdf");
        Console.WriteLine(_document.FullPath);
        // return Ok("/StaticFiles/Презенташка.pdf");
        
        var stream = new FileStream("static\\Презенташка.pdf", FileMode.Open);
        return File(stream, "application/pdf", "Презенташка.pdf");
    }
    
}