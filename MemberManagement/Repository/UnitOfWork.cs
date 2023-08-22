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
    public class UnitOfWork : IUnitOfWork
    {
        private MyDbContext _db;
        public IStudioRepository Studio { get; private set; }
        public IMemberRepository Member { get; private set; }
        public IRegisterRepository Register { get; private set; }
        public UnitOfWork(MyDbContext db)
        {
            _db = db;
            Studio = new StudioRepository(_db);
            Member = new MemberRepository(_db);
            Register = new RegisterRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
