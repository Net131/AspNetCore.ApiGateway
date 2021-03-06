### Api Gateway Web Sockets

The Api Gateway supports calling downstream Web Sockets.

## Downstream Hub

Let us say, you have a downstream ChatHub:

```C#
public class ChatHub : Hub
{
    public async Task SendMessage(Request request)
    {
        await base.Clients.All.SendAsync("ReceiveMessage", request.Name, request.Message);
    }
}
```

## Your Gateway API

You talk to this Hub, set up a Hub and Route in your Gateway API's **Api Orchestrator**.

```C#
orchestrator.AddHub("chatservice", BuildHubConnection, "2f85e3c6-66d2-48a3-8ff7-31a65073558b")
                    .AddRoute("room", new HubRouteInfo { InvokeMethod = "SendMessage", ReceiveMethod = "ReceiveMessage", ReceiveParameterTypes = new Type[] { typeof(string), typeof(string) } });

```

The **ReceiveKey** (eg. Guid) is to be specified if you want to receive notifications from the downstream hub.

```C#
private static HubConnection BuildHubConnection(HubConnectionBuilder builder)
{
    return builder.WithUrl("http://localhost:53353/ChatHub")
                  .AddNewtonsoftJsonProtocol()
                  .Build();
}
```

In this you set up the **Connection** to the downstream Web Sockets using the **Asp Net Core SignalR Client**.

The **InvokeMethod** is the method that is called in the downstream Hub.

The **ReceiveMethod** is the method that will receive the notification from the Hub.

The Gateway provides a **POST** endpoint for accepting requests for downstream Hubs.

You can pass the data in the body of this post request.

You can pass a max of 10 objects in the request.

In Swagger, you would call this endpoint as below:

![API Gateway Swagger](https://github.com/VeritasSoftware/AspNetCore.ApiGateway/blob/master/Docs/WebSockets.PNG)

There is a **GatewayHub** which the Client can subscribe to to get messages from the back end Hub.

To hook up the GatewayHub, in your Gateway API project Startup.cs:

```C#
public void ConfigureServices(IServiceCollection services)
{
    //Hook up GatewayHub using SignalR
    services.AddSignalR().AddNewtonsoftJsonProtocol();          

    //Api gateway
    services.AddApiGateway();

    services.AddControllers();

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api Gateway", Version = "v1" });
    });            
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    .
    .
    app.UseEndpoints(endpoints =>
    {
        //GatewayHub endpoint
        endpoints.MapHub<GatewayHub>("/gatewayhub");
        endpoints.MapControllers();
    });
}
```

In your Api Orchestration, provide the Url to the Gateway Hub:

```C#
orchestrator.StartGatewayHub = true;
orchestrator.GatewayHubUrl = "https://localhost:44360/GatewayHub";
```

**Note**

You can turn off the GatewayHub by

```C#
orchestrator.StartGatewayHub = false;
```

## Security

You can secure the POST endpoint by implementing interface **IHubPostGatewayAuthorization**.

Please see section [Authorization](/Docs/README_Authorization.md) to learn how to do this.

Also, if you are going to receive notifications from the downstream Hub,

you have to specify the **ReceiveKey**.

## Client

In your **Client**, connect to the GatewayHub and subscribe to **ReceiveMessage**.

```C#
var conn = new HubConnectionBuilder()
                .WithUrl("https://localhost:44360/GatewayHub")
                .AddNewtonsoftJsonProtocol()
                .Build();

conn.StartAsync().ConfigureAwait(false);

conn.On("ReceiveMessage", new Type[] { typeof(object), typeof(object) }, (arg1, arg2) =>
{
    //receive your data here
}, new object());
```
