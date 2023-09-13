namespace Notes.AuthServer.Data
{
    public static class DBInitializer
    {
        public static void Initialize(AuthDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
