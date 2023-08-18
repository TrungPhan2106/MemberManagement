using StudioManagement.Data;
using StudioManagement.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StudioManagement.Repository
{
    public class StudioRepository : Repository<Studio>, IStudioRepository
    {
        private MyDbContext _db;
        public StudioRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Studio obj)
        {
            _db.Studio.Update(obj);
        }
    }
}
