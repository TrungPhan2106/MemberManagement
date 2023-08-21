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
    public class MemberRepository : Repository<Member>, IMemberRepository
    {
        private MyDbContext _db;
        public MemberRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Member obj)
        {
            _db.Member.Update(obj);
        }
    }
}
