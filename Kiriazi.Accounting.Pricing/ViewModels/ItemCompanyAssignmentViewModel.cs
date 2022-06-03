using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiriazi.Accounting.Pricing.ViewModels
{
    public class ItemCompanyAssignmentViewModel : ViewModelBase
    {
        private bool _isAssigned = false;
        private string _alise = "";
        private Models.Group _group = null;
        private Models.Company _company;

        public Guid Id { get; set; } = Guid.NewGuid();

        public ItemCompanyAssignmentViewModel(Models.Company company)
        {
            _company = company;
        }
        public bool IsAssigned
        {
            get => _isAssigned;
            set
            {
                if (_isAssigned != value)
                {
                    _isAssigned = value;
                    OnPropertyChanged("IsAssigned");
                }
            }
        }
        public string Alise
        {
            get => _alise;
            set
            {
                if (_alise != value)
                {
                    _alise = value;
                    OnPropertyChanged(nameof(Alise));
                }
            }
        }
        public Models.Group Group
        {
            get => _group;
            set
            {
                if (_group != value)
                {
                    _group = value;
                    OnPropertyChanged(nameof(Group));
                }
            }
        }
        public string CompanyName => _company.Name;
        public Models.Company Company
        {
            get => _company;
            set
            {
                if (_company != value)
                {
                    _company = value;
                    OnPropertyChanged(nameof(Company));
                }
            }
        }
        public IList<Models.Group> Groups { get; set; }
    }
}
