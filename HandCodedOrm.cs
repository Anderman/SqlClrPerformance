using System;
using System.Data;
using System.Data.SqlClient;

namespace testApp
{
    public class HandCodedOrm : IDisposable
    {
        private readonly SqlParameter _idParam;
        private readonly SqlCommand _postCommand;
        private Post _result;
        private int i = 1;
        private SqlConnection _connection;

        public HandCodedOrm()
        {
            //_connection = new SqlConnection("context connection=true");
            _connection = new SqlConnection("Data Source=.;Initial Catalog=tempdb;Integrated Security=True");
            _connection.Open();
            EnsureDbSetup();
            _postCommand = new SqlCommand
            {
                Connection = _connection,
                CommandText = @"select * from Posts where Id = @Id"
            };
            _idParam = _postCommand.Parameters.Add("@Id", SqlDbType.Int);
        }

        public void Run()
        {
            if (i++ > 5000)
                i = 1;
            _result = SqlCommand();
        }

        public Post SqlCommand()
        {
            _idParam.Value = i;

            using (var reader = _postCommand.ExecuteReader())
            {
                reader.Read();
                var post = new Post
                {
                    Id = reader.GetInt32(0),
                    Text = GetNullableString(reader, 1),
                    CreationDate = reader.GetDateTime(2),
                    LastChangeDate = reader.GetDateTime(3),
                    Counter1 = GetNullableValue<int>(reader, 4),
                    Counter2 = GetNullableValue<int>(reader, 5),
                    Counter3 = GetNullableValue<int>(reader, 6),
                    Counter4 = GetNullableValue<int>(reader, 7),
                    Counter5 = GetNullableValue<int>(reader, 8),
                    Counter6 = GetNullableValue<int>(reader, 9),
                    Counter7 = GetNullableValue<int>(reader, 10),
                    Counter8 = GetNullableValue<int>(reader, 11),
                    Counter9 = GetNullableValue<int>(reader, 12)
                };

                return post;
            }
        }

        public static string GetNullableString(IDataReader reader, int index)
        {
            var tmp = reader.GetValue(index);
            return tmp != DBNull.Value ? (string)tmp : null;
        }

        public static T? GetNullableValue<T>(IDataReader reader, int index) where T : struct
        {
            var tmp = reader.GetValue(index);
            return tmp != DBNull.Value ? (T?)(T)tmp : null;
        }
        private void EnsureDbSetup()
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
If (Object_Id('Posts') Is Null)
Begin
	Create Table Posts
	(
		Id int identity primary key, 
		[Text] varchar(max) not null, 
		CreationDate datetime not null, 
		LastChangeDate datetime not null,
		Counter1 int,
		Counter2 int,
		Counter3 int,
		Counter4 int,
		Counter5 int,
		Counter6 int,
		Counter7 int,
		Counter8 int,
		Counter9 int
	);
	   
	Set NoCount On;
	Declare @i int = 0;

	While @i <= 5001
	Begin
		Insert Posts ([Text],CreationDate, LastChangeDate) values (replicate('x', 2000), GETDATE(), GETDATE());
		Set @i = @i + 1;
	End
End
";
            cmd.Connection = _connection;
            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _postCommand?.Dispose();
            _connection?.Dispose();
        }
    }
}