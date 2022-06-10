using Ascent.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var services = builder.Services;

services.AddHttpClient("Ascent.Server.API", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Ascent.Server.API"));

services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("OIDC", options.ProviderOptions);
    options.ProviderOptions.DefaultScopes.Add("email"); // email doesn't seem to be included by default
    options.ProviderOptions.DefaultScopes.Add("ascent-blazor");
    options.ProviderOptions.ResponseType = "code";
    options.UserOptions.NameClaim = "name";
    options.UserOptions.ScopeClaim = "scope";

    // According to the comments in OpenIddict's Balosar sample, a bug currently affects Blazor WASM
    // when using the "fragment" response mode.
    options.ProviderOptions.ResponseMode = "query";
});

await builder.Build().RunAsync();
