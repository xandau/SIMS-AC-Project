using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Repository.Tests
{
    [TestClass()]
    public class UserRepositoryTests
    {
        // Checks if User can be created when E-Mail is already existing - Debug Mode must be used
        [TestMethod()]
        public async Task GetUserByUsernameAsyncTest()
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            string? secretData = config["ConnectionStrings-SQL"]; 

            DbContextOptions options = new DbContextOptionsBuilder<SIMSContext>()
               .UseSqlServer(secretData)
               .Options;

            SIMSContext context = new SIMSContext(options);
            
            UserRepository repo = new UserRepository(context);

            List<User>? userList = await repo.GetAllAsync();

            User testUser = new User()
            {
                Email = userList[0].Email,
                UserName = "test",
                FirstName = "TestUser",
                LastName = "TestUser",
                Password = "password"
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => { await repo.CreateAsync(testUser); });
        }


    }
}