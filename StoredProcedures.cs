using Microsoft.SqlServer.Server;

namespace testApp
{
    public partial class StoredProcedures
    {
        public static ulong TestDuration = 10000000000;

        [SqlProcedure]
        public static void SqlStoredProcedure1()
        {
            var bm = new Benchmarks(SqlContext.Pipe.Send);
            bm.Run();
        }
    }
}