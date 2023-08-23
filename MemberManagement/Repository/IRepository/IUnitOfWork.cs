using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StudioManagement.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IStudioRepository Studio { get; }
        IMemberRepository Member { get; }
        void Save();
    }
}
