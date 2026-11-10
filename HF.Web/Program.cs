
using HF.Web.Components;
using HF.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor z interaktivnim strežniškim izvajanjem
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registracija HttpClient za komunikacijo z Web API
builder.Services.AddHttpClient<EventApiClient>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"]
        ?? throw new InvalidOperationException(
            "API BaseUrl ni konfiguriran.");

    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
