using Microsoft.AspNetCore.Builder;

public static class Routing
{
    public static void Include(IApplicationBuilder app)
    {
        app.UseMvc(routes =>
        {
            // /
            routes.MapRoute(null, "", new
            {
                controller = "Home",
                action = "Index",
                category = "",
                page = 1
            });

            // Page2
            routes.MapRoute(null,
                "Page{page}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    category = ""
                },
                new { page = @"\d+" }
            );
        }
        );
    }
}