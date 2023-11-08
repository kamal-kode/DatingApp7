using System.Text.Json;
using API.Helpers;

namespace API;

public static class HttpExtensions
{
    private const string pagination = "Pagination";

    public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header){
    var jsonOptions= new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
    response.Headers.Add(pagination, JsonSerializer.Serialize(header, jsonOptions));
    response.Headers.Add("Access-Control-Expose-Headers", pagination);

   }
}
