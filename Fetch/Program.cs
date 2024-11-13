using System.Text;
using System.Text.Json;

string address = string.Empty;
string requestType = "GET";
string postData = string.Empty;
string apiKey = string.Empty;

foreach (var arg in args)
{
    if (arg.StartsWith("http://") || arg.StartsWith("https://"))
    {
        address = arg;
    }
    else if (arg == "-p" || arg == "-P") 
    {
        requestType = "POST";
    }
    else if (arg.Contains("data="))
    {
        try
        {
            postData = arg.Split("data=")[1];
        }
        catch (IndexOutOfRangeException) 
        {
            throw new Exception("No post data was specified.");
        }
    }
    else if (arg.Contains("key="))
    {
        apiKey = arg.Split("key=")[1];
    }
    else
    {

    }
}

if (address != string.Empty)
{
    using var httpClient = new HttpClient();

    if (apiKey != string.Empty)
    {
        Console.WriteLine(httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", apiKey));
    }

    HttpResponseMessage? response = null;

    if (requestType == "GET")
    {
        try
        {
            response = await httpClient.GetAsync(address);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    else if (requestType == "POST")
    {
        StringContent? jsonString = null;

        if (postData != string.Empty)
            jsonString = new StringContent(JsonSerializer.Serialize(postData), Encoding.UTF8, "application/json");

        try
        {
            response = await httpClient.PostAsync(address, jsonString);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    if (response is null)
    {
        throw new Exception("Response was null.");
    }
    if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Request returned {response.StatusCode}.");
    }

    Console.WriteLine(response.Content);
}
else
{
    Console.Write("No address was specified.");
}