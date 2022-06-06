using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentManagementSystem.Migrations
{
    public partial class CreateDefaultAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Default Admin.  Email: admin@sms.com  Password: P@ssw0rd
            migrationBuilder.Sql("INSERT INTO [security].[Users] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'90c36e87-f42a-4e22-ba23-de42781fd495', N'Admin', N'Admin', N'admin@sms.com', N'ADMIN@SMS.COM', N'admin@sms.com', N'ADMIN@SMS.COM', 1, N'AQAAAAEAACcQAAAAEHH+2cZdcMu/5+p0hwJnoaOseUu380/hDMwthiNQBcYeF1I3uoVr4Wlo/NNYz04Y/w==', N'IUFNW2BLKNLB35PHOHYRKHFVUOEJPR7J', N'af3fcd46-1736-4d08-bb58-e9374ac52d09', NULL, 0, 0, NULL, 1, 0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[Users] WHERE Id = '90c36e87-f42a-4e22-ba23-de42781fd495'");
        }
    }
}
