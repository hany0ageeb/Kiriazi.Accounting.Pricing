using Kiriazi.Accounting.Pricing.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kiriazi.Accounting.Pricing.DAL
{
    public class ItemTypeRepository : Repository<ItemType>, IItemTypeRepository
    {
        private  ItemType _manufacturedItemType;
        private  ItemType _rawItemType;
        private  IList<ItemType> _allItemTypes;
        public ItemTypeRepository(PricingDBContext context)
            : base(context)
        {
           
        }
        public PricingDBContext PricingDBContext => base._context as PricingDBContext;
        public  ItemType ManufacturedItemType
        {
            get
            {
                if (_manufacturedItemType == null)
                {
                    _manufacturedItemType = PricingDBContext.ItemTypes.Where(it => it.Name == "صنف مصنع").FirstOrDefault();
                }
                return _manufacturedItemType;
            }
           
        } 
        public  ItemType RawItemType
        {
            get 
            {
                if (_rawItemType == null)
                {
                    _rawItemType = PricingDBContext.ItemTypes.Where(it => it.Name == "صنف خام").FirstOrDefault();
                }
                return _rawItemType;
            }
            
        }
        public  IList<ItemType> AllItemTypes
        {
            get
            {
                if (_allItemTypes == null)
                {
                    _allItemTypes = new List<ItemType>() { _rawItemType, _manufacturedItemType };
                }
                return _allItemTypes;
            }
        
        }
    }
}
