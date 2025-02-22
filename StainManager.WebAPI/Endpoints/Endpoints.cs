namespace StainManager.WebAPI.Endpoints;

public static class Endpoints
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapGroup("/api/species").MapSpeciesEndpoints();

        return app;
    }
}