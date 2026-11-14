using Microsoft.AspNetCore.Mvc;

namespace DemoMVC.Controllers;

[Route("demo-routing")]
public class RoutingDemoController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return Content("Route demo: GET /demo-routing");
    }

    [HttpGet("san-pham/{id:int}")]
    public IActionResult Product(int id)
    {
        return Content($"Mã sản phẩm: {id}");
    }

    [HttpGet("tim-kiem")]
    public IActionResult Search([FromQuery] string? keyword)
    {
        return Content($"Từ khóa: {keyword ?? "(trống)"}");
    }
}
