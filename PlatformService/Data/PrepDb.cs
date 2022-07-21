using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;
public static class PrebDb {
    public static void  PrepPopulation(IApplicationBuilder app, bool isProd){
        if(isProd){
                Console.WriteLine("true");
        }
        else{
                Console.WriteLine("false");
        }
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!,isProd);
            }
    }

    private static void SeedData(AppDbContext context, bool isProd){
        if(isProd){
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                        context.Database.Migrate();
                }
                catch (Exception ex)
                {
                        Console.WriteLine($"--> Could Not Run Migrations : {ex.Message}");                                                
                }
        }
            if(!context.Platforms.Any()){
                 Console.WriteLine("--> Seeding Data...");
                 context.AddRange(
                    new Platform{Name="DotNet" , Publisher="Microsoft" , Cost = "Free"},                    
                    new Platform{Name="Sql Server Express" , Publisher="Microsoft" , Cost = "Free"},                    
                    new Platform{Name="Kubernates" , Publisher="Cloud Native Computing Foundation" , Cost = "Free"}                    
                    );

                    context.SaveChanges();
            }
            else{
                 Console.WriteLine("--> We Already Have Data");
            }
    }
}