using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace WilliamDenton.AwsRdsPostgresDemo
{
	public static class NpgsqlAwsRdsDbContextOptionsBuilderExtensions
	{
		public static NpgsqlDbContextOptionsBuilder UseAwsIamAuthentication(this NpgsqlDbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.ProvidePasswordCallback(GenerateAwsIamAuthToken);
			return optionsBuilder;
		}

		static string GenerateAwsIamAuthToken(string host, int port, string database, string username)
		{
			if (host.EndsWith("rds.amazonaws.com")) {
				return Amazon.RDS.Util.RDSAuthTokenGenerator.GenerateAuthToken(host, port, username);
			} else {
				return $"{username}123";
			}
		}
	}
}
