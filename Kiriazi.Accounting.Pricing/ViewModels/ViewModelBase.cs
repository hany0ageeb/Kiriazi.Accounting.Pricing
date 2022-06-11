using Npoi.Mapper.Attributes;
using System;
using System.ComponentModel;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
    public class ItemRelationViewModel
    {
        public Guid RootId { get; set; }
        public string RootCode { get; set; }
        public string RootArabicName { get; set; }
        public string RootUomCode { get; set; }

        public Guid ComponentId { get; set; }
        public string ComponentCode { get; set; }
        public string ComponentArabicName { get; set; }
        public string ComponentUomCode { get; set; }
        public decimal ComponentQuantity { get; set; }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }

    }
}
