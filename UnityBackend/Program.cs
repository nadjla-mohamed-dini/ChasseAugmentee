using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDb>(option=> option.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/users", async  (AppDb db) =>await  db.Users.ToListAsync());
app.MapGet("/sessions",async (AppDb db) =>await  db.Sessions.ToListAsync());
app.MapPost("/register",async (AppDb db, Users user) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok(user);
});

app.MapPost("/login", async (AppDb db, Users user) =>
{
    var existingUser = db.Users.FirstOrDefault(e => e.Username == user.Username && e.Password == user.Password);
    if (existingUser != null)
    {
        return Results.Ok(existingUser);}
    else
    {
        return Results.NotFound("User not found");
    }
});
app.MapPost("/sessions",async (AppDb db, Sessions session) =>
{
    db.Sessions.Add(session);
    await db.SaveChangesAsync();
    return Results.Ok(session);
}
    
);
app.MapGet("/session/{userId}", async (AppDb db, int userId) =>
{
    var session = await db.Sessions
        .Where(s => s.User_Id == userId)
        .OrderByDescending(s => s.PlayedAt)
        .Join(db.Users, session => session.User_Id, user => userId,
        (session, user) => new {
            Username = user.Username,
            Score = session.Score,
            PlayetAt = session.PlayedAt
        })
        .ToListAsync();
    return Results.Ok(session);
});

    


app.Run();

