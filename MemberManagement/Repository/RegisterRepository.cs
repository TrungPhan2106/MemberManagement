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
    public class RegisterRepository : Repository<StudioRegister>, IRegisterRepository
    {
        private MyDbContext _db;
        public RegisterRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(StudioRegister obj)
        {
            _db.StudioRegister.Update(obj);
        }
    }
}
