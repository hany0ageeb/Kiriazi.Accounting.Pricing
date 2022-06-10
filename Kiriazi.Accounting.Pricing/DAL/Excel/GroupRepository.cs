using Kiriazi.Accounting.Pricing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.DAL.Excel
{
    public class GroupRepository : IGroupRepository
    {
        private readonly Npoi.Mapper.Mapper mapper;
        public GroupRepository(string filePath)
        {
            mapper = new Npoi.Mapper.Mapper(filePath);
            mapper.HasHeader = true;
            mapper.SkipBlankRows = true;
        }
        public void Add(Group entity)
        {
            mapper.Put<Group>(new List<Group>() { entity },0,false);
        }

        public void Add(IEnumerable<Group> entities)
        {
            mapper.Put<Group>(entities, 0, false);
        }

        public Group Find(object Id)
        {
            return mapper.Take<Group>().Select(r => r.Value).Where(g => g.Id == (Guid)Id).FirstOrDefault();
        }

        public IEnumerable<Group> Find()
        {
            return mapper.Take<Group>().Select(r => r.Value).AsEnumerable();
        }

        public IEnumerable<Group> Find(Func<IQueryable<Group>, IQueryable<Group>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Group> Find(Func<IQueryable<Group>, IOrderedQueryable<Group>> orderBy)
        {
            return orderBy(Find().AsQueryable());
        }

        public IEnumerable<Group> Find(Expression<Func<Group, bool>> predicate)
        {
            return Find().AsQueryable().Where(predicate);
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<Group, bool>> predicate, 
            Expression<Func<Group, TResult>> selector, 
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            var query = Find().AsQueryable().Where(predicate);
            if (orderBy != null)
            {
                return orderBy(query.Select(selector));
            }
            else
            {
                return query.Select(selector);
            }
        }

        public IEnumerable<Group> Find(
            Expression<Func<Group, bool>> predicate, 
            Func<IQueryable<Group>, IQueryable<Group>> include)
        {
            return include(Find().AsQueryable().Where(predicate));
        }

        public IEnumerable<Group> Find(
            Expression<Func<Group, bool>> predicate, 
            Func<IQueryable<Group>, IOrderedQueryable<Group>> orderBy)
        {
            return orderBy(Find().AsQueryable().Where(predicate));
        }

        public void Remove(Group entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<Group> entities)
        {
            throw new NotImplementedException();
        }
    }
}
