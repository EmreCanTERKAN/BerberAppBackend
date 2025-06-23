using BerberApp_Backend.Application.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace BerberApp_Backend.WebApi.Controllers;

[Route("odata")]
[ApiController]
[EnableQuery]
public class AppODataController(
    ISender sender) : ODataController
{
    public static IEdmModel GetEdmModel()
    {
        ODataConventionModelBuilder builder = new();
        builder.EnableLowerCamelCase();
        builder.EntitySet<EmployeeGetAllQueryResponse>("employees");
        return builder.GetEdmModel();
    }

    [HttpGet("employees")]
    public async Task<IActionResult> GetAllEmployees()
    {
        var response = await sender.Send(new EmployeeGetAllQuery());
        return Ok(response);
    }
}
