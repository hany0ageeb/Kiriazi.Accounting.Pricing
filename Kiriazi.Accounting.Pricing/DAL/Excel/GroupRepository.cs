using Kiriazi.Accounting.Pricing.Models;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.DAL.Excel
{
    public class ItemRepository : IItemRepository
    {
        private readonly Mapper mapper;
        public ItemRepository(string file)
        {
            mapper = new Mapper(file);
            mapper.Map<Item>("كود الوحدة", nameof(Item.Uom), (column, Target) =>
              {
                  if (column.HeaderValue == null || column.CurrentValue == null || string.IsNullOrEmpty(column.CurrentValue.ToString()))
                      return false;
                  Target =  new Uom() { Code = column.CurrentValue.ToString() };
                  return true;
              });
            mapper.Map<Item>("كود البند الجمركى", nameof(Item.Tarrif), (column, target) =>
            {
                if (column.HeaderValue == null || column.CurrentValue == null || string.IsNullOrEmpty(column.CurrentValue.ToString()))
                {
                    target = null;
                    return true;
                }
                target = new Tarrif() { Code = column.CurrentValue.ToString() };
                return true;
            });
            mapper.Map<Item>("نوع الصنف", nameof(Item.ItemType), (column, target) =>
              {
                  if (column.HeaderValue == null || column.CurrentValue == null || string.IsNullOrEmpty(column.CurrentValue.ToString()))
                      return false;
                  target = new ItemType() { Name = column.CurrentValue.ToString() };
                  return true;
              });
        }
        public void Add(Item entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<Item> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> Find(string code = "", string arabicName = "", string englishName = "", string nameAlias = "", Guid? companyId = null, Guid? itemTypeId = null, Func<IQueryable<Item>, IOrderedQueryable<Item>> orderBy = null, Func<IQueryable<Item>, IQueryable<Item>> include = null)
        {
            throw new NotImplementedException();
        }

        public Item Find(object Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> Find()
        {
            return mapper.Take<Item>().Select(r => r.Value);
        }

        public IEnumerable<Item> Find(Func<IQueryable<Item>, IQueryable<Item>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> Find(Func<IQueryable<Item>, IOrderedQueryable<Item>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> Find(Expression<Func<Item, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<Item, bool>> predicate, Expression<Func<Item, TResult>> selector, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> Find(Expression<Func<Item, bool>> predicate, Func<IQueryable<Item>, IQueryable<Item>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> Find(Expression<Func<Item, bool>> predicate, Func<IQueryable<Item>, IOrderedQueryable<Item>> orderBy)
        {
            throw new NotImplementedException();
        }

        public Item FindByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> FindItemsCodes()
        {
            throw new NotImplementedException();
        }

        public void Remove(Item entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<Item> entities)
        {
            throw new NotImplementedException();
        }
    }
    public class TarrifRepository : ITarrifRepository
    {
        private readonly Npoi.Mapper.Mapper mapper;

        public TarrifRepository(string file)
        {
            mapper = new Npoi.Mapper.Mapper(file);
        }
        public void Add(Tarrif entity)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<Tarrif> entities)
        {
            throw new NotImplementedException();
        }

        public Tarrif Find(object Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find()
        {
            return mapper.Take<Tarrif>().Select(r => r.Value);
        }

        public IEnumerable<Tarrif> Find(Func<IQueryable<Tarrif>, IQueryable<Tarrif>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find(Func<IQueryable<Tarrif>, IOrderedQueryable<Tarrif>> orderBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find(Expression<Func<Tarrif, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<Tarrif, bool>> predicate, Expression<Func<Tarrif, TResult>> selector, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find(Expression<Func<Tarrif, bool>> predicate, Func<IQueryable<Tarrif>, IQueryable<Tarrif>> include)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tarrif> Find(Expression<Func<Tarrif, bool>> predicate, Func<IQueryable<Tarrif>, IOrderedQueryable<Tarrif>> orderBy)
        {
            throw new NotImplementedException();
        }

        public void Remove(Tarrif entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<Tarrif> entities)
        {
            throw new NotImplementedException();
        }
    }
    public class GroupRepository : IGroupRepository
    {
        private readonly Npoi.Mapper.Mapper mapper;
        public GroupRepository(string filePath)
        {
            mapper = new Npoi.Mapper.Mapper(filePath);
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
