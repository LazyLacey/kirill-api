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
        public string image { get; set; }
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
    public async Task<ActionResult> AddImagePage(Req request)
    {

        // Create an empty page
        PdfPage page = _document.AddPage();
        page.Size = PageSize.A4;
        page.Orientation = PageOrientation.Landscape;
        


        // Get an XGraphics object for drawing
        XGraphics gfx = XGraphics.FromPdfPage(page);
        
        byte[] data = Convert.FromBase64String(FixBase64ForImage(request.image));
        
       
        XImage imageToPdf;

        
        using (MemoryStream memstr = new MemoryStream(data, 0, 0, true, true))
        {
            
            imageToPdf = XImage.FromStream(memstr);
            double x = (imageToPdf.PixelWidth / imageToPdf.PixelHeight) * page.Width;

            gfx.DrawImage(imageToPdf, 0, 0, x , page.Height);
        
            return Ok();
        }
        


        // HttpWebRequest req = (HttpWebRequest)WebRequest.Create(request.image);
        // HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        //
        // XImage imageToPdf;
        //
        // if (res.StatusCode == HttpStatusCode.OK)
        // {
        //    imageToPdf = XImage.FromStream(res.GetResponseStream());
        //    double x = (imageToPdf.PixelWidth / imageToPdf.PixelHeight) * page.Width;
        //
        //    // gfx.DrawImage(imageToPdf, x, 0);
        //
        //
        //    gfx.DrawImage(imageToPdf, 0, 0, x , page.Height);
        //
        //    return Ok();
        // }
        //
        

        // XImage imageToPdf = XImage.FromFile(request.image);
        
        // double x = (250 - imageToPdf.PixelWidth * 72 / imageToPdf.HorizontalResolution) / 2;
        //
        // gfx.DrawImage(imageToPdf, x, 0);
        //
        //
        // // gfx.DrawImage(imageToPdf, 0, 0, width, height);
        //
        return StatusCode(400);
    }
    
    
    [HttpGet]
    [Route("save_doc")]
    public ActionResult SavePdf()
    {
        
        _document.Save("Презенташка.pdf");
        Console.WriteLine(_document);
        return Ok();
    }
    
}