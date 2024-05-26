using ContactProvider.Data.Contexts;
using ContactProvider.Data.Entities;
using ContactProvider.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ContactProvider.Functions;

public class ContactRequestSender
{
    private readonly ILogger<ContactRequestSender> _logger;
    private readonly IDbContextFactory<DataContext> _contextFactory;

    public ContactRequestSender(ILogger<ContactRequestSender> logger, IDbContextFactory<DataContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    [Function("ContactRequestSender")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        ContactRequest model;
        try
        {
            model = JsonConvert.DeserializeObject<ContactRequest>(requestBody);
            if (model == null)
            {
                throw new JsonException("Deserialized object is null");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError($"JSON deserialization error: {ex.Message}");
            var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid request payload");
            return badRequestResponse;
        }

        try
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var entity = new ContactRequestEntity
                {
                    Fullname = model.Fullname,
                    Email = model.Email,
                    Service = model.Service,
                    Message = model.Message
                };

                context.ContactRequests.Add(entity);
                await context.SaveChangesAsync();
            }

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync("A contact request was sent!");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while saving the contact request: {ex.Message}");
            var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("An error occurred while processing your request.");
            return errorResponse;
        }
    }
}


