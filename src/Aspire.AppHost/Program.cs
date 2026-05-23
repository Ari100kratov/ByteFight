// Some VPN clients block Aspire DCP's default IPv6 loopback endpoint.
Environment.SetEnvironmentVariable("DCP_IP_VERSION_PREFERENCE", "IPv4");

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("database")
    .WithImage("postgres:17")
    .WithBindMount("../../.containers/db", "/var/lib/postgresql/data");

IResourceBuilder<PostgresDatabaseResource> database = postgres.AddDatabase("bytefight");
IResourceBuilder<PostgresDatabaseResource> chroniclesDatabase = postgres.AddDatabase(
    "bytefight-chronicles",
    "bytefight_chronicles");

IResourceBuilder<ContainerResource> minio = builder
    .AddContainer("minio", "minio/minio", "latest")
    .WithBindMount("../../.containers/minio", "/data")
    .WithEnvironment("MINIO_ROOT_USER", "admin")
    .WithEnvironment("MINIO_ROOT_PASSWORD", "password123")
    .WithArgs("server", "/data", "--console-address", ":9001")
    .WithEndpoint(port: 9000, targetPort: 9000, name: "api")
    .WithEndpoint(port: 9001, targetPort: 9001, name: "console");

IResourceBuilder<ProjectResource> migrator = builder.AddProject<Projects.Migrator>("migrator")
    .WithEnvironment("ConnectionStrings__Database", database)
    .WithEnvironment("ConnectionStrings__ChroniclesDatabase", chroniclesDatabase)
    .WithEnvironment("ConnectionStrings__GameRuntimeDatabase", database)
    .WithReference(database)
    .WithReference(chroniclesDatabase)
    .WaitFor(database)
    .WaitFor(chroniclesDatabase);

builder.AddProject<Projects.Web_Api>("web-api")
    .WithEnvironment("ConnectionStrings__Database", database)
    .WithEnvironment("ConnectionStrings__ChroniclesDatabase", chroniclesDatabase)
    .WithReference(database)
    .WithReference(chroniclesDatabase)
    .WithEnvironment("Minio__Endpoint", "localhost:9000")
    .WithEnvironment("Minio__AccessKey", "admin")
    .WithEnvironment("Minio__SecretKey", "password123")
    .WithEnvironment("Minio__UseSSL", "false")
    .WaitFor(database)
    .WaitFor(chroniclesDatabase)
    .WaitFor(minio)
    .WaitForCompletion(migrator);

builder.AddProject<Projects.Chronicles_Worker>("chronicles-worker")
    .WithEnvironment("ConnectionStrings__ChroniclesDatabase", chroniclesDatabase)
    .WithEnvironment("ConnectionStrings__GameRuntimeDatabase", database)
    .WithReference(chroniclesDatabase)
    .WithReference(database)
    .WaitFor(chroniclesDatabase)
    .WaitFor(database)
    .WaitForCompletion(migrator);

builder.Build().Run();
