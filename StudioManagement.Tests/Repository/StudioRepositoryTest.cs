using Microsoft.EntityFrameworkCore;
using StudioManagement.Data;
using StudioManagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManagement.Tests.Repository
{
    public class StudioRepositoryTest
    {
        private async Task<MyDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new MyDbContext(options);
            databaseContext.Database.EnsureCreated();
            if(await databaseContext.Studio.CountAsync() < 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Studio.Add(
                        new Studio()
                        {
                            StudioName = "Studio 1",
                            StudioAddress = "TP.HCM",
                            StudioPhone = "000000000",
                            StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
                        });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async void StudioRepository_Add_ReturnBool()
        {
            //arrange
            var studio = new Studio()
            {
                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var dbContext = await GetDbContext();
            var studioRepository = new StudioRepository(dbContext);
            //act
            studioRepository.Add(studio);
            //assert

        }
    }
}
